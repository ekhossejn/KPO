using System;
using big_hw_1.facades;
using big_hw_1.commands;
using Microsoft.Extensions.DependencyInjection;
using big_hw_1.models;

namespace big_hw_1.menu
{
	public class Menu
	{
        private readonly BankAccountFacade _bankAccountFacade;
        private readonly CategoryFacade _categoryFacade;
        private readonly OperationFacade _operationFacade;

        public Menu(ServiceProvider provider)
		{
            _bankAccountFacade = provider.GetRequiredService<BankAccountFacade>();
            _categoryFacade = provider.GetRequiredService<CategoryFacade>();
            _operationFacade = provider.GetRequiredService<OperationFacade>();
        }

        private static void PrintMenu()
        {
            Console.WriteLine("=====HSE BANK=====");
            Console.WriteLine("1. Print accounts");
            Console.WriteLine("2. Create account");
            Console.WriteLine("3. Change account's name");
            Console.WriteLine("4. Delete account");
            Console.WriteLine("==================");
            Console.WriteLine("5. Print categories");
            Console.WriteLine("6. Create category");
            Console.WriteLine("7. Change category");
            Console.WriteLine("8. Delete category");
            Console.WriteLine("==================");
            Console.WriteLine("9. Print operations");
            Console.WriteLine("10. Create operation");
            Console.WriteLine("11. Change operation");
            Console.WriteLine("12. Delete operation");
            Console.WriteLine("==================");
            Console.Write("Input number from 1 to 12: ");
        }

		public void Show()
		{
			while (true)
			{
                PrintMenu();
                var id = Console.ReadLine();
                switch (id)
                {
                    case "1": PrintBankAccounts();  break;
                    case "2": CreateBankAccount(); break;
                    case "3": ChangeBankAccount(); break;
                    case "4": DeleteBankAccount(); break;
                    case "5": PrintCategories(); break;
                    case "6": CreateCategory(); break;
                    case "7": ChangeCategory(); break;
                    case "8": DeleteCategory(); break;
                    case "9": PrintOperations(); break;
                    case "10": CreateOperation(); break;
                    case "11": ChangeOperation(); break;
                    case "12": DeleteOperation(); break;
                    default: Console.WriteLine("Invalid input\n"); break;
                }
                Console.WriteLine("Type anything to continue..");
                Console.ReadLine();
                Console.Clear();
            }
        }

        private void PrintShortlyBankAccounts()
        {
            var bankAccounts = _bankAccountFacade.Get();
            
            Console.WriteLine("Accounts to choose:");
            foreach (var bankAccount in bankAccounts)
            {
                Console.WriteLine($"Id: {bankAccount.Id} Name: {bankAccount.Name}");
                Console.WriteLine();
            }
        }

        private void PrintBankAccounts()
        {
            var bankAccounts = _bankAccountFacade.Get();
            if (!bankAccounts.Any())
            {
                Console.WriteLine(">>>>>>>>>>>>>");
                Console.WriteLine("Empty result");
                Console.WriteLine(">>>>>>>>>>>>>");
                return;
            }
            Console.WriteLine(">>>>>>>>>>>>>");
            foreach (var bankAccount in bankAccounts) {
                Console.WriteLine($"Id: {bankAccount.Id}");
                Console.WriteLine($"Name: {bankAccount.Name}");
                Console.WriteLine($"Balance: {bankAccount.Balance} монеток");
                Console.WriteLine();
            }
            Console.WriteLine(">>>>>>>>>>>>>");
        }

        private void CreateBankAccount()
        {
            Console.Write("Name: ");
            var name = Console.ReadLine();
            if (name == null)
            {
                Console.WriteLine("Name must not be null\n");
                return;
            }
            Console.Write("Balance: ");
            if (!decimal.TryParse(Console.ReadLine(), out var balance))
            {
                Console.WriteLine("Balance must be decimal\n");
                return;
            }

            try
            {
                var command = new CreateBankAccountCommand(_bankAccountFacade, name, balance);
                var time = new TimeMeasureDecorator(command, LogCallback);
                time.Execute();
                Console.WriteLine("Bank account created\n");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error while creating bank account: {ex.Message}\n");
            }
        }

        private void ChangeBankAccount()
        {
            Console.Write("Id: ");
            if (!Guid.TryParse(Console.ReadLine(), out var id))
            {
                Console.WriteLine("Id must be guid\n");
                return;
            }

            var bankAccount = _bankAccountFacade.Get(id);
            if (bankAccount == null)
            {
                Console.WriteLine("Bank account not found\n");
                return;
            }

            Console.Write("Input name:");
            var name = Console.ReadLine();
            if (name == null)
            {
                Console.WriteLine("Invalid input\n");
                return;
            }
            bankAccount.ChangeName(name);
            
        }

        private void DeleteBankAccount()
        {
            Console.Write("Id: ");
            if (!Guid.TryParse(Console.ReadLine(), out var id))
            {
                Console.WriteLine("Id must be guid\n");
                return;
            }

            _bankAccountFacade.Delete(id);
            Console.WriteLine("Deleted (even if there wasn't such account)\n");
        }

        private void PrintShortlyCategories()
        {
            var categories = _categoryFacade.Get();
            Console.WriteLine("Categories to choose: ");
            foreach (var category in categories)
            {
                Console.WriteLine($"Id: {category.Id} Name: {category.Name} Type: {category.Type}");
                Console.WriteLine();
            }
        }

        private void PrintCategories()
        {
            var categories = _categoryFacade.Get();
            if (!categories.Any())
            {
                Console.WriteLine(">>>>>>>>>>>>>");
                Console.WriteLine("Empty result");
                Console.WriteLine(">>>>>>>>>>>>>");
                return;
            }
            Console.WriteLine(">>>>>>>>>>>>>");
            foreach (var category in categories)
            {
                Console.WriteLine($"Id: {category.Id}");
                Console.WriteLine($"Name: {category.Name}");
                Console.WriteLine($"Type: {category.Type}");
                Console.WriteLine();
            }
            Console.WriteLine(">>>>>>>>>>>>>");
        }

        private void CreateCategory()
        {
            Console.Write("Name: ");
            var name = Console.ReadLine();
            if (name == null)
            {
                Console.WriteLine("Name must not be null\n");
                return;
            }
            Console.WriteLine("1. Income");
            Console.WriteLine("2. Expense");
            Console.Write("Input number from 1 to 2: ");
            if (!decimal.TryParse(Console.ReadLine(), out var num))
            {
                Console.WriteLine("Invalid input\n");
                return;
            }

            try
            {
                var command = new CreateCategoryCommand(_categoryFacade, name, num == 1 ? models.Type.Income : models.Type.Expense);
                var time = new TimeMeasureDecorator(command, LogCallback);
                time.Execute();

                Console.WriteLine("Category created\n");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error while creating category: {ex.Message}\n");
            }
        }

        private void ChangeCategory()
        {
            Console.Write("Id: ");
            if (!Guid.TryParse(Console.ReadLine(), out var id))
            {
                Console.WriteLine("Id must be guid\n");
                return;
            }

            var category = _categoryFacade.Get(id);
            if (category == null)
            {
                Console.WriteLine("Category not found\n");
                return;
            }

            Console.WriteLine("1. Name");
            Console.WriteLine("2. Type");
            Console.Write("Input number from 1 to 2: ");
            if (!int.TryParse(Console.ReadLine(), out var num) | num < 1 | num > 2)
            {
                Console.WriteLine("Invalid input\n");
                return;
            }

            if (num == 1)
            {
                Console.Write("Name: ");
                var name = Console.ReadLine();
                if (name == null)
                {
                    Console.WriteLine("Invalid input\n");
                    return;
                }
                category.ChangeName(name);
            }
            else
            {
                Console.WriteLine("1. Income");
                Console.WriteLine("2. Expense");
                Console.Write("Input number from 1 to 2: ");
                if (!int.TryParse(Console.ReadLine(), out var type) | type < 1 | type > 2)
                {
                    Console.WriteLine("Invalid input\n");
                    return;
                }
                category.ChangeType(type == 1 ? models.Type.Income : models.Type.Expense);
            }
        }

        private void DeleteCategory()
        {
            Console.Write("Id: ");
            if (!Guid.TryParse(Console.ReadLine(), out var id))
            {
                Console.WriteLine("Id must be guid\n");
                return;
            }

            _categoryFacade.Delete(id);
            Console.WriteLine("Deleted (even if there wasn't such category)\n");
        }

        private void PrintOperations()
        {
            var operations = _operationFacade.Get();
            if (!operations.Any())
            {
                Console.WriteLine(">>>>>>>>>>>>>");
                Console.WriteLine("Empty result");
                Console.WriteLine(">>>>>>>>>>>>>");
                return;
            }
            Console.WriteLine(">>>>>>>>>>>>>");
            foreach (var operation in operations)
            {
                Console.WriteLine($"Id: {operation.Id}");
                Console.WriteLine($"Type: {operation.Type}");
                Console.WriteLine($"Bank Account Id: {operation.BankAccountId}");
                Console.WriteLine($"Amount: {operation.Amount}");
                Console.WriteLine($"Date: {operation.Date}");
                Console.WriteLine($"Description: {operation.Description}");
                Console.WriteLine($"Category Id: {operation.CategoryId}");
                Console.WriteLine();
            }
            Console.WriteLine(">>>>>>>>>>>>>");
        }

        private void CreateOperation()
        {
            PrintShortlyBankAccounts();
            Console.Write("Bank Account Id: ");
            if (!Guid.TryParse(Console.ReadLine(), out var accountId))
            {
                Console.WriteLine("Id must be guid\n");
                return;
            }
            var bankAccount = _bankAccountFacade.Get(accountId);
            if (bankAccount == null)
            {
                Console.WriteLine("Bank account not found\n");
                return;
            }

            PrintShortlyCategories();
            Console.Write("Category Id: ");
            if (!Guid.TryParse(Console.ReadLine(), out var categoryId))
            {
                Console.WriteLine("Id must be guid\n");
                return;
            }
            var category = _categoryFacade.Get(categoryId);
            if (category == null)
            {
                Console.WriteLine("Category not found\n");
                return;
            }

            Console.Write("Amount: ");
            if (!decimal.TryParse(Console.ReadLine(), out var amount))
            {
                Console.WriteLine("Amount must be decimal\n");
                return;
            }

            Console.Write("Description: ");
            var description = Console.ReadLine();
            if (description == null)
            {
                Console.WriteLine("Invalid input\n");
                return;
            }

            try
            {
                var command = new CreateOperationCommand(_operationFacade, category.Type, accountId, amount, description, categoryId);
                var time = new TimeMeasureDecorator(command, LogCallback);
                time.Execute();

                Console.WriteLine("Operation created\n");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error while creating operation: {ex.Message}\n");
            }
        }

        private void ChangeOperation()
        {
            Console.Write("Id: ");
            if (!Guid.TryParse(Console.ReadLine(), out var id))
            {
                Console.WriteLine("Id must be guid\n");
                return;
            }

            var operation = _operationFacade.Get(id);
            if (operation == null)
            {
                Console.WriteLine("Operation not found\n");
                return;
            }

            Console.WriteLine("1. Description");
            Console.WriteLine("2. Category Id");
            Console.Write("Input number from 1 to 2: ");
            if (!int.TryParse(Console.ReadLine(), out var num) | num < 1 | num > 2)
            {
                Console.WriteLine("Invalid input\n");
                return;
            }

            if (num == 1)
            {
                Console.Write("Description: ");
                var description = Console.ReadLine();
                if (description == null)
                {
                    Console.WriteLine("Invalid input\n");
                    return;
                }
                operation.ChangeDescription(description);
            }
            else
            {
                Console.Write("Id: ");
                if (!Guid.TryParse(Console.ReadLine(), out var categoryId))
                {
                    Console.WriteLine("Invalid input\n");
                    return;
                }
                operation.ChangeCategoryId(categoryId);
            }
        }

        private void DeleteOperation()
        {
            Console.Write("Id: ");
            if (!Guid.TryParse(Console.ReadLine(), out var id))
            {
                Console.WriteLine("Id must be guid\n");
                return;
            }

            _operationFacade.Delete(id);
            Console.WriteLine("Deleted without recalculating balances (even if there wasn't such operation)\n");
        }

        private void LogCallback(string arg, TimeSpan span)
        {
            // Можно записывать в бд время исполнения каждой команды и по ним строить метрики.
            Console.WriteLine("Command was executed for " + arg + " in " + span.TotalSeconds * 1000 + "ms");
        }
    }
}

