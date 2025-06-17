import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { ApiService } from './api.service';

export interface ChecklistItem {
    id: number;
    text: string;
    completed: boolean;
}

export interface Task {
    id: number;
    title: string;
    description?: string;
    dueDate: string;
    priority: TaskPriority;
    status: TaskStatus;
    type: TaskType;
    userId: number;
    createdAt: string;
    updatedAt?: string;
    checklist?: ChecklistItem[];
}

export interface CreateTaskForm {
    title: string;
    description: string;
    type: TaskType;
    priority: TaskPriority;
    isRecurring: boolean;
    recurrenceEndDate?: string;
    startAt?: string;
}

export enum TaskStatus {
    Pending = 'Pending',
    InProgress = 'InProgress',
    Completed = 'Completed'
}

export enum TaskType {
    Daily = 'Daily',
    Weekly = 'Weekly',
    Monthly = 'Monthly'
}

export enum TaskPriority {
    Low = 'Low',
    Medium = 'Medium',
    High = 'High'
}

@Injectable({
    providedIn: 'root'
})
export class TaskService extends ApiService {
    constructor(http: HttpClient) {
        super(http);
    }

    getTasksByTypeByUser(type: TaskType, date: Date): Observable<Task[]> {
        const formattedDate = date.toISOString().split('T')[0];
        return this.http.get<Task[]>(this.getUrl(`/tasks/type/${type}?date=${formattedDate}`));
    }

    createTask(task: Partial<Task>): Observable<Task> {
        return this.http.post<Task>(this.getUrl('/tasks'), task);
    }

    createRecurringTasks(taskData: CreateTaskForm): Observable<Task[]> {
        // Preparar os dados para o backend
        const backendData = {
            title: taskData.title,
            description: taskData.description,
            type: taskData.type,
            priority: taskData.priority,
            isRecurring: taskData.isRecurring,
            recurrenceEndDate: taskData.recurrenceEndDate ? this.formatDateForBackend(taskData.recurrenceEndDate) : null,
            startAt: taskData.startAt ? this.formatDateForBackend(taskData.startAt) : null
        };

        console.log('Dados sendo enviados para o backend:', backendData);
        console.log('startAt sendo enviado:', backendData.startAt);

        return this.http.post<Task[]>(this.getUrl('/tasks'), backendData);
    }

    private formatDateForBackend(dateString: string): string {
        const [year, month, day] = dateString.split('-').map(Number);
        const date = new Date(year, month - 1, day, 12, 0, 0); 
        return date.toISOString();
    }

    updateTask(id: number, task: Partial<Task>): Observable<Task> {
        return this.http.put<Task>(this.getUrl(`/tasks/${id}`), task);
    }

    updateTaskStatus(id: number, status: TaskStatus): Observable<Task> {
        return this.http.put<Task>(this.getUrl(`/tasks/${id}`), { status });
    }

    deleteTask(id: number): Observable<void> {
        return this.http.delete<void>(this.getUrl(`/tasks/${id}`));
    }

    // Checklist methods
    addChecklistItem(taskId: number, text: string): Observable<ChecklistItem> {
        return this.http.post<ChecklistItem>(this.getUrl(`/tasks/${taskId}/checklist`), { text });
    }

    toggleChecklistItem(taskId: number, itemId: number): Observable<ChecklistItem> {
        return this.http.put<ChecklistItem>(this.getUrl(`/tasks/${taskId}/checklist/${itemId}/toggle`), {});
    }

    deleteChecklistItem(taskId: number, itemId: number): Observable<void> {
        return this.http.delete<void>(this.getUrl(`/tasks/${taskId}/checklist/${itemId}`));
    }

    getTaskById(id: number): Observable<Task> {
        return this.http.get<Task>(this.getUrl(`/tasks/${id}`));
    }
} 
