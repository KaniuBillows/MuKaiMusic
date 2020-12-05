import React, { useCallback, useContext, useEffect, useState } from "react";
import { ThemeButton, ThemeContext } from '../Theme';
import { LanguageContext } from "../Language";
import { Text, Image, View, Animated, Dimensions, Button } from "react-native";
import { MaterialCommunityIcons } from '@expo/vector-icons';
import { MusicNetwork } from '../service/Network';
import { MusicInfo, TrackList } from "../Abstract/Abstract";
import { PlayControlContext, PlayMode, PlayState } from "../AppPlayer";


export default function Recommend() {
    const { currentTheme } = useContext(ThemeContext);
    const control = useContext(PlayControlContext);
    const languagePack = useContext(LanguageContext);
    const [fmMusics, setMusics] = useState<MusicInfo[]>([]);
    const [dailyMusic, setDailyMusic] = useState<TrackList>(null);
    const fmPlayEnded = useCallback((e: PlayState) => {
        MusicNetwork.getPersonalFm().then(res => {
            if (res.code === 200) {
                setMusics(res.content);
                control.initPlaylist(res.content);
                control.changePlayMode(PlayMode.fm);
                if (e.isPlaying) {
                    control.start();
                }
            }
        })
    }, []);
    useEffect(() => {
        control.onFmPlayEnded(fmPlayEnded);
    }, []);
    useEffect(() => {
        MusicNetwork.getPersonalFm().then(res => {
            if (res.code === 200) {
                setMusics(res.content);
            }
        });
        MusicNetwork.getDaily30Musics().then(res => {
            if (res.code === 200) {
                setDailyMusic(res.content);
            }
        });
        return () => {
            setMusics([]);
            setDailyMusic(null);
        }
    }, []);
    const handleFmPress = useCallback(async () => {
        let musics: MusicInfo[];
        // if the player is fm mode already. try to get new musics
        if (control.playState.playMode === PlayMode.fm) {
            musics = (await MusicNetwork.getPersonalFm()).content;
            setMusics(musics);
        }
        else musics = fmMusics;
        control.initPlaylist(musics);
        control.changePlayMode(PlayMode.fm);
        control.start();
    }, [fmMusics, control.playState.playMode])
    const handleDailyPress = useCallback(() => {
        if (dailyMusic === null) return;
        control.initPlaylist(dailyMusic.musics);
        control.start();
    }, [dailyMusic]);
    return (
        <View>
            <Text style={{ fontSize: 18, color: currentTheme.colors.text }}>{languagePack.texts.todaysSong}</Text>
            <View style={{ display: 'flex', flexDirection: 'row' }}>
                <View style={{ display: 'flex', alignContent: 'center', flex: 2 }}>
                    {//TODO: while current music change. update the fm recommend.
                    }
                    <View style={{ display: 'flex', flexDirection: 'row' }} >
                        <View style={{ width: 140 }}>
                            <Image style={{
                                width: 140,
                                height: 110,
                                borderRadius: 15,
                            }} source={{ uri: fmMusics[0]?.album.picUrl }} />
                            <View style={{
                                position: 'absolute',
                                width: '100%',
                                top: 0,
                                bottom: 0,
                                left: 0,
                                right: 0,
                                justifyContent: 'center',
                                alignItems: 'center',
                                borderRadius: 15,
                                backgroundColor: 'rgba(0,0,0,.4)',
                            }}>
                                <ThemeButton icon={
                                    <MaterialCommunityIcons name={'radio-tower'} size={35} color={'rgba(255,255,255,.7)'}
                                    />}
                                    buttonStyle={{
                                        backgroundColor: 'transparent',
                                        width: 140,
                                        height: '100%',
                                        alignSelf: 'center'
                                    }}
                                    style={{ width: '100%' }}
                                    onPress={handleFmPress}
                                >
                                </ThemeButton>
                            </View>
                        </View>
                        <Image style={{
                            position: 'absolute',
                            borderRadius: 15,
                            left: 80,
                            bottom: 0,
                            zIndex: -1,
                            width: 110,
                            height: 85
                        }} source={{ uri: fmMusics[1]?.album.picUrl }} />
                        <Image source={{ uri: fmMusics[2]?.album.picUrl }} style={{
                            position: 'absolute',
                            borderRadius: 15,
                            left: 120,
                            bottom: 0,
                            zIndex: -2,
                            width: 110,
                            height: 85
                        }} />
                    </View>
                    <Text style={{ width: '100%', color: currentTheme.colors.secondaryText }}>{languagePack.texts.personalFm}</Text>
                </View>
                <View style={{ flex: 1, flexDirection: "column", alignContent: 'center' }} >
                    <View style={{
                        width: '100%', display: 'flex', flexDirection: 'row', justifyContent: 'flex-end',
                    }}>
                        <View style={{ width: '100%' }}>
                            <Image style={{
                                height: 110,
                                width: '100%',
                                borderRadius: 15
                            }} source={{ uri: dailyMusic?.musics[0].album.picUrl }} />
                            <View style={{
                                position: 'absolute',
                                width: '100%',
                                top: 0,
                                bottom: 0,
                                left: 0,
                                right: 0,
                                justifyContent: 'center',
                                alignItems: 'center',
                                borderRadius: 15,
                                backgroundColor: 'rgba(0,0,0,.2)'
                            }}>
                                <ThemeButton
                                    icon={<Text style={{ fontSize: 30, color: 'white' }}>Daily 30</Text>
                                    }
                                    buttonStyle={{
                                        backgroundColor: 'transparent',
                                        height: '100%'
                                    }}
                                    onPress={handleDailyPress}
                                >
                                </ThemeButton>
                            </View>
                        </View>
                    </View>
                    <View style={{
                        width: '100%', display: 'flex', flexDirection: 'row', justifyContent: 'flex-start',
                    }}>
                        <Text style={{ color: currentTheme.colors.secondaryText }}>{languagePack.texts.daily30}</Text>
                    </View>
                </View>
            </View>
        </View>
    )
}
