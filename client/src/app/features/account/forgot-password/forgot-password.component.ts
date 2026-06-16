import { Component, inject } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { AccountService } from '../../../core/services/account.service';
import { ResetPassword } from '../../../shared/models/resetPassword';
import { CommonModule } from '@angular/common';
import { strongPasswordValidator } from '../../../shared/validators/strongPasswordValidator';
import { PasswordFieldComponent } from "../../../shared/components/password-field/password-field.component";

@Component({
  selector: 'app-forgot-password',
  imports: [
    ReactiveFormsModule,
    CommonModule,
    PasswordFieldComponent
],
  templateUrl: './forgot-password.component.html',
  styleUrl: './forgot-password.component.scss',
})
export class ForgotPasswordComponent {
  private fb = inject(FormBuilder);
  private accountService = inject(AccountService);

  step = 1;
  loading = false;
  error = '';

  form = this.fb.group({
    email: ['', [Validators.required, Validators.email]],
    otp: [''],
    newPassword: ['', [Validators.required, strongPasswordValidator]]
  });

  sendOtp() {
    if (this.form.get('email')?.invalid) return;

    this.loading = true;
    this.accountService.sendForgotPasswordOtp(this.form.value.email!).subscribe({
      next: () => {
        this.step = 2;
        this.loading = false;
      },
      error: err => {
        this.error = err.error?.message || 'Failed to send OTP';
        this.loading = false;
      }
    });
  }

  resetPassword() {
    if (this.form.invalid) return;

    const payload: ResetPassword = {
      email: this.form.get('email')!.value!,
      otp: this.form.get('otp')!.value!,
      newPassword: this.form.get('newPassword')!.value!
    };

    this.loading = true;
    this.accountService.resetPassword(payload).subscribe({
      next: () => {
        this.step = 3;
        this.loading = false;
      },
      error: err => {
        this.error = err.error?.message || 'Invalid OTP';
        this.loading = false;
      }
    })
  }
}
