export class NetEaseLoginUserInfo {
    loginType: number;
    code: number;
    token: string;
    account: {
        id: number;
        userName: string;
        type: number;
        status: number;
        createTime: number;
        salt: string;
        viptypeVersion: number;
        anonimousUser: boolean;
    }
    profile: {
        avatarImgIdStr: string;
        province: number;
        defaultAvatar: number;
        avatarUrl: string;
        backgroundUrl: string;
        djStatus: number;
        userId: number;
        nickname: string;
        signature: string;
        followeds: number;
        follows: number;
        eventCount: number;
        playlistCount: number;
        playlistBeSubscribedCount: number;
    }
}

export class UserInfo {
    id: number;
    userName: string;
    email: string;
    phoneNumber: string;
    ncikName: string;
    avatarUrl: string;
}