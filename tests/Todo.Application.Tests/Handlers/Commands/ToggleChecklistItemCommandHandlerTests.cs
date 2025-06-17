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
    public class ToggleChecklistItemCommandHandlerTests
    {
        private readonly Mock<ITaskRepository> _mockTaskRepository;
        private readonly Mock<IMapper> _mockMapper;
        private readonly ToggleChecklistItemCommandHandler _handler;

        public ToggleChecklistItemCommandHandlerTests()
        {
            _mockTaskRepository = new Mock<ITaskRepository>();
            _mockMapper = new Mock<IMapper>();
            _handler = new ToggleChecklistItemCommandHandler(_mockTaskRepository.Object, _mockMapper.Object);
        }

        private Tasks CreateTask(int id, int userId, List<ChecklistItem>? checklist = null)
        {
            var task = new Tasks("Test Task", "Description", DateTime.UtcNow, TaskPriority.Medium, TaskType.Daily, userId);
            typeof(Tasks).GetProperty("Id")!.SetValue(task, id);
            if (checklist != null)
                task.Checklist = checklist;
            return task;
        }

        private ChecklistItem CreateChecklistItem(int id, string text, int taskId, bool completed = false)
        {
            var item = new ChecklistItem(text, taskId);
            typeof(ChecklistItem).GetProperty("Id")!.SetValue(item, id);
            item.Completed = completed;
            return item;
        }

        [Fact]
        public async Task Handle_ValidRequest_ShouldToggleFromFalseToTrue()
        {
            // Arrange
            var taskId = 1;
            var userId = 1;
            var itemId = 10;
            var checklistItem = CreateChecklistItem(itemId, "Test item", taskId, false);
            var task = CreateTask(taskId, userId, new List<ChecklistItem> { checklistItem });
            var command = new ToggleChecklistItemCommand { TaskId = taskId, ItemId = itemId, UserId = userId };

            var expectedDto = new ChecklistItemDto
            {
                Id = itemId,
                Text = "Test item",
                Completed = true
            };

            _mockTaskRepository.Setup(r => r.GetByIdAsync(taskId, x => x.Checklist))
                .ReturnsAsync(task);
            _mockTaskRepository.Setup(r => r.SaveChangesAsync())
                .ReturnsAsync(1);
            _mockMapper.Setup(m => m.Map<ChecklistItemDto>(checklistItem))
                .Returns(expectedDto);

            // Act
            var result = await _handler.Handle(command, default);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expectedDto.Id, result.Id);
            Assert.Equal(expectedDto.Text, result.Text);
            Assert.True(result.Completed);
            Assert.True(checklistItem.Completed);
            _mockTaskRepository.Verify(r => r.GetByIdAsync(taskId, x => x.Checklist), Times.Once);
            _mockTaskRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
            _mockMapper.Verify(m => m.Map<ChecklistItemDto>(checklistItem), Times.Once);
        }

        [Fact]
        public async Task Handle_ValidRequest_ShouldToggleFromTrueToFalse()
        {
            // Arrange
            var taskId = 1;
            var userId = 1;
            var itemId = 10;
            var checklistItem = CreateChecklistItem(itemId, "Test item", taskId, true);
            var task = CreateTask(taskId, userId, new List<ChecklistItem> { checklistItem });
            var command = new ToggleChecklistItemCommand { TaskId = taskId, ItemId = itemId, UserId = userId };

            var expectedDto = new ChecklistItemDto
            {
                Id = itemId,
                Text = "Test item",
                Completed = false
            };

            _mockTaskRepository.Setup(r => r.GetByIdAsync(taskId, x => x.Checklist))
                .ReturnsAsync(task);
            _mockTaskRepository.Setup(r => r.SaveChangesAsync())
                .ReturnsAsync(1);
            _mockMapper.Setup(m => m.Map<ChecklistItemDto>(checklistItem))
                .Returns(expectedDto);

            // Act
            var result = await _handler.Handle(command, default);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expectedDto.Id, result.Id);
            Assert.Equal(expectedDto.Text, result.Text);
            Assert.False(result.Completed);
            Assert.False(checklistItem.Completed);
            _mockTaskRepository.Verify(r => r.GetByIdAsync(taskId, x => x.Checklist), Times.Once);
            _mockTaskRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
            _mockMapper.Verify(m => m.Map<ChecklistItemDto>(checklistItem), Times.Once);
        }

        [Fact]
        public async Task Handle_TaskNotFound_ShouldThrowKeyNotFoundException()
        {
            // Arrange
            var command = new ToggleChecklistItemCommand { TaskId = 1, ItemId = 1, UserId = 1 };
            _mockTaskRepository.Setup(r => r.GetByIdAsync(command.TaskId, x => x.Checklist))
                .ReturnsAsync((Tasks?)null);

            // Act & Assert
            var ex = await Assert.ThrowsAsync<KeyNotFoundException>(() => _handler.Handle(command, default));
            Assert.Equal($"Task with ID {command.TaskId} not found", ex.Message);
            _mockTaskRepository.Verify(r => r.GetByIdAsync(command.TaskId, x => x.Checklist), Times.Once);
            _mockTaskRepository.Verify(r => r.SaveChangesAsync(), Times.Never);
            _mockMapper.Verify(m => m.Map<ChecklistItemDto>(It.IsAny<ChecklistItem>()), Times.Never);
        }

        [Fact]
        public async Task Handle_UnauthorizedUser_ShouldThrowUnauthorizedAccessException()
        {
            // Arrange
            var taskId = 1;
            var userId = 2;
            var command = new ToggleChecklistItemCommand { TaskId = taskId, ItemId = 1, UserId = 99 };
            var task = CreateTask(taskId, userId, new List<ChecklistItem>());
            _mockTaskRepository.Setup(r => r.GetByIdAsync(taskId, x => x.Checklist))
                .ReturnsAsync(task);

            // Act & Assert
            var ex = await Assert.ThrowsAsync<UnauthorizedAccessException>(() => _handler.Handle(command, default));
            Assert.Equal("You can only modify your own tasks", ex.Message);
            _mockTaskRepository.Verify(r => r.GetByIdAsync(taskId, x => x.Checklist), Times.Once);
            _mockTaskRepository.Verify(r => r.SaveChangesAsync(), Times.Never);
            _mockMapper.Verify(m => m.Map<ChecklistItemDto>(It.IsAny<ChecklistItem>()), Times.Never);
        }

        [Fact]
        public async Task Handle_ChecklistItemNotFound_ShouldThrowKeyNotFoundException()
        {
            // Arrange
            var taskId = 1;
            var userId = 2;
            var command = new ToggleChecklistItemCommand { TaskId = taskId, ItemId = 99, UserId = userId };
            var task = CreateTask(taskId, userId, new List<ChecklistItem>());
            _mockTaskRepository.Setup(r => r.GetByIdAsync(taskId, x => x.Checklist))
                .ReturnsAsync(task);

            // Act & Assert
            var ex = await Assert.ThrowsAsync<KeyNotFoundException>(() => _handler.Handle(command, default));
            Assert.Equal($"Checklist item with ID {command.ItemId} not found", ex.Message);
            _mockTaskRepository.Verify(r => r.GetByIdAsync(taskId, x => x.Checklist), Times.Once);
            _mockTaskRepository.Verify(r => r.SaveChangesAsync(), Times.Never);
            _mockMapper.Verify(m => m.Map<ChecklistItemDto>(It.IsAny<ChecklistItem>()), Times.Never);
        }

        [Fact]
        public async Task Handle_RepositoryThrowsException_ShouldPropagate()
        {
            // Arrange
            var taskId = 1;
            var userId = 2;
            var itemId = 10;
            var checklistItem = CreateChecklistItem(itemId, "Test item", taskId, false);
            var task = CreateTask(taskId, userId, new List<ChecklistItem> { checklistItem });
            var command = new ToggleChecklistItemCommand { TaskId = taskId, ItemId = itemId, UserId = userId };
            var expectedException = new InvalidOperationException("Database error");

            _mockTaskRepository.Setup(r => r.GetByIdAsync(taskId, x => x.Checklist))
                .ReturnsAsync(task);
            _mockTaskRepository.Setup(r => r.SaveChangesAsync())
                .ThrowsAsync(expectedException);

            // Act & Assert
            var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => _handler.Handle(command, default));
            Assert.Same(expectedException, ex);
            _mockTaskRepository.Verify(r => r.GetByIdAsync(taskId, x => x.Checklist), Times.Once);
            _mockTaskRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
            _mockMapper.Verify(m => m.Map<ChecklistItemDto>(It.IsAny<ChecklistItem>()), Times.Never);
        }

        [Fact]
        public async Task Handle_MapperThrowsException_ShouldPropagate()
        {
            // Arrange
            var taskId = 1;
            var userId = 2;
            var itemId = 10;
            var checklistItem = CreateChecklistItem(itemId, "Test item", taskId, false);
            var task = CreateTask(taskId, userId, new List<ChecklistItem> { checklistItem });
            var command = new ToggleChecklistItemCommand { TaskId = taskId, ItemId = itemId, UserId = userId };
            var expectedException = new AutoMapperMappingException("Mapping error");

            _mockTaskRepository.Setup(r => r.GetByIdAsync(taskId, x => x.Checklist))
                .ReturnsAsync(task);
            _mockTaskRepository.Setup(r => r.SaveChangesAsync())
                .ReturnsAsync(1);
            _mockMapper.Setup(m => m.Map<ChecklistItemDto>(checklistItem))
                .Throws(expectedException);

            // Act & Assert
            var ex = await Assert.ThrowsAsync<AutoMapperMappingException>(() => _handler.Handle(command, default));
            Assert.Same(expectedException, ex);
            Assert.True(checklistItem.Completed); // Should still be toggled
            _mockTaskRepository.Verify(r => r.GetByIdAsync(taskId, x => x.Checklist), Times.Once);
            _mockTaskRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
            _mockMapper.Verify(m => m.Map<ChecklistItemDto>(checklistItem), Times.Once);
        }

        [Fact]
        public async Task Handle_MultipleChecklistItems_ShouldToggleCorrectItem()
        {
            // Arrange
            var taskId = 1;
            var userId = 1;
            var itemId1 = 10;
            var itemId2 = 20;
            var checklistItem1 = CreateChecklistItem(itemId1, "Item 1", taskId, false);
            var checklistItem2 = CreateChecklistItem(itemId2, "Item 2", taskId, true);
            var task = CreateTask(taskId, userId, new List<ChecklistItem> { checklistItem1, checklistItem2 });
            var command = new ToggleChecklistItemCommand { TaskId = taskId, ItemId = itemId1, UserId = userId };

            var expectedDto = new ChecklistItemDto
            {
                Id = itemId1,
                Text = "Item 1",
                Completed = true
            };

            _mockTaskRepository.Setup(r => r.GetByIdAsync(taskId, x => x.Checklist))
                .ReturnsAsync(task);
            _mockTaskRepository.Setup(r => r.SaveChangesAsync())
                .ReturnsAsync(1);
            _mockMapper.Setup(m => m.Map<ChecklistItemDto>(checklistItem1))
                .Returns(expectedDto);

            // Act
            var result = await _handler.Handle(command, default);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expectedDto.Id, result.Id);
            Assert.True(result.Completed);
            Assert.True(checklistItem1.Completed); // Should be toggled
            Assert.True(checklistItem2.Completed); // Should remain unchanged
            _mockTaskRepository.Verify(r => r.GetByIdAsync(taskId, x => x.Checklist), Times.Once);
            _mockTaskRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
            _mockMapper.Verify(m => m.Map<ChecklistItemDto>(checklistItem1), Times.Once);
        }
    }
} 