import React, { useContext } from "react";
import { ThemeContext } from "./Theme";
import { MarqueeHorizontal } from 'react-native-marquee-ab';
import { useNavigation } from "@react-navigation/native";

export function MusicPlay(props) {
    const { currentTheme } = useContext(ThemeContext);
    
    return (<MarqueeHorizontal
        textList={[{ value: 'Super long piece of text is long. The quick brown fox jumps over the lazy dog.' }]}
        width={200}
        height={30}
        bgContainerStyle={{
            backgroundColor: 'transparent'
        }}
        textStyle={{
            color: currentTheme.colors.text
        }}
    />)

}