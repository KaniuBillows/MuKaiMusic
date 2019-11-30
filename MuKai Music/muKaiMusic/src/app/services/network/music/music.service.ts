import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';
import 'src/app/entities/music';
import { lyricInfo, musicDetailResult, personalizedResult, NetEaseUrlResult } from 'src/app/entities/music';
export const baseUrl: string = 'http://localhost:2000'
//export const baseUrl: string = '';

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
   * 获取推荐歌单
   */
  public getPersonalizedMusics(): Observable<personalizedResult> {
    return this.httpClient.get<personalizedResult>(baseUrl + '/api/music/personalized');
  }

  public getNeteaseUrl(id: number, br?: number): Observable<NetEaseUrlResult> {
    return this.httpClient.get<NetEaseUrlResult>(baseUrl + `/api/url?id=${id}&br=${br || 128000}`);
  }
}
