import React, { useEffect, useState } from 'react';
import Sound from 'react-native-sound';
import { MusicInfo, DataSource } from './Abstract/Abstract';
import { MusicNetwork } from './service/Network';
import Lyrics from './util/Lyrics';
import Playlist from './util/Playlist';

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
    switchLast: () => void
}

interface PlayState {
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
}

let _setState
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
let playlist: Playlist;

/**
 * the playlist for shuffle mode.
 * TODO: implement the shuffle mode.
 */
let shufflePlaylist: Playlist;

/**
 * the lyrics object.
 */
let lyrics: Lyrics;

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
        switchNext: switchNext
    });
    _setState = setState;
    _control = control;
    useEffect(() => {
        /**
         * TODO: this is just some test code here for the backend API
         */
        MusicNetwork.searchMusic('陈奕迅', DataSource.migu).then(musics => {
            playlist = new Playlist(musics, 0);
            setCurrentMusic(playlist.currentMusic());
        });

        setInterval(() => {
            if (currentSound && currentSound.isLoaded && _control.playState.isPlaying) {
                currentSound.getCurrentTime((secondes, isPlaying) => {
                    updatePlayState({ currentTime: secondes });
                });
                if (_control.playState.lyricReady) {
                    updatePlayState({ currentLyric: lyrics.getCurrent(_control.playState.currentTime) });
                }
            }
        }, 100)
    }, []);
    return (<PlayControlContext.Provider value={control} >{props.children}</PlayControlContext.Provider>);
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
function setCurrentMusic(musicinfo: MusicInfo, onFinish?: (success: boolean) => void) {
    updatePlayState({ isLoading: true, isLoaded: false, currentTime: 0, lyricReady: false });
    //free the current sound resource
    if (currentSound) {
        currentSound.release();
    }
    //the old lyrics.
    lyrics = null;
    updatePlayState({ currentMusicInfo: musicinfo, nextMusicInfo: playlist.getNext(), lastMusicInfo: playlist.getLast() });
    //TODO: try to get the music url.
    currentSound = new Sound(musicinfo.url, null, (error) => {
        updatePlayState({ isLoading: false });
        if (error) {
            //TODO: this is a DEBUG console.error
            console.error('failed to load the sound ', error);
            if (currentSound) {
                currentSound.release();
            }
            if (onFinish) onFinish(false);
            return;
        }
        updatePlayState({ totalTime: currentSound.getDuration(), isLoaded: true });
        if (onFinish) onFinish(true);
    });
    
    //get lyrics.
    MusicNetwork.getLyric(musicinfo).then(res => {
        if (res.code === 200) {
            lyrics = new Lyrics(res.content);
            updatePlayState({ currentLyric: lyrics.getCurrent(1) });
            updatePlayState({ lyricReady: true });
        }
    }).catch(err => {
        console.error(err);
    });
}

/**
* start to play a new Sound with the given url.
* @param musicInfo the music url.
* @param onEnd the callback function that will be called at when the playback finished successfully.
*/
function start(musicInfo?: MusicInfo, onEnd?: (success: boolean) => void) {
    if (!musicInfo) {
        musicInfo = playlist.currentMusic();
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
    if (currentSound?.isPlaying())
        currentSound.stop(() => {
            updatePlayState({ isPlaying: false })
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
    }
}

/**
 * pause the sound.
 * this function only effective when isPlaying is true.
 * @param callback this callback will be called when the sound has been paused.
 */
function pause(callback?: () => void) {
    if (currentSound?.isPlaying())
        currentSound.pause(() => {
            updatePlayState({ isPlaying: false });
            if (callback) callback();
        });
}

/**
 * call this function to switch to next track.
 * note: in shuffle mode,this function should change track from the suffle playlist.
 * TODO: shuffle mode next&last 
 */
function nextTrack() {
    start(playlist.next());
}

/**
 * call this function to switch to next track.
 * note: in shuffle mode,this function should change track from the suffle playlist.
 */
function lastTrack() {
    start(playlist.last());
}

/**
 * change the current music to next track.
 */
function switchNext() {
    if (currentSound.isPlaying())
        nextTrack();
    else
        setCurrentMusic(playlist.next());
}

/**
 * change the current music to next track.
 */
function switchLast() {
    if (currentSound.isPlaying())
        lastTrack();
    else
        setCurrentMusic(playlist.last());
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
 * @param playMode 
 */
function changePlayMode(playMode: PlayMode) {
    //TODO:change the playmode.
}

/**
 * this function will be called when the sound playback finished.
 * it will update the playState and execute logics with playMode.
 */
function defaultOnEnd() {
    //TODO:diffrent playMode.Like loop, single,list,random,once
    updatePlayState(updatePlayState({ isPlaying: false }));
}