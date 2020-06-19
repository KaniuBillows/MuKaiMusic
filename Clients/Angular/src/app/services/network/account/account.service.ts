import { Injectable, EventEmitter } from '@angular/core';
import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';
import { Observable, ObservableInput, from } from 'rxjs';
import { NetEaseLoginUserInfo, UserInfo } from 'src/app/entity/user';
import { Md5 } from "ts-md5/dist/md5";
import { Result } from 'src/app/entity/baseResult';
import { environment } from 'src/environments/environment';
import { Token } from 'src/app/entity/Token';
import { tap } from 'rxjs/operators';
import { AccountInterceptor, AccessToken, RefreshToken, User } from '../accountInterceptor';
import * as JsEncryptModule from 'jsencrypt';
import { AES, mode, pad, enc } from 'crypto-js';

const base62 = 'abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789';
const publicKey = '-----BEGIN PUBLIC KEY-----\nMIGfMA0GCSqGSIb3DQEBAQUAA4GNADCBiQKBgQC1tLW+9lJ7/jHsNQnXcbrqfciQbc5YeSIf7q9w5aG/p488x9AC94G0RHtoRIz8m+GUlO1mXntO1OFvhri8ygooPeGfVnO62FTuckY+2WSHR3N20QMWjCJckdZtNTo0jK4n5QLW4yo9mu4r/IIvTxcuubPzqJeWQdW813ETpHviXwIDAQAB\n-----END PUBLIC KEY-----';
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
    let aesKey = this.getKey();
    let str = JSON.stringify({
      "userName": usr,
      "password": pwd
    });
    let encrypt = this.aesEncrypt(str, aesKey);
    let encryptedKey = this.rsaEncrypt(aesKey);
    return this.httpClient.post<Result<Token>>(environment.baseUrl + `/api/account/login?key=${encryptedKey}`,
      encrypt
      , {
        headers: {
          "Content-Type": "application/json"
        }
      }).pipe(tap((res: Result<Token>) => {
        if (res.code == 200) {
          localStorage.setItem(AccessToken, res.content.accessToken);
          localStorage.setItem(RefreshToken, res.content.refreshToken);
        }
      }));
  }

  public register(user: UserInfo): Observable<Result<string>> {
    let aesKey = this.getKey();
    let str = JSON.stringify(user);
    let encrypt = this.aesEncrypt(str, aesKey);
    let encryptedKey = this.rsaEncrypt(aesKey);
    return this.httpClient.post<Result<string>>(environment.baseUrl + `/api/account/login?key=${encryptedKey}`,
      encrypt, {
      headers: {
        "Content-Type": "application/json"
      }
    });
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

  private aesEncrypt(content: string, key: string): string {
    let tmpAES = AES.encrypt(content, enc.Base64.parse(key), {
      mode: mode.ECB,
      padding: pad.Pkcs7
    });
    return tmpAES.toString();
  }

  private aesDecrypt(content: string): string {
    let Key = "NU90YjNiUDdDWFhGdHNwdg==";
    var key = enc.Base64.parse(Key);
    let tmpDeAES = AES.decrypt(content, Key, {
      mode: mode.ECB,
      padding: pad.Pkcs7
    });
    return tmpDeAES.toString(enc.Utf8);

  }

  private rsaEncrypt(content: string): string {
    let encrypt = new JsEncryptModule.JSEncrypt();
    encrypt.setPublicKey(publicKey);
    let signature = encrypt.encrypt(content);
    return signature;
  }

  private getKey(): string {
    let array = new Uint8Array(16);
    crypto.getRandomValues(array);
    let res = "";
    for (let i = 0; i < array.length; i++) {
      res += String.fromCharCode(base62.charAt(array[i] % 62).charCodeAt(0));
    }
    return btoa(res);
  }
}

