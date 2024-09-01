using DatapacTechnicalAssignment.Jobs;
using DatapacTechnicalAssignment.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Quartz;

var builder = WebApplication.CreateBuilder(args);

// Pridanie služieb do kontajnera
builder.Services.AddControllers();

// Nastavenie Entity Framework Core s in-memory databázou
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseInMemoryDatabase("InMemoryDb"));

// Nastavenie Quartz.NET pre plánovanie úloh
builder.Services.AddQuartz(q =>
{
    q.UseMicrosoftDependencyInjectionJobFactory();

    // Definovanie úlohy a jej spúšťania
    var jobKey = new JobKey("ReminderJob");
    q.AddJob<ReminderJob>(opts => opts.WithIdentity(jobKey));

    q.AddTrigger(opts => opts
        .ForJob(jobKey)
        .WithIdentity("ReminderJob-trigger")
        .WithCronSchedule("0 * * * * ?")); // Cron schedule: každú minútu pre testovacie účely
});

builder.Services.AddQuartzHostedService(q => q.WaitForJobsToComplete = true);

// Pridanie Swagger/OpenAPI na dokumentáciu API
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Library Management API", Version = "v1" });
});

var app = builder.Build();

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

app.Run();
