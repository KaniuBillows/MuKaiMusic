import React, { useCallback, useContext } from "react";
import { NativeSyntheticEvent, NativeTouchEvent, StyleSheet, View, } from "react-native";
import { ThemeButton, ThemeContext } from "./Theme";
import { PlayControlContext } from "./AppPlayer";

export function Explore(props) {
    const { currentTheme, setTheme } = useContext(ThemeContext);
    const control = useContext(PlayControlContext);
    const changeTheme = useCallback((ev: NativeSyntheticEvent<NativeTouchEvent>) => {
        setTheme(currentTheme.dark ? 'light' : 'dark');
    }, [currentTheme]);
    return (
        <View style={styles.container}>
            <ThemeButton title="theme" onPress={changeTheme}></ThemeButton>
        </View>
    );
}


const styles = StyleSheet.create({
    container: {
        flex: 1,
        alignItems: 'center',
        justifyContent: 'center'
    },
});




