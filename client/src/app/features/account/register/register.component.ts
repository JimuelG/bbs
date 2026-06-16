import { CommonModule } from '@angular/common';
import { Component, inject, OnInit } from '@angular/core';
import { AbstractControl, AsyncValidatorFn, FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatIcon } from '@angular/material/icon';
import { Router, RouterModule } from '@angular/router';
import { AccountService } from '../../../core/services/account.service';
import { catchError, map, of, switchMap } from 'rxjs';
import { SnackbarService } from '../../../core/services/snackbar.service';
import { PasswordFieldComponent } from "../../../shared/components/password-field/password-field.component";
import { strongPasswordValidator } from '../../../shared/validators/strongPasswordValidator';

@Component({
  selector: 'app-register',
  imports: [
    MatIcon,
    CommonModule,
    ReactiveFormsModule,
    RouterModule,
    PasswordFieldComponent,
],
  templateUrl: './register.component.html',
  styleUrl: './register.component.scss',
})
export class RegisterComponent implements OnInit {
  private fb = inject(FormBuilder);
  private accountService = inject(AccountService);
  private router = inject(Router);
  private snackbarService = inject(SnackbarService);

  registerForm: FormGroup;
  loading = false;
  selectedFile: File | null = null;
  fileError: string | null = null;
  currentStep = 1;
  emailChecking = false;
  emailTaken = false;
  hidePassword = true;

  private STEP1_STORAGE_KEY = 'barangay-register-step1';

  ngOnInit() {
    const saved = localStorage.getItem(this.STEP1_STORAGE_KEY);
    if (saved) {
      this.registerForm.patchValue(JSON.parse(saved));
    }

    this.registerForm.valueChanges.subscribe(values => {
      const step1Data = {
        firstName: values.firstName,
        middleName: values.middleName,
        lastName: values.lastName,
        birthDate: values.birthDate,
        civilStatus: values.civilStatus,
        purok: values.purok,
        phoneNumber: values.phoneNumber,
      };
      localStorage.setItem(this.STEP1_STORAGE_KEY, JSON.stringify(step1Data));
    })
  }

  constructor() {
    this.registerForm = this.fb.group({
      firstName: ['', Validators.required],
      middleName: [''],
      lastName: ['', Validators.required],
      email: ['', [Validators.required, Validators.email, this.emailTakenValidator]],
      purok: ['', Validators.required],
      civilStatus: ['', Validators.required],
      birthDate: ['', Validators.required],
      phoneNumber: ['', [Validators.required, Validators.pattern('^[0-9]*$')]],
      password: ['', [Validators.required, strongPasswordValidator]],
    })
  }

  nextStep() {
    if (this.isStepValid()) {
      this.currentStep = 2;
    } else {
      this.markStep1Touched();
    }
  }

  previousStep() {
    this.currentStep = 1;
  }

  isStepValid(): boolean {
    const controls = [
      'firstName',
      'lastName',
      'birthDate',
      'civilStatus',
      'purok',
      'phoneNumber'
    ]
    return controls.every(c => this.registerForm.get(c)?.valid);
  };

  markStep1Touched() {
    [
      'firstName',
      'lastName',
      'birthDate',
      'civilStatus',
      'purok',
      'phoneNumber',
    ].forEach(c => this.registerForm.get(c)?.markAsTouched());
  }

  emailTakenValidator(): AsyncValidatorFn {
    return (control: AbstractControl) => {
      if (!control.value) return of(null);

      return this.accountService.checkEmailExists(control.value).pipe(
        map(exists => (exists ? { emailTaken: true } : null )),
        catchError(() => of(null))
      )
    }
  }


  async onSubmit() {
    if (this.registerForm.invalid || !this.selectedFile) return;

    this.emailChecking = true;

    this.accountService.checkEmailExists(this.registerForm.value.email).subscribe({
      next: exists => {
        this.emailChecking = false;

        if (exists) {
          this.emailTaken = true;
          this.registerForm.get('email')?.setErrors({ emailTaken: true });
          return;
        }

        this.emailTaken = false;
        this.finalSubmit();
      },
      error: () => {
        this.emailChecking = false;
      }
    })
  }

  finalSubmit() {
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

  private completeRegistration() {
    this.loading = false;
    localStorage.removeItem(this.STEP1_STORAGE_KEY);
    this.snackbarService.success('Account successfully created. You can login your account.');
    this.router.navigateByUrl('/account/login');
  }

  onFileSelected(event: any) {
    const file: File = event.target.files[0];

    if (!file) {
      this.fileError = 'Valid ID is required.';
      return;
    }

    const allowedTypes = ['image/jpeg', 'image/jpg', 'image/png'];

    if (!allowedTypes.includes(file.type)) {
      this.fileError = 'Only JPG or PNG files are allowed.';
      return;
    }

    this.selectedFile = file;
    this.fileError = null;
  }

  hasError(control: string, error: string) {
    const c = this.registerForm.get(control);
    return c?.touched && c?.hasError(error);
  }
}
