import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';
import 'src/app/entity/music';
import { musicDetailResult, NetEaseUrlResult, Song, UrlInfo, Lyric } from 'src/app/entity/music';
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
    return this.httpClient.post<Result<Lyric[]>>(environment.baseUrl + '/api/music/lyric', this.getParma(song));
  }

  /**
   * 获取网易云歌曲详情
   * @param id 网易云歌曲Id
   */
  public getMusicDetail(id: number): Observable<musicDetailResult> {
    return this.httpClient.get<musicDetailResult>(environment.baseUrl + `/api/music/ne_detail?id=${id}`);
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
  public getUrl(parma: MusicParam): Observable<Result<UrlInfo>> {
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

  /**
   * 搜索歌曲
   * @param key 
   */
  public searchMusic(key: string, kuwoToken: string): Observable<Result<Song[]>> {
    return this.httpClient.get<Result<Song[]>>(environment.baseUrl + '/api/music/search?', {
      params: {
        token: kuwoToken,
        key: key
      }
    });
  }

  /**
   * 获取酷我token
   */
  public async getKuWoToken(): Promise<string> {
    let res = await this.httpClient.get<Result<string>>(environment.baseUrl + '/api/kuwo/token').toPromise();
    if (res.code == 200) return res.content;
    else return null;
  }

  /**
   * 请求歌曲的图片信息
   * @param song 
   */
  public async getPicture(song: Song): Promise<string> {
    let url: string = "";
    if (song.picUrl) return song.picUrl;
    try {
      switch (song.dataSource) {
        case DataSource.Kuwo: {
          url = `http://www.kuwo.cn/api/www/music/musicInfo?mid=${song.kuWo_Id}&reqId=${this.get_uuid()}`;
          let res = await this.httpClient.get<any>(url).toPromise();
          if (res.code != 200) return null;
          return res.data.pic;
        }
        case DataSource.Migu: {
          url = environment.baseUrl + `/api/music/migu_pic?id=${song.migu_Id}`;
          let res = await this.httpClient.get<any>(url).toPromise();
          if (res.returnCode != "000000") return null;
          return "http:" + res.smallPic;
        }
        case DataSource.NetEase: {
          let res = await this.getMusicDetail(song.ne_Id).toPromise();
          return res.songs[0].al.picUrl + '?param=240y240';
        }
      }
    } catch{
      return "../../../assets/img/logo.png";
    }
  }

  /**
   * 生成请求参数
   */
  public getParma(song: Song): MusicParam {
    let res = new MusicParam();
    res.dataSource = song.dataSource;
    switch (song.dataSource) {
      case DataSource.Kuwo: res.kuwoId = song.kuWo_Id;
        break;
      case DataSource.Migu: res.miguId = song.migu_CopyrightId;
        break;
      case DataSource.NetEase: res.neteaseId = song.ne_Id;
        break;
    }
    return res;
  }

  /**
   * 用于获取UUID 辅助函数
   */
  private S4() {
    return (((1 + Math.random()) * 0x10000) | 0).toString(16).substring(1);
  }
  private get_uuid() {
    return (this.S4() + this.S4() + "-" + this.S4() + "-" + this.S4() + "-" + this.S4() + "-" + this.S4() + this.S4() + this.S4());
  }

}
