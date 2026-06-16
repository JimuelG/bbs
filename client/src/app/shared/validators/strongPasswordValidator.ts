import { AbstractControl, ValidationErrors, ValidatorFn } from "@angular/forms";


export function strongPasswordValidator(): ValidatorFn {
    return (control: AbstractControl): ValidationErrors | null => {

        const value = control.value;
        
        if (!value) {
            return null;
        }
        
       const valueString = String(value);
    
        const hasMinLength = valueString.length >= 8;
        const hasCapital = /[A-Z]+/.test(valueString);
        const hasNumber = /[0-9]+/.test(valueString);
        const hasSpecial = /[^a-zA-Z0-9]+/.test(valueString);

        const validationErrors: ValidationErrors = {};
        if (!hasMinLength) {
        validationErrors['minlength'] = true;
        }
        if (!hasCapital) {
        validationErrors['capitalRequired'] = true;
        }
        if (!hasNumber) {
        validationErrors['numberRequired'] = true;
        }
        if (!hasSpecial) {
        validationErrors['specialRequired'] = true;
        }

        return Object.keys(validationErrors).length > 0 ? validationErrors : null;
    };
}