import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { ApiService } from './api.service';

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
}

export interface CreateTaskForm {
    title: string;
    description: string;
    type: TaskType;
    priority: TaskPriority;
    isRecurring: boolean;
    recurrenceEndDate?: string;
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
            recurrenceEndDate: taskData.recurrenceEndDate ? new Date(taskData.recurrenceEndDate).toISOString() : null
        };

        return this.http.post<Task[]>(this.getUrl('/tasks'), backendData);
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
} 