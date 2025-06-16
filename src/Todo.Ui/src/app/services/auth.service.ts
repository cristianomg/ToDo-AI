import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable } from 'rxjs';

export interface User {
    id: number;
    name: string;
    role: number;
}

@Injectable({
    providedIn: 'root'
})
export class AuthService {
    private currentUserSubject = new BehaviorSubject<User | null>(null);
    public currentUser$ = this.currentUserSubject.asObservable();

    constructor() {
        this.loadCurrentUser();
    }

    private loadCurrentUser(): void {
        const userStr = localStorage.getItem('currentUser');
        if (userStr) {
            try {
                const user = JSON.parse(userStr);
                this.currentUserSubject.next(user);
            } catch (error) {
                console.error('Erro ao carregar usuário do localStorage:', error);
                this.clearCurrentUser();
            }
        }
    }

    setCurrentUser(user: User): void {
        localStorage.setItem('currentUser', JSON.stringify(user));
        this.currentUserSubject.next(user);
    }

    getCurrentUser(): User | null {
        return this.currentUserSubject.value;
    }

    clearCurrentUser(): void {
        localStorage.removeItem('currentUser');
        this.currentUserSubject.next(null);
    }

    isLoggedIn(): boolean {
        return this.currentUserSubject.value !== null;
    }

    getUserId(): number | null {
        const user = this.getCurrentUser();
        return user ? user.id : null;
    }
} 