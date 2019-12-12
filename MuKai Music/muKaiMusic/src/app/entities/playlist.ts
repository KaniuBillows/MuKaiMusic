/**
 * 全部歌单分类
 */
export class CategoryResult {
    public all: {
        name: string,
        resourceCount: number;
        category: number;
        hot: boolean;
        type: number;
        activity: boolean;
        resourceType: number;
    };
    public sub: {
        name: string;
        resourceCount: number;
        hot: true;
        activity: true;
        resourceType: number;
        category: number;
        type: number;
    }[];
    public categories: any;
}

/**
 * 热门分类
 */
export class HotCaegoryResult {
    public tages: {
        playlistTag: {
            id: number,
            name: string,
            category: number,
            usedCount: number,
            type: number,
            position: number,
            createTime: number,
            highQuality: number,
            highQualityPos: number,
            officialPos: number
        },
        activity: boolean,
        usedCount: number,
        createTime: number,
        position: number,
        category: number,
        hot: number,
        name: string,
        id: number,
        type: number
    }[];
}

/**
 * 推荐歌单结果
 */
export class PersonalizedPlaylistResult {
    result: {
        id: number;
        type: number;
        name: string;
        copywriter: string;
        picUrl: string;
        canDislike: boolean;
        trackNumberUpdateTime: number;
        playCount: number;
        trackCount: number;
        highQuality: true;
        alg: string;
    }[];
}