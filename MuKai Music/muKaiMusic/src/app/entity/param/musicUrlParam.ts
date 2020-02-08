export class MusicUrlParam {
    public dataSource: DataSource;
    public miguId: string;
    public neteaseId: number;
    public kuwoId: number;
}
export enum DataSource {
    NetEase,
    Migu,
    Kuwo
}