import { Component, EventEmitter, Input, Output } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { TaskService, TaskType, TaskPriority, CreateTaskForm } from '../../services/task.service';

@Component({
  selector: 'app-create-task-modal',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './create-task-modal.component.html'
})
export class CreateTaskModalComponent {
  @Input() isOpen: boolean = false;
  @Output() isOpenChange = new EventEmitter<boolean>();
  @Output() taskCreated = new EventEmitter<CreateTaskForm>();

  form: CreateTaskForm = {
    title: '',
    description: '',
    type: TaskType.Daily,
    priority: TaskPriority.Medium,
    isRecurring: false,
    recurrenceEndDate: undefined
  };

  // Enums para o template
  TaskType = TaskType;
  TaskPriority = TaskPriority;

  ngOnInit(): void {
    // Inicializar com a data atual
  }

  // Método para formatar data para o input HTML (YYYY-MM-DD)
  formatDateForInput(date: Date): string {
    const year = date.getFullYear();
    const month = String(date.getMonth() + 1).padStart(2, '0');
    const day = String(date.getDate()).padStart(2, '0');
    return `${year}-${month}-${day}`;
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

  closeModal(): void {
    this.isOpenChange.emit(false);
    this.resetForm();
  }

  createTask(): void {
    if (this.form.title.trim()) {
      this.taskCreated.emit({ ...this.form });
      this.closeModal();
    }
  }

  onBackdropClick(event: Event): void {
    if (event.target === event.currentTarget) {
      this.closeModal();
    }
  }

  onRecurringChange(): void {
    if (!this.form.isRecurring) {
      this.form.recurrenceEndDate = undefined;
    }
  }

  private resetForm(): void {
    this.form = {
      title: '',
      description: '',
      type: TaskType.Daily,
      priority: TaskPriority.Medium,
      isRecurring: false,
      recurrenceEndDate: undefined
    };
  }
}
