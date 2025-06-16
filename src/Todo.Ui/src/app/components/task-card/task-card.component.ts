import { Component, Input, ChangeDetectionStrategy, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { CdkDrag } from '@angular/cdk/drag-drop';
import { Task, TaskPriority, TaskType } from '../../services/task.service';

@Component({
  selector: 'app-task-card',
  standalone: true,
  imports: [CommonModule, CdkDrag],
  templateUrl: './task-card.component.html',
  styleUrls: ['./task-card.component.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class TaskCardComponent {
  @Input() task!: Task;


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
      return 'task-date overdue';
    }
    return 'task-date';
  }
} 