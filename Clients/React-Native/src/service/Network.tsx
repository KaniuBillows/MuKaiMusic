import { DataSource, MusicInfo, Lyric, Result, TrackList } from "../Abstract/Abstract";


const baseUrl: string = 'https://api.kaniu.pro/';
class MusicNetwork {
    public async searchMusic(keyword: string, datasource: DataSource) {
        let response = await fetch(`${baseUrl}${DataSource[datasource]}/search?keyword=${keyword}`);
        return await response.json() as MusicInfo[];
    }

    public async getLyric(musicInfo: MusicInfo) {
        let response: Response;
        if (musicInfo.dataSource === DataSource.migu) {
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
}
const instance = new MusicNetwork();
export { instance as MusicNetwork };