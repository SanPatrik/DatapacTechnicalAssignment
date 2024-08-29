using DatapacTechnicalAssignment.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Quartz;

var builder = WebApplication.CreateBuilder(args);

// 1. Pridanie služieb do kontajnera
builder.Services.AddControllers();

// 2. Nastavenie Entity Framework Core s in-memory databázou
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseInMemoryDatabase("InMemoryDb"));

// 3. Nastavenie Quartz.NET
builder.Services.AddQuartz(q =>
{
    q.UseMicrosoftDependencyInjectionJobFactory();

    // Definovanie úlohy a jej spúšťania
    var jobKey = new JobKey("ReminderJob");
    q.AddJob<ReminderJob>(opts => opts.WithIdentity(jobKey));

    q.AddTrigger(opts => opts
        .ForJob(jobKey)
        .WithIdentity("ReminderJob-trigger")
        .WithCronSchedule("0 0 9 * * ?")); // Cron schedule: každý deň o 9:00
});

builder.Services.AddQuartzHostedService(q => q.WaitForJobsToComplete = true);

// 4. Pridanie Swagger/OpenAPI na dokumentáciu API
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Library Management API", Version = "v1" });
});

var app = builder.Build();

// 5. Nastavenie middleware

// Použitie Swaggeru v developmente
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Library Management API v1");
    });
}

// Pridanie HTTPS redirection a logovania požiadaviek
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

// Inicializácia databázy s mock dátami
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    SeedDatabase(dbContext);  // Zavolanie metódy na inicializáciu dát
}

app.Run();

// Definícia metódy na Seed databázy
void SeedDatabase(ApplicationDbContext context)
{
    context.Users.AddRange(
        new User { Name = "John Doe", Email = "john@example.com" },
        new User { Name = "Jane Doe", Email = "jane@example.com" }
    );

    context.Books.AddRange(
        new Book { Title = "1984", Author = "George Orwell" },
        new Book { Title = "Brave New World", Author = "Aldous Huxley" }
    );

    context.SaveChanges();
}

// Definícia Quartz Jobu ako samostatnej triedy
public class ReminderJob : IJob
{
    private readonly ILogger<ReminderJob> _logger;

    public ReminderJob(ILogger<ReminderJob> logger)
    {
        _logger = logger;
    }

    public Task Execute(IJobExecutionContext context)
    {
        // Tu pridáš logiku na odosielanie pripomienok
        _logger.LogInformation("ReminderJob is executing at {time}", DateTimeOffset.Now);
        return Task.CompletedTask;
    }
}
