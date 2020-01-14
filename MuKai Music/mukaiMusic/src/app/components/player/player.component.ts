import { Component, OnInit, ViewChild, ElementRef } from '@angular/core';
import { PlayerService } from 'src/app/services/player/player.service';
import { MatSlider, MatSliderChange } from '@angular/material/slider';
import { MusicService } from 'src/app/services/network/music/music.service';
import { song, musicDetailResult, album, artist } from 'src/app/entity/music';
import { ThemeService } from 'src/app/services/theme/theme.service';
import { UserService } from 'src/app/services/network/user/user.service';


@Component({
    selector: 'app-player',
    templateUrl: './player.component.html',
    styleUrls: ['./player.component.scss']
})
export class PlayerComponent implements OnInit {
    @ViewChild('playProgress', { static: true })
    progrees: MatSlider;

    @ViewChild('lyrics', { static: true })
    lyrics: ElementRef;

    private _volumeStatus: 'off' | 'mid' | 'high' = 'mid';

    private _volume: number = 45;

    private _stagingVolume: number;

    private _lyric_paras: {
        text: string,
        time: number,
    }[] = [];

    private _showPalette: boolean = false;

    private _currentMusicInfo: {
        name: string,
        id: number,
        albumName: string,
        albumId: number,
        picUrl: string,
        artistName: string;
        artistId: number
    } = { picUrl: '' } as any;

    private _currentLrcIndex: number = 0;

    private _playlist: song[] = [];

    constructor(
        private player: PlayerService,
        public theme: ThemeService,
        private musicNet: MusicService) {
    }
    ngOnInit() {
        this.player.onCurrentTimeChange.subscribe((time: number) =>
            this.onTimeChange(time)
        );
        this.player.onDurationChange.subscribe((duration: number) => {
            this.progrees.max = duration;
            this.progrees.value = 0.1;
        });
        this.player.onEnded.subscribe(() => {
            this.onNextTrackButtonClick();
        })
        this.getPersonalized();
    }

    public get playlist() {
        return this._playlist;
    }

    public get showPalette() {
        return this._showPalette;
    }
    

    /**
     * 当前播放歌曲
     */
    public get currentMusicInfo() {
        return this._currentMusicInfo;
    }

    public get currentLrcIndex() {
        return this._currentLrcIndex;
    }

    public set currentLrcIndex(value: number) {
        this._currentLrcIndex = value;
    }

    public get currentMusicIndex() {
        return this._playlist.findIndex(item =>
            item.id == this.currentMusicInfo.id
        );
    }

    public get isLastMusic() {
        return this.currentMusicIndex == this._playlist.length - 1;
    }
    public get isFirstMusic() {
        return this.currentMusicIndex == 0;
    }
    /**
     * 音量状态
     */
    public get volumeStatus() {
        return this._volumeStatus;
    }

    public set volumeStatus(value: 'off' | 'mid' | 'high') {
        this._volumeStatus = value;
    }

    /**
     * 显示歌词
     */
    public get lyric_paras() {
        return this._lyric_paras;
    }

    /**
     * 音量
     */
    public set volume(value: number) {
        this._volume = value;
        let v = value / 100;
        this.player.setVolume(v);
        if (value == 0) this.volumeStatus = 'off';
        if (0 < value && value <= 60) this.volumeStatus = 'mid';
        if (60 < value) this.volumeStatus = 'high';
    }

    public get volume() {
        return this._volume;
    }

    /**
     * 播放状态
     */
    public get stauts() {
        return this.player.status;
    }

    public get themeClass() {
        return this.theme.getThemeClass();
    }
    /**
     * 转换时间格式
     * @param time 
     */
    public getTimeFormat(time: number): string {
        return Math.floor(time / 60000).toString().padStart(2, '0') +
            ':' + Math.floor((time % 60000) / 1000).toString().padStart(2, '0');
    }
    /**
     * 播放按钮点击事件
     */
    public onPlayButtonClick() {
        if (this.player.src != '') {
            this.player.play();
        }
        else {
            this.startPlay();
        }
    }

    /**
     * 暂停
     */
    public onPauseButtonClick() {
        this.player.pause();
    }

    /**
     * 下一首
     */
    public onNextTrackButtonClick() {
        if (this._playlist.length < 1) {
            return;
        }
        if (this.isLastMusic) {
            this.getMusicDetail(this._playlist[0].id, () =>
                this.startPlay()
            );
        } else {
            this.getMusicDetail(this._playlist[this.currentMusicIndex + 1].id, () =>
                this.startPlay()
            );
        }
    }
    /**
     * 上一首
     */
    public onLastTrackButtonClick() {
        if (this._playlist.length < 1) {
            return;
        }
        if (this.isFirstMusic) {
            this.getMusicDetail(this._playlist[this._playlist.length - 1].id, () =>
                this.startPlay());
        } else {
            this.getMusicDetail(this._playlist[this.currentMusicIndex - 1].id, () =>
                this.startPlay());
        }
    }
    public onMuteClick() {
        this._stagingVolume = this._volume;
        this.volume = 0;
    }
    public onReductionClick() {
        this.volume = this._stagingVolume;
    }
    public progressInput(ev: MatSliderChange) {
        this.player.seek(ev.value);
    }

    //#region Actions
    /**
     * 点击播放歌曲
     * @param index 
     */
    public clickPlay(index: number) {
        this.getMusicDetail(this._playlist[index].id, () => this.startPlay());
    }

    /**
     * 点击下载歌曲
     * @param index 
     */
    public clickDownload(index: number) {
        let item = this.playlist[index];
        this.musicNet.getNeteaseUrl(item.id).subscribe(res => {
            if (res.data[0].url) {
                this.musicNet.downloadFile(res.data[0].url, item.name + " - " + item.artists[0].name);
            }
        })
    }
    //#endregion



    //#region private method

    /**
     * 获取当前歌曲的播放链接并开始播放
     */
    private startPlay() {
        this.player.status = 'loading';
        this.musicNet.getNeteaseUrl(this.currentMusicInfo.id).subscribe(res => {
            if (res.data[0].url) {
                this.player.onCurrentTimeChange.subscribe((time: number) =>
                    this.onTimeChange(time)
                )
                this.player.start(res.data[0].url);
            } else {
                alert('获取播放链接失败!');
                this.player.status = 'stop';
            }
        }, err => {
            this.player.status = 'stop';
            alert('获取播放链接失败!');
        });
    }
    /**
     * 获取歌词
     * @param id 
     */
    private getLyric(id: number) {
        this._lyric_paras = [];
        this.musicNet.getLyric(id).subscribe(res => {
            //匹配包含[00:00.000]格式的时间字符串
            let regex = /^\[[0-9]{2}\:[0-9]{2}\.[0-9]{2,3}\]/;
            let lrcs: string[] = res.lrc.lyric.split('\n');
            lrcs.forEach(lrc => {
                if (regex.test(lrc)) {
                    //时间信息字符串
                    let timeString = lrc.substring(lrc.indexOf('[') + 1, lrc.indexOf(']'));
                    //分析字符串得到时间信息
                    let time = Number.parseInt(timeString.substr(0, 2)) * 60
                        + Number.parseFloat(timeString.substring(timeString.indexOf(':') + 1));
                    this._lyric_paras.push({
                        text: lrc.substring(timeString.length + 2),
                        time: time
                    });
                } else {
                    this._lyric_paras.push({
                        text: lrc.substring(lrc.indexOf('[') + 1, lrc.indexOf(']')),
                        time: null
                    })
                }
            })
        });
    }
    /**
    * 获取推荐歌曲
    */
    private getPersonalized() {
        this.musicNet.getPersonalizedMusics().subscribe(res => {
            res.result.forEach(element => {
                this._playlist.push(element.song);
            });
            this.getMusicDetail(this.playlist[0].id);
        });
    }
    /**
     * 获取详情以及歌词
     * @param id 
     */
    private async getMusicDetail(id: number, callback?: () => void) {
        this.getLyric(id);
        return this.musicNet.getMusicDetail([id]).subscribe((res: musicDetailResult) => {
            if (res.songs && res.songs.length > 0) {
                this._currentMusicInfo.name = res.songs[0].name;
                this._currentMusicInfo.id = res.songs[0].id;
                this._currentMusicInfo.albumName = res.songs[0].al.name;
                this._currentMusicInfo.albumId = res.songs[0].al.id;
                this._currentMusicInfo.picUrl = res.songs[0].al.picUrl;
                this._currentMusicInfo.artistId = res.songs[0].ar[0].id;
                this._currentMusicInfo.artistName = res.songs[0].ar[0].name;
                document.getElementById('back-board-mask').style.backgroundColor = 'rgba(0,0,0,.4)';
            }
        }, null, callback);
    }
    /**
     * 播放时间改变事件
     */
    private onTimeChange(time: number) {
        this.progrees.writeValue(time);
        this.currentLrcIndex = this.lyric_paras.findIndex(item => item.time > time) - 1;
        let element = document.getElementById('par-' + this.currentLrcIndex);
        if (element) {
            this.lyrics.nativeElement.style.transform = `translateY(-${element.offsetTop}px)`;
        }
    }

    //#endregion
}
