import { EventEmitter } from '@react-navigation/native';
import React, { useCallback, useEffect, useState } from 'react';
import { DeviceEventEmitter } from 'react-native';
import Sound from 'react-native-sound';
import { MusicInfo, DataSource } from './Abstract/Abstract';
import { MusicNetwork } from './service/Network';
import Lyrics from './util/Lyrics';
import Playlist from './util/Playlist';
import { shuffle } from './util/Util';
import NotificationModule, { subscribeMediaEvent, unSubscribeMediaEvent } from './Module/NativeNotificationModule';
//TODO: use Expo Audio to replace the React-native Sound
export const PlayControlContext = React.createContext<PlayControl>(null);

export interface PlayControl {
    playState: PlayState,
    start: (musicInfo?: MusicInfo, onEnd?: (success: boolean) => void) => void,
    pause: (callback?: () => void) => void,
    stop: (callback?: () => void) => void,
    resume: (callback?: () => void) => void,
    seek: (time: number) => void,
    nextTrack: () => void,
    lastTrack: () => void,
    switchNext: () => void,
    switchLast: () => void,
    initPlaylist: (musics: MusicInfo[]) => void,
    changePlayMode: (mode: PlayMode) => void,
    onFmPlayEnded: (callback: (state: PlayState) => void) => void,
    removeFmPlayEnded: (callback: (state: PlayState) => void) => void,
    addAndPlay: (musicInfo: MusicInfo) => void
}

export interface PlayState {
    isPlaying: boolean,
    isLoaded: boolean,
    isLoading: boolean,
    lyricReady: boolean,
    currentTime: number,
    totalTime: number,
    playMode: PlayMode,
    currentMusicInfo: MusicInfo,
    nextMusicInfo: MusicInfo,
    lastMusicInfo: MusicInfo,
    currentLyric: { index: number, text: string },
}

export enum PlayMode {
    /**
     * the player in this mode will only play the current music again and again.
     */
    singleCycle,

    /**
     * the player will be stoped when the last music in the playlist playback is finish.
     */
    listOrder,

    /**
     * it will create a new out-of-order playlist based on the original playlist.
     * and it will play the new playlist tracks circularly.
     */
    shuffle,

    /**
     * in this mode,the player will be stoped after the current music playback is finish.
     */
    onlyOnce,

    /**
     * in this mode, the player will play the playlist tracks circularly.
     */
    listCycle,

    /**
     * in this mode, user cannot switch music to pre. they can only use next.
     */
    fm,
}

const ON_FM_PLAY_ENDED = "ON_FM_PLAY_ENDED";

const ON_MUSIC_PLAY_ENDED = "ON_MUSIC_PLAY_ENDED";


let _setState;
let _control: PlayControl

/**
 * currentSound object. it will be instantiated when the start function is called.
 * and the origionol sound will be release when user instantiated a new sound.
 */
let currentSound: Sound;

/**
 * Temporary storage the playback end callback.
 * the react-native-sound dosen't provid a resume function.
 * when call sound.play(),this will cover the original callback.
 * so storage the onEnd callback for expected user behavior.
 */
let _onEnd: (success: boolean) => void;

/**
 * the playlist
 * //TODO: the palylist should be a function that returns the cur
 */
let playlist: Playlist = new Playlist([]);

/**
 * the playlist for shuffle mode.
 * TODO: implement the shuffle mode.
 */
let shufflePlaylist: Playlist = new Playlist([]);

/**
 * the player's playmode.
 */
let playMode: PlayMode;

/**
 * the lyrics object.
 */
let lyrics: Lyrics;
// TODO: should read the playlist from the storage.
export function AppPlayer(props) {
    const [control, setState] = useState<PlayControl>({
        playState: {
            isPlaying: false,
            isLoaded: false,
            isLoading: false,
            lyricReady: false,
            currentTime: 0,
            totalTime: 1,
            playMode: PlayMode.listCycle,
            currentMusicInfo: null,
            nextMusicInfo: null,
            lastMusicInfo: null,
            currentLyric: null,
        },
        start: start,
        stop: stop,
        pause: pause,
        resume: resume,
        seek: seek,
        nextTrack: nextTrack,
        lastTrack: lastTrack,
        switchLast: switchLast,
        switchNext: switchNext,
        initPlaylist: initPlaylist,
        changePlayMode: changePlayMode,
        onFmPlayEnded: onFmPlayEnded,
        removeFmPlayEnded: removeFmPlayEnded,
        addAndPlay: addAndPlay
    });
    _setState = setState;
    _control = control;
    useEffect(() => {
        const id = setInterval(() => {
            if (currentSound && currentSound.isLoaded && _control.playState.isPlaying) {
                currentSound.getCurrentTime((secondes, isPlaying) => {
                    const update = {} as PlayState;
                    if (_control.playState.lyricReady) {
                        const lyric = lyrics.getCurrent(secondes)
                        if (_control.playState.currentLyric?.text !== lyric.text) {
                            update.currentLyric = lyric;
                        }
                    }
                    updatePlayState({ ...update, currentTime: secondes });
                });
            }
        }, 1000);
        return () => {
            clearInterval(id);
        }
    }, []);
    const handleNotifyPlay = useCallback(() => {
        if (control.playState.isLoaded) {
            if (control.playState.isPlaying) {
                control.pause();
            } else control.resume();
        } else control.start();
    }, [control.playState.isLoaded, control.playState.isPlaying]);
    useEffect(() => {
        subscribeMediaEvent("onNextPressed", nextTrack);
        subscribeMediaEvent("onPausePressed", pause);
        subscribeMediaEvent("onPrePressed", lastTrack);
        subscribeMediaEvent("onPlayPressed", handleNotifyPlay);
        return () => {
            unSubscribeMediaEvent("onNextPressed", nextTrack);
            unSubscribeMediaEvent("onPausePressed", pause);
            unSubscribeMediaEvent("onPrePressed", lastTrack);
            unSubscribeMediaEvent("onPlayPressed", handleNotifyPlay);
        }
    }, [handleNotifyPlay]);

    return (<PlayControlContext.Provider value={control} >{props.children}</PlayControlContext.Provider>);
}
React.memo(AppPlayer);
/**
 * init the playlist.
 */
function initPlaylist(musicInfos: MusicInfo[]) {
    playlist = new Playlist(musicInfos);
    shufflePlaylist = new Playlist(shuffle<MusicInfo>([...musicInfos]));
    updatePlayState({ playlist: playlist });
    let list = getplaylist();
    setCurrentMusic(list.currentMusic());
}

/**
 * internal call this function to update the state.
 * @param state 
 */
function updatePlayState(state) {
    _setState((old: PlayControl) => (
        { ...old, playState: { ...old.playState, ...state } }
    ))
}

/**
 * this function will only be called internal
 * this function will try to load sound and change the current music info.
 * it will also try to get the lyrics.
 */
async function setCurrentMusic(musicinfo: MusicInfo, onFinish?: (success: boolean) => void) {
    updatePlayState({ isLoading: true, isLoaded: false, currentTime: 0, lyricReady: false });
    //free the current sound resource
    if (currentSound) {
        currentSound.release();
    }
    //the old lyrics.
    lyrics = null;
    if (musicinfo === null) {
        updatePlayState({ isLoading: false });
        return;
    }
    updatePlayState({ currentMusicInfo: musicinfo, nextMusicInfo: playlist.getNext(), lastMusicInfo: playlist.getLast() });
    //some music's url may be null.so try to get it.
    if (musicinfo.url === null) {
        let res = await MusicNetwork.getMusicUrl(musicinfo);
        if (res.code === 200) musicinfo.url = res.content;
    }
    currentSound = new Sound(musicinfo.url, null, (error) => {
        if (error) {
            if (currentSound) {
                currentSound.release();
            }
            if (onFinish) onFinish(false);
            return;
        }
        updatePlayState({ totalTime: currentSound.getDuration(), isLoaded: true, isLoading: false });
        if (onFinish) onFinish(true);
    });
    //get lyrics.
    let lyricRes = await MusicNetwork.getLyric(musicinfo);
    if (lyricRes.code === 200) {
        lyrics = new Lyrics(lyricRes.content);
        updatePlayState({ currentLyric: lyrics.getCurrent(1), lyricReady: true });
    }
    NotificationModule.sendMediaNotification({
        name: musicinfo.name,
        isPlaying: false, artist: musicinfo.artists[0].name,
        picUrl: musicinfo.album.picUrl
    });
}

/**
* start to play a new Sound with the given url.
* @param musicInfo the music url.
* @param onEnd the callback function that will be called at when the playback finished successfully.
*/
function start(musicInfo?: MusicInfo, onEnd?: (success: boolean) => void) {
    if (!musicInfo) {
        musicInfo = getplaylist().currentMusic();
    }
    setCurrentMusic(musicInfo, (success) => {
        if (success) {
            _onEnd = onEnd;
            play();
        } else
            //TODO: some try to replay. 
            return;
    });
};

/** 
 * stop the sound, and this function will seek the postion to 0.
 * this function only effective when isPlaying is true.
 * @param callback this callback will be called when the sound has been stoped.
 */
function stop(callback?: () => void) {
    if (_control.playState.isPlaying)
        currentSound.stop(() => {
            updatePlayState({ isPlaying: false });
            NotificationModule.sendMediaNotification({
                name: _control.playState.currentMusicInfo.name,
                isPlaying: false, artist: _control.playState.currentMusicInfo.artists[0].name,
                picUrl: _control.playState.currentMusicInfo.album.picUrl
            });
            if (callback) callback();
        });
}

/**
 * resume play the sound.
 * @param callback 
 */
function resume(callback?: () => void) {
    play();
    if (callback) callback();
}

/**
 * play the sound.
 * the function will only be called internal.
 * the function will update the isPlay state to true. 
 */
function play() {
    if (currentSound) {
        currentSound.play((success) => {
            defaultOnEnd();
            if (_onEnd) _onEnd(success);
        });
        updatePlayState({ isPlaying: true });
        NotificationModule.sendMediaNotification({
            name: _control.playState.currentMusicInfo.name,
            isPlaying: true, artist: _control.playState.currentMusicInfo.artists[0].name,
            picUrl: _control.playState.currentMusicInfo.album.picUrl
        });
    }
}

/**
 * pause the sound.
 * this function only effective when isPlaying is true.
 * @param callback this callback will be called when the sound has been paused.
 */
function pause(callback?: () => void) {
    if (_control.playState.isPlaying)
        currentSound.pause(() => {
            updatePlayState({ isPlaying: false });
            NotificationModule.sendMediaNotification({
                name: _control.playState.currentMusicInfo.name,
                isPlaying: false, artist: _control.playState.currentMusicInfo.artists[0].name,
                picUrl: _control.playState.currentMusicInfo.album.picUrl
            });
            if (callback) callback();
        });
}

/**
 * call this function to switch to next track.
 * note: in shuffle mode,this function should change track from the suffle playlist. 
 */
function nextTrack() {
    let list = getplaylist();
    if (_control.playState.playMode === PlayMode.fm && list.isLast()) {
        DeviceEventEmitter.emit(ON_FM_PLAY_ENDED, _control.playState);
        updatePlayState({ isPlaying: _control.playState.isPlaying });
        return;
    }
    start(list.next());
}

/**
 * call this function to switch to next track.
 * note: in shuffle mode,this function should change track from the suffle playlist.
 */
function lastTrack() {
    if (playMode === PlayMode.fm) {
        start(_control.playState.currentMusicInfo);
        return;
    }
    start(getplaylist().last());
}

/**
 * change the current music to next track.
 */
function switchNext(play?: boolean) {
    if (_control.playState.isPlaying)
        nextTrack();
    else {
        let list = getplaylist();
        if (_control.playState.playMode === PlayMode.fm && list.isLast()) {
            DeviceEventEmitter.emit(ON_FM_PLAY_ENDED, _control.playState);
            return;
        }
        setCurrentMusic(getplaylist().next());
    }
}

/**
 * change the current music to next track.
 */
function switchLast(play?: boolean) {
    if (_control.playState.isPlaying)
        lastTrack();
    else {
        if (playMode === PlayMode.fm) {
            setCurrentMusic(_control.playState.currentMusicInfo);
            return;
        }
        setCurrentMusic(getplaylist().last());
    }
}

/**
 * seek the time to the target time
 * @param time the value must grater than 0 and less than the duration
 */
function seek(time: number) {
    if (currentSound && time > 0 && time <= currentSound.getDuration())
        currentSound.setCurrentTime(time);
}

/**
 * call this function to change the Player's mode.
 * this function will update the state
 * @param mode 
 */
function changePlayMode(mode: PlayMode) {
    playMode = mode;
    updatePlayState({ playMode: playMode });
}

/**
 * this function will be called when the sound playback finished.
 * it will update the playState and execute logics with playMode.
 */
function defaultOnEnd() {
    if (playMode === PlayMode.singleCycle) {
        start(_control.playState.currentMusicInfo);
    }
    else if (playMode === PlayMode.onlyOnce) {
        stop();
    }
    else if (playMode === PlayMode.listOrder) {
        let list = getplaylist();
        if (list.isLast()) {
            stop();
        } else {
            start(list.next());
        }
    } else if (playMode === PlayMode.fm) {

        let list = getplaylist();
        if (list.isLast())
            DeviceEventEmitter.emit(ON_FM_PLAY_ENDED, _control.playState);
        else start(list.next())
    }
    else {
        start(getplaylist().next());
    }
}

/**
 * get the current playlist.
 */
function getplaylist() {
    if (playMode === PlayMode.shuffle) return shufflePlaylist;
    else return playlist;
}

function addAndPlay(musicInfo: MusicInfo) {
    let index = getplaylist().addToNext(musicInfo);
    console.log(index);
    start(getplaylist().setMusic(index));
}

/**
 * a compoent should only call the function once.
 * this event will emit while player finish the play list in FM mode.
 * we just need emit the event. other component that subscribe the event
 * should call the init playlist function. and start new playback.
 * @param callback 
 */
function onFmPlayEnded(callback: (e: PlayState) => void) {
    DeviceEventEmitter.addListener(ON_FM_PLAY_ENDED, callback);
}

function removeFmPlayEnded(callback: (e: PlayState) => void) {
    DeviceEventEmitter.removeListener(ON_FM_PLAY_ENDED, callback);
}

