import { DataSource } from './param/musicUrlParam';
import { Song } from './music';


/**
 * 推荐歌单结果
 */
export class Playlist {
    public id: number;
    public dataSource: DataSource;
    public name: string;
    public picUrl: string;
    public musicCount: number;
    public musics: Song[];
}