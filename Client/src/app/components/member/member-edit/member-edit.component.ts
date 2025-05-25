import { Component, HostListener, inject, OnInit, ViewChild } from '@angular/core';
import { Member } from '../../../_models/member';
import { AccountService } from '../../../_services/account.service';
import { MembersService } from '../../../_services/members.service';
import { TabsModule } from 'ngx-bootstrap/tabs';
import { FormsModule, NgForm, NgModel } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-member-edit',
  imports: [TabsModule, FormsModule],
  templateUrl: './member-edit.component.html',
  styleUrl: './member-edit.component.css'
})
export class MemberEditComponent implements OnInit{

  //Since we can't directly access form even using "editForm" id
  //We make use of ViewChild since form is a child of our component
  @ViewChild('editForm') editForm?: NgForm;

  //We are using canDeactivate guard to notify user if they are sure about leaving the page 
  //but there are cases in which our guard won't work 
  //for example: our browser functions, if you click home or close the tab... in this cases guard won't be of any help
  //To tackel this situation we use following..
  //beforeunload in window event
  //[$event]  = is parameter we are asking for from event
  @HostListener('window:beforeunload', ['$event']) notify($event: any) {
    if(this.editForm?.dirty)
    {
      $event.returnValue = true; 
    }
  }

  private accountService = inject(AccountService);
  private memberService = inject(MembersService);
  private toastr = inject(ToastrService);
  member?: Member;

  ngOnInit(): void {
    this.loadMember();
  }

  loadMember() {
    const user = this.accountService.currentUser();
    if(!user) return;

    this.memberService.getMember(user.username).subscribe({
      next: member => this.member = member
    })
  }
  
  updateMember() {
    this.memberService.updateMember(this.editForm?.value).subscribe({
      next: _ => {
        this.toastr.success('Profile updated successfully');
        this.editForm?.reset(this.member); 
      }
    });
    
  }
}
