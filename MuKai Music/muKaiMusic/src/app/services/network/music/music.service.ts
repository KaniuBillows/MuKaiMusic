import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';
import 'src/app/entity/music';
import { lyricInfo, musicDetailResult, NetEaseUrlResult, Song, UrlInfo } from 'src/app/entity/music';
import { CategoryResult, HotCaegoryResult, PersonalizedPlaylistResult } from 'src/app/entity/playlist';
import { Result } from 'src/app/entity/baseResult';
import { environment } from "src/environments/environment"
import { MusicUrlParam } from 'src/app/entity/param/musicUrlParam';

@Injectable({
  providedIn: 'root'
})
export class MusicService {

  constructor(private httpClient: HttpClient) {

  }


  private _kuwoToken: string = null;

  public get kuwoToken(): string {
    return this._kuwoToken;
  }

  /**
   * 获取歌词
   * @param id 网易云歌曲Id
   */
  public getLyric(id: number): Observable<lyricInfo> {
    return this.httpClient.get<lyricInfo>(environment.baseUrl + '/api/lyric?id=' + id);
  }

  /**
   * 获取歌曲详情
   * @param ids 
   */
  public getMusicDetail(ids: number[]): Observable<musicDetailResult> {
    return this.httpClient.post<musicDetailResult>(environment.baseUrl + '/api/music/detail', ids);
  }

  /**
   * 获取推荐新歌
   */
  public getPersonalizedMusics(): Observable<Result<Song[]>> {
    return this.httpClient.get<Result<Song[]>>(environment.baseUrl + '/api/music/personalized');
  }

  /**
   * 获取推荐歌单
   * @param limit 数量
   */
  public getPersonalizedPlaylist(limit?: number): Observable<PersonalizedPlaylistResult> {
    return this.httpClient.get<PersonalizedPlaylistResult>(environment.baseUrl + `/api/playlist/personalized?limit=${limit | 5}`);
  }

  /**
   * 获取歌曲的URL
   * @param parma
   */
  public getNeteaseUrl(parma: MusicUrlParam): Observable<Result<UrlInfo>> {
    // return this.httpClient.get<NetEaseUrlResult>(environment.baseUrl + `/api/url?id=${id}&br=${br || 128000}`);
    return this.httpClient.post<Result<UrlInfo>>(environment.baseUrl + '/api/music/url', parma);
  }

  /**
   * 获取热门歌单分类
   */
  public getHotCategories(): Observable<HotCaegoryResult> {
    return this.httpClient.get<HotCaegoryResult>(environment.baseUrl + '/api/playlist/hotCategories');
  }

  /**
   * 获取全部歌单分类
   */
  public getAllCategories(): Observable<CategoryResult> {
    return this.httpClient.get<CategoryResult>(environment.baseUrl + '/api/playlist/categories');
  }

  /**
   * 下载歌曲
   * @param url 
   */
  public downloadFile(url: string, downloadInfo: string) {
    let x = new XMLHttpRequest();
    x.open("GET", url + '?t=' + new Date().getTime(), true);
    x.responseType = 'blob';
    x.onload = function (e) {
      let url = window.URL.createObjectURL(x.response);
      let a = document.createElement('a');
      a.href = url;
      a.download = downloadInfo;
      a.click();
    }
    x.send();
  }


  public async searchMusic(word: string) {
    if (await this.getKuWoToken()) {

    }
  }

  /**
   * 获取酷我token
   */
  public async  getKuWoToken(): Promise<boolean> {
    let result = await this.httpClient.get<Result<string>>(environment.baseUrl + '/api/kuwo/token').toPromise();
    if (result.code == 200) {
      this._kuwoToken = result.content;
      return true;
    } else return false;
  }

}
