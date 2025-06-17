using Moq;
using Todo.Application.Commands;
using Todo.Application.Handlers.Commands;
using ToDo.Domain.Entities;
using ToDo.Domain.Repositories;
using Xunit;

namespace Todo.Application.Tests.Handlers.Commands
{
    public class DeleteChecklistItemCommandHandlerTests
    {
        private readonly Mock<ITaskRepository> _mockTaskRepository;
        private readonly DeleteChecklistItemCommandHandler _handler;

        public DeleteChecklistItemCommandHandlerTests()
        {
            _mockTaskRepository = new Mock<ITaskRepository>();
            _handler = new DeleteChecklistItemCommandHandler(_mockTaskRepository.Object);
        }

        private Tasks CreateTask(int id, int userId, List<ChecklistItem>? checklist = null)
        {
            var task = new Tasks("Task", "Desc", DateTime.Now, ToDo.Domain.Enums.TaskPriority.Medium, ToDo.Domain.Enums.TaskType.Daily, userId);
            typeof(Tasks).GetProperty("Id")!.SetValue(task, id);
            if (checklist != null)
                task.Checklist = checklist;
            return task;
        }

        private ChecklistItem CreateChecklistItem(int id, int taskId)
        {
            var item = new ChecklistItem("Item", taskId);
            typeof(ChecklistItem).GetProperty("Id")!.SetValue(item, id);
            return item;
        }

        [Fact]
        public async Task Handle_ShouldDeleteChecklistItem_WhenValid()
        {
            // Arrange
            var taskId = 1;
            var userId = 2;
            var itemId = 10;
            var checklistItem = CreateChecklistItem(itemId, taskId);
            var task = CreateTask(taskId, userId, new List<ChecklistItem> { checklistItem });
            var command = new DeleteChecklistItemCommand { TaskId = taskId, ItemId = itemId, UserId = userId };

            _mockTaskRepository.Setup(r => r.GetByIdAsync(taskId, x=>x.Checklist))
                .ReturnsAsync(task);
            _mockTaskRepository.Setup(r => r.SaveChangesAsync())
                .ReturnsAsync(1);

            // Act
            var result = await _handler.Handle(command, default);

            // Assert
            Assert.True(result);
            Assert.Empty(task.Checklist);
            _mockTaskRepository.Verify(r => r.GetByIdAsync(taskId, x => x.Checklist), Times.Once);
            _mockTaskRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task Handle_TaskNotFound_ShouldThrowKeyNotFoundException()
        {
            // Arrange
            var command = new DeleteChecklistItemCommand { TaskId = 1, ItemId = 1, UserId = 1 };
            _mockTaskRepository.Setup(r => r.GetByIdAsync(command.TaskId, x => x.Checklist))
                .ReturnsAsync((Tasks?)null);

            // Act & Assert
            var ex = await Assert.ThrowsAsync<KeyNotFoundException>(() => _handler.Handle(command, default));
            Assert.Equal($"Task with ID {command.TaskId} not found", ex.Message);
            _mockTaskRepository.Verify(r => r.GetByIdAsync(command.TaskId, x => x.Checklist), Times.Once);
            _mockTaskRepository.Verify(r => r.SaveChangesAsync(), Times.Never);
        }

        [Fact]
        public async Task Handle_UnauthorizedUser_ShouldThrowUnauthorizedAccessException()
        {
            // Arrange
            var taskId = 1;
            var userId = 2;
            var command = new DeleteChecklistItemCommand { TaskId = taskId, ItemId = 1, UserId = 99 };
            var task = CreateTask(taskId, userId, new List<ChecklistItem>());
            _mockTaskRepository.Setup(r => r.GetByIdAsync(taskId, x => x.Checklist))
                .ReturnsAsync(task);

            // Act & Assert
            var ex = await Assert.ThrowsAsync<UnauthorizedAccessException>(() => _handler.Handle(command, default));
            Assert.Equal("You can only modify your own tasks", ex.Message);
            _mockTaskRepository.Verify(r => r.GetByIdAsync(taskId, x => x.Checklist), Times.Once);
            _mockTaskRepository.Verify(r => r.SaveChangesAsync(), Times.Never);
        }

        [Fact]
        public async Task Handle_ChecklistItemNotFound_ShouldThrowKeyNotFoundException()
        {
            // Arrange
            var taskId = 1;
            var userId = 2;
            var command = new DeleteChecklistItemCommand { TaskId = taskId, ItemId = 99, UserId = userId };
            var task = CreateTask(taskId, userId, new List<ChecklistItem>());
            _mockTaskRepository.Setup(r => r.GetByIdAsync(taskId, x => x.Checklist))
                .ReturnsAsync(task);

            // Act & Assert
            var ex = await Assert.ThrowsAsync<KeyNotFoundException>(() => _handler.Handle(command, default));
            Assert.Equal($"Checklist item with ID {command.ItemId} not found", ex.Message);
            _mockTaskRepository.Verify(r => r.GetByIdAsync(taskId, x => x.Checklist), Times.Once);
            _mockTaskRepository.Verify(r => r.SaveChangesAsync(), Times.Never);
        }

        [Fact]
        public async Task Handle_RepositoryThrowsException_ShouldPropagate()
        {
            // Arrange
            var taskId = 1;
            var userId = 2;
            var itemId = 10;
            var checklistItem = CreateChecklistItem(itemId, taskId);
            var task = CreateTask(taskId, userId, new List<ChecklistItem> { checklistItem });
            var command = new DeleteChecklistItemCommand { TaskId = taskId, ItemId = itemId, UserId = userId };

            _mockTaskRepository.Setup(r => r.GetByIdAsync(taskId, x => x.Checklist))
                .ReturnsAsync(task);
            _mockTaskRepository.Setup(r => r.SaveChangesAsync())
                .ThrowsAsync(new InvalidOperationException("DB error"));

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => _handler.Handle(command, default));
            _mockTaskRepository.Verify(r => r.GetByIdAsync(taskId, x => x.Checklist), Times.Once);
            _mockTaskRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
        }
    }
} 