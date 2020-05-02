import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router, ParamMap } from '@angular/router';
import { switchMap } from 'rxjs/operators';
import { MusicService } from 'src/app/services/network/music/music.service';
import { Song } from 'src/app/entity/music';
import { ThemeService } from 'src/app/services/theme/theme.service';
import { PlayerService } from 'src/app/services/player/player.service';
import { MatSnackBar } from '@angular/material/snack-bar';
import { SnackBarComponent } from '../snackBar/snackBar.component';

@Component({
  selector: 'app-search-result',
  templateUrl: './search-result.component.html',
  styleUrls: ['./search-result.component.scss']
})
export class SearchResultComponent implements OnInit {

  constructor(private route: ActivatedRoute,
    private snackBar: MatSnackBar,
    private musicNet: MusicService,
    private player: PlayerService,
    private theme: ThemeService) {
  }
  ngOnInit(): void {
    this.route.paramMap.subscribe(map => {
      this.search(decodeURI(map.get("key")));
      this.isloading = true;
    });

    let result_container = document.getElementById('result-container') as HTMLDivElement;
    let scroll = document.getElementById("r-scroll") as HTMLDivElement;
    let bar = document.getElementById("r-bar") as HTMLDivElement;
    let ul = document.getElementById("r-ul") as HTMLDivElement;

    //记录上一次滚动距离，用于鼠标滚动
    let oringin = 0;
    //拖动滚动条 内容滑动
    bar.onmousedown = function (e: MouseEvent) {

      //鼠标在滚动条中的位置
      let spaceY = e.clientY - bar.offsetTop;
      //2.2鼠标在页面上移动的时候，滚动条的位置
      document.onmousemove = function (e) {
        let y = e.clientY - spaceY;
        y = y < 0 ? 0 : y;
        y = y > scroll.offsetHeight - bar.offsetHeight ? scroll.offsetHeight - bar.offsetHeight : y;
        // 控制bar不能移除scroll
        oringin = y;
        bar.style.top = y + 'px';
        //3.当拖拽时，内容跟着滚动
        let offsetY = y * (ul.offsetHeight - result_container.offsetHeight) / (scroll.offsetHeight - bar.offsetHeight);
        ul.style.top = -offsetY + "px";
      }
    }
    document.onmouseup = function () {
      //移除鼠标移动事件
      document.onmousemove = null;
    }
    //设定是否允许鼠标滚动
    result_container.onmouseenter = (e: MouseEvent) => {
      this.allowScroll = true;
    }
    //设定是否允许鼠标滚动
    result_container.onmouseleave = (e: MouseEvent) => {
      this.allowScroll = false;
    }
    //鼠标滚轮滑动播放列表
    window.addEventListener('mousewheel', ((e: MouseWheelEvent) => {
      if (this.allowScroll) {
        oringin += e.deltaY / 5;
        //防止超出范围
        oringin = oringin < 0 ? 0 : oringin;
        oringin = oringin > scroll.clientHeight - bar.clientHeight ? scroll.clientHeight - bar.clientHeight : oringin;
        var barY = oringin;
        bar.style.top = barY + 'px';
        let offsetY = barY * (ul.offsetHeight - result_container.offsetHeight) / (scroll.offsetHeight - bar.offsetHeight);
        ul.style.top = -offsetY + "px";
      }
    }))
  }
  //#region 滚动

  /**
   * 当鼠标在播放列表范围内时，允许鼠标滚动
   */
  private allowScroll: boolean = false;

  //#endregion

  /**
   * 存放搜索结果
   */
  public searchResult: Song[] = [];

  public isloading: boolean = false;

  private async search(key: string) {
    this.isloading = true;
    this.searchResult = [];
    this.musicNet.searchMusic(key).subscribe(res => {
      this.isloading = false;
      this.searchResult = res.content;
      window.setTimeout(this.initScroll, 100);
    });
  }
  private initScroll() {
    let ul = document.getElementById("r-ul") as HTMLDivElement;
    let result_container = document.getElementById("result-container");
    let scroll = document.getElementById("r-scroll");
    let bar = document.getElementById("r-bar");
    let barHeight = 0;
    let visible = "hidden";
    this.allowScroll = false;
    barHeight = result_container.offsetHeight * scroll.offsetHeight / ul.offsetHeight;
    visible = "visible";
    this.allowScroll = true;
    bar.style.height = barHeight + "px";
    scroll.style.visibility = visible;
  }
  public get themeClass(): string {
    return this.theme.getThemeClass();
  }

  public startPlay(song: Song) {
    if (song.url)
      this.player.addAndPlay(song);
    else {
      this.musicNet.getUrl(song).subscribe(res => {
        if (res.code != 200 || res.content == null) {
          this.snackBar.openFromComponent(SnackBarComponent, { duration: 2500, data: "这首歌不让听了，试试其他的吧!" });
        } else {
          song.url = res.content;
          this.player.addAndPlay(song);
        }
      })
    }
  }

  public addToPlaylist(song: Song) {
    this.player.addToPlaylist(song);
  }

  public getTimeFormat(time: number): string {

    return time == null ? null : Math.floor(time / 60).toString().padStart(2, '0') +
      ':' + Math.floor((time % 60)).toString().padStart(2, '0');
  }
}
