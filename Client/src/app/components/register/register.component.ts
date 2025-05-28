import { Component, inject, OnInit, output } from '@angular/core';
import { AbstractControl, FormBuilder, FormControl, FormGroup, ReactiveFormsModule, ValidatorFn, Validators } from '@angular/forms';
import { AccountService } from '../../_services/account.service';
import { Router } from '@angular/router';
import { TextInputComponent } from "../_forms/text-input/text-input.component";
import { DatePickerComponent } from "../_forms/date-picker/date-picker.component";
import { MembersService } from '../../_services/members.service';

@Component({
  selector: 'app-register',
  imports: [ReactiveFormsModule, TextInputComponent, DatePickerComponent],
  templateUrl: './register.component.html',
  styleUrl: './register.component.css'
})
export class RegisterComponent implements OnInit {
  
  private accountService = inject(AccountService);
  private memberService = inject(MembersService);
  private fb = inject(FormBuilder);
  private router = inject(Router);
  cancelRegister = output<boolean>();  
  model : any = {};
  registerForm: FormGroup = new FormGroup({});
  maxDate = new Date();
  validationErrors: string[] | undefined;

  ngOnInit(): void {
    this.initializeRegisterForm();
    this.maxDate.setFullYear(this.maxDate.getFullYear() - 18);
  }
  
  initializeRegisterForm() {

    this.registerForm = this.fb.group({
      gender: ['male'],
      username: ['', Validators.required],
      knownAs: ['', Validators.required],
      dateOfBirth: ['', Validators.required],
      city: ['', Validators.required],
      country: ['', Validators.required],
      password: ['', [Validators.required, Validators.minLength(4), Validators.maxLength(8)]],
      confirmPassword: ['', [Validators.required, this.matchValues('password')]]
    });

    //Below is also a way of using formgroug
    // this.registerForm = new FormGroup({
    //   username: new FormControl('', Validators.required),
    //   password: new FormControl('', [Validators.required, Validators.minLength(4), Validators.maxLength(8)]),
    //   confirmPassword: new FormControl('', [Validators.required, this.matchValues('password')])
    // })

    //below finction is used to monitor changes in password field 
    //So whenever password change it will trigger updateValueValidity() for 'confirmPassword'
    //this way we can have validation on both way 
    this.registerForm.controls['password'].valueChanges.subscribe({
      next: () => {
        this.registerForm.controls['confirmPassword'].updateValueAndValidity();
      }
    })
  }

  matchValues(matchTo: string): ValidatorFn {
    return (control: AbstractControl) => {
      return control.value === control.parent?.get(matchTo)?.value ? null : {isMatching: true}; //Return tru means its not matching ... we will also be using it in form for validation feedback
    }  
  }

  register() {
    const dob = this.getDateOnly(this.registerForm.get("dateOfBirth")?.value);
    this.registerForm.patchValue({dateOfBirth: dob});
    this.accountService.register(this.registerForm.value).subscribe({
      next: _ => {
        this.memberService.reloadMembers.set(true);
        this.router.navigateByUrl("/members")
      },
      error: (err) => {
        this.validationErrors = err;
      }
    })
  } 

  cancel() {
    this.cancelRegister.emit(false);
  }  

  private getDateOnly(dob: string | undefined) {
    if(!dob) return;
    return new Date(dob).toISOString().slice(0,10);
  }
}
