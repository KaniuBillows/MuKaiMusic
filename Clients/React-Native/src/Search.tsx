import { View, Text, Button, FlatList, TouchableOpacity, StyleSheet } from "react-native";
import React, { useCallback, useContext, useEffect, useMemo, useRef, useState } from "react";
import { SafeAreaView } from "react-native-safe-area-context";
import { Header, SearchBar } from "react-native-elements";
import { ThemeContext } from "./Theme";
import { useNavigation } from "@react-navigation/native";
import { LanguageContext } from "./Language";
import { EvilIcons } from '@expo/vector-icons';
import { MusicNetwork } from "./service/Network";
import { DataSource, MusicInfo } from "./Abstract/Abstract";
import Highlighter from 'react-native-highlight-words';
import { PlayControlContext, PlayMode } from "./AppPlayer";


export default React.memo(function Search() {


    const { currentTheme } = useContext(ThemeContext);
    const languagePack = useContext(LanguageContext);

    const serachRef = useRef<SearchBar>();

    const handleFocus = useCallback(e => {
        serachRef.current.focus();
    }, []);
    const navigation = useNavigation();

    useEffect(() => {
        navigation.addListener('focus', handleFocus);
        return () => {
            navigation.removeListener('focus', handleFocus);
        }
    }, []);

    const [value, setValue] = useState<string>();
    const handleTextChange = useCallback((text: string) => {
        setValue(text);
    }, [])

    const [searchResult, setResult] = useState<MusicInfo[]>([]);
    //TODO:search loading.
    const submit = useCallback(() => {
        setResult([]);
        MusicNetwork.searchMusic(value).subscribe(res => {
            setResult(old => old.concat(res))
        })
    }, [value]);
    const bottomView = useCallback(() => (<View style={{ width: '100%', height: 60 }}></View>), [])
    const renderItem = useCallback(({ item }) => {
        return (<SearchResultItem music={item} value={value} />)
    }, [value]);
    return (
        <SafeAreaView style={{
            flex: 1, paddingLeft: '5%', paddingRight: '5%',
        }}>
            <View style={{
                display: 'flex', flexDirection: "row", justifyContent: "flex-start", alignItems: 'center',
            }}>
                <SearchBar ref={serachRef}
                    placeholder={languagePack.texts.placeHolder} platform="ios" showCancel={true}
                    onCancel={() => {
                        navigation.goBack();
                    }}
                    showLoading={true}
                    searchIcon={
                        <EvilIcons name="search" size={24} color={currentTheme.colors.text}
                            style={{ marginRight: -5, }}
                        />}
                    value={value}
                    returnKeyType="search"
                    onChangeText={handleTextChange}
                    inputStyle={{ color: currentTheme.colors.text }}
                    cancelButtonTitle={languagePack.texts.cancel}
                    onSubmitEditing={submit}
                    cancelButtonProps={{ color: currentTheme.colors.primary }}
                    inputContainerStyle={{ height: 30, backgroundColor: currentTheme.colors.contentBackground, left: '-20%' }}
                    containerStyle={{ backgroundColor: "transparent", flex: 9 }}
                />
            </View>
            <FlatList showsVerticalScrollIndicator={false} keyExtractor={item => (getSid(item))}
                data={searchResult}
                renderItem={renderItem}
                style={itemStyle}
                ListFooterComponent={bottomView}
                getItemLayout={(data, index) => ({
                    length: 50,
                    offset: 50 * index,
                    index
                })}
            />
        </SafeAreaView >
    )
})
const itemStyle = { width: '100%' }
const getSid = (music: MusicInfo) => {
    if (music.dataSource === DataSource.kuwo) {
        return music.kw_Id + DataSource[DataSource.kuwo];
    } else if (music.dataSource === DataSource.ne) {
        return music.ne_Id + DataSource[DataSource.ne];
    } else if (music.dataSource === DataSource.migu) {
        return music.migu_Id + DataSource[DataSource.migu];
    } else {
        return "";
    }
}


function SearchResultItem({ music, value }) {
    const { currentTheme } = useContext(ThemeContext);
    const control = useContext(PlayControlContext);
    const musicPress = useCallback(() => {
        if (control.playState.playMode === PlayMode.fm) {
            control.changePlayMode(PlayMode.listCycle);
            control.initPlaylist([]);
            control.addAndPlay(music);
        } else {
            control.addAndPlay(music);
        }        
    }, [music]);

    const highlightStyle = useMemo(() => ({ color: currentTheme.colors.primary }), [currentTheme.colors]);
    const primaryTextStyle = useMemo(() => ({ color: currentTheme.colors.text, fontSize: 18, maxWidth: "70%" }), [currentTheme.colors]);
    const secondaryTextStyle = useMemo(() => ({ color: currentTheme.colors.secondaryText, maxWidth: '45%' }), [currentTheme.colors]);
    const searchWords = useMemo(() => ([value]), [value]);

    return (
        <TouchableOpacity style={styles.touchable} onPress={musicPress} >
            <Highlighter textToHighlight={music.name}
                searchWords={searchWords}
                highlightStyle={highlightStyle}
                numberOfLines={1}
                style={primaryTextStyle} />
            <View style={styles.container}>
                <Highlighter textToHighlight={music.artists.map(ar => ar.name).join('/')}
                    searchWords={searchWords} highlightStyle={highlightStyle}
                    numberOfLines={1} style={secondaryTextStyle} />
                <Highlighter textToHighlight={'·' + music.album.name}
                    searchWords={searchWords} highlightStyle={highlightStyle}
                    numberOfLines={1} style={secondaryTextStyle} />
            </View>
        </TouchableOpacity>
    )
}
const styles = StyleSheet.create({
    container: { display: 'flex', flexDirection: "row", flex: 1, },
    touchable: { display: "flex", height: 40, marginBottom: 10 }
})
