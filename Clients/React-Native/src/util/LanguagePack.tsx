export interface LanguagePack {
    name: string,
    texts: LanguageContents
}
export interface LanguageContents {
    home: string,
    explore: string,
    play: string,
    pause: string,
    mine: string,
    recommend: string,
    /**
     * 音乐馆
     */
    pavilion: string,
    todaysSong: string,
    personalFm: string,
    cancel: string,
    placeHolder: string,
    daily30: string,
}

export const ChinesePack: LanguagePack = {
    name: '中文',
    texts: {
        home: '首页',
        explore: '发现',
        play: '播放',
        pause: '暂停',
        mine: '我的',
        recommend: '推荐',
        pavilion: '音乐馆',
        todaysSong: '今日推荐',
        personalFm: '私人FM',
        cancel: "取消",
        placeHolder: "搜索",
        daily30: '每日30首',
    }
}

export const EnglishPack: LanguagePack = {
    name: 'English',
    texts: {
        home: 'home',
        explore: 'explore',
        play: 'play',
        pause: 'pause',
        mine: 'mine',
        recommend: 'recommend',
        pavilion: 'pavilion',
        todaysSong: 'today\'s song',
        personalFm: 'Personal FM',
        cancel: "cancel",
        placeHolder: "Type Here...",
        daily30: "Daily 30"
    }
}