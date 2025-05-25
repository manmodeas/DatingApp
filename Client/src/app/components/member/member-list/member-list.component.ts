import { Component, inject, OnInit } from '@angular/core';
import { MembersService } from '../../../_services/members.service';
import { Member } from '../../../_models/member';
import { MemberCardComponent } from "../member-card/member-card.component";

@Component({
  selector: 'app-member-list',
  imports: [MemberCardComponent],
  templateUrl: './member-list.component.html',
  styleUrl: './member-list.component.css'
})
export class MemberListComponent implements OnInit{
  
  private memberService = inject(MembersService);
  members: Member[] = [];
  models: boolean = false;
  ngOnInit(): void {
    this.loadMembers();
  }

  loadMembers() {
    this.memberService.getmembers().subscribe({
      next: members => this.members = members
    })
  }
}
