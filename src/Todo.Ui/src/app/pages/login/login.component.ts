import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { UserService, User } from '../../services/user.service';
import { AuthService } from '../../services/auth.service';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './login.component.html'
})
export class LoginComponent implements OnInit {
  users: User[] = [];
  selectedUserId: number | null = null;

  constructor(
    private userService: UserService,
    private authService: AuthService,
    private router: Router
  ) { }

  ngOnInit(): void {
    this.loadUsers();
  }

  loadUsers(): void {
    this.userService.getAllUsers().subscribe({
      next: (users) => {
        this.users = users;
        console.log('Usuários carregados:', users);
      },
      error: (error) => {
        console.error('Erro ao carregar usuários:', error);
      }
    });
  }

  login(): void {
    if (this.selectedUserId) {
      const selectedUser = this.users.find(user => user.id == this.selectedUserId);
      if (selectedUser) {
        // Usar o AuthService para salvar o usuário
        this.authService.setCurrentUser(selectedUser);
        console.log(`Usuário logado: ${selectedUser.name}`);
        // Redirecionar para a página home
        this.router.navigate(['/home']);
      }
    } else {
      alert('Por favor, selecione um usuário');
    }
  }
}
