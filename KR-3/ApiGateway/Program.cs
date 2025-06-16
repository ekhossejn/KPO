var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "API Gateway",
        Version = "v1",
        Description = "API Gateway for Microservices Payment System"
    });
    var xmlFilename = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFilename);
    if (File.Exists(xmlPath))
    {
        options.IncludeXmlComments(xmlPath);
    }
});

builder.Services.AddHttpClient("OrdersService", client =>
{
    client.BaseAddress = new Uri("http://orders-service");
});

builder.Services.AddHttpClient("PaymentsService", client =>
{
    client.BaseAddress = new Uri("http://payments-service");
});

builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

var app = builder.Build();
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "API Gateway");
    c.RoutePrefix = "swagger";
});
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.MapReverseProxy();
app.Run();
