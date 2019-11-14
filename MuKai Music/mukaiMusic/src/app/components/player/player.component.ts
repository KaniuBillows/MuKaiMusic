import { Component, OnInit } from '@angular/core';
import { PlayerService } from 'src/app/services/player/player.service';

@Component({
  selector: 'app-player',
  templateUrl: './player.component.html',
  styleUrls: ['./player.component.scss']
})
export class PlayerComponent implements OnInit {

  constructor(private play: PlayerService) {

  }
  
  public get stauts() {
    return this.play.status;
  }
  ngOnInit() {
  }

}
