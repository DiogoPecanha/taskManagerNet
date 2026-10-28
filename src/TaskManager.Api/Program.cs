using Microsoft.EntityFrameworkCore;
using TaskManager.Api.Contracts;
using TaskManager.Api.Data;
using TaskManager.Api.Models;
using TaskManager.Api.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<TaskManagerDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("Default")));
builder.Services.AddScoped<SubscriptionService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapGet("/health", () => Results.Ok(new { status = "healthy" }))
   .WithName("HealthCheck")
   .WithOpenApi();

app.MapGet("/api/companies", async (TaskManagerDbContext db, CancellationToken ct) =>
    Results.Ok(await db.Companies.AsNoTracking()
        .Select(c => new { c.Id, c.Name, c.SubscriptionPlan, c.SubscriptionType })
        .ToListAsync(ct)))
    .WithName("ListCompanies").WithOpenApi();

app.MapGet("/api/companies/{companyId:guid}/projects", async (
    Guid companyId, TaskManagerDbContext db, CancellationToken ct) =>
{
    var companyExists = await db.Companies.AnyAsync(c => c.Id == companyId, ct);
    if (!companyExists) return Results.NotFound(new { message = "Company not found." });

    var projects = await db.Projects.AsNoTracking()
        .Where(p => p.CompanyId == companyId)
        .Select(p => new { p.Id, p.CompanyId, p.Name, TaskCount = p.Tasks.Count })
        .ToListAsync(ct);
    return Results.Ok(projects);
}).WithName("ListProjects").WithOpenApi();

app.MapPost("/api/companies/{companyId:guid}/projects", async (
    Guid companyId, CreateProjectRequest request, SubscriptionService service, CancellationToken ct) =>
{
    if (string.IsNullOrWhiteSpace(request.Name))
        return Results.BadRequest(new { message = "Project name is required." });

    var result = await service.CreateProjectAsync(companyId, request.Name.Trim(), ct);
    if (result.Project is not null)
        return Results.Created($"/api/companies/{companyId}/projects/{result.Project.Id}",
            new { result.Project.Id, result.Project.CompanyId, result.Project.Name });

    return Results.Json(new { message = result.Error }, statusCode: result.StatusCode);
}).WithName("CreateProject").WithOpenApi();

app.MapGet("/api/projects/{projectId:guid}/tasks", async (
    Guid projectId, TaskManagerDbContext db, CancellationToken ct) =>
{
    var projectExists = await db.Projects.AnyAsync(p => p.Id == projectId, ct);
    if (!projectExists) return Results.NotFound(new { message = "Project not found." });

    var tasks = await db.Tasks.AsNoTracking()
        .Where(t => t.ProjectId == projectId)
        .Select(t => new { t.Id, t.ProjectId, t.Title, t.IsCompleted })
        .ToListAsync(ct);
    return Results.Ok(tasks);
}).WithName("ListTasks").WithOpenApi();

app.MapPost("/api/projects/{projectId:guid}/tasks", async (
    Guid projectId, CreateTaskRequest request, SubscriptionService service, CancellationToken ct) =>
{
    if (string.IsNullOrWhiteSpace(request.Title))
        return Results.BadRequest(new { message = "Task title is required." });

    var result = await service.CreateTaskAsync(projectId, request.Title.Trim(), ct);
    if (result.Task is not null)
        return Results.Created($"/api/projects/{projectId}/tasks/{result.Task.Id}",
            new { result.Task.Id, result.Task.ProjectId, result.Task.Title, result.Task.IsCompleted });

    return Results.Json(new { message = result.Error }, statusCode: result.StatusCode);
}).WithName("CreateTask").WithOpenApi();

await InitializeDatabaseAsync(app);
app.Run();

static async Task InitializeDatabaseAsync(WebApplication app)
{
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<TaskManagerDbContext>();
    await db.Database.EnsureCreatedAsync();

    if (await db.Companies.AnyAsync()) return;

    var freemium = new Company
    {
        Id = Guid.Parse("11111111-1111-1111-1111-111111111111"),
        Name = "Acme Freemium",
        SubscriptionPlan = SubscriptionPlan.Freemium,
        SubscriptionType = SubscriptionType.None
    };
    var basic = new Company
    {
        Id = Guid.Parse("22222222-2222-2222-2222-222222222222"),
        Name = "Acme Basic",
        SubscriptionPlan = SubscriptionPlan.Basic,
        SubscriptionType = SubscriptionType.Monthly
    };
    var advanced = new Company
    {
        Id = Guid.Parse("33333333-3333-3333-3333-333333333333"),
        Name = "Acme Advanced",
        SubscriptionPlan = SubscriptionPlan.Advanced,
        SubscriptionType = SubscriptionType.Annual
    };

    var freemiumProject = new Project
    {
        Id = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"),
        CompanyId = freemium.Id,
        Name = "Projeto de demonstração"
    };
    db.Companies.AddRange(freemium, basic, advanced);
    db.Projects.Add(freemiumProject);
    await db.SaveChangesAsync();
}

public partial class Program { }
