import { Component, OnInit, ViewChild, ElementRef } from '@angular/core';
import { PlayerService, Playlist, CurrentMusicIndex } from 'src/app/services/player/player.service';
import { MusicService } from 'src/app/services/network/music/music.service';
import { Song } from 'src/app/entity/music';
import { ThemeService } from 'src/app/services/theme/theme.service';
import { Result } from 'src/app/entity/baseResult';


@Component({
    selector: 'app-player',
    templateUrl: './player.component.html',
    styleUrls: ['./player.component.scss']
})
export class PlayerComponent implements OnInit {


    @ViewChild('lyrics', { static: true })
    lyrics: ElementRef;

    constructor(
        public player: PlayerService,
        public theme: ThemeService,
        private musicNet: MusicService) {
    }
    ngOnInit() {
        document.getElementById("back-board").style.backgroundImage = "Url(../../../assets/img/music_white.jpg)";
        this.player.currentMusicChange.subscribe(() => {
            document.getElementById("back-board").style.backgroundImage = "Url(" + this.player.currentMusic.album.picUrl + ")";
        });
        this.getDefault();
    }


    /**
     * 当前播放歌曲,当前播放歌曲改变时
     */
    public get currentMusicInfo() {
        return this.player.currentMusic;
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
    public onPicError() {
        document.getElementById("back-board").style.backgroundImage = "Url(../../../assets/img/music_white.jpg)";
    }




    /**
     * 在默认情况下
     * 获取推荐歌曲，并将获取到的第一首歌作为当前歌曲
     */
    private getDefault() {
        let playlist = localStorage.getItem(Playlist);
        if (playlist == null) {
            this.musicNet.searchMusic("陈奕迅").subscribe((res: Result<Song[]>) => {
                this.player.initPlaylist(res.content);
            });
        } else {
            let currentMusic = localStorage.getItem(CurrentMusicIndex);
            if (currentMusic != null) {
                this.player.initPlaylist(JSON.parse(playlist), Number.parseInt(currentMusic));
            } else {
                this.player.initPlaylist(JSON.parse(playlist));
            }
        }
    }

    //#endregion
}
