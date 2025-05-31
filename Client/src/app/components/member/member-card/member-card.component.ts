import { Component, computed, inject, input } from '@angular/core';
import { Member } from '../../../_models/member';
import { RouterLink } from '@angular/router';
import { LikesService } from '../../../_services/likes.service';
import { PresenceService } from '../../../_services/presence.service';

@Component({
  selector: 'app-member-card',
  imports: [RouterLink],
  templateUrl: './member-card.component.html',
  styleUrl: './member-card.component.css'
})
export class MemberCardComponent {
  private presenceService = inject(PresenceService);
  private likeService = inject(LikesService);
  member  = input.required<Member>();
  hasLiked = computed(() => this.likeService.likeIds().includes(this.member().id))
  isOnline = computed(() => this.presenceService.onlineUsers().includes(this.member().userName));

  toggelLike() {
    this.likeService.toggelLike(this.member().id).subscribe({
      next: _ => {
        if(this.hasLiked())
          this.likeService.likeIds.update(ids => ids.filter(id => id != this.member().id))
        else
          this.likeService.likeIds.update(ids => [...ids, this.member().id])
      }
    })
  }
}
