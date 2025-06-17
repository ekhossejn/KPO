# Микросервисная Платежная Система

## Критерии реализации

### 1. Реализация основных требований к функциональности
- Payments Service реализует создание счета (`AccountsController.CreateAccount`), пополнение счета (`AccountsController.Deposit`), просмотр баланса (`AccountsController.GetAccount`)
- Orders Service реализует создание заказа (`OrdersController.CreateOrder`), просмотр списка заказов (`OrdersController.GetUserOrders`), просмотр статуса заказа (`OrdersController.GetOrder`)
- Реализованы следующие ручки:
   /api/Accounts POST
   /api/Accounts/{userId} GET
   /api/Accounts/deposit POST - все финансы вычисляются с точностью 18 знаков и 2 после запятой. Минимальные финансы 0.01.

   /api/Orders POST
   /api/Orders/{orderId} GET
   /api/Orders/user/{userId} GET - добавлена пагинация, чтобы возвращать ограниченное число заказов, если их слишком много
- Реализованы логи для откладки, вычисляется Reason для Cancelled заказов для откладки



### 2. Архитектурное проектирование
**Четкое разделение на сервисы:** Архитектура разделена на три отдельных сервиса - API Gateway, Orders Service и Payments Service, каждый со своей четкой ответственностью (маршрутизация / работа с заказами / работа с финансами)
**Логичное использование очередей сообщений:** RabbitMQ используется для асинхронной коммуникации между сервисами, обеспечивает надежную доставку сообщений

### 3. Документация API
Все API задокументированы с помощью Swagger:
- API Gateway: доступен по адресу http://localhost:8080/swagger
- Orders Service: доступен внутри контейнера по адресу http://localhost:8081/swagger
- Payments Service: доступен внутри контейнера по адресу http://localhost:8082/swagger

### 4. Тестовое покрытие
Проект имеет тестовое покрытие 19.2%:
- `OrdersService.Tests`
- `PaymentsService.Tests`
- Детальный отчет о покрытии доступен в папке TestResults и в файле `code_coverage_report.md` 

### 5. Docker контейнеризация
- Каждый сервис упакован в Docker контейнер с помощью `Dockerfile` в соответствующих директориях
- Система полностью разворачивается с помощью `docker-compose.yml`
- При запуске с помощью docker-compose система автоматически подготавливает базы данных благодаря миграциям Entity Framework в `Program.cs` сервисов

## Архитектура
Система построена с использованием следующих компонентов:
- **API Gateway**: Маршрутизирует запросы к соответствующим сервисам (реализовано в `ApiGateway/Controllers/`)
- **Orders Service**: Обрабатывает создание и управление заказами (реализовано в `OrdersService/`)
- **Payments Service**: Обрабатывает создание счетов, внесение депозитов и обработку платежей (реализовано в `PaymentsService/`)
- **PostgreSQL**: База данных для обоих сервисов (сконфигурировано в `docker-compose.yml`)
- **RabbitMQ**: Брокер сообщений для асинхронной коммуникации (используется в `MessageService` обоих сервисов)

### Паттерны проектирования
Система реализует несколько важных паттернов:
- **Transactional Outbox** в обоих сервисах для обеспечения надежной доставки сообщений:
  - Orders Service: Реализация в `OrderService.CreateOrderAsync()` и `OutboxProcessorService`
  - Payments Service: Реализация в `AccountService.ProcessPaymentAsync()` и соответствующем процессоре
- **Transactional Inbox** в Payments Service для идемпотентной обработки сообщений:
  - Реализовано через сохранение и проверку обработанных сообщений в `PaymentsDbContext`
  - Реализовано not more than once (version), not less than once (transactions)

## Начало работы

### Предварительные требования
- Docker и Docker Compose
- .NET 7 SDK

### Запуск приложения
1. Клонировать репозиторий
2. Выполнить следующую команду для запуска всех сервисов:
```bash
docker-compose up
```

**Внутренние шаги при создании заказа:**
1. Order Service создает в базе данных заказ и задачу на оплату (transactional outbox) в `OrderService.CreateOrderAsync()`
2. `OutboxProcessorService` асинхронно вычитывает задачу и отправляет в очередь RabbitMQ
3. Payments Service получает задачу на оплату заказа через `MessageService` и сохраняет в БД
4. Payments Service обрабатывает платеж в `AccountService.ProcessPaymentAsync()`:
   - Проверяет наличие счета
   - Проверяет достаточность средств
   - Атомарно изменяет баланс с учетом optimistic concurrency
   - Записывает событие результата платежа для отправки
5. Payments Service асинхронно отправляет результат оплаты в очередь
6. Order Service получает событие результата оплаты и обновляет статус заказа

Весь процесс обеспечивает гарантии доставки сообщений и семантику exactly once для платежей.
