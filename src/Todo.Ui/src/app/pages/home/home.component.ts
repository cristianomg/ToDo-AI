import { Component, OnInit, ChangeDetectionStrategy, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { CdkDragDrop, DragDropModule, moveItemInArray, transferArrayItem } from '@angular/cdk/drag-drop';
import { TaskService, Task, TaskStatus, TaskType, TaskPriority, CreateTaskForm } from '../../services/task.service';
import { AuthService, User } from '../../services/auth.service';
import { TaskCardComponent } from '../../components/task-card/task-card.component';
import { CreateTaskModalComponent } from '../../components/create-task-modal/create-task-modal.component';
import { CustomDatepickerComponent } from '../../components/custom-datepicker/custom-datepicker.component';
import { TaskSidebarComponent } from '../../components/task-sidebar/task-sidebar.component';

@Component({
  selector: 'app-home',
  standalone: true,
  imports: [CommonModule, FormsModule, DragDropModule, TaskCardComponent, CreateTaskModalComponent, CustomDatepickerComponent, TaskSidebarComponent],
  templateUrl: './home.component.html',
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

  // Sidebar
  isSidebarOpen: boolean = false;
  selectedTask: Task | null = null;

  // Arrays para drag and drop
  pendingTasks: Task[] = [];
  inProgressTasks: Task[] = [];
  completedTasks: Task[] = [];

  // Enums para o template
  TaskStatus = TaskStatus;
  TaskType = TaskType;
  TaskPriority = TaskPriority;

  darkMode: boolean = false;

  constructor(
    private router: Router,
    private taskService: TaskService,
    private authService: AuthService,
    private cdr: ChangeDetectorRef
  ) { }

  ngOnInit(): void {
    this.selectedDate = this.formatDateForInput(new Date());
    this.loadTasks();
    this.loadCurrentUser();
    this.loadTheme();
  }

  // Métodos da Sidebar
  openTaskSidebar(task: Task): void {
    this.selectedTask = task;
    this.isSidebarOpen = true;
    this.cdr.markForCheck();
  }

  closeTaskSidebar(): void {
    this.isSidebarOpen = false;
    this.selectedTask = null;
    this.cdr.markForCheck();
  }

  toggleDarkMode(): void {
    this.darkMode = !this.darkMode;
    if (this.darkMode) {
      document.documentElement.classList.add('dark');
      localStorage.setItem('theme', 'dark');
    } else {
      document.documentElement.classList.remove('dark');
      localStorage.setItem('theme', 'light');
    }
  }

  loadTheme(): void {
    const theme = localStorage.getItem('theme');
    if (theme === 'dark') {
      this.darkMode = true;
      document.documentElement.classList.add('dark');
    } else {
      this.darkMode = false;
      document.documentElement.classList.remove('dark');
    }
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

    this.taskService.createRecurringTasks(taskData).subscribe({
      next: (createdTasks) => {
        console.log(`${createdTasks.length} tarefa(s) criada(s) com sucesso`);
        this.selectedTaskType = taskData.type;
        this.selectedDate = this.formatDateForInput(new Date());
        this.loadTasks();
      },
      error: (error) => {
        console.error('Erro ao criar tarefa(s):', error);
        alert('Erro ao criar tarefa(s). Tente novamente.');
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
      case TaskPriority.Low: return 'bg-muted text-muted-foreground';
      case TaskPriority.Medium: return 'bg-orange-500 text-white';
      case TaskPriority.High: return 'bg-destructive text-destructive-foreground';
      default: return 'bg-muted text-muted-foreground';
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
    // Remove a tarefa dos arrays locais
    this.tasks = this.tasks.filter(task => task.id !== taskId);
    this.pendingTasks = this.pendingTasks.filter(task => task.id !== taskId);
    this.inProgressTasks = this.inProgressTasks.filter(task => task.id !== taskId);
    this.completedTasks = this.completedTasks.filter(task => task.id !== taskId);
    this.cdr.markForCheck();
  }
}
