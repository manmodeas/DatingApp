import { Component, inject, OnInit, ViewChild } from '@angular/core';
import { MembersService } from '../../../_services/members.service';
import { ActivatedRoute } from '@angular/router';
import { Member } from '../../../_models/member';
import { GalleryItem, GalleryModule, ImageItem } from 'ng-gallery';
import { TabDirective, TabsetComponent, TabsModule } from 'ngx-bootstrap/tabs';
import { TimeagoModule, TimeagoPipe } from 'ngx-timeago';
import { DatePipe } from '@angular/common';
import { MemberMessagesComponent } from "../member-messages/member-messages.component";
import { Message } from '../../../_models/message';
import { MessageService } from '../../../_services/message.service';

@Component({
  selector: 'app-member-detail',
  imports: [TabsModule, GalleryModule, TimeagoModule, DatePipe, MemberMessagesComponent],
  templateUrl: './member-detail.component.html',
  styleUrl: './member-detail.component.css'
})
export class MemberDetailComponent implements OnInit {  
  //static: false = Then we would have access to memberTabs after view in fully initiallized, which means it won't be available in OnInit()
  //static: true = Then it will be available in OnInit()
  @ViewChild('memberTabs', {static: true}) memberTabs?: TabsetComponent;
  private memberService = inject(MembersService);
  private messageService = inject(MessageService);
  private route = inject(ActivatedRoute);   //Since we are routing to this component.. there no way for us to pass "member" from Member-Car/Member-List to pass to this component, So all we can do is retrive the username and ask for the data to serve again 
  images: GalleryItem[] = [];
  //If we don't use resolver for member than be will have to make the 'member?' optiona/nullable
  //and would have add @If in html or user 'member!.___' like this 
  //So to save us the heache of doing that we can just retrive member befoer initializing the component with the help of "Resolver" 
  member: Member = {} as Member;  //We are sure we will get it from resolver 
  activeTab?: TabDirective;
  messages: Message[] = [];
  
  ngOnInit(): void {
    this.route.data.subscribe({
      next: data => {
        this.member = data['member'];
        this.member && this.member.photos.map(p => this.images.push(new ImageItem({src: p.url, thumb: p.url})))
      }
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

  onUpdateMessage(event: Message) {
    this.messages.push(event);
  }

  onTabActivated(data: TabDirective) {
    this.activeTab = data;
    if(this.activeTab.heading === "Messages" && this.messages.length === 0 && this.member) {
        this.messageService.getMessageThread(this.member.userName).subscribe({
        next: response => this.messages = response
      })
    }
  }
}
