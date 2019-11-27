export class lyricInfo {
    lrc: {
        version: number,
        lyric: string,
    }
}
/**
 * 歌曲详情信息
 */
export class musicDetailResult {
    songs: {
        name: string,
        id: number,
        ar: artist[],
        publishTime: number,
        al: album
    }[];
    code: number;
}
export class NetEaseUrlResult {
    data: {
        id: number;
        url: string
    }[];
}
export class personalizedResult {
    code: number;
    category: number;
    result: {
        id: number,
        type: number,
        name: string,
        song: song
    }[]
}
export class song {
    name: string;
    id: number;
    artists: artist[];
    album: album;
    duration: number;
}
export class artist {
    name: string;
    id: number;
}
export class album {
    name: string;
    id: number;
    picUrl: string
}