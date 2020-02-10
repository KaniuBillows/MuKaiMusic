import { Component, OnInit, Input, ViewChild, ElementRef, EventEmitter, Output } from '@angular/core';
import { PlayerService } from 'src/app/services/player/player.service';
import { MusicService } from 'src/app/services/network/music/music.service';
import { ThemeService } from 'src/app/services/theme/theme.service';
import { Lyric, Song } from 'src/app/entity/music';

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
    private theme: ThemeService) { }

  ngOnInit() {
    this.player.onCurrentTimeChange.subscribe((time: number) =>
      this.onTimeChange(time)
    );
  }

  public get themeClass(): string {
    return this.theme.getThemeClass();
  }

  private _currentLrcIndex: number = -1;

  public get currentLrcIndex() {
    return this._currentLrcIndex;
  }

  private _msuicInfo: Song;
  @Input()
  public get musicInfo() {
    return this._msuicInfo;
  }
  public set musicInfo(value: Song) {
    this._msuicInfo = value;
    this.getPic(value);
  }

  private _picUrl: string = null;
  public get picUrl(): string {
    return this._picUrl == null ? "../../../assets/img/logo.png" : this._picUrl;
  }
  public set picUrl(value: string) {
    this._picUrl = value;
    this.pictureChange.emit(this.picUrl);
  }


  private _lyric_paras: Lyric[] = [];
  @Input()
  public get lyric_paras(): Lyric[] {
    return this._lyric_paras;
  }
  public set lyric_paras(value: Lyric[]) {
    this._lyric_paras = value;
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
  private getPic(song: Song) {
    this.picUrl = null;
    this.musicNet.getPicture(song).then(res => {
      this.picUrl = res;
    })
  }

  /**
   * 图片改变事件，由palyer组件订阅，改变背景图片
   */
  @Output()
  public pictureChange = new EventEmitter<string>();
}
