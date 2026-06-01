import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { TaskDto, CreateTaskRequest } from '../models/interfaces';

@Injectable({ providedIn: 'root' })
export class TaskService {
  private apiUrl = 'https://localhost:7140/api/Tasks';
  
  constructor(private http: HttpClient) {}

  getTasks(): Observable<TaskDto[]> {
    return this.http.get<TaskDto[]>(this.apiUrl);
  }

  createTask(request: CreateTaskRequest): Observable<TaskDto> {
    return this.http.post<TaskDto>(this.apiUrl, request);
  }

  completeTask(id: string): Observable<void> {
    return this.http.patch<void>(`${this.apiUrl}/${id}/complete`, {});
  }
}