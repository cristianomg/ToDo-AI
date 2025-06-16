import { Component, OnInit, ChangeDetectionStrategy, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { CdkDragDrop, DragDropModule, moveItemInArray, transferArrayItem } from '@angular/cdk/drag-drop';
import { TaskService, Task, TaskStatus, TaskType, TaskPriority } from '../../services/task.service';
import { AuthService, User } from '../../services/auth.service';
import { TaskCardComponent } from '../../components/task-card/task-card.component';
import { CreateTaskModalComponent, CreateTaskForm } from '../../components/create-task-modal/create-task-modal.component';

@Component({
  selector: 'app-home',
  standalone: true,
  imports: [CommonModule, FormsModule, DragDropModule, TaskCardComponent, CreateTaskModalComponent],
  templateUrl: './home.component.html',
  styleUrl: './home.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class HomeComponent implements OnInit {
  tasks: Task[] = [];
  filteredTasks: Task[] = [];

  // Filtros
  selectedTaskType: TaskType = TaskType.Daily;
  selectedDate: string = '';
  currentUser: User | null = null;

  // Modal
  isCreateModalOpen: boolean = false;

  // Arrays para drag and drop
  pendingTasks: Task[] = [];
  inProgressTasks: Task[] = [];
  completedTasks: Task[] = [];

  // Enums para o template
  TaskStatus = TaskStatus;
  TaskType = TaskType;
  TaskPriority = TaskPriority;

  constructor(
    private router: Router,
    private taskService: TaskService,
    private authService: AuthService,
    private cdr: ChangeDetectorRef
  ) { }

  ngOnInit(): void {
    this.selectedDate = this.formatDateForInput(new Date());
    console.log('Data inicializada:', this.selectedDate);
    this.loadTasks();
    this.loadCurrentUser();
  }

  // Método para formatar data para o input HTML (YYYY-MM-DD)
  formatDateForInput(date: Date): string {
    const year = date.getFullYear();
    const month = String(date.getMonth() + 1).padStart(2, '0');
    const day = String(date.getDate()).padStart(2, '0');
    const formattedDate = `${year}-${month}-${day}`;
    console.log('Data formatada:', formattedDate);
    return formattedDate;
  }

  // Método para converter string de data para Date
  parseDateFromInput(dateString: string): Date {
    return new Date(dateString);
  }

  // Método trackBy para melhor performance do ngFor
  trackByTaskId(index: number, task: Task): number {
    return task.id;
  }

  loadCurrentUser(): void {
    this.currentUser = this.authService.getCurrentUser();
    if (!this.currentUser) {
      this.router.navigate(['/login']);
    }
  }

  loadTasks(): void {
    if (this.selectedTaskType && this.selectedDate) {
      const dateObj = this.parseDateFromInput(this.selectedDate);
      this.taskService.getTasksByTypeByUser(this.selectedTaskType, dateObj).subscribe({
        next: (tasks) => {
          this.tasks = tasks;
          this.updateTaskArrays();
          this.cdr.markForCheck();
        },
        error: (error) => {
          console.error('Erro ao carregar tarefas:', error);
        }
      });
    }
  }

  updateTaskArrays(): void {
    this.pendingTasks = this.tasks.filter(task => task.status === TaskStatus.Pending);
    this.inProgressTasks = this.tasks.filter(task => task.status === TaskStatus.InProgress);
    this.completedTasks = this.tasks.filter(task => task.status === TaskStatus.Completed);
  }

  // Métodos da modal
  openCreateModal(): void {
    this.isCreateModalOpen = true;
    this.cdr.markForCheck();
  }

  closeCreateModal(): void {
    this.isCreateModalOpen = false;
    this.cdr.markForCheck();
  }

  onCreateTask(taskData: CreateTaskForm): void {
    if (!this.currentUser) return;

    const newTask = {
      title: taskData.title,
      description: taskData.description,
      type: taskData.type,
      priority: taskData.priority
    };

    this.taskService.createTask(newTask).subscribe({
      next: (createdTask) => {
        this.selectedTaskType = newTask.type;
        this.selectedDate = this.formatDateForInput(new Date());
        this.loadTasks();
      },
      error: (error) => {
        console.error('Erro ao criar tarefa:', error);
      }
    });
  }

  onDrop(event: CdkDragDrop<Task[]>): void {

    if (event.previousContainer === event.container) {
      moveItemInArray(event.container.data, event.previousIndex, event.currentIndex);
    } else {
      transferArrayItem(
        event.previousContainer.data,
        event.container.data,
        event.previousIndex,
        event.currentIndex
      );

      const movedTask = event.container.data[event.currentIndex];
      const newStatus = this.getStatusFromContainer(event.container.id);

      this.cdr.markForCheck();

      if (movedTask && newStatus) {
        this.taskService.updateTaskStatus(movedTask.id, newStatus).subscribe({
          next: (updatedTask) => {
            const taskIndex = this.tasks.findIndex(t => t.id === updatedTask.id);
            if (taskIndex !== -1) {
              this.tasks[taskIndex] = updatedTask;
              this.updateTaskArrays();
              this.cdr.markForCheck();
            }
          },
          error: (error) => {
            console.error('Erro ao atualizar status da tarefa:', error);
            this.loadTasks();
          }
        });
      }
    }
  }

  getStatusFromContainer(containerId: string): TaskStatus | null {
    switch (containerId) {
      case 'pending-container':
        return TaskStatus.Pending;
      case 'in-progress-container':
        return TaskStatus.InProgress;
      case 'completed-container':
        return TaskStatus.Completed;
      default:
        return null;
    }
  }

  getTasksByStatus(status: TaskStatus): Task[] {
    return this.tasks.filter(task => task.status === status);
  }

  getStatusLabel(status: TaskStatus): string {
    switch (status) {
      case TaskStatus.Pending: return 'A Fazer';
      case TaskStatus.InProgress: return 'Em Andamento';
      case TaskStatus.Completed: return 'Concluídas';
      default: return 'Desconhecido';
    }
  }

  getTypeLabel(type: TaskType): string {
    switch (type) {
      case TaskType.Daily: return 'Diária';
      case TaskType.Weekly: return 'Semanal';
      case TaskType.Monthly: return 'Mensal';
      default: return 'Desconhecido';
    }
  }

  getPriorityLabel(priority: TaskPriority): string {
    switch (priority) {
      case TaskPriority.Low: return 'Baixa';
      case TaskPriority.Medium: return 'Média';
      case TaskPriority.High: return 'Alta';
      default: return 'Desconhecido';
    }
  }

  getPriorityClass(priority: TaskPriority): string {
    switch (priority) {
      case TaskPriority.Low: return 'priority-low';
      case TaskPriority.Medium: return 'priority-medium';
      case TaskPriority.High: return 'priority-high';
      default: return '';
    }
  }

  onTypeChange(): void {
    this.loadTasks();
  }

  onDateChange(): void {
    this.loadTasks();
  }
  clearFilters(): void {
    this.selectedTaskType = TaskType.Daily;
    this.selectedDate = this.formatDateForInput(new Date());
    this.loadTasks();
  }
  logout(): void {
    this.authService.clearCurrentUser();
    this.router.navigate(['/login']);
  }

  onTaskDeleted(taskId: number): void {
    this.loadTasks();
  }
}
