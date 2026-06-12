import { CommonModule } from '@angular/common';
import { Component, inject } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatIcon } from '@angular/material/icon';
import { Router, RouterModule } from '@angular/router';
import { AccountService } from '../../../core/services/account.service';
import { of, switchMap } from 'rxjs';
import { SnackbarService } from '../../../core/services/snackbar.service';

@Component({
  selector: 'app-register',
  imports: [
    MatIcon,
    CommonModule,
    ReactiveFormsModule,
    RouterModule
  ],
  templateUrl: './register.component.html',
  styleUrl: './register.component.scss',
})
export class RegisterComponent {
  private fb = inject(FormBuilder);
  private accountService = inject(AccountService);
  private router = inject(Router);
  private snackbarService = inject(SnackbarService);

  registerForm: FormGroup;
  loading = false;
  selectedFile: File | null = null;

  constructor() {
    this.registerForm = this.fb.group({
      firstName: ['', Validators.required],
      middleName: [''],
      lastName: ['', Validators.required],
      email: ['', [Validators.required, Validators.email]],
      purok: ['', Validators.required],
      civilStatus: ['', Validators.required],
      phoneNumber: ['', [Validators.required, Validators.pattern('^[0-9]*$')]],
      password: ['', [Validators.required, Validators.minLength(6)]],
    })
  }

  onSubmit() {
    if (this.registerForm.valid) {
      this.loading = true;

      this.accountService.register(this.registerForm.value).pipe(
        switchMap(() => {
          if (this.selectedFile) {
            return this.accountService.uploadIdCard(this.registerForm.value.email, this.selectedFile);
          }
          return of(null);
        })
      ).subscribe({
        next: () => this.completeRegistration(),
        error: () => this.loading = false
      });
    }
  }

  private completeRegistration() {
    this.loading = false;
    this.snackbarService.success('Account successfully created. You can login your account.');
    this.router.navigateByUrl('/login');
  }

  onFileSelected(event: any) {
    const file: File = event.target.files[0];
    if (file) {
      this.selectedFile = file;
    }
  }
}
