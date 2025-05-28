import { NgIf } from '@angular/common';
import { Component, input, Self } from '@angular/core';
import { ControlValueAccessor, FormControl, NgControl, ReactiveFormsModule } from '@angular/forms';

@Component({
  selector: 'app-text-input',
  imports: [ReactiveFormsModule, NgIf],
  templateUrl: './text-input.component.html',
  styleUrl: './text-input.component.css'
})
export class TextInputComponent implements ControlValueAccessor {
  label = input<string>('');
  type = input<string>('text');

  //@Self() -> Only look for the dependency on this component/directive. Don’t search parents.
  //Use when -	You want to limit where Angular looks for a service and fail fast if not found.
  //Combine with -	@Optional() if you want to avoid errors when the service is not found.

  /*
  Analogy: The Vending Machine
      Imagine you're in a building and every floor has its own vending machine (injector).
      You want a can of soda (a service).

      By default, if the vending machine on your floor doesn't have it, you go up to the next floor, and keep going until you find one.

      Now:
      If you use @Self(), you're saying:
      "Check only this floor's vending machine. If there’s no soda here, I don't want it at all. Do not go upstairs!"
  */


  /*
    NgControl is the base class for all Angular form control directives (FormControlDirective, FormControlName, etc.).
    If a component wants to connect itself to Angular forms, it can use NgControl to hook into the form control system.
  */
  constructor(@Self() public ngControl: NgControl) {
    /*
    This line is critical.
    It tells Angular:
    "I (this component) am the ControlValueAccessor for this form control."
    This means Angular will now use your component’s methods (writeValue, registerOnChange, etc.) to manage the value and state of the form field.
    */
    this.ngControl.valueAccessor = this;
  }

  writeValue(obj: any): void {
    
  }
  registerOnChange(fn: any): void {
    
  }
  registerOnTouched(fn: any): void {
    
  }
  //getter
  get control(): FormControl {
    return this.ngControl.control as FormControl
  }
}
