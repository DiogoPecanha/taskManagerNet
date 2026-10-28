using Microsoft.EntityFrameworkCore;
using TaskManager.Api.Data;
using TaskManager.Api.Models;
using TaskManager.Api.Domain;

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
        if (string.IsNullOrWhiteSpace(name))
        {
            return (null, "Company name is required.", StatusCodes.Status400BadRequest);
        }

        var companyPlan = await _db.Companies
            .AsNoTracking()
            .Where(c => c.Id == companyId)
            .Select(c => (SubscriptionPlan?)c.SubscriptionPlan)
            .FirstOrDefaultAsync(cancellationToken);

        if (companyPlan is null)
        {
            return (null, $"Company '{companyId}' was not found.", StatusCodes.Status404NotFound);
        }

        var maxProjects = companyPlan.Value.GetMaxProjects();

        if(maxProjects is not null)
        {
            var currentCount = await _db.Projects
                .CountAsync(p => p.CompanyId == companyId, cancellationToken);

            if (currentCount >= maxProjects)
                return (null, $"Project limit reached for plan '{companyPlan}'.", StatusCodes.Status409Conflict);   
        }

        var project = await SaveAsync(new Project
        {
            CompanyId = companyId,
            Name = name.Trim()
        }, 
        cancellationToken);

        return (project, null, StatusCodes.Status201Created);
        
    }

    public async Task<(TaskItem? Task, string? Error, int StatusCode)> CreateTaskAsync(
        Guid projectId, string title, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(title))
        {
            return (null, "Task name is required.", StatusCodes.Status400BadRequest);
        }

        var projectPlan = await _db.Projects
            .AsNoTracking()
            .Where(p => p.Id == projectId)
            .Select(p => p.Company!.SubscriptionPlan)
            .FirstOrDefaultAsync(cancellationToken);

        if (projectPlan is null)
        {
            return (null, $"Project '{projectId}' was not found.", StatusCodes.Status404NotFound);
        }

        var maxTasks = projectPlan.Value.GetMaxTasksPerProject();

        if(maxTasks is not null)
        {
            var currentCount = await _db.TaskItem
            .CountAsync(t => t.ProjectId == projectId, cancellationToken);

            if (currentCount >= maxTasks)
                return (null, $"Task limit of {maxTasks.Value} reached for plan '{projectPlan}'.", StatusCodes.Status409Conflict);   
        }

        var task = await SaveAsync(new TaskItem
        {
            ProjectId = projectId,
            Title = title.Trim()
        }, 
        cancellationToken);

        return (task, null, StatusCodes.Status201Created);
    }

    private async Task<T> SaveAsync<T>(T entity, CancellationToken cancellationToken)
        where T : class
    {
        _db.Add(entity);
        await _db.SaveChangesAsync(cancellationToken);
        return entity;
    }


    // Add private helpers or domain exceptions if they improve clarity.
}
