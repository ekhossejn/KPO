using System.Reflection;
using System.Text.Json.Serialization;
using MediatR;
using Microsoft.OpenApi.Models;
using Zoo.Application.EventHandler;
using Zoo.Application.Interfaces;
using Zoo.Application.Services;
using Zoo.Domain.Entities;
using Zoo.Domain.Events;
using Zoo.Domain.ValueObjects;
using Zoo.Infrastructure.Interfaces;
using Zoo.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddMediatR(a => a.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));

// Zoo.Application.EventHandler
builder.Services.AddTransient<INotificationHandler<AnimalMovedEvent>, AnimalMovedEventHandler>();
builder.Services.AddTransient<INotificationHandler<FeedingTimeEvent>, FeedingTimeEventHandler>();

// Zoo.Infrastructure.Repositories
builder.Services.AddSingleton<IAnimalRepository, AnimalRepository>();
builder.Services.AddSingleton<IEnclosureRepository, EnclosureRepository>();
builder.Services.AddSingleton<IFeedingScheduleRepository, FeedingScheduleRepository>();

// Zoo.Application.Services
builder.Services.AddScoped<IAnimalReleaseService, AnimalReleaseService>();
builder.Services.AddScoped<IAnimalTransferService, AnimalTransferService>();
builder.Services.AddScoped<IFeedingOrganizationService, FeedingOrganizationService>();
builder.Services.AddScoped<IZooStatisticsService, ZooStatisticsService>();

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "Animal API", Version = "v1" });

    // Автоматическое отображение перечислений
    options.SchemaFilter<EnumSchemaFilter>();
});

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

// Zoo.Presentation.Controllers
app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var animals = scope.ServiceProvider.GetRequiredService<IAnimalRepository>();
    var enclosures = scope.ServiceProvider.GetRequiredService<IEnclosureRepository>();

    var aqua = new Enclosure(EnclosureType.ForAquarium, new PositiveInt(2), new PositiveInt(2));
    var birds = new Enclosure(EnclosureType.ForBirds, new PositiveInt(5), new PositiveInt(5));
    var herb = new Enclosure(EnclosureType.ForHerbivores, new PositiveInt(10), new PositiveInt(3));
    var pred = new Enclosure(EnclosureType.ForPredators, new PositiveInt(10), new PositiveInt(1));

    await enclosures.AddEnclosureAsync(aqua);
    await enclosures.AddEnclosureAsync(birds);
    await enclosures.AddEnclosureAsync(herb);
    await enclosures.AddEnclosureAsync(pred);

    var kate = new Animal(AnimalSpecies.Eagle, "Kate", new DateField("20.07.2005"),
        AnimalGender.Female, "Mouse", AnimalHealthStatus.Healthy, EnclosureType.ForBirds);
    var ilya = new Animal(AnimalSpecies.Horse, "Ilya", new DateField("23.07.2005"),
        AnimalGender.Male, "Autumn grass", AnimalHealthStatus.Healthy, EnclosureType.ForHerbivores);
    var dasha = new Animal(AnimalSpecies.Fox, "Dasha", new DateField("02.02.2006"),
        AnimalGender.Female, "Syrniki", AnimalHealthStatus.Healthy, EnclosureType.ForPredators);
    var misha = new Animal(AnimalSpecies.Fish, "Misha", new DateField("01.01.1900"),
        AnimalGender.Male, "Kroshki", AnimalHealthStatus.Healthy, EnclosureType.ForAquarium);
    await animals.AddAnimalAsync(kate);
    await animals.AddAnimalAsync(ilya);
    await animals.AddAnimalAsync(dasha);
    await animals.AddAnimalAsync(misha);
}

app.Run();

