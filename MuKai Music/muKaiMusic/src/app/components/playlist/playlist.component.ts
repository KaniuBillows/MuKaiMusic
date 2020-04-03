import { Component, OnInit, Input, EventEmitter, Output } from '@angular/core';
import { Song } from 'src/app/entity/music';
import { ThemeService } from 'src/app/services/theme/theme.service';
import { PlayerService } from 'src/app/services/player/player.service';
import { Result } from 'src/app/entity/baseResult';
import { UrlInfo } from 'src/app/entity/music';
import { MusicService } from 'src/app/services/network/music/music.service';
import { from } from 'rxjs';

@Component({
  selector: 'app-playlist',
  templateUrl: './playlist.component.html',
  styleUrls: ['./playlist.component.scss']
})
export class PlaylistComponent implements OnInit {

  constructor(
    public theme: ThemeService,
    private musicNet: MusicService,
    public player: PlayerService
  ) {
    //当播放列表元素改变时,更新scroll bar状态
    this.player.playlistChange.subscribe(() => {
      window.setInterval(() => {
        this.initScroll();
      }, 100);
    });
  }
  ngOnInit() {
    this.scrollBar();

  }
  //#region 滚动条相关

  private scrollBar() {
    let playlist_container = document.getElementById("playlist-box") as HTMLDivElement;
    let scroll = document.getElementById("scroll") as HTMLDivElement;
    let bar = document.getElementById("bar") as HTMLDivElement;
    let ul = document.getElementById("ul") as HTMLDivElement;
    //记录上一次滚动距离，用于鼠标滚动
    let oringin = 0;
    //拖动滚动条 内容滑动
    bar.onmousedown = function (e: MouseEvent) {
      //鼠标在滚动条中的位置
      let spaceY = e.clientY - bar.offsetTop;
      //2.2鼠标在页面上移动的时候，滚动条的位置
      document.onmousemove = function (e) {
        let y = e.clientY - spaceY;//滑动条的举例
        y = y < 0 ? 0 : y;
        y = y > scroll.offsetHeight - bar.offsetHeight ? scroll.offsetHeight - bar.offsetHeight : y;
        // 控制bar不能移除scroll
        oringin = y;
        bar.style.top = y + 'px';
        //3.当拖拽时，内容跟着滚动
        let offsetY = y * (ul.offsetHeight - playlist_container.offsetHeight) / (scroll.offsetHeight - bar.offsetHeight);
        ul.style.top = -offsetY + "px";
      }
    }
    document.onmouseup = function () {
      //移除鼠标移动事件
      document.onmousemove = null;
    }
    //设定是否允许鼠标滚动
    playlist_container.onmouseenter = (e: MouseEvent) => {
      this.allowScroll = true;
    }
    //设定是否允许鼠标滚动
    playlist_container.onmouseleave = (e: MouseEvent) => {
      this.allowScroll = false;
    }
    //鼠标滚轮滑动播放列表
    window.addEventListener('mousewheel', window.onmousewheel = (e: MouseWheelEvent) => {
      if (this.allowScroll && (!this.contentShow)) {
        oringin += e.deltaY / 5;
        //防止超出范围
        oringin = oringin < 0 ? 0 : oringin;
        oringin = oringin > scroll.clientHeight - bar.clientHeight ? scroll.clientHeight - bar.clientHeight : oringin;
        var barY = oringin;
        bar.style.top = barY + 'px';
        let offsetY = barY * (ul.offsetHeight - playlist_container.offsetHeight) / (scroll.offsetHeight - bar.offsetHeight);
        ul.style.top = -offsetY + "px";
      }
    })

    //播放歌曲切换时，自动进行滑动
    this.player.currentMusicChange.subscribe(() => {
      let barY = scroll.offsetHeight * (this.currentPlayIndex - 9) / this.playlist.length;
      barY = barY < 0 ? 0 : barY;
      barY = barY > scroll.clientHeight - bar.clientHeight ? scroll.clientHeight - bar.clientHeight : barY;
      bar.style.top = barY + 'px';
      let offsetY = barY * (ul.offsetHeight - playlist_container.offsetHeight) / (scroll.offsetHeight - bar.offsetHeight);
      ul.style.top = -offsetY + "px";
    });
  }

  /**
   * 当鼠标在播放列表范围内时，允许鼠标滚动
   */
  private allowScroll: boolean = false;

  private get contentShow(): boolean {
    return location.href.includes('/content/');
  }
  /**
   * scroll bar相关
   * 由播放列表中元素数量决定
   * 控制滚动条长度以及是否显示滚动条
   */
  private initScroll() {
    let playlist_container = document.getElementById("playlist-box") as HTMLDivElement;
    let scroll = document.getElementById("scroll") as HTMLDivElement;
    let bar = document.getElementById("bar") as HTMLDivElement;
    let ul = document.getElementById("ul") as HTMLDivElement;
    let barHeight = 0;
    let visible = "hidden";
    this.allowScroll = false;
    if (this.playlist.length > 10) {
      barHeight = playlist_container.offsetHeight * scroll.offsetHeight / ul.offsetHeight;
      visible = "visible";
      this.allowScroll = true;
    }
    bar.style.height = barHeight + "px";
    scroll.style.visibility = visible;
  }

  //#endregion


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
    this.player.start(this.player.playlist[index]);
  }

  /**
   * 点击下载此歌曲
   * @param index 歌曲索引
   */
  public clickDownload(index: number) {
    let item = this.playlist[index];
    if (item.url) {
      this.musicNet.downloadFile(item.url, item.name + " - " + item.artists[0].name);
    } else {
      this.musicNet.getUrl(item).subscribe((res: Result<string>) => {
        if (res.content == null) {
          alert('这居然不让下载，试试其他的吧!');
          this.player.deleteFromPlaylist(index);
          return;
        }
        this.musicNet.downloadFile(res.content, item.name + " - " + item.artists[0].name);
      });
    }
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
