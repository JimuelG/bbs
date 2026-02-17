import { CommonModule } from '@angular/common';
import { Component, inject } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatIcon } from '@angular/material/icon';
import { Router, RouterModule } from '@angular/router';
import { AccountService } from '../../../core/services/account.service';

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

  registerForm: FormGroup;
  loading = false;

  constructor() {
    this.registerForm = this.fb.group({
      firstName: ['', Validators.required],
      lastName: ['', Validators.required],
      purok: ['', Validators.required],
      phoneNumber: ['', [Validators.required, Validators.pattern('^[0-9]*$')]],
      email: ['', [Validators.required, Validators.email]],
      password: ['', Validators.required, Validators.minLength(6)],
      idUrl: ['']
    })
  }

  onSubmit() {
    if (this.registerForm.valid) {
      this.loading = true;
      this.accountService.register(this.registerForm.value).subscribe({
        next: () => {
          this.router.navigateByUrl('/login');
        },
        error: (err) => {
          this.loading = false;
          
        }
      })
    }
  }
}
