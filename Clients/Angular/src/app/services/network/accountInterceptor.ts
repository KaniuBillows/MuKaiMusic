import { Injectable, EventEmitter } from "@angular/core"
import { HttpEvent, HttpInterceptor, HttpHandler, HttpRequest, HttpClient, HttpResponse } from '@angular/common/http';
import { Observable, of, Subject } from 'rxjs';
import { mergeMap, tap } from 'rxjs/operators';
import { Result } from 'src/app/entity/baseResult';
import { Token } from 'src/app/entity/Token';
import { environment } from 'src/environments/environment';
import { __await, __awaiter } from 'tslib';
export const AccessToken: string = "ACCESSTOKEN";
export const RefreshToken: string = "REFRESHTOKEN";
export const User: string = "USERINFO";

@Injectable({
    providedIn: 'root'
})
export class AccountInterceptor implements HttpInterceptor {

    public constructor(private httpClient: HttpClient) {

    }

    intercept(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
        //如果请求为刷新Token，不做处理
        if (req.headers.has("Re_T")) {
            return next.handle(req);
        }
        let token = localStorage.getItem(AccessToken);
        //如果token没在localStorage中，咱就不管了
        if (token == null) {
            return next.handle(req);
        }
        //如果AccessToken未过期，直接添加Token到请求中
        if (!this.IsTokenExpire(token)) {
            let auth = req.clone({ setHeaders: { "Authorization": "Bearer " + token } })
            return next.handle(auth);
        } else {
            let refreshToken = localStorage.getItem(RefreshToken);
            //如果RefreshToken过期，咱也不管了
            if (this.IsTokenExpire(refreshToken)) {
                localStorage.removeItem(User);
                localStorage.removeItem(AccessToken);
                localStorage.removeItem(RefreshToken);
                this.accountExpire.emit();
                return next.handle(req);
            } else {
                //先生成一个subject对象。
                const subject = new Subject<any>();
                this.reTokenAsync(req, next, subject, refreshToken);
                // 返回被委托的对象，让真正的业务请求隐匿起来。
                return subject;
            }
        }
    }
    // 重新获取token 
    async reTokenAsync(req: HttpRequest<any>, next: HttpHandler, sub: Subject<any>, refreshToken: string) {
        try {
            const res = await this.Refresh(refreshToken);
            localStorage.setItem(AccessToken, res.content.accessToken);
            localStorage.setItem(RefreshToken, res.content.refreshToken);
            req = req.clone({
                setHeaders: {
                    'Authorization': "Bearer " + res.content.accessToken,
                }
            });
        } catch{
            localStorage.removeItem(User);
            localStorage.removeItem(AccessToken);
            localStorage.removeItem(RefreshToken);
            this.accountExpire.emit();
        } finally {
            // 让真的业务重新出现
            next.handle(req).pipe(
                tap(data => {
                    sub.next(data);  // 数据到达，转达下发
                    return data;
                }, (error) => {

                    sub.error(error); //数据报错，转达出错
                })
            ).subscribe();
        }

        //由于该Observable对象已经没有人去主动订阅它了。所以我们手动订阅一下
    }

    /**
     * 返回此token是否过期
     * @param token 
     */
    private IsTokenExpire(token: string): boolean {
        let encodePaylod = token.split(".")[1];
        let payload: {
            "exp": number,
        } = JSON.parse(window.atob(encodePaylod));
        let timeNow = Math.round(Date.now() / 1000);
        return payload.exp < timeNow;
    }

    /**
     * 获取新的AccessToken
     */
    private Refresh(token: string): Promise<Result<Token>> {
        return this.httpClient.get<Result<Token>>(environment.baseUrl + "/api", {
            headers: {
                "Re_T": token
            }
        }).toPromise();
    }

    public accountExpire = new EventEmitter();
}
