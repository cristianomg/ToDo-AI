using Moq;
using Todo.Application.Commands;
using Todo.Application.Handlers.Commands;
using ToDo.Domain.Entities;
using ToDo.Domain.Enums;
using ToDo.Domain.Repositories;

namespace Todo.Application.Tests.Handlers.Commands
{
    public class CreateTaskCommandHandlerTests
    {
        private readonly Mock<ITaskRepository> _mockTaskRepository;
        private readonly CreateTaskCommandHandler _handler;

        public CreateTaskCommandHandlerTests()
        {
            _mockTaskRepository = new Mock<ITaskRepository>();
            _handler = new CreateTaskCommandHandler(_mockTaskRepository.Object);
        }

        [Fact]
        public async Task Handle_ShouldCreateSingleTask_WhenNotRecurring()
        {
            // Arrange
            var command = new CreateTaskCommand
            {
                Title = "Tarefa Simples",
                Description = "Descrição",
                DueDate = DateTime.UtcNow.AddDays(1),
                Priority = TaskPriority.Medium,
                Type = TaskType.Daily,
                UserId = 1,
                IsRecurring = false
            };

            _mockTaskRepository.Setup(r => r.AddAsync(It.IsAny<Tasks>()))
                .ReturnsAsync((Tasks t) => t);
            _mockTaskRepository.Setup(r => r.SaveChangesAsync())
                .ReturnsAsync(1);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.Single(result);
            Assert.Equal(command.Title, result[0].Title);
            Assert.Equal(command.Description, result[0].Description);
            Assert.Equal(command.Priority, result[0].Priority);
            Assert.Equal(command.Type, result[0].Type);
            Assert.Equal(command.UserId, result[0].UserId);
            _mockTaskRepository.Verify(r => r.AddAsync(It.IsAny<Tasks>()), Times.Once);
            _mockTaskRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldCreateDailyRecurringTasks()
        {
            // Arrange
            var start = DateTime.UtcNow.Date;
            var end = start.AddDays(2); // 3 dias
            var command = new CreateTaskCommand
            {
                Title = "Tarefa Diária",
                Description = "Descrição",
                Priority = TaskPriority.High,
                Type = TaskType.Daily,
                UserId = 2,
                IsRecurring = true,
                RecurrenceEndDate = end
            };

            _mockTaskRepository.Setup(r => r.AddAsync(It.IsAny<Tasks>()))
                .ReturnsAsync((Tasks t) => t);
            _mockTaskRepository.Setup(r => r.SaveChangesAsync())
                .ReturnsAsync(3);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.Equal(3, result.Count);
            foreach (var task in result)
            {
                Assert.Equal(command.Title, task.Title);
                Assert.Equal(command.Type, task.Type);
                Assert.Equal(command.UserId, task.UserId);
            }
            _mockTaskRepository.Verify(r => r.AddAsync(It.IsAny<Tasks>()), Times.Exactly(3));
            _mockTaskRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldCreateWeeklyRecurringTasks()
        {
            // Arrange
            var start = DateTime.UtcNow.Date;
            var end = start.AddDays(14); // 2 semanas
            var command = new CreateTaskCommand
            {
                Title = "Tarefa Semanal",
                Description = "Descrição",
                Priority = TaskPriority.Low,
                Type = TaskType.Weekly,
                UserId = 3,
                IsRecurring = true,
                RecurrenceEndDate = end
            };

            _mockTaskRepository.Setup(r => r.AddAsync(It.IsAny<Tasks>()))
                .ReturnsAsync((Tasks t) => t);
            _mockTaskRepository.Setup(r => r.SaveChangesAsync())
                .ReturnsAsync(It.IsAny<int>());

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.True(result.Count >= 2); // Pode ser 2 ou 3 dependendo do dia da semana
            foreach (var task in result)
            {
                Assert.Equal(command.Title, task.Title);
                Assert.Equal(command.Type, task.Type);
                Assert.Equal(command.UserId, task.UserId);
            }
            _mockTaskRepository.Verify(r => r.AddAsync(It.IsAny<Tasks>()), Times.AtLeast(2));
            _mockTaskRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldCreateMonthlyRecurringTasks()
        {
            // Arrange
            var start = DateTime.UtcNow.Date;
            var end = start.AddMonths(2); // 3 meses
            var command = new CreateTaskCommand
            {
                Title = "Tarefa Mensal",
                Description = "Descrição",
                Priority = TaskPriority.High,
                Type = TaskType.Monthly,
                UserId = 4,
                IsRecurring = true,
                RecurrenceEndDate = end
            };

            _mockTaskRepository.Setup(r => r.AddAsync(It.IsAny<Tasks>()))
                .ReturnsAsync((Tasks t) => t);
            _mockTaskRepository.Setup(r => r.SaveChangesAsync())
                .ReturnsAsync(It.IsAny<int>());

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.True(result.Count >= 2); // Pode ser 2 ou 3 dependendo do mês
            foreach (var task in result)
            {
                Assert.Equal(command.Title, task.Title);
                Assert.Equal(command.Type, task.Type);
                Assert.Equal(command.UserId, task.UserId);
            }
            _mockTaskRepository.Verify(r => r.AddAsync(It.IsAny<Tasks>()), Times.AtLeast(2));
            _mockTaskRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task Handle_RepositoryThrowsException_ShouldPropagate()
        {
            // Arrange
            var command = new CreateTaskCommand
            {
                Title = "Falha",
                Description = "Teste",
                Priority = TaskPriority.Low,
                Type = TaskType.Daily,
                UserId = 5,
                IsRecurring = false
            };
            _mockTaskRepository.Setup(r => r.AddAsync(It.IsAny<Tasks>()))
                .ThrowsAsync(new InvalidOperationException("DB error"));

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => _handler.Handle(command, CancellationToken.None));
            _mockTaskRepository.Verify(r => r.AddAsync(It.IsAny<Tasks>()), Times.Once);
            _mockTaskRepository.Verify(r => r.SaveChangesAsync(), Times.Never);
        }
    }
}
