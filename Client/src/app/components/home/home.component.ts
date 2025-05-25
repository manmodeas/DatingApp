import { Component } from '@angular/core';
import { RegisterComponent } from "../register/register.component";

@Component({
  selector: 'app-home',
  imports: [RegisterComponent],
  templateUrl: './home.component.html',
  styleUrl: './home.component.css'
})
export class HomeComponent {
  registerMode = false;
  users : any;

  OnRegister() {
    this.registerMode = !this.registerMode;
  }

  OnCancelRegister(status : boolean) {
    this.registerMode = !this.registerMode;
    console.log("registeration cancelled.");
  }
}
