import { Component, OnInit, ViewChild, ElementRef, EventEmitter } from '@angular/core';
import { PlayerService, Playlist, CurrentMusicIndex } from 'src/app/services/player/player.service';
import { MusicService } from 'src/app/services/network/music/music.service';
import { album, artist, Song, UrlInfo, Lyric } from 'src/app/entity/music';
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


    private _currentLrcIndex: number = 0;


    constructor(
        public player: PlayerService,
        public theme: ThemeService,
        private musicNet: MusicService) {
    }
    ngOnInit() {
        this.player.currentMusicChange.subscribe(() => {
            document.getElementById("back-board").style.backgroundImage = "Url(" + this.player.currentMusic.album.picUrl + ")";
        });
        this.getDefault();
    }


    /**
     * 当前播放歌曲,当前播放歌曲改变时，会调用获取歌词方法，加载歌词
     * 并且会更改背景图片
     */
    public get currentMusicInfo() {
        return this.player.currentMusic;
    }
    public set currentMusicInfo(value: Song) {
        this.player.currentMusic = value;
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
     * 获取当前主题class
     */
    public get themeClass() {
        return this.theme.getThemeClass();
    }



    //#region Actions

    /**
     * 订阅music-info组件产生的音乐图片改变事件
     * @param pic 
     */
    public onPictureChange(pic: string) {

    }

    //#endregion



    //#region private method




    /**
     * 在默认情况下
     * 获取推荐歌曲，并将获取到的第一首歌作为当前歌曲
     */
    private getDefault() {

        let playlist = localStorage.getItem(Playlist);
        if (playlist == null) {
            this.musicNet.searchMusic("陈奕迅").subscribe((res: Result<Song[]>) => {
                if (res.content.length > 0) this.currentMusicInfo = res.content[0];
                this.player.initPlaylist(res.content);
            });
        } else {
            let currentMusic = localStorage.getItem(CurrentMusicIndex);
            if (currentMusic != null) {
                this.player.initPlaylist(JSON.parse(playlist),Number.parseInt(currentMusic));
            } else {
                this.player.initPlaylist(JSON.parse(playlist));
            }
        }
    }

    //#endregion
}
