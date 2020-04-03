import { Component, OnInit, EventEmitter, Input, Output, ViewChild } from '@angular/core';
import { Song } from 'src/app/entity/music';
import { MatSliderChange, MatSlider } from '@angular/material/slider';
import { PlayerService } from 'src/app/services/player/player.service';

@Component({
  selector: 'app-control',
  templateUrl: './control.component.html',
  styleUrls: ['./control.component.scss']
})
export class ControlComponent implements OnInit {
  @ViewChild('playProgress', { static: true })
  progrees: MatSlider;

  constructor(private player: PlayerService) { }

  ngOnInit() {

    //订阅当前播放时间改变事件
    this.player.onCurrentTimeChange.subscribe((time: number) =>
      this.onTimeChange(time)
    );

    //订阅时长更改事件，对进度条总长度进行处理，并初始化进度值
    this.player.onDurationChange.subscribe((duration: number) => {
      this.progrees.max = duration;
      this.progrees.value = 0.1;
    });

    this.player.onEnded.subscribe(() => {
      this.onNextTrackButtonClick();
    });
    this.player.currentMusicDelete.subscribe(() => {
      this.player.stop();
      if (this.player.playlist.length == 0) return;
      let currentIndex = this.player.currentMusicIndex;
      if (currentIndex == this.player.playlist.length - 1) {
        this.player.currentMusic = this.player.playlist[0];
      } else {
        this.player.currentMusic = this.player.playlist[currentIndex + 1];
      }
    })
  }
  //#region Properties

  public get durationInfo(): string {
    if (this.player.duration != null) {
      if (this.player.status == 'stop') return "00:00/" + this.getTimeFormat(this.player.duration);
      else return this.getTimeFormat(this.player.currentTime) + "/" + this.getTimeFormat(this.player.duration);
    } else {
      return "";
    }
  }

  public get musicInfo(): Song {
    return this.player.currentMusic;
  }

  /**
   * 音量值
   */
  public get volume(): number {
    return this._volume;
  }

  public set volume(value: number) {
    this._volume = value;
    let v = value / 100;
    if (value == 0) this.volumeStatus = 'off';
    if (0 < value && value <= 60) this.volumeStatus = 'mid';
    if (60 < value) this.volumeStatus = 'high';
    this.player.setVolume(v);
  }
  private _volume: number = 45;

  /**
   * 用于保存静音前音量值
   */
  private reductVolume: number = 45;

  /**
    * 音量状态
    */
  public get volumeStatus() {
    return this._volumeStatus;
  }
  public set volumeStatus(value: 'off' | 'mid' | 'high') {
    this._volumeStatus = value;
  }
  private _volumeStatus: 'off' | 'mid' | 'high' = 'mid';

  /**
   * 播放器播放状态
   */
  public get playStatus() {
    return this.player.status;
  }

  //#endregion

  //#region methods

  /**
   * 上一首
   */
  public onLastTrackButtonClick() {
    if (this.player.playlist.length == 0) return;
    let currentIndex = this.player.currentMusicIndex;
    if (currentIndex == 0) {
      this.player.start(this.player.playlist[this, this.player.playlist.length - 1]);
    } else {
      this.player.start(this.player.playlist[currentIndex - 1]);
    }
  }

  /**
   * 下一首
   */
  public onNextTrackButtonClick() {
    if (this.player.playlist.length == 0) return;
    let currentIndex = this.player.currentMusicIndex;
    if (currentIndex == this.player.playlist.length - 1) {
      this.player.start(this.player.playlist[0]);
    } else {
      this.player.start(this.player.playlist[currentIndex + 1]);
    }
  }

  /**
   * 播放按钮点击事件
   * 当播放器状态不为'stop'时，恢复播放
   * 否则触发开始新歌曲播放事件，并将进度条复原
   */
  public playButtonClick() {
    if (this.player.status != "stop") {
      this.player.play();
    }
    else {
      this.player.start(this.player.currentMusic);
      this.progrees.value = 0.1;
    }
  }

  /**
   * 暂停按钮点击事件，直接控制播放器的状态
   */
  public pauseButtonClick() {
    this.player.pause();
  }

  /**
   * 静音还原，读取保存的静音前音量并赋值给volume属性
   */
  public reductionClick() {
    this.volume = this.reductVolume;
  }

  /**
   * 静音，将当前音量保存，并设置音量为0
   */
  public muteClick() {
    this.reductVolume = this.volume;
    this.volume = 0;
  }

  /**
   * 用户拖动进度条处理，播放器进行快进
   * @param ev 
   */
  public progressInput(ev: MatSliderChange) {
    this.player.seek(ev.value);
  }

  /**
   * 播放时间改变事件，直接写入进度条当前值
   */
  private onTimeChange(time: number) {
    this.progrees.writeValue(time);

  }

  /**
   * 格式化时间
   * @param time 
   */
  private getTimeFormat(time: number): string {
    return Math.floor(time / 60).toString().padStart(2, '0') +
      ':' + Math.floor((time % 60)).toString().padStart(2, '0');
  }

  //#endregion




}
