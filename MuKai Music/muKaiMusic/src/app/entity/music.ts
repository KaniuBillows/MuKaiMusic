import { DataSource } from './param/musicUrlParam';

export class Lyric {
    public time: number
    public text: string
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
        url: string;
        type: string;
        encodeType: string;
    }[];
}

export class UrlInfo {
    public url: string;
    public dataSource: DataSource;
    public netEaseId: number;
    public kuwoId: number;
    public miguId: string;
}

export class Song {
    public id: number;
    public name: string;
    public ne_Id: number;
    public kuWo_Id: number;
    public migu_Id: string;
    public duration: number;
    public ne_ArtistId: number;
    public artistName: string;
    public ne_AlbumId: string
    public albumName: string;
    public picUrl: string;
    public dataSource: DataSource
}

export class artist {
    public name: string;
    public id: number;
    public dataSource: DataSource;
    public migu_Id: string;
}
export class album {
    public name: string;
    public id: number;
    public picUrl: string
}