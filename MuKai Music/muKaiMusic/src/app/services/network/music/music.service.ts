import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';
import 'src/app/entity/music';
import { Song, UrlInfo, Lyric } from 'src/app/entity/music';
import { CategoryResult, HotCaegoryResult, PersonalizedPlaylistResult } from 'src/app/entity/playlist';
import { Result } from 'src/app/entity/baseResult';
import { environment } from "src/environments/environment"
import { MusicParam, DataSource } from 'src/app/entity/param/musicUrlParam';

@Injectable({
  providedIn: 'root'
})
export class MusicService {

  constructor(private httpClient: HttpClient) {

  }



  /**
   * 获取歌词
   * @param song 
   */
  public getLyric(song: Song): Observable<Result<Lyric[]>> {
    let url: string;
    if (song.dataSource == DataSource.Migu) {
      url = environment.baseUrl + `/api/music/lyric?id=${song.migu_CopyrightId}&source=${song.dataSource}`;
      return this.httpClient.get<Result<Lyric[]>>(url);
    } else {
      url = environment.baseUrl + `/api/music/lyric?${this.getRoute(song)}`;
      return this.httpClient.get<Result<Lyric[]>>(url);
    }

  }

  /**
   * 获取网易云歌曲详情
   * @param id 网易云歌曲Id
   */
  // public getMusicDetail(id: number): Observable<musicDetailResult> {
  //   return this.httpClient.get<musicDetailResult>(environment.baseUrl + `/api/music/ne_detail?id=${id}`);
  // }

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
  public getUrl(song: Song): Observable<Result<string>> {
    let url = environment.baseUrl + `/api/music/url?${this.getRoute(song)}`
    return this.httpClient.get<Result<string>>(url);
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

  /**
   * 搜索歌曲
   * @param key 
   */
  public searchMusic(key: string): Observable<Result<Song[]>> {
    return this.httpClient.get<Result<Song[]>>(environment.baseUrl + `/api/music/search?key=${key}`);
  }

  /**
   * 请求歌曲的图片信息
   * @param song 
   */
  public async getPicture(song: Song): Promise<string> {
    if (song.album.picUrl != null) return song.album.picUrl;
    try {
      let id: any = '';
      switch (song.dataSource) {
        case DataSource.Kuwo: {
          id = song.kw_Id;
        } break;
        case DataSource.Migu: {
          id = song.migu_Id;
        } break;
        case DataSource.NetEase: {
          id = song.ne_Id;
        } break;
      }
      let res = await this.httpClient.get<Result<string>>(environment.baseUrl + `/api/music/pic?=${id}`).toPromise();
      if (res.code == 200) return res.content;
    } catch{
      return "../../../assets/img/logo.png";
    }
  }

  private getRoute(song: Song): string {
    let param: any = 'id=';
    switch (song.dataSource) {
      case DataSource.Kuwo: {
        param += song.kw_Id;

      } break;
      case DataSource.Migu: {
        param += song.migu_Id;
      } break;
      case DataSource.NetEase: {
        param += song.ne_Id;
      } break;
    }
    param += `&source=${song.dataSource}`;
    return param;
  }


}
