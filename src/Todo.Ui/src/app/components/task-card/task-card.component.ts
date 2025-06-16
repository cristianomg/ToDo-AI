import { Component, Input, ChangeDetectionStrategy, OnInit, Output, EventEmitter } from '@angular/core';
import { CommonModule } from '@angular/common';
import { CdkDrag } from '@angular/cdk/drag-drop';
import { Task, TaskPriority, TaskType, TaskService } from '../../services/task.service';

@Component({
  selector: 'app-task-card',
  standalone: true,
  imports: [CommonModule, CdkDrag],
  templateUrl: './task-card.component.html',
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class TaskCardComponent {
  @Input() task!: Task;
  @Output() taskDeleted = new EventEmitter<number>();

  constructor(private taskService: TaskService) { }

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

  getTypeLabel(type: TaskType): string {
    console.log('type', type);

    switch (type) {
      case TaskType.Daily: return 'Diária';
      case TaskType.Weekly: return 'Semanal';
      case TaskType.Monthly: return 'Mensal';
      default: return 'Desconhecido';
    }
  }

  isOverdue(): boolean {
    const today = new Date();
    const dueDate = new Date(this.task.dueDate);
    return dueDate < today && this.task.status !== 'Completed';
  }

  getDateLabel(): string {
    if (this.isOverdue()) {
      return 'Vencida em:';
    }
    return 'Vence em:';
  }

  getDateClass(): string {
    if (this.isOverdue()) {
      return 'bg-destructive/10 text-destructive border-l-destructive';
    }
    return 'bg-primary/10 text-primary border-l-primary';
  }

  deleteTask(): void {
    if (confirm('Tem certeza que deseja excluir esta tarefa?')) {
      this.taskService.deleteTask(this.task.id).subscribe({
        next: () => {
          this.taskDeleted.emit(this.task.id);
        },
        error: (error) => {
          console.error('Erro ao excluir tarefa:', error);
          alert('Erro ao excluir tarefa. Tente novamente.');
        }
      });
    }
  }
} 
