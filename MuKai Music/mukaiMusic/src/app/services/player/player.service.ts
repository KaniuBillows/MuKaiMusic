import { Injectable, EventEmitter } from '@angular/core';
import { Song } from 'src/app/entity/music';
import { MusicService } from '../network/music/music.service';
import { Title } from '@angular/platform-browser';
import { DataSource } from 'src/app/entity/param/musicUrlParam';
import { MatSnackBar } from '@angular/material/snack-bar';
import { SnackBarComponent } from 'src/app/components/snackBar/snackBar.component';
export const CurrentMusicIndex = 'CURRENTMUSICINDEX';
export const Playlist = 'PLAYLIST';
export const PlayMode = 'PLAYMODE';
@Injectable({
  providedIn: 'root'
})
export class PlayerService {

  private player: HTMLAudioElement;

  private _status: 'pause' | 'loading' | 'playing' | 'stop' = 'stop';

  private _mode: 'single' | 'normal' | 'random';

  constructor(
    private musicNet: MusicService,
    private title: Title,
    private snackBar: MatSnackBar
  ) {
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
    this.playlistChange.subscribe(() => {
      localStorage.setItem(Playlist, JSON.stringify(this._playlist));
    });
    this.player.onerror = (ev) => {
      this.currentMusic.url = null;
      this.start(this.currentMusic);
    };
    let mode: any = localStorage.getItem(PlayMode);
    this.mode = mode != null ? mode : 'normal';
  }

  //#region public property

  private _playlist: Song[] = [];

  public get playlist(): Song[] {
    return this._playlist;
  }

  public get currentMusicIndex(): number {
    return this.playlist.indexOf(this.currentMusic);
  }

  public _currentMusic: Song = {} as any;

  public set currentMusic(value: Song) {
    this._currentMusic = value;
    this.title.setTitle(this.currentMusic.name + ' - ' + this.currentMusic.artists[0].name);
    this.currentMusicChange.emit(this._currentMusic);
    if (!this.playlist.includes(this._currentMusic)) {
      this.playlist.push(this._currentMusic);
    }
    localStorage.setItem(CurrentMusicIndex, this.currentMusicIndex.toString());
  }
  public get currentMusic() {
    return this._currentMusic;
  }

  public set status(value: 'pause' | 'loading' | 'playing' | 'stop') {
    this._status = value;
  }

  public set mode(value: 'single' | 'normal' | 'random') {
    this._mode = value;
    localStorage.setItem(PlayMode, value);
  }

  public get mode() {
    return this._mode;
  }

  public get status() {
    return this._status;
  }

  public get src() {
    return this.player.src;
  }

  public get duration() {
    if (this.currentMusic?.duration) {
      return this.currentMusic.duration
    }
    if (this.player.src) {
      //this.currentMusic.duration = this.player.duration;
      return this.player.duration;
    }
    return null;
  }

  public get currentTime() {
    return this.player.currentTime;
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

  /**
   * 获取播放链接，并开始播放
   * 成功获取播放链接，产生当前音乐改变事件
   * @param song 
   */
  public async start(song: Song) {
    this._status = 'loading';
    if (song.url != null) {
      if (song.dataSource != DataSource.Migu)
        song.url = song.url.replace("http://", "https://");
      this.player.src = song.url;
      this.play();
      this.currentMusic = song;
    }
    else {
      this.musicNet.getUrl(song).subscribe(res => {
        if (res.content == null) {
          this.snackBar.openFromComponent(SnackBarComponent, { duration: 2500, data: "这首歌居然不让听了，试试其他的吧" });
          this.deleteFromPlaylist(this.playlist.indexOf(song));
          this.onEnded.emit();
          return;
        }
        if (song.dataSource != DataSource.Migu)
          res.content = res.content.replace("http://", "https://");
        this.player.src = res.content;
        this.play();
        this.currentMusic = song;
      }, err => {
        alert("无法访问服务器");
      });
    }
  }

  /**
   * 将一首歌曲添加到播放列表，并立即开始播放
   * @param song 
   */
  public async addAndPlay(song: Song) {
    if (song.url != null)
      this.player.src = song.url;
    else {
      let result = await this.musicNet.getUrl(song).toPromise();
      if (result.content == null) {
        alert("这首歌居然不让听了! 试试其他的吧!");
        return;
      }
      this.player.src = result.content;
    }
    this.playlist.push(song);
    this.playlistChange.emit();
    this.play();
    this.currentMusic = song;
  }

  /**
   * 删除播放列表中的某一首歌曲
   */
  public deleteFromPlaylist(index: number) {
    if (index == this.currentMusicIndex) {
      this.player.pause();
      this._playlist.splice(index, 1);
      if (this._playlist.length == 0) return;
      if (index == this.playlist.length - 1) this.currentMusic = this.playlist[0];
      this.currentMusic = this.playlist[index];
      if (this._status == 'playing')
        this.start(this.currentMusic);
      this.playlistChange.emit();
      return;
    }
    this._playlist.splice(index, 1);
    this.playlistChange.emit();
  }

  /**
   * 将歌曲添加到播放列表，会产生播放列表改变事件
   * @param song 
   */
  public addToPlaylist(song: Song[] | Song) {
    this._playlist = this._playlist.concat(song);
    this.playlistChange.emit();
  }

  public setVolume(volume: number) {
    this.player.volume = volume;
  }

  public seek(value: number) {
    this.player.currentTime = value;
  }

  public initPlaylist(playlist: Song[], index?: number) {
    this._playlist = playlist;
    this.playlistChange.emit();
    if (index != null) {
      if (index >= 0 && index < playlist.length) {
        this.currentMusic = playlist[index];
      }
      else {
        this.currentMusic = playlist[0];
      }
    } else {
      this.currentMusic = playlist[0];
    }
  }

  //#endregion

  //#region events

  public onCurrentTimeChange = new EventEmitter<number>();

  public onDurationChange = new EventEmitter<number>();

  public onEnded = new EventEmitter();

  /**
   * 当前歌曲切换事件
   */
  public currentMusicChange = new EventEmitter<Song>();

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
