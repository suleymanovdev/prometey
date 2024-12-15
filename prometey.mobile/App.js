import * as React from 'react';
import { Provider as PaperProvider, DefaultTheme } from 'react-native-paper';
import { NavigationContainer } from '@react-navigation/native';
import { createBottomTabNavigator } from '@react-navigation/bottom-tabs';
import { createStackNavigator } from '@react-navigation/stack';
import MainPage from './screens/MainPage';
import LoginPage from './screens/LoginPage';
import ProfilePage from './screens/ProfilePage';
import WelcomePage from './screens/WelcomePage';
import PostDetailPage from './screens/PostDetailPage';
import Icon from 'react-native-vector-icons/MaterialIcons';
import AsyncStorage from '@react-native-async-storage/async-storage';
import { useEffect, useState } from 'react';

const theme = {
    ...DefaultTheme,
    roundness: 4,
    colors: {
        ...DefaultTheme.colors,
        primary: '#17214a',
        accent: '#fdfdfd',
    },
};

const MainStack = createStackNavigator();
function MainStackScreen() {
    return (
        <MainStack.Navigator>
            <MainStack.Screen 
                name="MainFeed" 
                component={MainPage}
            />
            <MainStack.Screen 
                name="PostDetail" 
                component={PostDetailPage}
                options={{ title: 'Post Details' }}
            />
        </MainStack.Navigator>
    );
}

const Tab = createBottomTabNavigator();

function MainTabs() {
    return (
        <Tab.Navigator
            screenOptions={({ route }) => ({
                tabBarIcon: ({ focused, color, size }) => {
                    let iconName;

                    if (route.name === 'MainStack') {
                        iconName = 'home';
                    } else if (route.name === 'Profile') {
                        iconName = 'person';
                    } else if (route.name === 'Logout') {
                        iconName = 'logout';
                    }

                    return <Icon name={iconName} size={size} color={color} />;
                },
                tabBarActiveTintColor: '#17214a',
                tabBarInactiveTintColor: 'gray',
                tabBarStyle: {
                    backgroundColor: '#fdfdfd',
                },
            })}
        >
            <Tab.Screen 
                name="MainStack" 
                component={MainStackScreen}
                options={{ title: 'Main', headerShown: false }}
            />
            <Tab.Screen name="Profile" component={ProfilePage} />
            <Tab.Screen
                name="Logout"
                component={LoginPage}
                listeners={({ navigation }) => ({
                    tabPress: async () => {
                        await AsyncStorage.clear();
                        navigation.reset({
                            index: 0,
                            routes: [{ name: 'Welcome' }],
                        });
                    },
                })}
            />
        </Tab.Navigator>
    );
}

const AuthStack = createStackNavigator();
function AuthNavigator() {
    return (
        <AuthStack.Navigator initialRouteName="Welcome">
            <AuthStack.Screen name="Welcome" component={WelcomePage} />
            <AuthStack.Screen name="Login" component={LoginPage} />
            <AuthStack.Screen
                name="MainTabs"
                component={MainTabs}
                options={{ headerShown: false }}
            />
        </AuthStack.Navigator>
    );
}

export default function App() {
    const [isAuthenticated, setIsAuthenticated] = useState(false);

    useEffect(() => {
        const checkAuthStatus = async () => {
            const token = await AsyncStorage.getItem('@auth_token');
            setIsAuthenticated(!!token);
        };

        checkAuthStatus();
    }, []);

    return (
        <PaperProvider theme={theme}>
            <NavigationContainer>
                {isAuthenticated ? <MainTabs /> : <AuthNavigator />}
            </NavigationContainer>
        </PaperProvider>
    );
}