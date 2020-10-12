import React from "react";
import { ThemeContext } from "./Theme";
import { MarqueeHorizontal } from 'react-native-marquee-ab';

export class MusicPlay extends React.Component {
    constructor(props) {
        super(props);
    }

    render() {
        return (<MarqueeHorizontal
            textList={[{ value: 'Super long piece of text is long. The quick brown fox jumps over the lazy dog.' }]}
            width={200}
            height={30}
            bgContainerStyle={{
                backgroundColor: 'transparent'
            }}
            textStyle={{
                color: this.context.currentTheme.colors.text
            }}
        />)
    }
}

MusicPlay.contextType = ThemeContext