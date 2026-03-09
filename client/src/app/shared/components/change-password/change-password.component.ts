import { Component, inject } from '@angular/core';
import { AbstractControl, FormBuilder, FormGroup, ReactiveFormsModule, ValidationErrors, Validators } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { User } from '../../models/user';
import { ValidationError } from '@angular/forms/signals';
import { CommonModule } from '@angular/common';
import { AccountService } from '../../../core/services/account.service';
import { ChangePassword } from '../../models/changePassword';
import { SnackbarService } from '../../../core/services/snackbar.service';

@Component({
  selector: 'app-change-password',
  imports: [
    CommonModule,
    ReactiveFormsModule
  ],
  templateUrl: './change-password.component.html',
  styleUrl: './change-password.component.scss',
})
export class ChangePasswordComponent {
  private fb = inject(FormBuilder);
  private dialogRef = inject(MatDialogRef);
  private accountService = inject(AccountService);
  private snackbarService = inject(SnackbarService);

  public data = inject<User | null>(MAT_DIALOG_DATA);

  passwordForm: FormGroup;
  loading = false;

  hideCurrent = true;
  hideNew = true;
  hideConfirm = true;

  constructor() {
    this.passwordForm = this.fb.group({
      oldPassword: ['', Validators.required],
      newPassword: ['', [Validators.required, Validators.minLength(0)]],
      confirmPassword: ['', Validators.required]
    }, { validators: this.passwordMismatchValidator })
  }

  passwordMismatchValidator(control: AbstractControl): ValidationErrors | null {
    const newPass = control.get('newPassword')?.value;
    const confirmPass = control.get('confirmPassword')?.value;
    return newPass === confirmPass ? null : { mismatch: true }
  }

  onSubmit() {
    if (this.passwordForm.valid) {
      this.loading = true;

      const payload: ChangePassword = {
        oldPassword: this.passwordForm.value.oldPassword,
        newPassword: this.passwordForm.value.newPassword
      }

      this.accountService.changePassword(this.data?.id!, payload).subscribe({
        next: () => {
          this.snackbarService.success(`Password updated successfully`);
          this.dialogRef.close(true);
        },
        error: (err) => {
          this.snackbarService.error(`Password update failed: ${err}`)
          this.loading = false;
        }
      })
    }
  }

  onCancel() {
    this.dialogRef.close();
  }
}
