import { Component, OnInit } from '@angular/core';
import { MusicService } from 'src/app/services/network/music/music.service';
import { Playlist } from 'src/app/entity/playlist';
import { PlayerService } from 'src/app/services/player/player.service';
import { ExpireContent } from 'src/app/entity/expireContent';

@Component({
  selector: 'app-boutique',
  templateUrl: './boutique.component.html',
  styleUrls: ['./boutique.component.scss']
})
export class BoutiqueComponent implements OnInit {
  /**
   * 推荐歌单缓存Key
   */
  private static readonly personlizedPlaylistKey = 'PERSONLIZEDPLAYLISTKEY';

  /**
   * 推荐歌单缓存过期时间
   */
  private readonly expire = 3600 * 6 * 1000;
  public playlists: Playlist[] = [];
  constructor(
    private musicNet: MusicService,
    private player: PlayerService
  ) {

  }

  ngOnInit() {
    this.initPlaylists();
  }

  public playAll(playlist: Playlist) {
    document.body.style.cursor = "wait";
    this.musicNet.getPlaylistDetail(playlist.id).subscribe(res => {
      if (res.code == 200) {
        this.player.initPlaylist(res.content.musics, 0);
        this.player.start(res.content.musics[0]);
      } else {
        alert(res.message);
      }
      document.body.style.cursor = "default";
    })
  }

  public initPlaylists() {
    let cachedStr = localStorage.getItem(BoutiqueComponent.personlizedPlaylistKey);
    if (cachedStr) {
      let cachedContent = (JSON.parse(cachedStr) as ExpireContent<Playlist[]>);
      if (cachedContent.expire > new Date()) {
        this.playlists = cachedContent.content;
        return;
      }
    }
    this.musicNet.getPersonalizedPlaylist().subscribe(res => {
      if (res.code == 200) {
        this.playlists = res.content;
        window.setTimeout(() => {
          localStorage.setItem(BoutiqueComponent.personlizedPlaylistKey, JSON.stringify(new ExpireContent({
            content: res.content,
            expire: new Date(Date.now() + this.expire)
          })));
        }, 0);
      } else {
        alert("服务出错了!");
      }
    });
  }
}
