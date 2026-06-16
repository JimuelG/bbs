import { Component, forwardRef, Input } from '@angular/core';
import { ControlValueAccessor, NG_VALUE_ACCESSOR } from '@angular/forms';
import { MatIcon } from '@angular/material/icon';

@Component({
  selector: 'app-password-field',
  imports: [
    MatIcon
  ],
  templateUrl: './password-field.component.html',
  styleUrl: './password-field.component.scss',
  providers: [
    {
      provide: NG_VALUE_ACCESSOR,
      useExisting: forwardRef(() => PasswordFieldComponent),
      multi: true
    }
  ]
})

export class PasswordFieldComponent implements ControlValueAccessor {
  @Input() label = 'Password';

  value = '';
  showPassword = false;
  disabled = false;

  onChange = (value: string) => {};
  onTouched = () => {};

  writeValue(value: string): void {
    this.value = value || '';    
  }

  registerOnChange(fn: any): void {
    this.onChange = fn;
  }

  registerOnTouched(fn: any): void {
    this.onTouched = fn;
  }

  setDisabledState(isDisabled: boolean): void {
    this.disabled = isDisabled;
  }

  toggleVisibility() {
    this.showPassword = !this.showPassword;
  }

  updateValue(value: string) {
    this.value = value;
    this.onChange(value);
  }

  rules = {
    length: false,
    uppercase: false,
    number: false,
    special: false
  }

  checkRules(password: string) {
    this.rules.length = password.length >= 8;
    this.rules.uppercase = /[A-Z]/.test(password);
    this.rules.number = /\d/.test(password);
    this.rules.special = /[^A-Za-z0-9]/.test(password);
  }

  get strength(): number {
    return Object.values(this.rules).filter(Boolean).length;
  }

  get strengthLabel(): string {
    return ['Weak', 'Fair', 'Good', 'Strong'][this.strength - 1] || 'Weak';
  }

  get strengthColor(): string {
    return ['bg-red-500', 'bg-yellow-400', 'bg-blue-500', 'bg-green-600'][this.strength - 1] || 'bg-red-500';
  }

}
