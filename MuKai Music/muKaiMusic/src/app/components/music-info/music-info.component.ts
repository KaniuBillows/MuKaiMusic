import { Component, OnInit, Input, ViewChild, ElementRef, EventEmitter, Output } from '@angular/core';
import { PlayerService } from 'src/app/services/player/player.service';
import { MusicService } from 'src/app/services/network/music/music.service';
import { ThemeService } from 'src/app/services/theme/theme.service';
import { Lyric, Song } from 'src/app/entity/music';
import { Result } from 'src/app/entity/baseResult';

@Component({
  selector: 'app-music-info',
  templateUrl: './music-info.component.html',
  styleUrls: ['./music-info.component.scss']
})
export class MusicInfoComponent implements OnInit {
  @ViewChild('lyrics', { static: true })
  lyrics: ElementRef;
  constructor(public player: PlayerService,
    public musicNet: MusicService,
    private theme: ThemeService) {
    this.player.currentMusicChange.subscribe(async () => {
      await this.getPic();
      this.getLyric();
    });
  }

  ngOnInit() {
    this.player.onCurrentTimeChange.subscribe((time: number) =>
      this.onTimeChange(time));
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
    this._currentLrcIndex = this.lyric_paras.findIndex(item => item.time > time) - 1;
    if (this._currentLrcIndex < 0) return;
    let element = document.getElementById('par-' + this.currentLrcIndex);
    if (element) {
      this.lyrics.nativeElement.style.transform = `translateY(-${element.offsetTop}px)`;
    }
  }

  /**
   * 从网络获取图片地址
   * @param song 
   */
  private async getPic() {
    if (this.player.currentMusic?.album?.picUrl) return;
    let res = await this.musicNet.getPicture(this.player.currentMusic);
    this.player.currentMusic.album.picUrl = res;
  }

  private getLyric() {
    this._lyric_paras = [{ text: "正在加载歌词...", time: 0 }];
    this.musicNet.getLyric(this.player.currentMusic).subscribe((res: Result<Lyric[]>) => {
      if (res.code == 200) {
        this._lyric_paras = res.content;
      } else {
        this._lyric_paras = [{ text: "暂无歌词", time: 0 }];
      }
    }, (err) => {
      this._lyric_paras = [{ text: "暂无歌词", time: 0 }];
    });

  }
}
