import { DataSource, MusicInfo, Lyric, Result, TrackList } from "../Abstract/Abstract";
import Playlist from "../util/Playlist";
import { Observable } from 'rxjs';


const baseUrl: string = 'https://api.kaniu.pro/';
class MusicNetwork {
    // public async searchMusic(keyword: string, datasource: DataSource) {
    //     let response = await fetch(`${baseUrl}${DataSource[datasource]}/search?keyword=${keyword}`);
    //     return await response.json() as MusicInfo[];
    // }

    public async getLyric(musicInfo: MusicInfo) {
        let response: Response;
        if (musicInfo.dataSource === DataSource.migu) {
            /**Migu require the copyright Id for lyric. */
            response = await fetch(`${baseUrl}${DataSource[musicInfo.dataSource]}/lyric?id=${musicInfo.migu_CopyrightId}`);
        } else if (musicInfo.dataSource === DataSource.ne) {
            response = await fetch(`${baseUrl}${DataSource[musicInfo.dataSource]}/lyric?id=${musicInfo.ne_Id}`)
        } else {
            response = await fetch(`${baseUrl}${DataSource[musicInfo.dataSource]}/lyric?id=${musicInfo.kw_Id}`)
        }
        return await response.json() as Result<Lyric[]>;
    }

    public async getPersonalizedPlaylist(limit: number = 6) {
        //TODO:all platforms' playlist.
        let response = await fetch(`${baseUrl}${DataSource[DataSource.ne]}/personalized?limit=${limit}`);
        return await response.json() as Result<TrackList[]>;
    }

    public async getSearchHotKey() {
        let response = await fetch(`${baseUrl}${DataSource[DataSource.kuwo]}/search_hotkey`);
        return await response.json() as Result<string[]>;
    }

    public async getDaily30Musics() {
        let response = await fetch(`${baseUrl}${DataSource[DataSource.kuwo]}/playlist/daily30`);
        return await response.json() as Result<TrackList>;
    }

    public async getPersonalFm() {
        let response = await fetch(`${baseUrl}${DataSource[DataSource.ne]}/personal_fm`);
        return await response.json() as Result<MusicInfo[]>;
    }
    public async getMusicUrl(musicInfo: MusicInfo) {
        let id = this.getId(musicInfo);
        let response = await fetch(`${baseUrl}${DataSource[musicInfo.dataSource]}/url?id=${id}`);
        return await response.json() as Result<string>;
    }

    public searchMusic(keyword: string): Observable<MusicInfo[]> {
        const responses = [
            fetch(`${baseUrl}ne/search?keyword=${keyword}`),
            fetch(`${baseUrl}kuwo/search?keyword=${keyword}`),
            fetch(`${baseUrl}migu/search?keyword=${keyword}`)
        ];
        let resolvedCount = 0;
        return new Observable(observer => {
            responses.forEach(res => {
                res.then(async data => {
                    let result = await data.json() as MusicInfo[];
                    observer.next(result);
                }).catch(() => { }).finally(() => {
                    resolvedCount++;
                    if (resolvedCount === responses.length) {
                        observer.complete();
                        resolvedCount = 0;
                    }
                })
            })
        })
    }

    private getId(musicInfo: MusicInfo): number {
        if (musicInfo.dataSource === DataSource.kuwo) return musicInfo.kw_Id;
        else if (musicInfo.dataSource === DataSource.ne) return musicInfo.ne_Id;
        else if (musicInfo.dataSource === DataSource.migu) return musicInfo.migu_Id;
    }
}
const instance = new MusicNetwork();
export { instance as MusicNetwork };