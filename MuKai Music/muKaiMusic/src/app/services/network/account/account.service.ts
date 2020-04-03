import { Injectable, EventEmitter } from '@angular/core';
import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';
import { Observable, ObservableInput } from 'rxjs';
import { NetEaseLoginUserInfo, UserInfo } from 'src/app/entity/user';
import { Md5 } from "ts-md5/dist/md5";
import { Result } from 'src/app/entity/baseResult';
import { onError, onResult } from '../resultHandle';
import { environment } from 'src/environments/environment';
import { Token } from 'src/app/entity/Token';
import { mergeMap, tap } from 'rxjs/operators';
import { AccountInterceptor, AccessToken, RefreshToken, User } from '../accountInterceptor';



@Injectable({
  providedIn: 'root'
})
export class AccountService {

  constructor(private httpClient: HttpClient,
    private accountInterceptor: AccountInterceptor
  ) {
    this.accountInterceptor.accountExpire.subscribe(() => {
      this._userInfo = null;
      localStorage.removeItem(User);
      localStorage.removeItem(AccessToken);
      localStorage.removeItem(RefreshToken);
    });
  }



  private _userInfo: UserInfo;

  public get userInfo() {
    if (this._userInfo != null) return this._userInfo;
    let storeUser = localStorage.getItem(User);
    if (storeUser != null) {
      this._userInfo = JSON.parse(storeUser);
    }
    return this._userInfo;
  }

  public logInNetEasePhone(phonenumber: string, pass: string): Observable<NetEaseLoginUserInfo> {
    let md5 = Md5.hashStr(pass);
    return this.httpClient.get<NetEaseLoginUserInfo>(environment.baseUrl + `/api/netease/login?countrycode=86&phone=${phonenumber}&password=${md5}`);
  }

  public login(usr: string, pwd: string): Observable<Result<Token>> {
    let header = { "Content-Type": "application/x-www-form-urlencoded" }
    return this.httpClient.post<Result<Token>>(environment.baseUrl + '/api/account/login',
      "username=" + encodeURIComponent(usr) + "&" +
      "password=" + encodeURIComponent(pwd)
      , {
        headers: header
      }).pipe(tap((res: Result<Token>) => {
        if (res.code == 200) {
          localStorage.setItem(AccessToken, res.content.accessToken);
          localStorage.setItem(RefreshToken, res.content.refreshToken);
        }
      }));
  }

  public getUserInfo(usrId?: number): Observable<Result<UserInfo>> {
    return this.httpClient.get<Result<UserInfo>>(environment.baseUrl + "/api/account/info", {
      params: usrId != null ? new HttpParams().append("id", usrId.toString()) : null
    })
      .pipe(tap((res: Result<UserInfo>) => {
        if (res.code == 200) {
          //将用户对象存入缓存
          localStorage.setItem(User, JSON.stringify(res.content));
          this._userInfo = res.content;
        }
      }));
  }

  public async logOut() {
    localStorage.removeItem(User);
    this._userInfo = null;
    await this.httpClient.get(environment.baseUrl + "/api/account/logout").toPromise().then(() => {
      localStorage.removeItem(AccessToken);
      localStorage.removeItem(RefreshToken);
    }).catch(() => {
      localStorage.removeItem(AccessToken);
      localStorage.removeItem(RefreshToken);
    });
  }


}

