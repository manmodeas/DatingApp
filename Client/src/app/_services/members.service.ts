import { HttpClient, HttpParams, HttpResponse } from '@angular/common/http';
import { inject, Injectable, model, signal, Signal } from '@angular/core';
import { environment } from '../../environments/environment';
import { Member } from '../_models/member';
import { Photo } from '../_models/photo';
import { PaginatedResult } from '../_models/pagination';
import { UserParam } from '../_models/userParams';
import { of } from 'rxjs';
import { AccountService } from './account.service';
import { setPaginatedResponse, setPaginationHeaders } from './paginationHelper';

@Injectable({
  providedIn: 'root'
})
export class MembersService {

  private http = inject(HttpClient);
  private accountService = inject(AccountService);
  baseUrl = environment.apiUrl;
  // members = signal<Member[]>([]);
  reloadMembers = signal<Boolean>(false); //Used when 
  paginatedResult = signal<PaginatedResult<Member[]> | null>(null);
  memberCache = new Map();
  

  getmembers() {
    
    // console.log(Object.values(this.userParams()).join('-'));
    const response = this.memberCache.get(Object.values(this.accountService.userParams()).join('-'));
    if(response) return setPaginatedResponse(response, this.paginatedResult);
    // console.log('I am here')
    let params = setPaginationHeaders(this.accountService.userParams().pageNumber, this.accountService.userParams().pageSize);

    params = params.append('minAge', this.accountService.userParams().minAge);
    params = params.append('maxAge', this.accountService.userParams().maxAge);
    params = params.append('gender', this.accountService.userParams().gender);
    params = params.append('orderBy', this.accountService.userParams().orderBy);

    return this.http.get<Member[]>(this.baseUrl+'users', {observe: 'response', params}).subscribe({
      next: response => {
        setPaginatedResponse(response, this.paginatedResult);
        this.memberCache.set(Object.values(this.accountService.userParams()).join('-'), response);
        this.reloadMembers.set(false);
      }
    });
  }

  getMember(username : string)
  {
    //old- const member = this.members().find(x => x.userName === username);
    //old- if(member !== undefined) return of(member);

    const member: Member = [...this.memberCache.values()]
        .reduce((arr, elem) => arr.concat(elem.body), [])
        .find((m: Member) => m.userName == username);

    if(member) return of(member);

    return this.http.get<Member>(this.baseUrl + 'users/' + username); 
  }

  updateMember(member: Member) {
    return this.http.put(this.baseUrl + 'users', member).pipe(
      // tap (() => {
      //   this.members.update(members => members.map(m => m.userName === member.userName ? member : m))
      // })
    );
  }

  setMainPhoto(photo: Photo) {
    return this.http.put(this.baseUrl + 'users/set-main-photo/' + photo.id, {}).pipe(
      // tap(() => {
      //   this.members.update(members => members.map(m => {
      //     if(m.photos.includes(photo))
      //     {
      //       m.photoUrl = photo.url;
      //     }
      //     return m;
      //   }))
      // })
    );
  }

  deletePhoto(photo: Photo) {
    return this.http.delete(this.baseUrl + 'users/delete-photo/' + photo.id).pipe(
      // tap(() => {
      //   this.members.update( members => members.map(m => {
      //     m.photos = m.photos.filter(p => p.id !== photo.id);
      //     return m;
      //   }))
      // })
    );
  }

}
