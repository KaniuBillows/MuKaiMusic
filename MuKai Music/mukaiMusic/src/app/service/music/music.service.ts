import { Injectable, EventEmitter } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class MusicService {
  private player: HTMLAudioElement;

  constructor() {
    this.player = document.createElement("audio");
    this.player.onplaying = () => this.onPlaying.emit();
    this.player.onpause = () => this.onPause.emit();
  }

  /**
   * 以src为播放源开始播放
   * @param src 播放源
   */
  public startPlay(src: string) {
    this.player.src = src;
    this.player.play();
  }

  /**
   * 从暂停中恢复播放
   */
  public play() {
    if (this.player.src) this.player.play();
  }

  /**
   * 暂停
   */
  public pause() {
    this.player.pause();
  }

  /**
   * 开始播放事件
   */
  public onPlaying = new EventEmitter();

  /**
   * 暂停事件
   */
  public onPause = new EventEmitter();

}
