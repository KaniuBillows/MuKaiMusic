import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';
import 'src/app/entity/music';
import { lyricInfo, musicDetailResult, personalizedResult, NetEaseUrlResult } from 'src/app/entity/music';
import { CategoryResult, HotCaegoryResult, PersonalizedPlaylistResult } from 'src/app/entity/playlist';
//export const baseUrl: string = 'http://117.48.203.23:2000';
//export const baseUrl: string = 'http://localhost:2000';
export const baseUrl: string = '';

@Injectable({
  providedIn: 'root'
})
export class MusicService {

  constructor(private httpClient: HttpClient) {

  }
  /**
   * 获取歌词
   * @param id 网易云歌曲Id
   */
  public getLyric(id: number): Observable<lyricInfo> {
    return this.httpClient.get<lyricInfo>(baseUrl + '/api/lyric?id=' + id);
  }

  /**
   * 获取歌曲详情
   * @param ids 
   */
  public getMusicDetail(ids: number[]): Observable<musicDetailResult> {
    return this.httpClient.post<musicDetailResult>(baseUrl + '/api/music/detail', ids);
  }

  /**
   * 获取推荐新歌
   */
  public getPersonalizedMusics(): Observable<personalizedResult> {
    return this.httpClient.get<personalizedResult>(baseUrl + '/api/music/personalized');
  }

  /**
   * 获取推荐歌单
   * @param limit 数量
   */
  public getPersonalizedPlaylist(limit?: number): Observable<PersonalizedPlaylistResult> {
    return this.httpClient.get<PersonalizedPlaylistResult>(baseUrl + `/api/playlist/personalized?limit=${limit | 5}`);
  }
  /**
   * 获取网易云歌曲的URL
   * @param id 
   * @param br 
   */
  public getNeteaseUrl(id: number, br?: number): Observable<NetEaseUrlResult> {
    return this.httpClient.get<NetEaseUrlResult>(baseUrl + `/api/url?id=${id}&br=${br || 128000}`);
  }

  /**
   * 获取热门歌单分类
   */
  public getHotCategories(): Observable<HotCaegoryResult> {
    return this.httpClient.get<HotCaegoryResult>(baseUrl + '/api/playlist/hotCategories');
  }

  /**
   * 获取全部歌单分类
   */
  public getAllCategories(): Observable<CategoryResult> {
    return this.httpClient.get<CategoryResult>(baseUrl + '/api/playlist/categories');
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
}
