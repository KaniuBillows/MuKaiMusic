import { Lyric } from "../Abstract/Abstract";

export default class Lyrics {
    constructor(lyrics: Lyric[]) {
        if (!lyrics) {           
            throw new Error("argument cannot be null");
        }
        this._lyrics = lyrics;
    }
    private _lyrics: Lyric[];

    /**
     * get current lyrics based on time
     * @param time 
     */
    public getCurrent(time: number): { index: number, text: string } {
        let index = this.findLastIndexOf(this._lyrics, item => item.time != null && item.time <= time);
        let s = {
            index: index === -1 ? 0 : index,
            text: this._lyrics[index === -1 ? 0 : index]?.text ?? ''
        };
        return s;
    }

    public getLyricList() {
        return this._lyrics;
    }

    private findLastIndexOf<T>(array: Array<T>, func: (T: T) => boolean): number {
        for (let i = array.length - 1; i >= 0; i--) {
            if (func(array[i]))
                return i;
        }
        return -1;
    }
}