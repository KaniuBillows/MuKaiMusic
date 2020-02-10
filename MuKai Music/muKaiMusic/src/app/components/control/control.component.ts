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
  }
  //#region Properties

  /**
   * 当前歌曲信息，由player组件传递
   */
  @Input()
  public get currentMusicInfo(): Song {
    return this._currentMusicInfo;
  }
  public set currentMusicInfo(value: Song) {
    this._currentMusicInfo = value;
  }
  private _currentMusicInfo: Song;

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
   * 上一首按钮点击事件
   * 将事件抛出给player组件，由palyer组件处理
   */
  public onLastTrackButtonClick() {
    this.lastButtonClick.emit();
  }

  /**
   * 下一首按钮点击事件
   * 将事件抛出给player组件，由palyer组件处理
   */
  public onNextTrackButtonClick() {
    this.nextButtonClick.emit();
  }

  /**
   * 播放按钮点击事件
   * 当播放器状态不为'stop'时，恢复播放
   * 否则触发开始新歌曲播放事件，并将进度条复原
   */
  public onPlayButtonClick() {
    if (this.player.status != "stop") {
      this.player.play();
    }
    else {
      this.startPlay.emit(this.currentMusicInfo);
      this.progrees.value = 0.1;
    }
  }

  /**
   * 暂停按钮点击事件，直接控制播放器的状态
   */
  public onPauseButtonClick() {
    this.player.pause();
  }

  /**
   * 静音还原，读取保存的静音前音量并赋值给volume属性
   */
  public onReductionClick() {
    this.volume = this.reductVolume;
  }

  /**
   * 静音，将当前音量保存，并设置音量为0
   */
  public onMuteClick() {
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
  //#endregion

  //#region events
  /**
   * 下一首点击事件，当点击按钮时触发
   */
  @Output()
  public nextButtonClick = new EventEmitter();
  /**
   * 上一首点击事件，当点击按钮触发
   */
  @Output()
  public lastButtonClick = new EventEmitter();

  /**
   * 开始以当前歌曲播放事件，在播放器状态为’stop‘时点击播放按钮触发
   */
  @Output()
  public startPlay = new EventEmitter<Song>();

  //#endregion


}
