import { MusicInfo } from "../Abstract/Abstract";


export default class Playlist {
    constructor(tracks: MusicInfo[], index?: number) {
        if (!tracks) {
            //TODO:DEBUG console.error
            console.error("tracks is null or undefined");
            throw new Error('argument cannot be null!');
        }
        this._playlist = tracks;
        if (tracks.length == 0) {
            index = -1;
        } else
            this.currentIndex = index ?? 0;
    }

    private _playlist: MusicInfo[];

    private currentIndex: number

    /**
     * return the current music info object.
     * the return value will be null if the playlist is empty.
     */
    public currentMusic(): MusicInfo {
        if (this.isEmpty())
            return null;
        else return this._playlist[this.currentIndex];
    }

    /**
     * set the current index move to next and return the new current music info.
     */
    public next() {
        if (this.isEmpty()) return null;
        if (this.currentIndex != this._playlist.length - 1) this.currentIndex++;
        else this.currentIndex = 0;
        return this._playlist[this.currentIndex];
    }

    public getNext() {
        let index = this.currentIndex;
        if (this.isEmpty()) return null;
        if (index != this._playlist.length - 1) index++;
        else index = 0;
        return this._playlist[index];
    }

    /**
     * set the current index move to last and return the new current music info.
     */
    public last() {
        if (this.isEmpty()) return null;
        if (this.currentIndex != 0) this.currentIndex--;
        else this.currentIndex = this._playlist.length - 1;
        return this._playlist[this.currentIndex];
    }

    public getLast() {
        let index = this.currentIndex;
        if (this.isEmpty()) return null;
        if (index != 0) index--;
        else index = this._playlist.length - 1;
        return this._playlist[index];
    }

    /**
     * return the playlist.
     * note: Do not directly manipulate the array through this function!!!
     * It should only apply to the view!!!
     */
    public playlist() {
        return this._playlist;
    }

    /**
     * remove the track from the playlist
     * @param index 
     */
    public removeTrack(index: number) {
        if (index >= 0 && index < this._playlist.length) {
            this._playlist.splice(index);
            this.currentIndex--;
        }
    }

    /**
     * Add elements to the end of the list
     * @param musicinfo 
     */
    public addTrack(musicinfo: MusicInfo) {
        this._playlist.push(musicinfo);
    }

    /**
     * Add elements to the next postion.
     * @param musicInfo 
     */
    public addToNext(musicInfo: MusicInfo) {
        this._playlist.splice(this.currentIndex + 1, 0, musicInfo);
    }

    /**
     * return is the playlist empty.
     */
    public isEmpty() {
        return this.count() <= 0;
    }

    /**
     * return the playlist's count.
     */
    public count() {
        return this._playlist.length;
    }
}   