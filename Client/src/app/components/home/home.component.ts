import { Component, inject, OnInit } from '@angular/core';
import { RegisterComponent } from "../register/register.component";
import { AccountService } from '../../_services/account.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-home',
  imports: [RegisterComponent],
  templateUrl: './home.component.html',
  styleUrl: './home.component.css'
})
export class HomeComponent implements OnInit{
  accountService = inject(AccountService);
  private router = inject(Router);
  registerMode = false;
  users : any;

  ngOnInit(): void {
    if(this.accountService.currentUser()) {
      this.router.navigateByUrl("/members");
    }
  }

  OnRegister() {
    this.registerMode = !this.registerMode;
  }

  OnCancelRegister(status : boolean) {
    this.registerMode = !this.registerMode;
    console.log("registeration cancelled.");
  }
}
