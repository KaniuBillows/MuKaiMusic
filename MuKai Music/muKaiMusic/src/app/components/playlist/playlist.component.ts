import { Component, OnInit, Input, EventEmitter, Output } from '@angular/core';
import { Song } from 'src/app/entity/music';
import { ThemeService } from 'src/app/services/theme/theme.service';
import { PlayerService } from 'src/app/services/player/player.service';

@Component({
  selector: 'app-playlist',
  templateUrl: './playlist.component.html',
  styleUrls: ['./playlist.component.scss']
})
export class PlaylistComponent implements OnInit {

  constructor(
    public theme: ThemeService,
    public player: PlayerService
  ) { }
  ngOnInit() {

    let playlist_container = document.getElementById("playlist-container") as HTMLDivElement;
    let scroll = document.getElementById("scroll") as HTMLDivElement;
    let bar = document.getElementById("bar") as HTMLDivElement;
    let ul = document.getElementById("ul") as HTMLUListElement;
    this.playlist_container = playlist_container;
    this.scroll = scroll;
    this.bar = bar;
    let getPage = this.getPage;
    //记录上一次滚动距离，用于鼠标滚动
    let oringin = 0;
    //拖动滚动条 内容滑动
    this.bar.onmousedown = function (e: MouseEvent) {
      //e = e || window.event;
      //鼠标在滚动条中的位置
      var y = getPage(e).pageY - playlist_container.offsetTop - bar.offsetTop;
      //2.2鼠标在页面上移动的时候，滚动条的位置
      document.onmousemove = function (e) {
        var barY = getPage(e).pageY - y - playlist_container.offsetTop;
        // 控制bar不能移除scroll
        barY = barY < 0 ? 0 : barY;
        barY = barY > scroll.clientHeight - bar.clientHeight ? scroll.clientHeight - bar.clientHeight : barY;
        oringin = barY;
        bar.style.top = barY + 'px';
        //3.当拖拽时，内容跟着滚动
        let offsetY = (ul.clientHeight + 46 - playlist_container.clientHeight) * (barY / (playlist_container.clientHeight - bar.clientHeight));
        ul.style.transform = `translateY(-${offsetY}px)`;
      }
    }
    document.onmouseup = function () {
      //移除鼠标移动事件
      document.onmousemove = null;
    }
    //当播放列表元素改变时,更新scroll bar状态
    this.player.playlistChange.subscribe(() => {
      this.initScroll();
    });
    //设定是否允许鼠标滚动
    this.playlist_container.onmouseenter = (e: MouseEvent) => {
      this.allowScroll = true;
    }
    //设定是否允许鼠标滚动
    this.playlist_container.onmouseleave = (e: MouseEvent) => {
      this.allowScroll = false;
    }
    //鼠标滚轮滑动播放列表
    window.onmousewheel = (e: MouseWheelEvent) => {
      if (this.allowScroll) {
        oringin += e.deltaY / 5;
        //防止超出范围
        oringin = oringin < 0 ? 0 : oringin;
        oringin = oringin > scroll.clientHeight - bar.clientHeight ? scroll.clientHeight - bar.clientHeight : oringin;
        var barY = oringin;
        bar.style.top = barY + 'px';
        let offsetY = (ul.clientHeight + 46 - playlist_container.clientHeight) * (barY / (playlist_container.clientHeight - bar.clientHeight));
        ul.style.transform = `translateY(-${offsetY}px)`;
      }
    }
    //播放歌曲切换时，自动进行滑动
    this.player.onMusicChange.subscribe(() => {
      let barY = (this.currentPlayIndex - 9) * 46;
      barY = barY < 0 ? 0 : barY;
      barY = barY > scroll.clientHeight - bar.clientHeight ? scroll.clientHeight - bar.clientHeight : barY;
      bar.style.top = barY + 'px';
      let offsetY = (ul.clientHeight + 46 - playlist_container.clientHeight) * (barY / (playlist_container.clientHeight - bar.clientHeight));
      ul.style.transform = `translateY(-${offsetY}px)`;
    });
  }

  /**
   * 播放列表box
   */
  private playlist_container: HTMLDivElement;

  /**
   * 滚动条
   */
  private scroll: HTMLDivElement;

  /**
   * 滚动滑块
   */
  private bar: HTMLDivElement;

  /**
   * 当鼠标在播放列表范围内时，允许鼠标滚动
   */
  private allowScroll: boolean = false;



  /**
   * 当前播放的歌曲索引，由player组件产生
   */
  @Input()
  public get currentPlayIndex() {
    return this._currentPlayIndex;
  }
  public set currentPlayIndex(value: number) {
    this._currentPlayIndex = value;
  }
  private _currentPlayIndex: number;

  /**
   * 播放列表，涉及到歌曲的删除所以
   * 与player组件进行双向绑定
   */
  @Input()
  public get playlist(): Song[] {
    return this._playlist;
  }
  public set playlist(value: Song[]) {
    this._playlist = value;
    //this.playlistChange.emit();
  }
  private _playlist: Song[] = [];

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
    return Math.floor(time / 60).toString().padStart(2, '0') +
      ':' + Math.floor((time % 60)).toString().padStart(2, '0');
  }

  /** 
   * 点击播放此歌曲 
   * @param index 歌曲索引
  */
  public clickPlay(index: number) {
    this.item_play_Click.emit(index);
  }

  /**
   * 点击下载此歌曲
   * @param index 歌曲索引
   */
  public clickDownload(index: number) {
    this.item_download_Click.emit(index);
  }

  /**
   * 删除播放列表中的歌曲 并触发播放列表改变事件
   * 如果删除当前播放歌曲，则触发'currentMusicDelete'事件，由player组件订阅处理
   * @param index 歌曲索引
   */
  public deleteMusic(index: number) {
    this.player.deleteFromPlaylist(index);
  }

  /**
   * scroll bar相关
   * 由播放列表中元素数量决定
   * 控制滚动条长度以及是否显示滚动条
   */
  private initScroll() {
    let barHeight = 0;
    let visible = "hidden";
    this.allowScroll = false;
    if (this.playlist.length > 10) {
      barHeight = this.playlist_container.clientHeight * (10 / this.playlist.length);
      visible = "visible";
      this.allowScroll = true;
    }
    this.bar.style.height = barHeight + "px";
    this.scroll.style.visibility = visible;

  }

  /**
   * scroll bar相关
   * 对滚动条的点击位置进行处理
   * @param e 
   */
  private getPage(e) {
    var pageX = e.pageX || e.clientX + this.getScroll().scrollLeft;
    var pageY = e.pageY || e.clientY + this.getScroll().scrollTop;
    return {
      pageX: pageX,
      pageY: pageY
    }
  }

  /**
   * scroll bar相关
   * 对滚动条进行处理
   */
  private getScroll() {
    var scrollLeft = document.body.scrollLeft || document.documentElement.scrollLeft;
    var scrollTop = document.body.scrollTop || document.documentElement.scrollTop;
    return {
      scrollLeft: scrollLeft,
      scrollTop: scrollTop
    }
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
