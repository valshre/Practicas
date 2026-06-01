import { Component, OnInit, ChangeDetectorRef, TemplateRef, ViewChild } from '@angular/core';
import { CommonModule } from '@angular/common';
import { DragDropModule, CdkDragDrop, moveItemInArray, transferArrayItem } from '@angular/cdk/drag-drop';
import { MatCardModule } from '@angular/material/card';
import { MatIconModule } from '@angular/material/icon';
import { MatDialog, MatDialogModule } from '@angular/material/dialog';
import { FormsModule } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatInputModule } from '@angular/material/input';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';

import { TaskDto, CreateTaskRequest } from '../../models/interfaces';
import { TaskService } from '../../services/task.service';

@Component({
  selector: 'app-kanban',
  standalone: true,
  imports: [
    CommonModule, 
    DragDropModule, 
    MatCardModule, 
    MatIconModule,
    MatDialogModule,
    FormsModule,
    MatButtonModule,
    MatInputModule,
    MatFormFieldModule,
    MatSnackBarModule
  ],
  templateUrl: './kanban.component.html',
  styleUrls: ['./kanban.component.scss']
})
export class KanbanComponent implements OnInit {
  @ViewChild('createTaskDialog') createTaskDialog!: TemplateRef<any>;
  
  newTask = { title: '', description: '' };
  todo: TaskDto[] = [];
  done: TaskDto[] = [];
  successMessage: string | null = null;

  constructor(
    private taskService: TaskService, 
    private cdr: ChangeDetectorRef,
    private dialog: MatDialog,
    private snackBar: MatSnackBar
  ) {}

  ngOnInit(): void {
    this.loadTasks();
  }

  loadTasks() {
    this.taskService.getTasks().subscribe({
      next: (tasks: TaskDto[]) => {
        this.todo = tasks.filter(t => !t.isCompleted);
        this.done = tasks.filter(t => t.isCompleted);
        this.cdr.detectChanges();
      },
      error: () => {
        this.snackBar.open('Error al cargar tareas', 'Cerrar', {
          duration: 5000,
          panelClass: ['error-snackbar']
        });
      }
    });
  }

  openCreateDialog() {
    this.newTask = { title: '', description: '' };
    this.dialog.open(this.createTaskDialog, {
      width: '400px',
    });
  }

  closeDialog() {
    this.dialog.closeAll();
  }

  addTask() {
    if (!this.newTask.title.trim()) return;

    const request: CreateTaskRequest = {
      title: this.newTask.title,
      description: this.newTask.description
    };

    this.taskService.createTask(request).subscribe({
      next: (task: TaskDto) => {
        this.todo.unshift(task);
        this.closeDialog();
        
        this.successMessage = 'Tarea creada con éxito.';
        this.cdr.detectChanges();
        
        setTimeout(() => {
          this.successMessage = null;
          this.cdr.detectChanges();
        }, 3000);
      },
      error: () => {
        this.snackBar.open('Hubo un error al crear la tarea. Intenta de nuevo.', 'Cerrar', {
          duration: 5000,
          panelClass: ['error-snackbar']
        });
      }
    });
  }

  drop(event: CdkDragDrop<TaskDto[]>) {
    if (event.previousContainer === event.container) {
      moveItemInArray(event.container.data, event.previousIndex, event.currentIndex);
    } else {
      const task = event.previousContainer.data[event.previousIndex];
      this.taskService.completeTask(task.id).subscribe({
        next: () => {
          transferArrayItem(
            event.previousContainer.data,
            event.container.data,
            event.previousIndex,
            event.currentIndex
          );
        },
        error: () => {
          this.snackBar.open('No se pudo actualizar la tarea', 'Cerrar', {
            duration: 5000,
            panelClass: ['error-snackbar']
          });
        }
      });
    }
  }
}