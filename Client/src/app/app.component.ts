
import { HttpClient } from '@angular/common/http';
import { Component, inject, Inject, OnInit } from '@angular/core';
import { NavbarComponent } from "./components/navbar/navbar.component";
import { AccountService } from './_services/account.service';
import { Router, RouterOutlet } from '@angular/router';

@Component({
  selector: 'app-root',
  imports: [RouterOutlet, NavbarComponent],
  templateUrl: './app.component.html',
  styleUrl: './app.component.css'
})
export class AppComponent implements OnInit {

  private http = inject(HttpClient);
  private accountService = inject(AccountService);
  private router = inject(Router);
  title = 'Dating App';
  users : any;

  ngOnInit(): void {

      this.setCurrentUser();
  }

  setCurrentUser() {
    const userString = localStorage.getItem('user');
    if(!userString) return;

    const user  = JSON.parse(userString);
    this.accountService.currentUser.set(user);
    this.router.navigateByUrl('/members');
  }


}
