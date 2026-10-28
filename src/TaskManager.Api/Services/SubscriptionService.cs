using Microsoft.EntityFrameworkCore;
using TaskManager.Api.Data;
using TaskManager.Api.Models;

namespace TaskManager.Api.Services;

/// <summary>
/// Implement the subscription limits here. Keep the business rules out of the HTTP endpoints.
/// </summary>
public sealed class SubscriptionService
{
    private readonly TaskManagerDbContext _db;

    public SubscriptionService(TaskManagerDbContext db)
    {
        _db = db;
    }

    public async Task<(Project? Project, string? Error, int StatusCode)> CreateProjectAsync(
        Guid companyId, string name, CancellationToken cancellationToken = default)
    {
        // TODO: Load the company, return 404 when missing, count its projects,
        // enforce Freemium=1, Basic=10, Advanced=unlimited, then persist the project.
        await Task.CompletedTask;
        return (null, "Project creation is not implemented yet.", StatusCodes.Status501NotImplemented);
    }

    public async Task<(TaskItem? Task, string? Error, int StatusCode)> CreateTaskAsync(
        Guid projectId, string title, CancellationToken cancellationToken = default)
    {
        // TODO: Load project and company/plan, return 404 when missing,
        // enforce Freemium=20 tasks per project; Basic and Advanced are unlimited,
        // then persist the task.
        await Task.CompletedTask;
        return (null, "Task creation is not implemented yet.", StatusCodes.Status501NotImplemented);
    }

    // Add private helpers or domain exceptions if they improve clarity.
}
