import { Component, OnInit } from '@angular/core';
import { MusicService } from 'src/app/services/network/music/music.service';
import { Playlist } from 'src/app/entity/playlist';

@Component({
  selector: 'app-boutique',
  templateUrl: './boutique.component.html',
  styleUrls: ['./boutique.component.scss']
})
export class BoutiqueComponent implements OnInit {

  public playlists: Playlist[] = [];
  constructor(private musicNet: MusicService) {

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

}
