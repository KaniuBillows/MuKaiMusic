import { NavigationContainer, Theme } from '@react-navigation/native';
import React, { useContext } from 'react';
import Home from "./src/Home";
import { Explore } from './src/Explore';
import { createBottomTabNavigator } from '@react-navigation/bottom-tabs';
import { MaterialCommunityIcons } from '@expo/vector-icons';
import { AppTheme, ThemeContext } from './src/Theme';
import { StatusBar, View, Text } from 'react-native';
import MiniPlayer from './src/MiniPlayer';
import { AppPlayer } from './src/AppPlayer';
import FadeInImage from './src/FadeInView';
import { LanguageContext, MultiLan } from './src/Language';
import { LanguagePack } from './src/util/LanguagePack';

export default function App() {

  return (
    <AppTheme>
      <MultiLan>
        <LanguageContext.Consumer>
          {(language) => (
            <AppPlayer>
              <ThemeContext.Consumer>
                {(theme) =>
                  mainScreen(theme.currentTheme, language)
                }
              </ThemeContext.Consumer>
            </AppPlayer>
          )}
        </LanguageContext.Consumer>
      </MultiLan>
    </AppTheme >
  );
}

const Tab = createBottomTabNavigator();


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

const mainScreen = (theme: Theme, languagePack: LanguagePack) => {
  return (
    <NavigationContainer theme={theme}>
      <StatusBar translucent={true} backgroundColor={'rgba(255,255,255,0)'} barStyle={theme.dark ? "light-content" : "dark-content"}></StatusBar>
      <Tab.Navigator screenOptions={({ route }) => screenOption(route, languagePack)}
        tabBarOptions={{
          activeTintColor: theme.colors.primary,
          inactiveTintColor: theme.colors.text,
          tabStyle: {
            top: 25,
            maxHeight: 50
          },
          style: {
            height: 75,
            elevation: 0,
            shadowOpacity: 0
          }
        }}>
        <Tab.Screen name={languagePack.texts.home} component={Home} />
        <Tab.Screen name={languagePack.texts.explore} component={Explore} />
      </Tab.Navigator>
      <MiniPlayer style={{
        position: 'absolute', bottom: 50,
        borderBottomRightRadius: 25,
        borderTopRightRadius: 25,
      }} />

    </NavigationContainer >)
}