import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../environments/environment';

@Injectable({
    providedIn: 'root'
})
export class ApiService {
    protected baseUrl = environment.apiUrl;

    constructor(protected http: HttpClient) { }

    protected getUrl(endpoint: string): string {
        return `${this.baseUrl}${endpoint}`;
    }
} 