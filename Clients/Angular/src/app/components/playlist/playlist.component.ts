import { Component, OnInit, Input, EventEmitter, Output } from '@angular/core';
import { Song } from 'src/app/entity/music';
import { ThemeService } from 'src/app/services/theme/theme.service';
import { PlayerService } from 'src/app/services/player/player.service';
import { Result } from 'src/app/entity/baseResult';
import { UrlInfo } from 'src/app/entity/music';
import { MusicService } from 'src/app/services/network/music/music.service';
import BScroll from '@better-scroll/core';
import ScrollBar from '@better-scroll/scroll-bar'
import MouseWheel from '@better-scroll/mouse-wheel';
import ObserveDOM from '@better-scroll/observe-dom';
import { MatSnackBar } from '@angular/material/snack-bar';
import { SnackBarComponent } from '../snackBar/snackBar.component';

@Component({
  selector: 'app-playlist',
  templateUrl: './playlist.component.html',
  styleUrls: ['./playlist.component.scss']
})
export class PlaylistComponent implements OnInit {
  private scroll: BScroll;
  constructor(
    public theme: ThemeService,
    private musicNet: MusicService,
    public player: PlayerService,
    public snackBar: MatSnackBar
  ) {
  }
  ngOnInit() {
    let wrapper = document.getElementById('playlist-box');
    BScroll.use(MouseWheel);
    BScroll.use(ScrollBar);
    BScroll.use(ObserveDOM);
    this.scroll = new BScroll(wrapper, {
      scrollY: true,
      click: true,
      mouseWheel: {
        speed: 20,
        invert: false,
        easeTime: 300
      },
      scrollbar: true,
      probeType: 3,
      observeDOM: true
    });
    let scrollbar = document.querySelector<HTMLDivElement>(".bscroll-indicator");
    scrollbar.style.border = 'none';

    this.player.currentMusicChange.subscribe(() => {
      let scrollElm = document.getElementById("playlist-item-" + this.player.currentMusicIndex);
      this.scroll.scroller.scrollToElement(scrollElm, 200, false, true);
    });
  }


  private get contentShow(): boolean {
    return location.href.includes('/content/');
  }



  public get currentPlayIndex() {
    return this.player.currentMusicIndex;
  }

  public get playlist(): Song[] {
    return this.player.playlist;
  }


  /**
   * 获取当前主题class 用于当前歌曲特殊显示
   */
  public get themeClass() {
    return this.theme.getThemeClass();
  }

  /**
   * 转换时间格式,用于播放列表显示
   * @param time 
   */
  public getTimeFormat(time: number): string {
    return time != null ? Math.floor(time / 60).toString().padStart(2, '0') +
      ':' + Math.floor((time % 60)).toString().padStart(2, '0') : "未知";
  }

  /** 
   * 点击播放此歌曲 
   * @param index 歌曲索引
   */
  public clickPlay(index: number) {
    let song = this.player.playlist[index];
    if (song.url)
      this.player.start(song);
    else {
      this.musicNet.getUrl(song).subscribe(res => {
        if (res.code != 200 || res.content == null) {
          this.snackBar.openFromComponent(SnackBarComponent, { duration: 2500, data: "这首歌不让听了，试试其他的吧" });
          this.player.deleteFromPlaylist(index);
        } else {
          song.url = res.content;
          this.player.start(song);
        }
      });
    }
  }

  /**
   * 点击下载此歌曲
   * @param index 歌曲索引
   */
  public clickDownload(index: number) {
    let item = this.playlist[index];
    this.musicNet.getUrl(item).subscribe((res: Result<string>) => {
      if (res.content == null) {
        this.snackBar.openFromComponent(SnackBarComponent, { duration: 2500, data: "这居然不让下载，试试其他的吧!" });
        this.player.deleteFromPlaylist(index);
        return;
      }
      this.musicNet.downloadFile(res.content, item.name + " - " + item.artists[0].name);
    });
  }

  /**
   * 删除播放列表中的歌曲
   * @param index 歌曲索引
   */
  public deleteMusic(index: number) {
    this.player.deleteFromPlaylist(index);
  }



  //#region Component Event

  /**
   * 歌曲播放点击事件，由player组件订阅处理，
   * 涉及对currentMusicInfo,currentMusicIndex更改,导致playlist与当前歌曲相关的内容变化
   */
  @Output()
  public item_play_Click = new EventEmitter<number>();

  /**
   * 歌曲下载点击事件，由player组件订阅处理
   */
  @Output()
  public item_download_Click = new EventEmitter<number>();

  //#endregion
}
