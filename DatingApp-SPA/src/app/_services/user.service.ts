import { map } from 'rxjs/operators';
import { PaginatedResult } from './../_models/pagination';
import { Photo } from './../_models/photo';
import { User } from './../_models/user';
import { Observable } from 'rxjs';
import { HttpClient, HttpHeaders, HttpResponse, HttpParams } from '@angular/common/http';
import { environment } from './../../environments/environment';
import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class UserService {
  baseUrl: string = environment.apiUrl;

  constructor(private httpClient: HttpClient) { }

  getUsers(pageNumber?, pageSize?, userParams?, userLikes?): Observable<PaginatedResult<User[]>> {

    const paginatedResult: PaginatedResult<User[]> = new PaginatedResult<User[]>();
    let params = new HttpParams();

    if (pageNumber != null) {
      params = params.append('pageNumber', pageNumber);
    }

    if (pageSize != null) {
      params = params.append('pageSize', pageSize);
    }

    if (userParams != null) {
      params = params.append('minAge', userParams.minAge);
      params = params.append('maxAge', userParams.maxAge);
      params = params.append('gender', userParams.gender);
      params = params.append('orderBy', userParams.orderBy);
    }

    if (userLikes != null) {
      if (userLikes === 'likers') {
        params = params.append('likers', 'true');
      } else {
        params = params.append('likees', 'true');
      }
    }

    return this.httpClient.get<User[]>(this.baseUrl + 'users', {observe: 'response', params})
            .pipe(
              map(resp => {
                paginatedResult.result = resp.body;

                if (resp.headers.get('Pagination') != null) {
                  paginatedResult.pagination = JSON.parse(resp.headers.get('Pagination'));
                }
                return paginatedResult;
              })
            );
  }

  getUser(id: number): Observable<User> {
    return this.httpClient.get<User>(this.baseUrl + 'users/' + id);
  }

  updateUser(id: number, user: User) {
    return this.httpClient.post(this.baseUrl + 'users/' + id, user);
  }

  setMainPhoto(userId: number, id: number) {
    return this.httpClient.post(this.baseUrl + 'users/' + userId + '/photos/' + id + '/setMain', {});
  }

  deletePhoto(userId: number, id: number) {
    return this.httpClient.delete(this.baseUrl + 'users/' + userId + '/photos/' + id);
  }

  likeUser(likerId: number, likeeId: number) {
    return this.httpClient.post(this.baseUrl + 'users/' + likerId + '/like/' + likeeId, {});
  }
}
