
using System;
using System.Collections.Generic;
using System.Text;
using TaskManagement.Application.DTOs;
namespace TaskManagement.Application.Interfaces;

public interface ITaskService
{
    Task<IEnumerable<TaskDto>> GetTasksAsync();
    Task<TaskDto> CreateTaskAsync(CreateTaskRequest request);
    Task CompleteTaskAsync(Guid id);
}