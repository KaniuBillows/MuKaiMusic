import { Injectable, EventEmitter } from '@angular/core';
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
export class AccountService {

  constructor(private httpClient: HttpClient) {
    this.loginSuccess.subscribe(user => {
      this._userInfo = user;
      localStorage.setItem("userInfo", JSON.stringify(user));
    });
    this.tokenUpdate.subscribe(token => {
      localStorage.setItem("token", token);
    });
    this.initVerify();
  }

  //token
  private initVerify() {
    let token = localStorage.getItem("token");
    if (token != null)
      this._token = token;
  }

  private _token: string = null;

  public get token(): string {
    return this._token;
  }

  private _userInfo: UserInfo;

  public get userInfo() {
    if (this._userInfo != null) return this._userInfo;
    let storeUser = localStorage.getItem("userInfo");
    if (storeUser != null) {
      this._userInfo = JSON.parse(storeUser);
    }
    return this._userInfo;
  }

  public logInNetEasePhone(phonenumber: string, pass: string): Observable<NetEaseLoginUserInfo> {
    let md5 = Md5.hashStr(pass);
    return this.httpClient.get<NetEaseLoginUserInfo>(baseUrl + `/api/netease/login?countrycode=86&phone=${phonenumber}&password=${md5}`);
  }

  private login(usr: string, pwd: string): Observable<Result<UserInfo>> {
    let header = { "Content-Type": "application/x-www-form-urlencoded" }
    return this.httpClient.post<Result<UserInfo>>(baseUrl + '/api/account/login',
      "username=" + encodeURIComponent(usr) + "&" +
      "password=" + encodeURIComponent(pwd)
      , {
        headers: header
      });
  }

  public logIn(username: string, password: string): void {
    this.login(username, password).subscribe(res => {
      if (onResult(res)) {
        this.loginSuccess.emit(res.content);
        this.tokenUpdate.emit(res.content.token);
      } else {
        this.loginFailed.emit(res.error);
      }
    });
  }

  public logOut(): void {
    localStorage.removeItem("userInfo");
    localStorage.removeItem("token");
    this._userInfo = null;
  }

  public loginSuccess = new EventEmitter<UserInfo>();

  public loginFailed = new EventEmitter<string>();

  public logout = new EventEmitter();

  private tokenUpdate = new EventEmitter<string>();

  public getHeader(): HttpHeaders {
    let header: HttpHeaders = new HttpHeaders();
    if (this.token) {
      header.append("Authorization", "Bearer " + this.token);
    }
    return header;
  }
}

