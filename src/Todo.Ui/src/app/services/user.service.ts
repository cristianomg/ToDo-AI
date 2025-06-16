import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { ApiService } from './api.service';

export interface User {
    id: number;
    name: string;
    role: number;
}

@Injectable({
    providedIn: 'root'
})
export class UserService extends ApiService {
    constructor(http: HttpClient) {
        super(http);
    }

    getAllUsers(): Observable<User[]> {
        return this.http.get<User[]>(this.getUrl('/users'));
    }
} 