import { Component, inject, OnInit } from '@angular/core';
import { MembersService } from '../../../_services/members.service';
import { PaginationModule } from 'ngx-bootstrap/pagination';
import { ButtonsModule } from 'ngx-bootstrap/buttons';
import { MemberCardComponent } from "../member-card/member-card.component";
import { UserParam } from '../../../_models/userParams';
import { FormsModule } from '@angular/forms';
import { AccountService } from '../../../_services/account.service';

@Component({
  selector: 'app-member-list',
  imports: [MemberCardComponent, PaginationModule, FormsModule, ButtonsModule],
  templateUrl: './member-list.component.html',
  styleUrl: './member-list.component.css'
})
export class MemberListComponent implements OnInit{
  memberService = inject(MembersService);
  accountService = inject(AccountService);
  genderList = [{value: 'male', display: 'Males'}, {value: 'female', display: 'Females'}]

  ngOnInit(): void {
    if(!this.memberService.paginatedResult() || this.memberService.reloadMembers()) this.loadMembers();
  } 

  loadMembers() {
    this.memberService.getmembers();
  }

  resetFilters() {
    this.loadMembers();
  }
  
  pagedChanged(event: any) {
    if(this.accountService.userParams().pageNumber != event.page) {
      this.accountService.userParams().pageNumber = event.page;
      this.loadMembers();
    }
  }

}
