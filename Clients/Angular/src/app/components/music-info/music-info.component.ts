import { Component, OnInit, Input, ViewChild, ElementRef, EventEmitter, Output } from '@angular/core';
import { PlayerService } from 'src/app/services/player/player.service';
import { MusicService } from 'src/app/services/network/music/music.service';
import { ThemeService } from 'src/app/services/theme/theme.service';
import { Lyric, Song } from 'src/app/entity/music';
import { Result } from 'src/app/entity/baseResult';
import BScroll from '@better-scroll/core';
import ObserveDOM from '@better-scroll/observe-dom';

@Component({
  selector: 'app-music-info',
  templateUrl: './music-info.component.html',
  styleUrls: ['./music-info.component.scss']
})
export class MusicInfoComponent implements OnInit {
  private scroll: BScroll;
  constructor(public player: PlayerService,
    public musicNet: MusicService,
    private theme: ThemeService) {
    this.player.currentMusicChange.subscribe(async () => {
      this.getLyric();
      await this.getPic();
    });
  }

  ngOnInit() {
    this.player.onCurrentTimeChange.subscribe((time: number) =>
      this.onTimeChange(time));
    let wrapper = document.getElementById('lyric-box');
    BScroll.use(ObserveDOM);
    this.scroll = new BScroll(wrapper, {
      scrollY: true,
      click: true,
      probeType: 3,
      observeDOM: true
    });

  }

  public get themeClass(): string {
    return this.theme.getThemeClass();
  }

  private _currentLrcIndex: number = -1;

  public get currentLrcIndex() {
    return this._currentLrcIndex;
  }

  public get musicInfo() {
    return this.player.currentMusic;
  }


  public get picUrl(): string {
    return this.musicInfo?.album?.picUrl == null ? "../../../assets/img/music_white.jpg" : this.musicInfo.album.picUrl.replace("http://", "https://");
  }

  private _lyric_paras: Lyric[] = [];
  public get lyric_paras(): Lyric[] {
    return this._lyric_paras;
  }

  /**
    * 订阅播放时间改变事件，实现歌词滚动
    */
  private onTimeChange(time: number) {
    let nowIndex = this.findLastIndexOf(this.lyric_paras, item => item.time != null && item.time <= time);
    if (nowIndex < 0) return;
    let element = document.getElementById('par-' + nowIndex);
    if (element && nowIndex > this.currentLrcIndex) {
      this.scroll.scroller.scrollToElement(element, 200, false, true);
    }
    this._currentLrcIndex = nowIndex;
  }

  /**
   * 从网络获取图片地址
   * @param song 
   */
  private async getPic(): Promise<string> {
    if (this.player.currentMusic?.album?.picUrl) return "../../../assets/img/music_white.jpg";
    let res = await this.musicNet.getPicture(this.player.currentMusic);
    this.player.currentMusic.album.picUrl = res;
    return res;
  }

  private async getLyric() {
    this._lyric_paras = [{ text: "正在加载歌词...", time: 0 }];
    let lyricRes: Result<Lyric[]> = await this.musicNet.getLyric(this.player.currentMusic).toPromise()
    if (lyricRes.code == 200) {
      this._lyric_paras = lyricRes.content;
    } else {
      this._lyric_paras = [{ text: "暂无歌词", time: 0 }];
    }
    if (this.scroll)
      this.scroll.refresh();
  }

  private findLastIndexOf<T>(array: Array<T>, func: (T: T) => boolean): number {
    for (let i = array.length - 1; i >= 0; i--) {
      if (func(array[i]))
        return i;
    }
    return -1;
  }
}
