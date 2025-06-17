import { Component, Input, Output, EventEmitter, OnChanges, SimpleChanges, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Task, TaskPriority, TaskType, ChecklistItem, TaskService } from '../../services/task.service';
import { Observable } from 'rxjs';

@Component({
    selector: 'app-task-sidebar',
    standalone: true,
    imports: [CommonModule, FormsModule],
    templateUrl: './task-sidebar.component.html',
    styleUrls: ['./task-sidebar.component.scss']
})
export class TaskSidebarComponent implements OnChanges {
    @Input() task: Task | null = null;
    @Input() isOpen: boolean = false;
    @Output() close = new EventEmitter<void>();

    loadedTask: Task | null = null;
    loading = false;
    newChecklistText: string = '';

    constructor(private taskService: TaskService,
        private cdr: ChangeDetectorRef
    ) { }

    ngOnChanges(changes: SimpleChanges): void {
        if (changes['isOpen'] && this.isOpen && this.task) {
            this.loadTaskDetails();
        }
        if (changes['isOpen'] && !this.isOpen) {
            this.loadedTask = null;
            this.loading = false;
            this.cdr.markForCheck();
        }
    }

    get checklist(): ChecklistItem[] {
        return this.loadedTask?.checklist || [];
    }

    private loadTaskDetails(): void {
        if (!this.task) return;
        this.loading = true;
        this.cdr.markForCheck();
        this.taskService.getTaskById(this.task.id).subscribe({
            next: (task: Task) => {
                this.loadedTask = task;
                this.loading = false;
                this.cdr.markForCheck();
            },
            error: (error: any) => {
                this.loading = false;
                this.cdr.markForCheck();
                console.error('Erro ao carregar detalhes da tarefa:', error);
            }
        });
    }

    private reloadTask(): void {
        this.loadTaskDetails();
    }

    addChecklistItem(event: KeyboardEvent): void {
        if (event.key === 'Enter' && this.newChecklistText.trim() && this.loadedTask) {
            this.taskService.addChecklistItem(this.loadedTask.id, this.newChecklistText.trim()).subscribe({
                next: () => {
                    this.reloadTask();
                    this.newChecklistText = '';
                },
                error: (error) => {
                    console.error('Erro ao adicionar item do checklist:', error);
                }
            });
        }
    }

    toggleChecklistItem(item: ChecklistItem): void {
        if (!this.loadedTask || !this.loadedTask.checklist) return;
        this.taskService.toggleChecklistItem(this.loadedTask.id, item.id).subscribe({
            next: () => {
                this.reloadTask();
            },
            error: (error) => {
                console.error('Erro ao alternar item do checklist:', error);
            }
        });
    }

    deleteChecklistItem(item: ChecklistItem): void {
        if (!this.loadedTask || !this.loadedTask.checklist) return;
        this.taskService.deleteChecklistItem(this.loadedTask.id, item.id).subscribe({
            next: () => {
                this.reloadTask();
            },
            error: (error) => {
                console.error('Erro ao deletar item do checklist:', error);
            }
        });
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