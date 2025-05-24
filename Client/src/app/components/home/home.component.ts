import { Component, inject } from '@angular/core';
import { RegisterComponent } from "../register/register.component";
import { HttpClient } from '@angular/common/http';

@Component({
  selector: 'app-home',
  imports: [RegisterComponent],
  templateUrl: './home.component.html',
  styleUrl: './home.component.css'
})
export class HomeComponent {
  registerMode = false;
  private http = inject(HttpClient);
  users : any;

  OnRegister() {
    this.registerMode = !this.registerMode;
  }

  OnCancelRegister(status : boolean) {
    this.registerMode = !this.registerMode;
    console.log("registeration cancelled.");
  }

    getUsers()
  {
    this.http.get("https://localhost:7286/api/users").subscribe({
        next: (response : any) => { this.users = response; },
        error: (error : any) => {
          console.error('Error fetching data:', error);
        },
        complete: () => {
          console.log('HTTP request completed');
        }
      });
  }
}
