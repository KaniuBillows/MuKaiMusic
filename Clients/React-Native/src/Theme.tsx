import { DarkTheme, DefaultTheme } from "@react-navigation/native";
import React, { useContext, useState } from "react";
import { Button } from 'react-native-elements';
export interface Theme {
    dark: boolean
    colors: {
        text: string,
        primary: string,
        card: string,
        background: string,
        border: string,
        notification: string,
        secondaryText: string,
        contentBackground: string
    }
}
const darkTheme: Theme = {
    ...DarkTheme,
    colors: {
        ...DarkTheme.colors,
        //card: "rgb(0,0,0)",
        // border: 'transparent',
        //primary: '#0071E6',
        // secondaryText:'rgb'
        contentBackground: 'rgb(44,44,46)',
        text: 'rgb(229,229,234)',
        secondaryText: 'rgb(142,142,147)'
    },
}
const lightTheme: Theme = {
    ...DefaultTheme,
    colors: {
        ...DefaultTheme.colors,
        // background: 'rgb(255, 255, 255)',
        //border: 'transparent',
        // card: 'rgb(255,255,255)',
        //primary: '#0D84FF',
        contentBackground: 'rgb(229,229,234)',
        secondaryText: 'rgb(99,99,102)'
    }
}

export const ThemeContext = React.createContext({ currentTheme: lightTheme, setTheme: ((theme: 'dark' | 'light') => { }) });


export function AppTheme(props) {
    const [state, setState] = useState({
        currentTheme: lightTheme,
        setTheme: function _setTheme(theme: 'dark' | 'light') {
            setState({
                currentTheme: theme === 'dark' ? darkTheme : lightTheme,
                setTheme: _setTheme
            } as any);
        }
    });
    return <ThemeContext.Provider value={state} >{props.children}</ThemeContext.Provider>;

}

/**
 * a button which is themeable with the ThemeContext
 * @param props 
 */
export function ThemeButton(props) {
    const { currentTheme } = useContext(ThemeContext);
    return (
        <Button buttonStyle={{
            backgroundColor: currentTheme.colors.primary,
            borderColor: currentTheme.colors.border,
        }}
            {...props}></Button>
    )
}