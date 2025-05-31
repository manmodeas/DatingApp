import { Component, inject, OnDestroy, OnInit, ViewChild } from '@angular/core';
import { MembersService } from '../../../_services/members.service';
import { ActivatedRoute, Router } from '@angular/router';
import { Member } from '../../../_models/member';
import { GalleryItem, GalleryModule, ImageItem } from 'ng-gallery';
import { TabDirective, TabsetComponent, TabsModule } from 'ngx-bootstrap/tabs';
import { TimeagoModule, TimeagoPipe } from 'ngx-timeago';
import { DatePipe } from '@angular/common';
import { MemberMessagesComponent } from "../member-messages/member-messages.component";
import { Message } from '../../../_models/message';
import { MessageService } from '../../../_services/message.service';
import { PresenceService } from '../../../_services/presence.service';
import { AccountService } from '../../../_services/account.service';
import { HubConnectionState } from '@microsoft/signalr';

@Component({
  selector: 'app-member-detail',
  imports: [TabsModule, GalleryModule, TimeagoModule, DatePipe, MemberMessagesComponent],
  templateUrl: './member-detail.component.html',
  styleUrl: './member-detail.component.css'
})
export class MemberDetailComponent implements OnInit, OnDestroy {  
  //static: false = Then we would have access to memberTabs after view in fully initiallized, which means it won't be available in OnInit()
  //static: true = Then it will be available in OnInit()
  @ViewChild('memberTabs', {static: true}) memberTabs?: TabsetComponent;
  presenceService = inject(PresenceService);
  private accountService = inject(AccountService);
  private memberService = inject(MembersService);
  private messageService = inject(MessageService);
  private route = inject(ActivatedRoute);   //Since we are routing to this component.. there no way for us to pass "member" from Member-Car/Member-List to pass to this component, So all we can do is retrive the username and ask for the data to serve again 
  private router = inject(Router);
  images: GalleryItem[] = [];
  //If we don't use resolver for member than be will have to make the 'member?' optiona/nullable
  //and would have add @If in html or user 'member!.___' like this 
  //So to save us the heache of doing that we can just retrive member befoer initializing the component with the help of "Resolver" 
  member: Member = {} as Member;  //We are sure we will get it from resolver 
  activeTab?: TabDirective;
  
  ngOnInit(): void {
    this.route.data.subscribe({
      next: data => {
        this.member = data['member'];
        this.member && this.member.photos.map(p => this.images.push(new ImageItem({src: p.url, thumb: p.url})))
      }
    })

    this.route.paramMap.subscribe({
      next: _ => this.onRouteParamsChange()
    })

    this.route.queryParams.subscribe({
      next: params => {
        params['tab'] && this.selectTab(params['tab'])
      }
    })
  }

  selectTab(heading: string) {
    if(this.memberTabs) {
      const messagetab = this.memberTabs.tabs.find(x => x.heading ===  heading);
      if(messagetab)  messagetab.active = true; 
    }
  }

  // onUpdateMessage(event: Message) {
  //   this.messages.push(event);
  // }
  onRouteParamsChange() {
    const user = this.accountService.currentUser();
    if(!user) return;
    if(this.messageService.hubConnection?.state === HubConnectionState.Connected && this.activeTab?.heading == 'Messages') {
      this.messageService.hubConnection.stop().then(() => {
        this.messageService.createHubConnection(user, this.member.userName);
      })
    }
  }

  onTabActivated(data: TabDirective) {
    this.activeTab = data;
    this.router.navigate([], {
      relativeTo: this.route,
      queryParams: {tab: this.activeTab.heading},
      queryParamsHandling: 'merge'
    });
    if(this.activeTab.heading === "Messages" && this.member) {
      //   this.messageService.getMessageThread(this.member.userName).subscribe({
      //   next: response => this.messages = response
      // })
      const user = this.accountService.currentUser();
      if(!user) return;
      this.messageService.createHubConnection(user, this.member.userName);
    } else {
      this.messageService.stopHubConnection();
    }
  }

  ngOnDestroy(): void {
    this.messageService.stopHubConnection();
  }
}
