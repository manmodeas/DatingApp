import { Component, inject, OnInit, ViewChild } from '@angular/core';
import { MembersService } from '../../../_services/members.service';
import { PaginationModule } from 'ngx-bootstrap/pagination';
import { FormsModule } from '@angular/forms';
import { AccountService } from '../../../_services/account.service';
import { MemberCardComponent } from "../../member/member-card/member-card.component";
import { Member } from '../../../_models/member';
import { PhotoManagementService } from '../../../_services/photo-management.service';
import { GalleryComponent, GalleryItem, GalleryModule, GalleryRef, ImageItem } from 'ng-gallery';
import { BehaviorSubject } from 'rxjs';

@Component({
  selector: 'app-photo-management',
  imports: [PaginationModule, FormsModule, MemberCardComponent, GalleryModule],
  templateUrl: './photo-management.component.html',
  styleUrl: './photo-management.component.css'
})
export class PhotoManagementComponent implements OnInit{
  @ViewChild('myGallery') myGallery!: GalleryComponent;
  memberService = inject(MembersService);
  accountService = inject(AccountService);
  photoManageService = inject(PhotoManagementService)
  member?: Member;
  images: GalleryItem[] = [];
  imageIndex = 0;

  ngOnInit(): void {
    if(!this.photoManageService.paginatedResult()) this.loadMembers();
  } 

  loadMembers() {
    this.photoManageService.getmembers();
  }

  resetFilters() {
    this.loadMembers();
  }
  
  pagedChanged(event: any) {
    if(this.photoManageService.pageNumber != event.page) {
      this.photoManageService.pageNumber = event.page;
      this.loadMembers();
    }
  }

  onClick(member: Member) {
    this.member = member;
    this.member.photos = this.member.photos.filter(p => !p.isApproved)
    this.images = [];
    this.member && this.member.photos.map(p => this.images.push(new ImageItem({src: p.url, thumb: p.url})))
    // console.log(this.images.length)
  }

  onItemChange(index: any) {
    // console.log(index)
    this.imageIndex = index.currIndex;
    
  }
 
  onApprove() {
    const photoId = this.member?.photos[this.imageIndex].id;
    if(photoId)
    {
      this.photoManageService.imageApproved(this.member!, photoId);
      this.myGallery.remove(this.myGallery.galleryRef.stateSnapshot.currIndex!)
    }
    
  }
}
