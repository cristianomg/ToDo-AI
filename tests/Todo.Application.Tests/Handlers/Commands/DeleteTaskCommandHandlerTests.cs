using Moq;
using Todo.Application.Commands;
using Todo.Application.Handlers.Commands;
using ToDo.Domain.Entities;
using ToDo.Domain.Repositories;
using Xunit;

namespace Todo.Application.Tests.Handlers.Commands
{
    public class DeleteTaskCommandHandlerTests
    {
        private readonly Mock<ITaskRepository> _mockTaskRepository;
        private readonly DeleteTaskCommandHandler _handler;

        public DeleteTaskCommandHandlerTests()
        {
            _mockTaskRepository = new Mock<ITaskRepository>();
            _handler = new DeleteTaskCommandHandler(_mockTaskRepository.Object);
        }

        private Tasks CreateTask(int id, int userId)
        {
            var task = new Tasks("Task", "Desc", DateTime.UtcNow, ToDo.Domain.Enums.TaskPriority.Medium, ToDo.Domain.Enums.TaskType.Daily, userId);
            typeof(Tasks).GetProperty("Id")!.SetValue(task, id);
            return task;
        }

        [Fact]
        public async Task Handle_ShouldDeleteTask_WhenValid()
        {
            // Arrange
            var taskId = 1;
            var userId = 2;
            var task = CreateTask(taskId, userId);
            var command = new DeleteTaskCommand { TaskId = taskId, UserId = userId };

            _mockTaskRepository.Setup(r => r.GetByIdAsync(taskId))
                .ReturnsAsync(task);
            _mockTaskRepository.Setup(r => r.DeleteByIdAsync(taskId))
                .Returns(Task.CompletedTask);
            _mockTaskRepository.Setup(r => r.SaveChangesAsync())
                .ReturnsAsync(1);

            // Act
            await _handler.Handle(command, default);

            // Assert
            _mockTaskRepository.Verify(r => r.GetByIdAsync(taskId), Times.Once);
            _mockTaskRepository.Verify(r => r.DeleteByIdAsync(taskId), Times.Once);
            _mockTaskRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task Handle_TaskNotFound_ShouldThrowKeyNotFoundException()
        {
            // Arrange
            var command = new DeleteTaskCommand { TaskId = 1, UserId = 1 };
            _mockTaskRepository.Setup(r => r.GetByIdAsync(command.TaskId))
                .ReturnsAsync((Tasks?)null);

            // Act & Assert
            var ex = await Assert.ThrowsAsync<KeyNotFoundException>(() => _handler.Handle(command, default));
            Assert.Equal($"Task with ID {command.TaskId} not found", ex.Message);
            _mockTaskRepository.Verify(r => r.GetByIdAsync(command.TaskId), Times.Once);
            _mockTaskRepository.Verify(r => r.DeleteByIdAsync(It.IsAny<int>()), Times.Never);
            _mockTaskRepository.Verify(r => r.SaveChangesAsync(), Times.Never);
        }

        [Fact]
        public async Task Handle_UnauthorizedUser_ShouldThrowUnauthorizedAccessException()
        {
            // Arrange
            var taskId = 1;
            var userId = 2;
            var command = new DeleteTaskCommand { TaskId = taskId, UserId = 99 };
            var task = CreateTask(taskId, userId);
            _mockTaskRepository.Setup(r => r.GetByIdAsync(taskId))
                .ReturnsAsync(task);

            // Act & Assert
            var ex = await Assert.ThrowsAsync<UnauthorizedAccessException>(() => _handler.Handle(command, default));
            Assert.Equal($"User {command.UserId} is not authorized to delete task {command.TaskId}", ex.Message);
            _mockTaskRepository.Verify(r => r.GetByIdAsync(taskId), Times.Once);
            _mockTaskRepository.Verify(r => r.DeleteByIdAsync(It.IsAny<int>()), Times.Never);
            _mockTaskRepository.Verify(r => r.SaveChangesAsync(), Times.Never);
        }

        [Fact]
        public async Task Handle_RepositoryThrowsException_ShouldPropagate()
        {
            // Arrange
            var taskId = 1;
            var userId = 2;
            var task = CreateTask(taskId, userId);
            var command = new DeleteTaskCommand { TaskId = taskId, UserId = userId };
            _mockTaskRepository.Setup(r => r.GetByIdAsync(taskId))
                .ReturnsAsync(task);
            _mockTaskRepository.Setup(r => r.DeleteByIdAsync(taskId))
                .ThrowsAsync(new InvalidOperationException("DB error"));

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => _handler.Handle(command, default));
            _mockTaskRepository.Verify(r => r.GetByIdAsync(taskId), Times.Once);
            _mockTaskRepository.Verify(r => r.DeleteByIdAsync(taskId), Times.Once);
            _mockTaskRepository.Verify(r => r.SaveChangesAsync(), Times.Never);
        }

        [Fact]
        public async Task Handle_NullRequest_ShouldThrowArgumentNullException()
        {
            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => _handler.Handle(null!, default));
            _mockTaskRepository.Verify(r => r.GetByIdAsync(It.IsAny<int>()), Times.Never);
            _mockTaskRepository.Verify(r => r.DeleteByIdAsync(It.IsAny<int>()), Times.Never);
            _mockTaskRepository.Verify(r => r.SaveChangesAsync(), Times.Never);
        }
    }
} 