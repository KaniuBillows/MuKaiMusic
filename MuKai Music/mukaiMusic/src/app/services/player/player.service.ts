import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class PlayerService {

  private player: HTMLAudioElement;

  private _status: 'pause' | 'playing' | 'stop' = 'stop';

  constructor() {
    this.player = document.createElement('audio');
  }

  public set status(value: 'pause' | 'playing' | 'stop') {
    this._status = value;
  }

  public get status() {
    return this._status;
  }

  public play() {
    if (this.player.src) {
      this.status = 'playing';
      this.player.play();
    }
  }

  public pause() {
    this.status = 'pause';
    this.player.pause();
  }

  public start(url: string) {
    this.player.src = url;
    this.player.pause();
  }

}
