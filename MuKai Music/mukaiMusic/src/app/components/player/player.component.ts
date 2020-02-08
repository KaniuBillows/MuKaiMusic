import { Component, OnInit, ViewChild, ElementRef, EventEmitter } from '@angular/core';
import { PlayerService } from 'src/app/services/player/player.service';
import { MatSlider, MatSliderChange } from '@angular/material/slider';
import { MusicService } from 'src/app/services/network/music/music.service';
import { musicDetailResult, album, artist, Song, UrlInfo } from 'src/app/entity/music';
import { ThemeService } from 'src/app/services/theme/theme.service';
import { AccountService } from 'src/app/services/network/account/account.service';
import { Result } from 'src/app/entity/baseResult';
import { MusicUrlParam, DataSource } from 'src/app/entity/param/musicUrlParam';


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

    // private _currentMusicInfo: {
    //     name: string,
    //     id: number,
    //     albumName: string,
    //     albumId: number,
    //     picUrl: string,
    //     artistName: string;
    //     artistId: number
    // } = { picUrl: '' } as any;

    private _currentMusicInfo: Song = { picUrl: "" } as any;

    private _currentLrcIndex: number = 0;

    private _playlist: Song[] = [];

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

    public set currentMusicInfo(value: Song) {
        this._currentMusicInfo = value;
        document.getElementById("back-board").style.backgroundImage = "Url(" + this._currentMusicInfo.picUrl + ")";
    }

    public get currentLrcIndex() {
        return this._currentLrcIndex;
    }

    public set currentLrcIndex(value: number) {
        this._currentLrcIndex = value;
    }

    public get currentMusicIndex() {
        return this._playlist.findIndex(item =>
            item.ne_Id == this.currentMusicInfo.ne_Id
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
            this.startPlay(this.currentMusicIndex);
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
            this.getLyric(this._playlist[0].ne_Id, () =>
                this.startPlay(0)
            );
        } else {
            this.getLyric(this._playlist[this.currentMusicIndex + 1].ne_Id, () =>
                this.startPlay(this.currentMusicIndex + 1)
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
            this.getLyric(this._playlist[this._playlist.length - 1].ne_Id, () =>
                this.startPlay(this._playlist.length - 1));
        } else {
            this.getLyric(this._playlist[this.currentMusicIndex - 1].ne_Id, () =>
                this.startPlay(this.currentMusicIndex - 1));
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
        this.currentMusicInfo = this.playlist[index];
        this.getLyric(this.playlist[index].ne_Id, () => this.startPlay(index))
        //this.getMusicDetail(this._playlist[index].id, () => this.startPlay());
    }

    /**
     * 点击下载歌曲
     * @param index 
     */
    public clickDownload(index: number) {
        let item = this.playlist[index];
        this.musicNet.getNeteaseUrl({
            neteaseId: item.ne_Id,
            dataSource: DataSource.NetEase
        } as MusicUrlParam).subscribe((res: Result<UrlInfo>) => {
            if (res.content.url) {
                this.musicNet.downloadFile(res.content.url, item.name + " - " + item.artistName);
            }
        })
    }

    public async test() {
        await this.musicNet.searchMusic("");
    }

    //#endregion



    //#region private method

    /**
     * 获取当前歌曲的播放链接并开始播放
     */
    private startPlay(index: number) {
        this.player.status = 'loading';
        this.currentMusicInfo = this.playlist[index];

        this.musicNet.getNeteaseUrl({
            neteaseId: this.currentMusicInfo.ne_Id,
            dataSource: DataSource.NetEase
        } as MusicUrlParam).subscribe((res: Result<UrlInfo>) => {
            if (res.content.url) {
                this.player.onCurrentTimeChange.subscribe((time: number) =>
                    this.onTimeChange(time)
                )
                this.player.start(res.content.url);
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
    private getLyric(id: number, callback?: () => void) {
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
        }, null, callback);
    }
    /**
    * 获取推荐歌曲
    */
    private getPersonalized() {
        this.musicNet.getPersonalizedMusics().subscribe((res: Result<Song[]>) => {
            if (res.content.length > 0) this.currentMusicInfo = res.content[0];
            this._playlist = this._playlist.concat(res.content)
            this.getLyric(this.playlist[0].ne_Id);
        });
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
