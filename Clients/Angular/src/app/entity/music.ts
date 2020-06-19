import { DataSource } from './param/musicUrlParam';

export class Lyric {
    public time: number
    public text: string
}

export class UrlInfo {
    public url: string;
    public dataSource: DataSource;
}

export class Song {
    public name: string;
    public ne_Id: number;
    public kw_Id: number;
    public migu_CopyrightId: string;
    public migu_Id: string;
    public duration: number;
    public url: string;
    public artists: artist[];
    public album: album;
    public dataSource: DataSource
}

export class artist {
    public name: string;
    public id: number;
}
export class album {
    public name: string;
    public id: number;
    public picUrl: string
}