import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';
import 'src/app/entity/music';
import { Song, UrlInfo, Lyric } from 'src/app/entity/music';
import { Playlist } from 'src/app/entity/playlist';
import { Result } from 'src/app/entity/baseResult';
import { environment } from "src/environments/environment"
import { MusicParam, DataSource } from 'src/app/entity/param/musicUrlParam';
import { EditDistanceService } from '../../Util/ed.service';

@Injectable({
  providedIn: 'root'
})
export class MusicService {

  constructor(private httpClient: HttpClient,
    private editDistance: EditDistanceService) {

  }



  /**
   * 获取歌词
   * @param song 
   */
  public getLyric(song: Song): Observable<Result<Lyric[]>> {
    let url = `${environment.baseUrl}${this.getRoute(song)}lyric?id=`;
    if (song.dataSource == DataSource.Migu) {
      url += song.migu_CopyrightId;
    } else {
      url += this.getSongId(song);
    }
    return this.httpClient.get<Result<Lyric[]>>(url);
  }

  // /**
  //  * 获取推荐新歌
  //  */
  // public getPersonalizedMusics(): Observable<Result<Song[]>> {
  //   return this.httpClient.get<Result<Song[]>>(environment.baseUrl + '/ne/music/personalized');
  // }

  /**
   * 获取推荐歌单
   * @param limit 数量
   */
  public getPersonalizedPlaylist(limit?: number): Observable<Result<Playlist[]>> {
    return this.httpClient.get<Result<Playlist[]>>(environment.baseUrl + `/ne/playlist/personlized?limit=${limit | 10}`);
  }

  /**
   * 获取歌单的详情，包含歌单中的音乐
   * @param id 
   */
  public getPlaylistDetail(id: number): Observable<Result<Playlist>> {
    return this.httpClient.get<Result<Playlist>>(`${environment.baseUrl}/ne/playlist/detail?id=${id}`);
  }

  /**
   * 获取歌曲的URL
   * @param parma
   */
  public getUrl(song: Song): Observable<Result<string>> {
    let url = environment.baseUrl + `${this.getRoute(song)}url?id=${this.getSongId(song)}`
    return this.httpClient.get<Result<string>>(url);
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
  public async searchMusic(key: string): Promise<Song[]> {
    let neRes = await this.httpClient.get<Song[]>(environment.baseUrl + `/ne/search?keyword=${key}`).toPromise();
    let kuwoRes = await this.httpClient.get<Song[]>(environment.baseUrl + `/kuwo/search?keyword=${key}`).toPromise();
    let miguRes = await this.httpClient.get<Song[]>(environment.baseUrl + `/migu/search?keyword=${key}`).toPromise();
    let songs: Song[] = [];
    songs = songs.concat(kuwoRes);
    songs = songs.concat(miguRes);
    songs = songs.concat(neRes);
    songs = songs.sort((s1, s2): number =>
      this.editDistance.getEditDistance(key, s1.name + s1.artists[0].name)
      -
      this.editDistance.getEditDistance(key, s2.name + s2.artists[0].name)
    );
    return songs;
  }

  /**
   * 请求歌曲的图片信息
   * @param song 
   */
  public async getPicture(song: Song): Promise<string> {
    if (song.album.picUrl != null) return song.album.picUrl;
    try {
      let res = await this.httpClient.get<Result<string>>(environment.baseUrl + `${this.getRoute(song)}/pic?id=${this.getSongId(song)}`).toPromise();
      if (res.code == 200) return res.content;
    } catch{
      return "../../../assets/img/logo.png";
    }
  }

  private getRoute(song: Song): string {
    switch (song.dataSource) {
      case DataSource.Kuwo: {
        return "/kuwo/";
      }
      case DataSource.Migu: {
        return "/migu/";
      }
      case DataSource.NetEase: {
        return "/ne/";
      }
      default: return "";
    }
  }

  private getSongId(song: Song): string {
    switch (song.dataSource) {
      case DataSource.Kuwo: {
        return song.kw_Id.toString();
      }
      case DataSource.Migu: {
        return song.migu_Id.toString();
      }
      case DataSource.NetEase: {
        return song.ne_Id.toString();
      }
      default: return "";
    }
  }

}
