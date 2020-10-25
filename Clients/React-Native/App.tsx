import { NavigationContainer, useNavigation } from '@react-navigation/native';
import React, { useCallback, useContext, useEffect, useRef, useState } from 'react';
import Home from "./src/Home/Home";
import { Explore } from './src/Explore';
import { createBottomTabNavigator } from '@react-navigation/bottom-tabs';
import { MaterialCommunityIcons } from '@expo/vector-icons';
import { AppTheme, ThemeContext } from './src/Theme';
import { StatusBar, View, Text, BackHandler } from 'react-native';
import MiniPlayer from './src/MiniPlayer';
import { AppPlayer } from './src/AppPlayer';
import { LanguageContext, MultiLan } from './src/Language';
import { LanguagePack } from './src/util/LanguagePack';
import { createStackNavigator, TransitionPresets } from '@react-navigation/stack';
import Search from './src/Search';
import { MusicPlay } from './src/MusicPlay';
import { RootSiblingParent } from 'react-native-root-siblings';
import Toast from 'react-native-root-toast';
import RNExitApp from 'react-native-exit-app';
let lastBackPressed = Date.now();
export default function App() {
  const navigatorRef = React.useRef(null);
  const [style, setStyle] = useState({ display: 'flex', bottom: 50 });
  return (
    <RootSiblingParent>
      <AppPlayer>
        <AppTheme>
          <MultiLan>
            <ThemeContext.Consumer>
              {(theme) => (
                <NavigationContainer ref={navigatorRef} theme={theme.currentTheme} 
                onStateChange={(state) => {
                  if (navigatorRef.current.getCurrentRoute().name === 'search') {
                    setStyle({ bottom: 10, display: 'flex' }); return;
                  }
                  else if (navigatorRef.current.getCurrentRoute().name === 'music') {
                    setStyle({ bottom: 50, display: 'none' }); return;
                  } else {
                    setStyle({ bottom: 50, display: 'flex' }); return;
                  }
                }}>
                  <RootStack.Navigator headerMode="none">
                    <RootStack.Screen name="main" component={MainScreen} />
                    <RootStack.Screen options={{ ...TransitionPresets.SlideFromRightIOS, gestureDirection: "horizontal" }} name="search" component={Search} />
                    <RootStack.Screen name="music" component={MusicPlay} />
                  </RootStack.Navigator>
                  <MiniPlayer style={{
                    ...style,
                    borderBottomRightRadius: 25,
                    borderTopRightRadius: 25,
                  }} />
                </NavigationContainer>
              )}
            </ThemeContext.Consumer>
          </MultiLan>
        </AppTheme >
      </AppPlayer>
    </RootSiblingParent>
  );
}

const Tab = createBottomTabNavigator();
const RootStack = createStackNavigator();

const screenOption = (route, languagePack: LanguagePack) => ({
  tabBarIcon: ({ focused, color, size }) => {
    let iconName: string;
    if (route.name === languagePack.texts.home) {
      iconName = focused
        ? 'music-circle'
        : 'music-circle-outline';
    } else if (route.name === languagePack.texts.explore) {
      iconName = focused ? 'compass' : 'compass-outline';
    } else iconName = "home"
    // You can return any component that you like here!
    return (<MaterialCommunityIcons name={iconName} size={size} color={color} />);
  }
});


function MainScreen(props) {
  const navigation = useNavigation();

  const onBackAndroid = useCallback(() => {
    if (!navigation.isFocused()) return false;
    if (lastBackPressed && lastBackPressed + 2000 >= Date.now()) {
      RNExitApp.exitApp();
      return false
    } else {
      lastBackPressed = Date.now()
      Toast.show('再按一次返回退出', {
        duration: 2000,
        position: Toast.positions.CENTER,
        shadow: true,
        animation: true,
        hideOnPress: true,
        delay: 0,
      });
      return true
    }
  }, [lastBackPressed]);
  useEffect(() => {
    BackHandler.addEventListener('hardwareBackPress', onBackAndroid);
    return () => {
      BackHandler.removeEventListener('hardwareBackPress', onBackAndroid);
    }
  }, [])

  const { currentTheme } = useContext(ThemeContext);
  const languagePack = useContext(LanguageContext);
  return (
    <>
      <StatusBar translucent={true} backgroundColor={'rgba(255,255,255,0)'} barStyle={currentTheme.dark ? "light-content" : "dark-content"}></StatusBar>
      <Tab.Navigator screenOptions={({ route }) => screenOption(route, languagePack)}
        tabBarOptions={{
          activeTintColor: currentTheme.colors.primary,
          inactiveTintColor: currentTheme.colors.text,
          tabStyle: {
            top: 25,
            maxHeight: 50,
            backgroundColor: currentTheme.colors.background,
            borderColor: "transparent"
          },
          labelStyle: {
            bottom: 5
          },
          style: {
            height: 75,
            elevation: 0,
            shadowOpacity: 0,
            backgroundColor: currentTheme.colors.background,
            borderTopColor: "transparent"
          },
        }}>
        <Tab.Screen name={languagePack.texts.home} component={Home} />
        <Tab.Screen name={languagePack.texts.explore} component={Explore} />
      </Tab.Navigator>
    </>
  )
}

