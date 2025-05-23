import { NgFor } from '@angular/common';
import { HttpClient } from '@angular/common/http';
import { Component, inject, Inject, OnInit } from '@angular/core';
import { RouterOutlet } from '@angular/router';

@Component({
  selector: 'app-root',
  imports: [NgFor],
  templateUrl: './app.component.html',
  styleUrl: './app.component.css'
})
export class AppComponent implements OnInit {

  private http = inject(HttpClient);
  title = 'Client';
  users : any;

  ngOnInit(): void {
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
