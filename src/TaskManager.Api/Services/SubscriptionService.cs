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

    public static class PlanLimits
    {
        private static readonly IReadOnlyDictionary<CompanyPlan, int?> MaxProjects =
            new Dictionary<CompanyPlan, int?>
            {
                [CompanyPlan.Freemium] = 1,
                [CompanyPlan.Basic] = 10,
                [CompanyPlan.Advanced] = null
            };

        public static int? GetMaxProjects(CompanyPlan plan) =>
            MaxProjects.TryGetValue(plan, out var limit)
                ? limit
                : throw new ArgumentOutOfRangeException(nameof(plan), plan, "Unknown plan.");
    }

    public async Task<(Project? Project, string? Error, int StatusCode)> CreateProjectAsync(
        Guid companyId, string name, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw;
        }

        var company = await _db.Companies
            .AsNoTracking()
            .Where(c => c.Id == companyId)
            .Select(c => new { c.Id, c.SubscriptionPlan })
            .FirstOrDefaultAsync(cancellationToken);

        if (company is null)
        {
            throw;
        }

        var maxProjects = PlanLimits.GetMaxProjects(company.SubscriptionPlan);
        
        var currentCount = await _db.Projects
            .CountAsync(p => p.CompanyId == companyId, cancellationToken);

        if (currentCount >= maxProjects.Value)
            return (null, $"Project limit reached for plan '{company.Plan}' ({maxProjects.Value}).", StatusCodes.Status409Conflict);

        await SaveProjectAsync();
        
        
        // TODO: Load the company, return 404 when missing, count its projects,
        // enforce Freemium=1, Basic=10, Advanced=unlimited, then persist the project.
        await Task.CompletedTask;
        return (null, "Project creation is not implemented yet.", StatusCodes.Status501NotImplemented);
    }

    public async Task<int>? SaveProjectAsync()
    {
        var project = new Project
        {
            Id = Guid.NewGuid(),
            CompanyId = companyId,
            Name = trimmedName,
            CreatedAt = DateTime.UtcNow
        };

        _db.Projects.Add(project);

        await _dbContext.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);  
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
