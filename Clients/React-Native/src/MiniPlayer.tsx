import React, { useCallback, useContext, useRef, useState } from 'react';
import { Image, RegisteredStyle, Text, View, ViewPropTypes, ViewStyle } from "react-native";
import PropTypes from 'prop-types';
import { MaterialCommunityIcons } from '@expo/vector-icons';
import { Theme } from '@react-navigation/native';
import { ThemeButton, ThemeContext } from './Theme';
import ProgressBar from 'react-native-progress/Bar';
import { PlayControl, PlayControlContext } from './AppPlayer';
import Swiper from 'react-native-swiper';
import FadeInImage from './FadeInView';

export default function MiniPlayer(props: { style?: ViewStyle }) {
    const { currentTheme } = useContext(ThemeContext);
    const [swiperIndex, setIndex] = useState(0);
    const control = useContext(PlayControlContext);
    const handlePlay = useCallback((ev) => {
        if (control.playState.isLoaded) {
            if (control.playState.isPlaying) {
                control.pause();
            } else control.resume();
        } else control.start();
    }, [control]);
    const imageSource = useCallback(control.playState.currentMusicInfo ? { uri: control.playState.currentMusicInfo.album.picUrl }
        : require('../assets/default_cover.png'), [control.playState.currentMusicInfo]);
    return (
        <View style={{
            position: "absolute",
            height: 50,
            alignSelf: 'center',
            width: '90%',
            display: "flex",
            flexDirection: 'row',
            backgroundColor: currentTheme.dark ? 'rgb(40,40,40)' : 'rgb(240,240,240)',
            borderColor: currentTheme.colors.border,
            borderRadius: 5,
            overflow: 'hidden',
            ...props.style,
        }}>

            <FadeInImage style={{ flex: 1, height: '100%' }} source={imageSource} />

            <View style={{
                flex: 6,
                width: '82%',
                height: '100%',
                display: 'flex',
                flexDirection: 'column'
            }}>
                <View style={{ justifyContent: 'center', flexDirection: 'row', height: '96%', display: 'flex' }}>
                    <View style={{ flex: 3 }}>
                        <Swiper index={0} showsPagination={false} onIndexChanged={index => {
                            setTimeout(() => {
                                let temp = index - swiperIndex
                                if (temp === 1 || temp === -2) control.switchNext();
                                if (temp === -1 || temp === 2) control.switchLast();
                                setIndex(index)
                            })
                        }}>
                            <View style={{ display: 'flex', justifyContent: 'center', alignItems: 'center', height: '95%' }}>
                                <Text numberOfLines={1} style={{ maxWidth: '80%', position: 'relative', color: currentTheme.colors.text }}>
                                    {swiperIndex === 0 ? control.playState.currentMusicInfo?.name ?? 'Mukai Music' :
                                        swiperIndex === 1 ? control.playState.lastMusicInfo?.name ?? 'Mukai Music' :
                                            control.playState.nextMusicInfo?.name ?? 'Mukai Music'
                                    }
                                </Text>
                                <Text numberOfLines={1} style={{ maxWidth: '80%', fontSize: 8, color: 'gray' }}>
                                    {swiperIndex === 0 && control.playState.lyricReady ? control.playState.currentLyric.text : ''}
                                </Text>
                            </View>
                            <View style={{ display: 'flex', justifyContent: 'center', alignItems: 'center', height: '95%' }}>
                                <Text numberOfLines={1} style={{ maxWidth: '80%', position: 'relative', color: currentTheme.colors.text }}>
                                    {swiperIndex === 1 ? control.playState.currentMusicInfo?.name ?? 'Mukai Music' :
                                        swiperIndex === 2 ? control.playState.lastMusicInfo?.name ?? 'Mukai Music' :
                                            control.playState.nextMusicInfo?.name ?? 'Mukai Music'
                                    }
                                </Text>
                                <Text numberOfLines={1} style={{ maxWidth: '80%', fontSize: 8, color: 'gray' }}>
                                    {swiperIndex === 1 && control.playState.lyricReady ? control.playState.currentLyric.text : ''}
                                </Text>
                            </View>
                            <View style={{ display: 'flex', justifyContent: 'center', alignItems: 'center', height: '95%' }}>
                                <Text numberOfLines={1} style={{ maxWidth: '80%', position: 'relative', color: currentTheme.colors.text }}>
                                    {swiperIndex === 2 ? control.playState.currentMusicInfo?.name ?? 'Mukai Music' :
                                        swiperIndex === 0 ? control.playState.lastMusicInfo?.name ?? 'Mukai Music' :
                                            control.playState.nextMusicInfo?.name ?? 'Mukai Music'
                                    }
                                </Text>
                                <Text numberOfLines={1} style={{ maxWidth: '80%', fontSize: 8, color: 'gray' }}>
                                    {swiperIndex === 2 && control.playState.lyricReady ? control.playState.currentLyric.text : ''}
                                </Text>
                            </View>
                        </Swiper>
                    </View>
                    <View style={{ flex: 1, display: 'flex', justifyContent: 'center', alignItems: 'center' }}>
                        <ThemeButton icon={
                            <MaterialCommunityIcons
                                color={currentTheme.colors.text}
                                name={control.playState.isPlaying ? 'pause-circle-outline' : 'play-circle-outline'}
                                size={26}
                            />}
                            onPress={handlePlay}
                            buttonStyle={{ backgroundColor: 'transparent' }}
                            containerStyle={{ borderRadius: 20 }}
                            loading={control.playState.isLoading}
                            loadingProps={{ color: currentTheme.colors.primary }}
                            disabled={control.playState.isLoading}
                        >
                        </ThemeButton >
                    </View>
                </View>
                <ProgressBar progress={control.playState.currentTime / control.playState.totalTime}
                    width={200} height={2} borderWidth={0} >
                </ProgressBar>
            </View>
        </View >
    );
}
