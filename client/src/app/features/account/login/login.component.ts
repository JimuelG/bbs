import { Component, inject } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatIconModule } from '@angular/material/icon';
import { AccountService } from '../../../core/services/account.service';
import { Router, RouterModule } from '@angular/router';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-login',
  imports: [
    CommonModule,
    ReactiveFormsModule,
    RouterModule,
    MatIconModule
  ],
  templateUrl: './login.component.html',
  styleUrl: './login.component.scss',
})
export class LoginComponent {
  private fb = inject(FormBuilder);
  private accountService = inject(AccountService);
  private router = inject(Router);

  loginForm: FormGroup;
  loading = false;
  hidePassword = true;

  constructor() {
    this.loginForm = this.fb.group({
      email: ['', [Validators.required, Validators.email]],
      password: ['', [Validators.required, Validators.minLength(6)]]
    })
  }

  onSubmit() {
    if (this.loginForm.valid) {
      this.loading = true;
      this.accountService.login(this.loginForm.value).subscribe({
        next: () => {
          this.router.navigateByUrl('/');
        },
        error: (err) => {
          this.loading = false
        }
      });
    }
  }
}
