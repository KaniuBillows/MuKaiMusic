import { Component, OnInit } from '@angular/core';
import { MusicService } from 'src/app/services/network/music/music.service';
import { Playlist } from 'src/app/entity/playlist';
import { PlayerService } from 'src/app/services/player/player.service';

@Component({
  selector: 'app-boutique',
  templateUrl: './boutique.component.html',
  styleUrls: ['./boutique.component.scss']
})
export class BoutiqueComponent implements OnInit {

  public playlists: Playlist[] = [];
  constructor(
    private musicNet: MusicService,
    private player: PlayerService
  ) {

  }

  ngOnInit() {
    this.musicNet.getPersonalizedPlaylist().subscribe(res => {
      if (res.code == 200) {
        this.playlists = res.content;
      } else {
        alert("服务出错了!");
      }
    });
  }

  public playAll(playlist: Playlist) {
    document.body.style.cursor = "wait";
    this.musicNet.getPlaylistDetail(playlist.id).subscribe(res => {
      if (res.code == 200) {
        this.player.initPlaylist(res.content.musics, 0);
        this.player.start(res.content.musics[0]);
      } else {
        alert(res.error);
      }
      document.body.style.cursor = "default";
    })
  }
}
