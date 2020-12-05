import React, { useCallback, useContext, useRef, useState } from "react";
import { Text, Image, View, Animated, Dimensions, findNodeHandle, UIManager } from "react-native";
import { Header } from 'react-native-elements';
import Swiper from "react-native-swiper";
import Searchbar from "../Components/Searchbar";
import { LanguageContext } from "../Language";
import { ThemeContext } from '../Theme';
import Recommend from "./Recommend";

const windowWidth = Dimensions.get("window").width * 0.9;

export default function Home() {
    const { currentTheme } = useContext(ThemeContext);
    const languagePack = useContext(LanguageContext);
    const [currentScreen, setScrren] = useState<'recommend' | 'pavilion'>('recommend');
    const paddingLeft = useRef(new Animated.Value(0)).current;
    const swiperRef = useRef<Swiper>();
    const pressRecommend = useCallback(() => {
        if (currentScreen === 'recommend') return;
        swiperRef.current.scrollTo(0);
        setScrren('recommend');
    }, [currentScreen, swiperRef]);
    const pressPavilion = useCallback(() => {
        if (currentScreen === 'pavilion') return;
        swiperRef.current.scrollTo(1);
        setScrren('pavilion');
    }, [currentScreen, swiperRef]);
    const indexChange = useCallback((index: number) => {
        setTimeout(() => {
            if (index === 0)
                setScrren('recommend');
            else
                setScrren('pavilion');
        }, 0)
    }, [currentScreen])
    return (

        <View style={{
            paddingLeft: '5%',
            paddingRight: '5%',
            height: '100%'
        }}>
            <Header backgroundColor={currentTheme.colors.background}
                leftComponent={(
                    <>
                        <View style={{ display: "flex", flexDirection: "column", width: 140 }}>
                            <View style={{
                                display: 'flex', flexDirection: 'row',
                                justifyContent: 'space-around', width: '100%',
                                alignContent: 'flex-end',
                            }}>
                                <View style={{ flex: 1, display: 'flex', flexDirection: 'row', justifyContent: 'center' }}>
                                    <Text style={{
                                        color: currentTheme.colors.text,
                                        fontSize: 20,
                                        //bottom: currentScreen === 'recommend' ? 2 : 0
                                    }} onPress={pressRecommend}>{languagePack.texts.recommend}</Text>
                                </View>
                                <View style={{ flex: 1, display: 'flex', flexDirection: 'row', justifyContent: 'center' }}>
                                    <Text style={{
                                        color: currentTheme.colors.text,
                                        fontSize: 20,
                                        //bottom: currentScreen === 'pavilion' ? 2 : 0
                                    }} onPress={pressPavilion}>{languagePack.texts.pavilion}</Text>
                                </View>
                            </View>
                            <View style={{ width: '100%', flexDirection: 'row' }}>
                                {(() => {
                                    //TODO:animated update the bar's width. this is a low priority demand.
                                    const left = paddingLeft.interpolate({
                                        inputRange: [0, windowWidth],
                                        outputRange: [0, 70],
                                    })
                                    return (<Animated.View style={{
                                        left: left,
                                        width: '50%',
                                        display: 'flex',
                                        flexDirection: 'row',
                                        justifyContent: 'center'
                                    }}>
                                        <View style={{
                                            backgroundColor: currentTheme.colors.primary, height: 2, borderRadius: 5,
                                            width: '65%'
                                        }}></View>
                                    </Animated.View>)
                                })()}

                            </View>
                        </View>
                    </>
                )}
                containerStyle={{ borderBottomColor: 'transparent' }}
                leftContainerStyle={{
                    left: -20,
                }}
            />
            <Searchbar style={{ bottom: 10 }} />
            <Swiper ref={swiperRef} showsPagination={false} loop={false} onIndexChanged={indexChange}
                onScroll={Animated.event([{ nativeEvent: { contentOffset: { x: paddingLeft } } }], { useNativeDriver: false })}>
                <Recommend></Recommend>
                <Pavilion></Pavilion>
            </Swiper>
        </View>
    );
}

function Pavilion() {
    return (<Text >Pavilion</Text>)
}