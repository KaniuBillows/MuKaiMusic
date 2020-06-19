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


    constructor(
        public player: PlayerService,
        public theme: ThemeService,
        private musicNet: MusicService) {
    }
    ngOnInit() {
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

    public get picUrl() {
        return this.currentMusicInfo?.album?.picUrl != null ? this.currentMusicInfo.album.picUrl : '../../../assets/img/music_white.jpg';
    }



    /**
     * 在默认情况下
     * 获取推荐歌曲，并将获取到的第一首歌作为当前歌曲
     */
    private getDefault() {
        let playlist = localStorage.getItem(Playlist);
        if (playlist == null) {
            this.musicNet.searchMusic("陈奕迅").then(songs =>
                this.player.initPlaylist(songs)
            );
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
