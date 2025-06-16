using Microsoft.EntityFrameworkCore;
using PaymentsService.Data;
using PaymentsService.Services;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<PaymentsDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("PaymentsDb")));
builder.Services.AddDbContextFactory<PaymentsDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("PaymentsDb")));
builder.Services.AddScoped<IAccountService, AccountService>();
builder.Services.AddSingleton<IMessageService, MessageService>();
builder.Services.AddHostedService<OutboxProcessorService>();
builder.Services.AddHostedService<InboxProcessorService>();
var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI(c => 
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Payments Service API v1");
    c.RoutePrefix = "swagger";
});

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<PaymentsDbContext>();
    dbContext.Database.Migrate();
    var messageService = scope.ServiceProvider.GetRequiredService<IMessageService>();
    var accountService = scope.ServiceProvider.GetRequiredService<IAccountService>();
    
    if (messageService is MessageService ms)
    {
        ms.SetAccountService(accountService);
        ms.StartConsumingPaymentRequests();
    }
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

public class InboxProcessorService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<InboxProcessorService> _logger;

    public InboxProcessorService(
        IServiceProvider serviceProvider,
        ILogger<InboxProcessorService> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Inbox Processor Service is starting");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = _serviceProvider.CreateScope();
                var messageService = scope.ServiceProvider.GetRequiredService<IMessageService>();
                await messageService.ProcessInboxMessagesAsync(stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred in Inbox Processor Service");
                await Task.Delay(5000, stoppingToken);
            }
        }
    }
}
