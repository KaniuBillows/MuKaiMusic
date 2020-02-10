import { Injectable, EventEmitter } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class PlayerService {

  private player: HTMLAudioElement;

  private _status: 'pause' | 'loading' | 'playing' | 'stop' = 'stop';

  constructor() {
    this.player = document.createElement('audio');
    this.player.preload = "load";
    this.player.ontimeupdate = () => {
      this.onCurrentTimeChange.emit(this.player.currentTime);
    }
    this.player.ondurationchange = () => {
      this.onDurationChange.emit(this.player.duration);
    }
    this.player.onended = () => {
      this.onEnded.emit();
    }
  }

  //#region public property
  public set status(value: 'pause' | 'loading' | 'playing' | 'stop') {
    this._status = value;
  }

  public get status() {
    return this._status;
  }

  public get src() {
    return this.player.src;
  }

  public get duration() {
    return this.player.duration;
  }
  //#endregion

  //#region public methods
  public play() {
    if (this.player.src) {
      this.status = 'playing';
      this.player.play();
    }
  }

  public stop() {
    this.player.pause();
    this.status = "stop";
  }

  public pause() {
    this.status = 'pause';
    this.player.pause();
  }

  public start(url: string) {
    this.player.src = url;
    this.play();
  }

  public setVolume(volume: number) {
    this.player.volume = volume;
  }

  public seek(value: number) {
    this.player.currentTime = value;
  }

  //#endregion

  //#region events

  public onCurrentTimeChange = new EventEmitter<number>();

  public onDurationChange = new EventEmitter<number>();

  public onEnded = new EventEmitter();

  //#endregion
}
