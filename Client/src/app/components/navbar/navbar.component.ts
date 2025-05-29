import { Component, inject, OnInit } from '@angular/core';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { AccountService } from '../../_services/account.service';
import { BsDropdownModule } from 'ngx-bootstrap/dropdown';
import { Router, RouterLink, RouterLinkActive } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { MembersService } from '../../_services/members.service';
import { HasRoleDirective } from '../../_directives/has-role.directive';

@Component({
  selector: 'app-navbar',
  imports: [FormsModule, BsDropdownModule, RouterLink, RouterLinkActive, HasRoleDirective],
  templateUrl: './navbar.component.html',
  styleUrl: './navbar.component.css'
})
export class NavbarComponent {

  accountService = inject(AccountService);
  private memberService = inject(MembersService);
  private router = inject(Router);
  private toastr = inject(ToastrService);
  model : any = {};
 
  login() {
    this.accountService.login(this.model).subscribe({
      next : () => {
        this.memberService.paginatedResult.set(null);
        this.memberService.reloadMembers.set(true);
        this.router.navigateByUrl('/members');
      },
      error : (err) => {
        console.log(err);
        this.toastr.error(err.error);
      }
    });
  }

  logout()
  {
    this.accountService.logout();
    this.router.navigateByUrl('/');
  }
}
