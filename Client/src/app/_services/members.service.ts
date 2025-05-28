import { HttpClient, HttpParams, HttpResponse } from '@angular/common/http';
import { inject, Injectable, model, signal, Signal } from '@angular/core';
import { environment } from '../../environments/environment';
import { Member } from '../_models/member';
import { Photo } from '../_models/photo';
import { PaginatedResult } from '../_models/pagination';
import { UserParam } from '../_models/userParams';
import { of } from 'rxjs';
import { AccountService } from './account.service';

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
  user = this.accountService.currentUser();
  userParams = signal<UserParam>(new UserParam(this.user));

  resetUserParams() {
    this.userParams.set(new UserParam(this.user));
  }

  getmembers() {
    
    // console.log(Object.values(this.userParams()).join('-'));
    const response = this.memberCache.get(Object.values(this.userParams()).join('-'));
    if(response) return this.setPaginatedResponse(response);
    
    let params = this.setPaginationHeaders(this.userParams().pageNumber, this.userParams().pageSize);

    params = params.append('minAge', this.userParams().minAge);
    params = params.append('maxAge', this.userParams().maxAge);
    params = params.append('gender', this.userParams().gender);
    params = params.append('orderBy', this.userParams().orderBy);

    return this.http.get<Member[]>(this.baseUrl+'users', {observe: 'response', params}).subscribe({
      next: response => {
        this.setPaginatedResponse(response);
        this.memberCache.set(Object.values(this.userParams).join('-'), response);
      }
    });
  }

  private setPaginatedResponse(response: HttpResponse<Member[]>) {
    this.paginatedResult.set({
      items: response.body as Member[],
      pagination: JSON.parse(response.headers.get("Pagination")!)
    })
    this.reloadMembers.set(false);
  }

  private setPaginationHeaders(pageNumber: number, pageSize: number) {
    let params = new HttpParams();

    if(pageNumber && pageSize){
      params = params.append("pageNumber", pageNumber);
      params = params.append("pageSize", pageSize);
    }

    return params;
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
