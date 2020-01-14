import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';
import { NetEaseLoginUserInfo, UserInfo } from 'src/app/entity/user';
import { Md5 } from "ts-md5/dist/md5";
import { baseUrl } from '../music/music.service';
import { Result } from 'src/app/entity/baseResult';
import { onError, onResult } from '../resultHandle';
@Injectable({
  providedIn: 'root'
})
export class UserService {

  constructor(private httpClient: HttpClient) {
  }

  public logInNetEasePhone(phonenumber: string, pass: string): Observable<NetEaseLoginUserInfo> {
    let md5 = Md5.hashStr(pass);
    return this.httpClient.get<NetEaseLoginUserInfo>(baseUrl + `/api/netease/login?countrycode=86&phone=${phonenumber}&password=${md5}`);
  }

  public login(usr: string, pwd: string): Observable<Result<UserInfo>> {
    let header = { "Content-Type": "application/x-www-form-urlencoded" }

    return this.httpClient.post<Result<UserInfo>>(baseUrl + '/api/account/login',
      "username=" + encodeURIComponent(usr) + "&" +
      "password=" + encodeURIComponent(pwd)
      , {
        headers: header
      });
  }

  public async logIn(username: string, password: string): Promise<UserInfo> {

    let result: any = await this.login(username, password).toPromise().catch(onError);
    onResult(result);
    let user = result.content as UserInfo;
    return user
  }
}

