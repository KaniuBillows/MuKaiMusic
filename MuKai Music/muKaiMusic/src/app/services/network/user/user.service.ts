import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { LoginUserInfo } from 'src/app/entity/user';
import { Md5 } from "ts-md5/dist/md5";
import { baseUrl } from '../music/music.service';
@Injectable({
  providedIn: 'root'
})
export class UserService {

  constructor(private httpClient: HttpClient) {
  }

  public logInNetEasePhone(phonenumber: string, pass: string): Observable<LoginUserInfo> {
    let md5 = Md5.hashStr(pass);
    return this.httpClient.get<LoginUserInfo>(baseUrl + `/api/netease/login?countrycode=86&phone=${phonenumber}&password=${md5}`);
  }

  
}
