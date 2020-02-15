import { Injectable, EventEmitter } from '@angular/core';
import { Song } from 'src/app/entity/music';

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

  public playlist: Song[] = [];

  public currentMusic: Song = {} as any;

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

  public start(url: string, song: Song) {
    this.player.src = url;
    this.play();
    this.currentMusic = song;
    this.onMusicChange.emit();
  }

  public addAndPlay(song: Song) {
    this.currentMusic = song;
    this.playlist.push(song);
    this.addMusicAndPlay.emit(song);
    this.playlistChange.emit();
  }

  /**
   * 删除播放列表中的某一首歌曲
   */
  public deleteFromPlaylist(index: number) {
    if (index == this.playlist.findIndex(item => item === this.currentMusic)) {
      this.playlist.splice(index, 1);
      this.currentMusicDelete.emit();
      this.playlistChange.emit();
      return;
    }
    this.playlist.splice(index, 1);
    this.playlistChange.emit();
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

  /**
   * 当前歌曲切换事件，主要用于playlist定位当前播放歌曲
   */
  public onMusicChange = new EventEmitter<Song>();

  /**
   * 添加歌曲到播放列表并开始播放事件
   * 由player组件订阅处理，拿到被添加的歌曲后，请求URL并开始播放
   */
  public addMusicAndPlay = new EventEmitter<Song>();

  /**
   * 播放列表改变事件，主要由playlist订阅处理，重新计算searchbar
   */
  public playlistChange = new EventEmitter();

  /**
 * 当前歌曲被删除事件，由player组件进行调度处理
 * 涉及到music-info组件的内容显示，播放器的播放状态更改，control组件的内容显示
 */
  public currentMusicDelete = new EventEmitter();
  //#endregion
}
