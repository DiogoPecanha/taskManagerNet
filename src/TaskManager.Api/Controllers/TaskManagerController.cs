using Microsoft.AspNetCore.Mvc;
using TaskManager.Api.Services;
using TaskManager.Api.Contracts;

namespace TaskManager.Api.Controllers;


[ApiController]
[Route("api")]
public sealed class TaskManagerController : ControllerBase
{
    private readonly SubscriptionService _service;

    public TaskManagerController(SubscriptionService service)
    {
        _service = service;
    }

    [HttpPost("companies/{companyId:guid}/projects")]
    public async Task<IActionResult> CreateProject(
        Guid companyId, CreateProjectRequest request, CancellationToken cancellationToken)
    {
        var (project, error, statusCode) =
            await _service.CreateProjectAsync(companyId, request.Name, cancellationToken);

        if (project is null)
            return Problem(detail: error, statusCode: statusCode);

        return Created($"/api/projects/{project.Id}", project);
    }

    [HttpPost("projects/{projectId:guid}/tasks")]
    public async Task<IActionResult> CreateTask(
        Guid projectId, CreateTaskRequest request, CancellationToken cancellationToken)
    {
        var (task, error, statusCode) =
            await _service.CreateTaskAsync(projectId, request.Title, cancellationToken);

        if (task is null)
            return Problem(detail: error, statusCode: statusCode);

        return Created($"/api/tasks/{task.Id, task}");
    }
}