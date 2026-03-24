import { Component, inject } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatIcon } from '@angular/material/icon';
import { Router } from '@angular/router';
import { AccountService } from '../../../../core/services/account.service';
import { SnackbarService } from '../../../../core/services/snackbar.service';

@Component({
  selector: 'app-admin-login',
  imports: [
    MatIcon,
    ReactiveFormsModule
  ],
  templateUrl: './admin-login.component.html',
  styleUrl: './admin-login.component.scss',
})
export class AdminLoginComponent {
  private fb = inject(FormBuilder);
  private accountService = inject(AccountService);
  private snackbarService = inject(SnackbarService);
  private router = inject(Router);

  hidePassword = true;

  loginForm = this.fb.group({
    email: ['', [Validators.required, Validators.email]],
    password: ['', [Validators.required, Validators.minLength(6)]]
  });

  onSubmit() {
    if (this.loginForm.valid) {
      const { email, password } = this.loginForm.value;

      const loginRequest = {
        email: email ?? '',
        password: password ?? ''
      }

      this,this.accountService.adminLogin(loginRequest).subscribe({
        next: () => {
          this.accountService.getUserInfo().subscribe({
            next: () => {
              this.router.navigate(['admin/dashboard']);
              this.snackbarService.success("Successfully Login");
            }
          })
        },
        error: err => {
          this.snackbarService.error("Login failed")
        }
      })
    }


  }
}
