import { HttpClient } from '@angular/common/http';
import { Component, inject, signal } from '@angular/core';
import { AuthenticationService } from '../services/authentication.service';
import { finalize } from 'rxjs/operators';
import { Router } from '@angular/router';
import { PageTabService } from '../../../core/layout/tabs/page-tab.service';
import { switchMap } from 'rxjs/operators';

@Component({
  selector: 'app-login',
  imports: [],
  templateUrl: './login.html',
  styleUrl: './login.scss',
})
export class Login {

 router = inject(Router);
  authenticationService = inject(AuthenticationService);
  pageTabService = inject(PageTabService);

  userName  = signal('');
  password  = signal('');
  isLoading = signal(false);

  login(): void {
    if (!this.validateForm()) {
      alert('Please enter both username and password.');
      return;
    }

    this.isLoading.set(true);

    this.authenticationService
      .login({
        userNameOrEmail: this.userName(),
        password: this.password()
      })
      .pipe(
        // Use switchMap to ensure only the latest login attempt triggers loadMe, cancelling any previous pending requests.
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
        error: error => {
          console.error(error);
          alert('Login failed or failed to load user data after login.');
        }
      });
  }

  private validateForm(): boolean {
    return this.userName() !== '' && this.password() !== '';
  }
}
