using Microsoft.EntityFrameworkCore;
using OrdersService.Data;
using OrdersService.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<OrdersDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("OrdersDb")));
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddSingleton<IMessageService, MessageService>();
builder.Services.AddHostedService<OutboxProcessorService>();
var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI(c => 
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Orders Service API v1");
    c.RoutePrefix = "swagger";
});
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
await InitializeDatabaseAndMessagingAsync(app);
app.Run();

static async Task InitializeDatabaseAndMessagingAsync(WebApplication app)
{
    using var scope = app.Services.CreateScope();
    var dbContext = scope.ServiceProvider.GetRequiredService<OrdersDbContext>();
    dbContext.Database.Migrate();
    var messageService = scope.ServiceProvider.GetRequiredService<IMessageService>();
    await messageService.StartConsumingPaymentResultsAsync();
}

app.Run();


public class OutboxProcessorService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<OutboxProcessorService> _logger;
    public OutboxProcessorService(
        IServiceProvider serviceProvider,
        ILogger<OutboxProcessorService> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Outbox Processor Service is starting");
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = _serviceProvider.CreateScope();
                var messageService = scope.ServiceProvider.GetRequiredService<IMessageService>();
                await messageService.ProcessOutboxMessagesAsync(stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred in Outbox Processor Service");
                await Task.Delay(5000, stoppingToken);
            }
        }
    }
}