import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router, ParamMap } from '@angular/router';
import { switchMap } from 'rxjs/operators';
import { MusicService } from 'src/app/services/network/music/music.service';
import { Song } from 'src/app/entity/music';
import { ThemeService } from 'src/app/services/theme/theme.service';
import { PlayerService } from 'src/app/services/player/player.service';

@Component({
  selector: 'app-search-result',
  templateUrl: './search-result.component.html',
  styleUrls: ['./search-result.component.scss']
})
export class SearchResultComponent implements OnInit {

  constructor(private route: ActivatedRoute,
    private router: Router,
    private musicNet: MusicService,
    private player: PlayerService,
    private theme: ThemeService) {
  }
  ngOnInit(): void {
    this.route.paramMap.subscribe(map => {
      this.search(decodeURI(map.get("key")));
      this.isloading = true;
    }
    );
  }
  /**
   * 存放搜索结果
   */
  public searchResult: Song[] = [];

  public isloading: boolean = false;

  private async search(key: string) {
    this.isloading = true;
    this.searchResult = [];
    let token = await this.musicNet.getKuWoToken();
    this.musicNet.searchMusic(key, token).subscribe(res => {
      this.isloading = false;
      this.searchResult = res.content;
    });
  }

  public get themeClass(): string {
    return this.theme.getThemeClass();
  }

  public startPlay(song: Song) {
    this.player.addAndPlay(song);
  }

  public addToPlaylist(song: Song) {

  }

  public getTimeFormat(time: number): string {

    return time == null ? null : Math.floor(time / 60).toString().padStart(2, '0') +
      ':' + Math.floor((time % 60)).toString().padStart(2, '0');
  }
}
