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
export class PhotoManagementService {

  private http = inject(HttpClient);
  baseUrl = environment.apiUrl;
  paginatedResult = signal<PaginatedResult<Member[]> | null>(null);
  memberCache = new Map();
  pageNumber = 1;
  pageSize = 5;

  getmembers() {    
    // console.log(Object.values(this.userParams()).join('-'));
    const response = this.memberCache.get(this.pageNumber);
    if(response) return setPaginatedResponse(response, this.paginatedResult);
    // console.log('I am here')
    let params = setPaginationHeaders(this.pageNumber, this.pageSize);

    return this.http.get<Member[]>(this.baseUrl+'users/all-users', {observe: 'response', params}).subscribe({
      next: response => {
        setPaginatedResponse(response, this.paginatedResult);
        this.memberCache.set(this.pageNumber, response);
      }
    });
  }  

  imageApproved(user: Member, photoId: number) {
    let params = new HttpParams();
    params.append('username', user.userName)
    params.append('photoId', photoId)
    // console.log(this.baseUrl + 'users/approve-photo/' + user.userName + '?photoId=' + photoId)
    return this.http.put(this.baseUrl + 'users/approve-photo/' + user.userName + '?photoid=' + photoId, {}).subscribe({
      next: _ => {
        const member = this.paginatedResult()?.items?.find(m => m.userName === user.userName);
        if(member) {
          const photo = member.photos.find(x => x.id === photoId);
          if(photo) {
            photo.isApproved = true;
            if(!member.photoUrl) member.photoUrl = photo.url;
          }
        }
      }
    });
  }

}
