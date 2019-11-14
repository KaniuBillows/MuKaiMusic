import { Component, OnInit } from '@angular/core';
import { MusicService } from 'src/app/service/music/music.service';
import { MatIconRegistry } from '@angular/material/icon';
import { DomSanitizer } from '@angular/platform-browser';

@Component({
  selector: 'app-player',
  templateUrl: './player.component.html',
  styleUrls: ['./player.component.scss']
})
export class PlayerComponent implements OnInit {

  constructor(iconRegistry: MatIconRegistry,
    sanitizer: DomSanitizer) {
    iconRegistry.addSvgIcon("pause_button", sanitizer.bypassSecurityTrustResourceUrl('assets/icon/pause.svg'))
    iconRegistry.addSvgIcon("play_button", sanitizer.bypassSecurityTrustResourceUrl('assets/icon/play.svg'))
  }

  ngOnInit() {

  }


}
