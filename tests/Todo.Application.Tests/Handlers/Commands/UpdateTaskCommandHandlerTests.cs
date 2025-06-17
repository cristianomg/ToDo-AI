using AutoMapper;
using Moq;
using Todo.Application.Commands;
using Todo.Application.Handlers.Commands;
using ToDo.Domain.DTOs;
using ToDo.Domain.Entities;
using ToDo.Domain.Repositories;
using ToDo.Domain.Enums;
using Xunit;

namespace Todo.Application.Tests.Handlers.Commands
{
    public class UpdateTaskCommandHandlerTests
    {
        private readonly Mock<ITaskRepository> _mockTaskRepository;
        private readonly Mock<IMapper> _mockMapper;
        private readonly UpdateTaskCommandHandler _handler;

        public UpdateTaskCommandHandlerTests()
        {
            _mockTaskRepository = new Mock<ITaskRepository>();
            _mockMapper = new Mock<IMapper>();
            _handler = new UpdateTaskCommandHandler(_mockTaskRepository.Object, _mockMapper.Object);
        }

        private Tasks CreateTask(int id, int userId)
        {
            var task = new Tasks("Original Title", "Original Description", DateTime.Now, TaskPriority.Medium, TaskType.Daily, userId);
            typeof(Tasks).GetProperty("Id")!.SetValue(task, id);
            return task;
        }

        [Fact]
        public async Task Handle_ValidRequest_ShouldUpdateAllProperties()
        {
            // Arrange
            var taskId = 1;
            var userId = 1;
            var task = CreateTask(taskId, userId);
            var command = new UpdateTaskCommand
            {
                TaskId = taskId,
                UserId = userId,
                Title = "Updated Title",
                Description = "Updated Description",
                Status = TasksStatus.InProgress
            };

            var expectedDto = new TaskDto
            {
                Id = taskId,
                Title = "Updated Title",
                Description = "Updated Description",
                Status = TasksStatus.InProgress
            };

            _mockTaskRepository.Setup(r => r.GetByIdAsync(taskId))
                .ReturnsAsync(task);
            _mockTaskRepository.Setup(r => r.SaveChangesAsync())
                .ReturnsAsync(1);
            _mockMapper.Setup(m => m.Map<TaskDto>(task))
                .Returns(expectedDto);

            // Act
            var result = await _handler.Handle(command, default);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expectedDto.Title, result.Title);
            Assert.Equal(expectedDto.Description, result.Description);
            Assert.Equal(expectedDto.Status, result.Status);
            Assert.Equal("Updated Title", task.Title);
            Assert.Equal("Updated Description", task.Description);
            Assert.Equal(TasksStatus.InProgress, task.Status);
            _mockTaskRepository.Verify(r => r.GetByIdAsync(taskId), Times.Once);
            _mockTaskRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
            _mockMapper.Verify(m => m.Map<TaskDto>(task), Times.Once);
        }

        [Fact]
        public async Task Handle_UpdateOnlyTitle_ShouldUpdateOnlyTitle()
        {
            // Arrange
            var taskId = 1;
            var userId = 1;
            var task = CreateTask(taskId, userId);
            var originalDescription = task.Description;
            var originalStatus = task.Status;
            var command = new UpdateTaskCommand
            {
                TaskId = taskId,
                UserId = userId,
                Title = "Updated Title"
            };

            var expectedDto = new TaskDto
            {
                Id = taskId,
                Title = "Updated Title",
                Description = originalDescription,
                Status = originalStatus
            };

            _mockTaskRepository.Setup(r => r.GetByIdAsync(taskId))
                .ReturnsAsync(task);
            _mockTaskRepository.Setup(r => r.SaveChangesAsync())
                .ReturnsAsync(1);
            _mockMapper.Setup(m => m.Map<TaskDto>(task))
                .Returns(expectedDto);

            // Act
            var result = await _handler.Handle(command, default);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Updated Title", task.Title);
            Assert.Equal(originalDescription, task.Description);
            Assert.Equal(originalStatus, task.Status);
            _mockTaskRepository.Verify(r => r.GetByIdAsync(taskId), Times.Once);
            _mockTaskRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
            _mockMapper.Verify(m => m.Map<TaskDto>(task), Times.Once);
        }

        [Fact]
        public async Task Handle_UpdateOnlyDescription_ShouldUpdateOnlyDescription()
        {
            // Arrange
            var taskId = 1;
            var userId = 1;
            var task = CreateTask(taskId, userId);
            var originalTitle = task.Title;
            var originalStatus = task.Status;
            var command = new UpdateTaskCommand
            {
                TaskId = taskId,
                UserId = userId,
                Description = "Updated Description"
            };

            var expectedDto = new TaskDto
            {
                Id = taskId,
                Title = originalTitle,
                Description = "Updated Description",
                Status = originalStatus
            };

            _mockTaskRepository.Setup(r => r.GetByIdAsync(taskId))
                .ReturnsAsync(task);
            _mockTaskRepository.Setup(r => r.SaveChangesAsync())
                .ReturnsAsync(1);
            _mockMapper.Setup(m => m.Map<TaskDto>(task))
                .Returns(expectedDto);

            // Act
            var result = await _handler.Handle(command, default);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(originalTitle, task.Title);
            Assert.Equal("Updated Description", task.Description);
            Assert.Equal(originalStatus, task.Status);
            _mockTaskRepository.Verify(r => r.GetByIdAsync(taskId), Times.Once);
            _mockTaskRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
            _mockMapper.Verify(m => m.Map<TaskDto>(task), Times.Once);
        }

        [Fact]
        public async Task Handle_UpdateOnlyStatus_ShouldUpdateOnlyStatus()
        {
            // Arrange
            var taskId = 1;
            var userId = 1;
            var task = CreateTask(taskId, userId);
            var originalTitle = task.Title;
            var originalDescription = task.Description;
            var command = new UpdateTaskCommand
            {
                TaskId = taskId,
                UserId = userId,
                Status = TasksStatus.Completed
            };

            var expectedDto = new TaskDto
            {
                Id = taskId,
                Title = originalTitle,
                Description = originalDescription,
                Status = TasksStatus.Completed
            };

            _mockTaskRepository.Setup(r => r.GetByIdAsync(taskId))
                .ReturnsAsync(task);
            _mockTaskRepository.Setup(r => r.SaveChangesAsync())
                .ReturnsAsync(1);
            _mockMapper.Setup(m => m.Map<TaskDto>(task))
                .Returns(expectedDto);

            // Act
            var result = await _handler.Handle(command, default);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(originalTitle, task.Title);
            Assert.Equal(originalDescription, task.Description);
            Assert.Equal(TasksStatus.Completed, task.Status);
            _mockTaskRepository.Verify(r => r.GetByIdAsync(taskId), Times.Once);
            _mockTaskRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
            _mockMapper.Verify(m => m.Map<TaskDto>(task), Times.Once);
        }

        [Fact]
        public async Task Handle_UpdateWithNullValues_ShouldNotUpdateProperties()
        {
            // Arrange
            var taskId = 1;
            var userId = 1;
            var task = CreateTask(taskId, userId);
            var originalTitle = task.Title;
            var originalDescription = task.Description;
            var originalStatus = task.Status;
            var command = new UpdateTaskCommand
            {
                TaskId = taskId,
                UserId = userId,
                Title = null,
                Description = null,
                Status = null
            };

            var expectedDto = new TaskDto
            {
                Id = taskId,
                Title = originalTitle,
                Description = originalDescription,
                Status = originalStatus
            };

            _mockTaskRepository.Setup(r => r.GetByIdAsync(taskId))
                .ReturnsAsync(task);
            _mockTaskRepository.Setup(r => r.SaveChangesAsync())
                .ReturnsAsync(1);
            _mockMapper.Setup(m => m.Map<TaskDto>(task))
                .Returns(expectedDto);

            // Act
            var result = await _handler.Handle(command, default);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(originalTitle, task.Title);
            Assert.Equal(originalDescription, task.Description);
            Assert.Equal(originalStatus, task.Status);
            _mockTaskRepository.Verify(r => r.GetByIdAsync(taskId), Times.Once);
            _mockTaskRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
            _mockMapper.Verify(m => m.Map<TaskDto>(task), Times.Once);
        }

        [Fact]
        public async Task Handle_TaskNotFound_ShouldThrowKeyNotFoundException()
        {
            // Arrange
            var command = new UpdateTaskCommand { TaskId = 1, UserId = 1 };
            _mockTaskRepository.Setup(r => r.GetByIdAsync(command.TaskId))
                .ReturnsAsync((Tasks?)null);

            // Act & Assert
            var ex = await Assert.ThrowsAsync<KeyNotFoundException>(() => _handler.Handle(command, default));
            Assert.Equal($"Task with ID {command.TaskId} not found", ex.Message);
            _mockTaskRepository.Verify(r => r.GetByIdAsync(command.TaskId), Times.Once);
            _mockTaskRepository.Verify(r => r.SaveChangesAsync(), Times.Never);
            _mockMapper.Verify(m => m.Map<TaskDto>(It.IsAny<Tasks>()), Times.Never);
        }

        [Fact]
        public async Task Handle_UnauthorizedUser_ShouldThrowUnauthorizedAccessException()
        {
            // Arrange
            var taskId = 1;
            var userId = 2;
            var command = new UpdateTaskCommand { TaskId = taskId, UserId = 99 };
            var task = CreateTask(taskId, userId);
            _mockTaskRepository.Setup(r => r.GetByIdAsync(taskId))
                .ReturnsAsync(task);

            // Act & Assert
            var ex = await Assert.ThrowsAsync<UnauthorizedAccessException>(() => _handler.Handle(command, default));
            Assert.Equal("You can only update your own tasks", ex.Message);
            _mockTaskRepository.Verify(r => r.GetByIdAsync(taskId), Times.Once);
            _mockTaskRepository.Verify(r => r.SaveChangesAsync(), Times.Never);
            _mockMapper.Verify(m => m.Map<TaskDto>(It.IsAny<Tasks>()), Times.Never);
        }

        [Fact]
        public async Task Handle_RepositoryThrowsException_ShouldPropagate()
        {
            // Arrange
            var taskId = 1;
            var userId = 1;
            var task = CreateTask(taskId, userId);
            var command = new UpdateTaskCommand { TaskId = taskId, UserId = userId, Title = "Updated Title" };
            var expectedException = new InvalidOperationException("Database error");

            _mockTaskRepository.Setup(r => r.GetByIdAsync(taskId))
                .ReturnsAsync(task);
            _mockTaskRepository.Setup(r => r.SaveChangesAsync())
                .ThrowsAsync(expectedException);

            // Act & Assert
            var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => _handler.Handle(command, default));
            Assert.Same(expectedException, ex);
            _mockTaskRepository.Verify(r => r.GetByIdAsync(taskId), Times.Once);
            _mockTaskRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
            _mockMapper.Verify(m => m.Map<TaskDto>(It.IsAny<Tasks>()), Times.Never);
        }

        [Fact]
        public async Task Handle_MapperThrowsException_ShouldPropagate()
        {
            // Arrange
            var taskId = 1;
            var userId = 1;
            var task = CreateTask(taskId, userId);
            var command = new UpdateTaskCommand { TaskId = taskId, UserId = userId, Title = "Updated Title" };
            var expectedException = new AutoMapperMappingException("Mapping error");

            _mockTaskRepository.Setup(r => r.GetByIdAsync(taskId))
                .ReturnsAsync(task);
            _mockTaskRepository.Setup(r => r.SaveChangesAsync())
                .ReturnsAsync(1);
            _mockMapper.Setup(m => m.Map<TaskDto>(task))
                .Throws(expectedException);

            // Act & Assert
            var ex = await Assert.ThrowsAsync<AutoMapperMappingException>(() => _handler.Handle(command, default));
            Assert.Same(expectedException, ex);
            Assert.Equal("Updated Title", task.Title); // Should still be updated
            _mockTaskRepository.Verify(r => r.GetByIdAsync(taskId), Times.Once);
            _mockTaskRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
            _mockMapper.Verify(m => m.Map<TaskDto>(task), Times.Once);
        }

        [Fact]
        public async Task Handle_NullTitle_ShouldNotUpdateTitle()
        {
            // Arrange
            var taskId = 1;
            var userId = 1;
            var task = CreateTask(taskId, userId);
            var originalTitle = task.Title;
            var command = new UpdateTaskCommand { TaskId = taskId, UserId = userId, Title = null };

            var expectedDto = new TaskDto
            {
                Id = taskId,
                Title = originalTitle,
                Description = task.Description,
                Status = task.Status
            };

            _mockTaskRepository.Setup(r => r.GetByIdAsync(taskId))
                .ReturnsAsync(task);
            _mockTaskRepository.Setup(r => r.SaveChangesAsync())
                .ReturnsAsync(1);
            _mockMapper.Setup(m => m.Map<TaskDto>(task))
                .Returns(expectedDto);

            // Act
            var result = await _handler.Handle(command, default);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(originalTitle, task.Title); // Title should remain unchanged
            Assert.Equal(originalTitle, result.Title);
            _mockTaskRepository.Verify(r => r.GetByIdAsync(taskId), Times.Once);
            _mockTaskRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
            _mockMapper.Verify(m => m.Map<TaskDto>(task), Times.Once);
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        public async Task Handle_EmptyTitle_ShouldThrowArgumentException(string title)
        {
            // Arrange
            var taskId = 1;
            var userId = 1;
            var task = CreateTask(taskId, userId);
            var command = new UpdateTaskCommand { TaskId = taskId, UserId = userId, Title = title };

            _mockTaskRepository.Setup(r => r.GetByIdAsync(taskId))
                .ReturnsAsync(task);

            // Act & Assert
            var ex = await Assert.ThrowsAsync<ArgumentException>(() => _handler.Handle(command, default));
            Assert.Equal("Title cannot be empty (Parameter 'title')", ex.Message);
            _mockTaskRepository.Verify(r => r.GetByIdAsync(taskId), Times.Once);
            _mockTaskRepository.Verify(r => r.SaveChangesAsync(), Times.Never);
            _mockMapper.Verify(m => m.Map<TaskDto>(It.IsAny<Tasks>()), Times.Never);
        }

        [Fact]
        public async Task Handle_UpdateStatusToCompleted_ShouldUpdateStatus()
        {
            // Arrange
            var taskId = 1;
            var userId = 1;
            var task = CreateTask(taskId, userId);
            var command = new UpdateTaskCommand
            {
                TaskId = taskId,
                UserId = userId,
                Status = TasksStatus.Completed
            };

            var expectedDto = new TaskDto
            {
                Id = taskId,
                Status = TasksStatus.Completed
            };

            _mockTaskRepository.Setup(r => r.GetByIdAsync(taskId))
                .ReturnsAsync(task);
            _mockTaskRepository.Setup(r => r.SaveChangesAsync())
                .ReturnsAsync(1);
            _mockMapper.Setup(m => m.Map<TaskDto>(task))
                .Returns(expectedDto);

            // Act
            var result = await _handler.Handle(command, default);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(TasksStatus.Completed, task.Status);
            Assert.Equal(TasksStatus.Completed, result.Status);
            _mockTaskRepository.Verify(r => r.GetByIdAsync(taskId), Times.Once);
            _mockTaskRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
            _mockMapper.Verify(m => m.Map<TaskDto>(task), Times.Once);
        }

        [Fact]
        public async Task Handle_UpdateStatusToInProgress_ShouldUpdateStatus()
        {
            // Arrange
            var taskId = 1;
            var userId = 1;
            var task = CreateTask(taskId, userId);
            var command = new UpdateTaskCommand
            {
                TaskId = taskId,
                UserId = userId,
                Status = TasksStatus.InProgress
            };

            var expectedDto = new TaskDto
            {
                Id = taskId,
                Status = TasksStatus.InProgress
            };

            _mockTaskRepository.Setup(r => r.GetByIdAsync(taskId))
                .ReturnsAsync(task);
            _mockTaskRepository.Setup(r => r.SaveChangesAsync())
                .ReturnsAsync(1);
            _mockMapper.Setup(m => m.Map<TaskDto>(task))
                .Returns(expectedDto);

            // Act
            var result = await _handler.Handle(command, default);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(TasksStatus.InProgress, task.Status);
            Assert.Equal(TasksStatus.InProgress, result.Status);
            _mockTaskRepository.Verify(r => r.GetByIdAsync(taskId), Times.Once);
            _mockTaskRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
            _mockMapper.Verify(m => m.Map<TaskDto>(task), Times.Once);
        }
    }
} 