import React, { useCallback, useContext, useEffect, useState } from "react";
import { View, ViewStyle } from "react-native";
import { MaterialIcons } from '@expo/vector-icons';
import { ThemeContext } from "../Theme";
import { MarqueeVertical } from 'react-native-marquee-ab';
import { MusicNetwork } from "../service/Network";
import { useNavigation } from "@react-navigation/native";
import { EvilIcons } from '@expo/vector-icons';
import { TouchableOpacity } from "react-native-gesture-handler";
interface SearchbarPorp {
    style?: ViewStyle
    inputStyle?: ViewStyle
    iconSize?: number,
    placeHolder?: string[]
}
export default function Searchbar(props: SearchbarPorp) {
    const { currentTheme } = useContext(ThemeContext);
    const [hotkeys, setHotkey] = useState<{ label: string, value: string }[]>([]);
    const navigation = useNavigation();
    //try to get the search hot key.
    useEffect(() => {
        MusicNetwork.getSearchHotKey().then(res => {
            if (res.code === 200) {
                setHotkey(res.content.map((val, index) => ({
                    label: index.toString(),
                    value: val
                })));
            } else {
                setHotkey([{ label: '0', value: "用音乐打开每一天" }]);
            }
        }).catch(() => {
            setHotkey([{ label: '0', value: "用音乐打开每一天" }]);
        })
    }, []);
    const onPress = useCallback(() => {
        navigation.navigate("search");
    }, [navigation])
    return (
        <>
            <TouchableOpacity onPress={onPress} style={{
                width: "100%", backgroundColor: 'transaprent', display: "flex", height: 40, flexDirection: 'row',
                alignContent: 'center',
                justifyContent: 'center'
                , ...props.style
            }}>
                <View style={{
                    width: "100%", backgroundColor: currentTheme.colors.contentBackground,
                    borderRadius: 10, height: 30, ...props.inputStyle,
                    display: 'flex',
                    flexDirection: "row",
                    alignContent: "center",
                    alignItems: "center",
                    justifyContent: "flex-start",
                }}>
                    <EvilIcons onPress={onPress}
                        name="search" size={props.iconSize ?? 20} color={currentTheme.colors.text} style={{
                            marginLeft: 10
                        }} />
                    {/* <Text numberOfLines={1} style={{ marginLeft: 5 }}>用音乐打开每一天</Text> */}
                    <MarqueeVertical delay={3000} height={30} duration={1000} textList={hotkeys} width={290}
                        bgContainerStyle={{
                            backgroundColor: 'transparent',
                            marginLeft: 5
                        }} textStyle={{
                            color: currentTheme.dark ? "rgb(200,200,200)" : "gray"
                        }} />
                </View>
            </TouchableOpacity>
        </>
    )
}
