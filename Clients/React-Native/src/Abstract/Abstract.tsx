export enum DataSource {
    migu = 1,
    kuwo = 2,
    ne = 0
}
export interface MusicInfo {
    migu_CopyrightId: string,
    migu_Id: number,
    ne_Id: number,
    kw_Id: number,
    name: string,
    artists: Artist[],
    album: Album,
    duration: number,
    url: string,
    dataSource: DataSource
}
export interface Artist {
    id: number,
    name: string
}
export interface Album {
    id: number,
    name: string,
    picUrl: string
}
export interface Lyric {
    time: number,
    text: string
}
export interface TrackList {
    dataSource: number,
    id: number,
    name: string,
    picUrl: string,
    musicCount: number,
    musics: MusicInfo[]
}

export interface Result<T> {
    code: number,
    message: string,
    content: T
}