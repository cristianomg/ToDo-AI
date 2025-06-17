import { Component, Input, Output, EventEmitter } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Task, TaskPriority, TaskType } from '../../services/task.service';

@Component({
    selector: 'app-task-sidebar',
    standalone: true,
    imports: [CommonModule],
    templateUrl: './task-sidebar.component.html',
    styleUrls: ['./task-sidebar.component.scss']
})
export class TaskSidebarComponent {
    @Input() task: Task | null = null;
    @Input() isOpen: boolean = false;
    @Output() close = new EventEmitter<void>();

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
        switch (type) {
            case TaskType.Daily: return 'Diária';
            case TaskType.Weekly: return 'Semanal';
            case TaskType.Monthly: return 'Mensal';
            default: return 'Desconhecido';
        }
    }

    getStatusLabel(status: string): string {
        switch (status) {
            case 'Pending': return 'A Fazer';
            case 'InProgress': return 'Em Andamento';
            case 'Completed': return 'Concluída';
            default: return 'Desconhecido';
        }
    }

    getStatusClass(status: string): string {
        switch (status) {
            case 'Pending': return 'bg-blue-500 text-white';
            case 'InProgress': return 'bg-orange-500 text-white';
            case 'Completed': return 'bg-green-500 text-white';
            default: return 'bg-muted text-muted-foreground';
        }
    }

    isOverdue(): boolean {
        if (!this.task?.dueDate) return false;
        const today = new Date();
        const dueDate = new Date(this.task.dueDate);
        return dueDate < today && this.task.status !== 'Completed';
    }

    closeSidebar(): void {
        this.close.emit();
    }

    formatDate(date: string | Date): string {
        if (!date) return 'Não definida';
        return new Date(date).toLocaleDateString('pt-BR', {
            day: '2-digit',
            month: '2-digit',
            year: 'numeric',
            hour: '2-digit',
            minute: '2-digit'
        });
    }
} 