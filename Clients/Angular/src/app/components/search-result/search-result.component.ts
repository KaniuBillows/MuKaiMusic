import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router, ParamMap } from '@angular/router';
import { switchMap } from 'rxjs/operators';
import { MusicService } from 'src/app/services/network/music/music.service';
import { Song } from 'src/app/entity/music';
import { ThemeService } from 'src/app/services/theme/theme.service';
import { PlayerService } from 'src/app/services/player/player.service';
import { MatSnackBar } from '@angular/material/snack-bar';
import { SnackBarComponent } from '../snackBar/snackBar.component';
import BScroll from '@better-scroll/core';
import ScrollBar from '@better-scroll/scroll-bar'
import MouseWheel from '@better-scroll/mouse-wheel';
import ObserveDOM from '@better-scroll/observe-dom';
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
    let wrapper = document.getElementById("result-container");
    BScroll.use(MouseWheel);
    BScroll.use(ObserveDOM);
    BScroll.use(ScrollBar);
    let scroll = new BScroll(wrapper, {
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
    let scrollbars = document.querySelectorAll<HTMLDivElement>(".bscroll-indicator");
    scrollbars.forEach(bar => bar.style.border = 'none');

  }

  /**
   * 存放搜索结果
   */
  public searchResult: Song[] = [];

  public isloading: boolean = false;

  private async search(key: string) {
    this.isloading = true;
    this.searchResult = [];
    this.musicNet.searchMusic(key).then(songs => {
      this.isloading = false;
      this.searchResult = songs;
    });
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
