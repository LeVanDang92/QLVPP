import { Component, inject, signal } from '@angular/core';
import { AuthenticationService } from '../services/authentication.service';
import { finalize, switchMap } from 'rxjs/operators';
import { Router } from '@angular/router';
import { PageTabService } from '../../../core/layout/tabs/page-tab.service';

@Component({
  selector: 'app-login',
  imports: [],
  templateUrl: './login.html',
  styleUrl: './login.scss',
})
export class Login {
  private router = inject(Router);
  private authenticationService = inject(AuthenticationService);
  private pageTabService = inject(PageTabService);

  userName = signal('');
  password = signal('');
  isLoading = signal(false);
  isPasswordVisible = signal(false);
  errorMessage = signal('');

  login(): void {
    this.errorMessage.set('');

    if (!this.validateForm()) {
      this.errorMessage.set('Please enter both username and password.');
      return;
    }

    this.isLoading.set(true);

    this.authenticationService
      .login({
        userNameOrEmail: this.userName().trim(),
        password: this.password(),
      })
      .pipe(
        switchMap(() => this.authenticationService.loadMe()),
        finalize(() => {
          this.isLoading.set(false);
        })
      )
      .subscribe({
        next: () => {
          this.pageTabService.clearSavedTabs();
          this.router.navigate(['/dashboard']);
        },
        error: (error) => {
          console.error(error);
          this.errorMessage.set('Login failed. Please check your username or password.');
        },
      });
  }

  togglePassword(): void {
    this.isPasswordVisible.update((value) => !value);
  }

  private validateForm(): boolean {
    return this.userName().trim() !== '' && this.password() !== '';
  }
}
