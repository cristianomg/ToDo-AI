using MediatR;
using Microsoft.AspNetCore.Mvc;
using Todo.Application.Commands;
using Todo.Application.Queries;
using ToDo.Domain.Enums;

namespace Todo.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TasksController : ControllerBase
{
    private readonly IMediator _mediator;

    public TasksController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("type/{type}")]
    public async Task<IActionResult> GetAllByType([FromRoute]TaskType type, [FromQuery] DateTime date, [FromHeader(Name = "X-User-Id")] int userId)
    {
        var query = new GetAllByTypeQuery { Type = type, Date = date, UserId = userId };
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id, [FromHeader(Name = "X-User-Id")] int userId)
    {
        var query = new GetTaskByIdQuery { TaskId = id, UserId = userId };
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateTaskCommand command, [FromHeader(Name = "X-User-Id")] int userId)
    {
        command.UserId = userId;
        var result = await _mediator.Send(command);
        
        return Ok(result);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateTaskCommand command, [FromHeader(Name = "X-User-Id")] int userId)
    {
        command.UserId = userId;
        command.TaskId = id;
        var result = await _mediator.Send(command);
        return Ok(result);
    }
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id, [FromHeader(Name = "X-User-Id")] int userId)
    {
        var command = new DeleteTaskCommand
        {
            TaskId = id,
            UserId = userId
        };
        await _mediator.Send(command);
        return Ok();
    }
} 