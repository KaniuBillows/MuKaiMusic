import { Component, OnInit, ViewChild, ElementRef, EventEmitter } from '@angular/core';
import { PlayerService } from 'src/app/services/player/player.service';
import { MatSlider, MatSliderChange } from '@angular/material/slider';
import { MusicService } from 'src/app/services/network/music/music.service';
import { musicDetailResult, album, artist, Song, UrlInfo, Lyric } from 'src/app/entity/music';
import { ThemeService } from 'src/app/services/theme/theme.service';
import { AccountService } from 'src/app/services/network/account/account.service';
import { Result } from 'src/app/entity/baseResult';
import { MusicParam, DataSource } from 'src/app/entity/param/musicUrlParam';


@Component({
    selector: 'app-player',
    templateUrl: './player.component.html',
    styleUrls: ['./player.component.scss']
})
export class PlayerComponent implements OnInit {


    @ViewChild('lyrics', { static: true })
    lyrics: ElementRef;


    private _lyric_paras: Lyric[] = [];

    private _currentMusicInfo: Song = {} as any;

    private _currentLrcIndex: number = 0;

    private _playlist: Song[] = [];

    constructor(
        public player: PlayerService,
        public theme: ThemeService,
        private musicNet: MusicService) {
    }
    ngOnInit() {
        this.player.onEnded.subscribe(() => {
            this.onNextTrackButtonClick();
        })
        this.getPersonalized();
    }

    /**
     * 播放列表
     */
    public get playlist() {
        return this._playlist;
    }

    /**
     * 当前播放歌曲,当前播放歌曲改变时，会调用获取歌词方法，加载歌词
     * 并且会更改背景图片
     */
    public get currentMusicInfo() {
        return this._currentMusicInfo;
    }
    public set currentMusicInfo(value: Song) {
        this._currentMusicInfo = value;
        this.getLyric(value);
    }

    /**
     * 当前歌词索引
     */
    public get currentLrcIndex() {
        return this._currentLrcIndex;
    }
    public set currentLrcIndex(value: number) {
        this._currentLrcIndex = value;
    }

    /**
     * 当前歌曲索引
     */
    public get currentMusicIndex() {
        this._currentMusicIndex = this._playlist.findIndex(item =>
            item.ne_Id == this.currentMusicInfo.ne_Id
        );
        return this._currentMusicIndex;
    }
    private _currentMusicIndex: number = this.currentMusicIndex;

    /**
     * 获取当前歌曲是否为最后一首
     */
    public get isLastMusic() {
        return this.currentMusicIndex == this._playlist.length - 1;
    }

    /**
     * 当前歌曲是否为第一首
     */
    public get isFirstMusic() {
        return this.currentMusicIndex == 0;
    }

    /**
     * 获取当前主题class
     */
    public get themeClass() {
        return this.theme.getThemeClass();
    }

    /**
     * 歌词段落
     */
    public get lyric_paras() {
        return this._lyric_paras;
    }

    /**
     * 订阅control组件的下一首事件
     * 当位于最后时，跳转到第一首播放
     */
    public onNextTrackButtonClick() {
        if (this._playlist.length < 1) {
            return;
        }
        if (this.isLastMusic) {
            this.getLyric(this._playlist[0], () =>
                this.startPlay(this._playlist[0])
            );
        } else {
            this.getLyric(this._playlist[this.currentMusicIndex + 1], () =>
                this.startPlay(this._playlist[this.currentMusicIndex + 1])
            );
        }
    }

    /**
     * 订阅control组件的下一首事件
     * 当位于首位时，跳转到最后一首播放
     */
    public onLastTrackButtonClick() {
        if (this._playlist.length < 1) {
            return;
        }
        if (this.isFirstMusic) {
            this.getLyric(this._playlist[this._playlist.length - 1], () =>
                this.startPlay(this._playlist[this._playlist.length - 1]));
        } else {
            this.getLyric(this._playlist[this.currentMusicIndex - 1], () =>
                this.startPlay(this._playlist[this.currentMusicIndex - 1]));
        }
    }


    //#region Actions

    /**
     * 订阅playlist组件点击播放歌曲事件
     * 设置当前播放歌曲到对应歌曲
     * 并开始获取歌词，获取歌词完成后，开始播放
     * @param index 
     */
    public clickPlay(index: number) {
        this.currentMusicInfo = this.playlist[index];
        this.getLyric(this.playlist[index], () => this.startPlay(this.currentMusicInfo))
    }

    /**
     * 订阅playlist点击下载歌曲事件
     * 首先获取播放连接，然后读取歌曲名称和歌手名称，生成下载
     * @param index 
     */
    public clickDownload(index: number) {
        let item = this.playlist[index];
        this.musicNet.getNeteaseUrl({
            neteaseId: item.ne_Id,
            dataSource: DataSource.NetEase
        } as MusicParam).subscribe((res: Result<UrlInfo>) => {
            if (res.content.url) {
                this.musicNet.downloadFile(res.content.url, item.name + " - " + item.artistName);
            }
        })
    }

    /**
     * 订阅playlist当前歌曲被删除事件
     * 停止当前歌曲的播放，并将播放器状态设置为’stop‘
     * 如果此时播放列表已经为空，则将当前歌曲设置为空对象
     * 否则将下一首歌设置为当前歌曲,此时播放状态仍为'stop'
     */
    public onCurrentMusicDelete() {
        //此时playlist的length已经-1
        this.player.stop();
        if (this.playlist.length == 0) {
            this.currentMusicInfo = {} as any;
        } else {
            if (this._currentMusicIndex == this.playlist.length)
                this.currentMusicInfo = this.playlist[0];
            else this.currentMusicInfo = this.playlist[this._currentMusicIndex];
        }
    }

    /**
     * 订阅music-info组件产生的音乐图片改变事件
     * @param pic 
     */
    public onPictureChange(pic: string) {
        document.getElementById("back-board").style.backgroundImage = "Url(" + pic + ")";

    }

    public async test() {
        await this.musicNet.searchMusic("");
    }

    //#endregion



    //#region private method

    /**
     * 获取歌曲的播放链接并开始播放
     */
    public startPlay(song: Song) {
        this.player.status = 'loading';
        this.currentMusicInfo = song;
        this.musicNet.getNeteaseUrl({
            neteaseId: song.ne_Id,
            dataSource: DataSource.NetEase
        } as MusicParam).subscribe((res: Result<UrlInfo>) => {
            if (res.content.url) {
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
     * 获取歌词，并在歌词获取完毕之后，对callback进行调用
     * 一般用于获取完毕歌词进行播放
     * @param song
     */
    private getLyric(song: Song, callback?: () => void) {
        this._lyric_paras = [];
        this.musicNet.getLyric(song).subscribe((res: Result<Lyric[]>) => {
            this._lyric_paras = res.content
        }, null, callback);
    }

    /**
     * 在默认情况下
     * 获取推荐歌曲，并将获取到的第一首歌作为当前歌曲
     */
    private getPersonalized() {
        this.musicNet.getPersonalizedMusics().subscribe((res: Result<Song[]>) => {
            if (res.content.length > 0) this.currentMusicInfo = res.content[0];
            this._playlist = this._playlist.concat(res.content);
        });
    }

    //#endregion
}
