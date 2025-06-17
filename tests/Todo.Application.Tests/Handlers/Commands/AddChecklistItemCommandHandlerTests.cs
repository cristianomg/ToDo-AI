using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Moq;
using Todo.Application.Commands;
using Todo.Application.Handlers.Commands;
using ToDo.Domain.DTOs;
using ToDo.Domain.Entities;
using ToDo.Domain.Repositories;
using ToDo.Domain.Enums;

namespace Todo.Application.Tests.Handlers.Commands
{
    public class AddChecklistItemCommandHandlerTests
    {
        private readonly Mock<ITaskRepository> _mockTaskRepository;
        private readonly Mock<IMapper> _mockMapper;
        private readonly AddChecklistItemCommandHandler _handler;

        public AddChecklistItemCommandHandlerTests()
        {
            _mockTaskRepository = new Mock<ITaskRepository>();
            _mockMapper = new Mock<IMapper>();
            _handler = new AddChecklistItemCommandHandler(_mockTaskRepository.Object, _mockMapper.Object);
        }

        private Tasks CreateTask(int id, int userId, List<ChecklistItem>? checklist = null)
        {
            var task = new Tasks("Test Task", "Description", DateTime.UtcNow, TaskPriority.Medium, TaskType.Daily, userId);
            // Set Id via reflection (for testing only)
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
        public async Task Handle_ValidRequest_ShouldAddChecklistItemAndReturnDto()
        {
            // Arrange
            var taskId = 1;
            var userId = 1;
            var text = "Test checklist item";
            var command = new AddChecklistItemCommand
            {
                TaskId = taskId,
                UserId = userId,
                Text = text
            };

            var task = CreateTask(taskId, userId, new List<ChecklistItem>());
            var expectedDto = new ChecklistItemDto
            {
                Id = 1,
                Text = text,
                Completed = false
            };

            _mockTaskRepository.Setup(x => x.GetByIdAsync(taskId))
                .ReturnsAsync(task);
            _mockTaskRepository.Setup(x => x.SaveChangesAsync())
                .Returns(Task.FromResult(1));
            _mockMapper.Setup(x => x.Map<ChecklistItemDto>(It.IsAny<ChecklistItem>()))
                .Returns(expectedDto);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expectedDto.Text, result.Text);
            Assert.Equal(expectedDto.Completed, result.Completed);
            Assert.Single(task.Checklist);
            var addedItem = task.Checklist.First();
            Assert.Equal(text, addedItem.Text);
            Assert.Equal(taskId, addedItem.TaskId);
            Assert.False(addedItem.Completed);
            _mockTaskRepository.Verify(x => x.GetByIdAsync(taskId), Times.Once);
            _mockTaskRepository.Verify(x => x.SaveChangesAsync(), Times.Once);
            _mockMapper.Verify(x => x.Map<ChecklistItemDto>(It.IsAny<ChecklistItem>()), Times.Once);
        }

        [Fact]
        public async Task Handle_TaskNotFound_ShouldThrowKeyNotFoundException()
        {
            // Arrange
            var taskId = 999;
            var userId = 1;
            var command = new AddChecklistItemCommand
            {
                TaskId = taskId,
                UserId = userId,
                Text = "Test item"
            };
            _mockTaskRepository.Setup(x => x.GetByIdAsync(taskId))
                .ReturnsAsync((Tasks?)null);
            // Act & Assert
            var exception = await Assert.ThrowsAsync<KeyNotFoundException>(
                () => _handler.Handle(command, CancellationToken.None));
            Assert.Equal($"Task with ID {taskId} not found", exception.Message);
            _mockTaskRepository.Verify(x => x.GetByIdAsync(taskId), Times.Once);
            _mockTaskRepository.Verify(x => x.SaveChangesAsync(), Times.Never);
            _mockMapper.Verify(x => x.Map<ChecklistItemDto>(It.IsAny<ChecklistItem>()), Times.Never);
        }

        [Fact]
        public async Task Handle_UnauthorizedUser_ShouldThrowUnauthorizedAccessException()
        {
            // Arrange
            var taskId = 1;
            var taskUserId = 1;
            var requestUserId = 2; // Different user
            var command = new AddChecklistItemCommand
            {
                TaskId = taskId,
                UserId = requestUserId,
                Text = "Test item"
            };
            var task = CreateTask(taskId, taskUserId, new List<ChecklistItem>());
            _mockTaskRepository.Setup(x => x.GetByIdAsync(taskId))
                .ReturnsAsync(task);
            // Act & Assert
            var exception = await Assert.ThrowsAsync<UnauthorizedAccessException>(
                () => _handler.Handle(command, CancellationToken.None));
            Assert.Equal("You can only modify your own tasks", exception.Message);
            _mockTaskRepository.Verify(x => x.GetByIdAsync(taskId), Times.Once);
            _mockTaskRepository.Verify(x => x.SaveChangesAsync(), Times.Never);
            _mockMapper.Verify(x => x.Map<ChecklistItemDto>(It.IsAny<ChecklistItem>()), Times.Never);
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData(null)]
        public async Task Handle_EmptyOrNullText_ShouldThrowArgumentException(string text)
        {
            // Arrange
            var taskId = 1;
            var userId = 1;
            var command = new AddChecklistItemCommand
            {
                TaskId = taskId,
                UserId = userId,
                Text = text
            };
            var task = CreateTask(taskId, userId, new List<ChecklistItem>());
            _mockTaskRepository.Setup(x => x.GetByIdAsync(taskId))
                .ReturnsAsync(task);
            // Act & Assert
            var exception = await Assert.ThrowsAsync<ArgumentException>(
                () => _handler.Handle(command, CancellationToken.None));
            Assert.Equal("Checklist item text cannot be empty", exception.Message);
            _mockTaskRepository.Verify(x => x.GetByIdAsync(taskId), Times.Once);
            _mockTaskRepository.Verify(x => x.SaveChangesAsync(), Times.Never);
            _mockMapper.Verify(x => x.Map<ChecklistItemDto>(It.IsAny<ChecklistItem>()), Times.Never);
        }

        [Fact]
        public async Task Handle_ExistingChecklist_ShouldAddToExistingList()
        {
            // Arrange
            var taskId = 1;
            var userId = 1;
            var text = "New checklist item";
            var command = new AddChecklistItemCommand
            {
                TaskId = taskId,
                UserId = userId,
                Text = text
            };
            var existingItem = CreateChecklistItem(1, "Existing item", taskId, true);
            var task = CreateTask(taskId, userId, new List<ChecklistItem> { existingItem });
            var expectedDto = new ChecklistItemDto
            {
                Id = 2,
                Text = text,
                Completed = false
            };
            _mockTaskRepository.Setup(x => x.GetByIdAsync(taskId))
                .ReturnsAsync(task);
            _mockTaskRepository.Setup(x => x.SaveChangesAsync())
                .Returns(Task.FromResult(1));
            _mockMapper.Setup(x => x.Map<ChecklistItemDto>(It.IsAny<ChecklistItem>()))
                .Returns(expectedDto);
            // Act
            var result = await _handler.Handle(command, CancellationToken.None);
            // Assert
            Assert.NotNull(result);
            Assert.Equal(expectedDto.Text, result.Text);
            Assert.Equal(expectedDto.Completed, result.Completed);
            Assert.Equal(2, task.Checklist.Count);
            var addedItem = task.Checklist.Last();
            Assert.Equal(text, addedItem.Text);
            Assert.Equal(taskId, addedItem.TaskId);
            Assert.False(addedItem.Completed);
            _mockTaskRepository.Verify(x => x.GetByIdAsync(taskId), Times.Once);
            _mockTaskRepository.Verify(x => x.SaveChangesAsync(), Times.Once);
            _mockMapper.Verify(x => x.Map<ChecklistItemDto>(It.IsAny<ChecklistItem>()), Times.Once);
        }

        [Fact]
        public async Task Handle_RepositoryThrowsException_ShouldPropagateException()
        {
            // Arrange
            var taskId = 1;
            var userId = 1;
            var command = new AddChecklistItemCommand
            {
                TaskId = taskId,
                UserId = userId,
                Text = "Test item"
            };
            var expectedException = new InvalidOperationException("Database connection failed");
            _mockTaskRepository.Setup(x => x.GetByIdAsync(taskId))
                .ThrowsAsync(expectedException);
            // Act & Assert
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(
                () => _handler.Handle(command, CancellationToken.None));
            Assert.Same(expectedException, exception);
            _mockTaskRepository.Verify(x => x.GetByIdAsync(taskId), Times.Once);
            _mockTaskRepository.Verify(x => x.SaveChangesAsync(), Times.Never);
            _mockMapper.Verify(x => x.Map<ChecklistItemDto>(It.IsAny<ChecklistItem>()), Times.Never);
        }

        [Fact]
        public async Task Handle_SaveChangesThrowsException_ShouldPropagateException()
        {
            // Arrange
            var taskId = 1;
            var userId = 1;
            var command = new AddChecklistItemCommand
            {
                TaskId = taskId,
                UserId = userId,
                Text = "Test item"
            };
            var task = CreateTask(taskId, userId, new List<ChecklistItem>());
            var expectedException = new InvalidOperationException("Save failed");
            _mockTaskRepository.Setup(x => x.GetByIdAsync(taskId))
                .ReturnsAsync(task);
            _mockTaskRepository.Setup(x => x.SaveChangesAsync())
                .ThrowsAsync(expectedException);
            // Act & Assert
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(
                () => _handler.Handle(command, CancellationToken.None));
            Assert.Same(expectedException, exception);
            _mockTaskRepository.Verify(x => x.GetByIdAsync(taskId), Times.Once);
            _mockTaskRepository.Verify(x => x.SaveChangesAsync(), Times.Once);
            _mockMapper.Verify(x => x.Map<ChecklistItemDto>(It.IsAny<ChecklistItem>()), Times.Never);
        }

        [Fact]
        public async Task Handle_MapperThrowsException_ShouldPropagateException()
        {
            // Arrange
            var taskId = 1;
            var userId = 1;
            var command = new AddChecklistItemCommand
            {
                TaskId = taskId,
                UserId = userId,
                Text = "Test item"
            };
            var task = CreateTask(taskId, userId, new List<ChecklistItem>());
            var expectedException = new AutoMapperMappingException("Mapping failed");
            _mockTaskRepository.Setup(x => x.GetByIdAsync(taskId))
                .ReturnsAsync(task);
            _mockTaskRepository.Setup(x => x.SaveChangesAsync())
                .Returns(Task.FromResult(1));
            _mockMapper.Setup(x => x.Map<ChecklistItemDto>(It.IsAny<ChecklistItem>()))
                .Throws(expectedException);
            // Act & Assert
            var exception = await Assert.ThrowsAsync<AutoMapperMappingException>(
                () => _handler.Handle(command, CancellationToken.None));
            Assert.Same(expectedException, exception);
            _mockTaskRepository.Verify(x => x.GetByIdAsync(taskId), Times.Once);
            _mockTaskRepository.Verify(x => x.SaveChangesAsync(), Times.Once);
            _mockMapper.Verify(x => x.Map<ChecklistItemDto>(It.IsAny<ChecklistItem>()), Times.Once);
        }
    }
}
