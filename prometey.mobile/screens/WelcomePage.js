import React from 'react';
import { View, StyleSheet, ImageBackground, Image } from 'react-native';
import { Button, Text, useTheme } from 'react-native-paper';

export default function WelcomePage({ navigation }) {
    const { colors } = useTheme();

    return (
        <ImageBackground source={require('../assets/images/wall.jpg')} style={styles.background}>
            <View style={styles.container}>
                <Image source={require('../assets/images/prometey.png')} style={styles.logo} />
                <Text style={styles.title}>Welcome</Text>
                <Button
                    mode="contained"
                    onPress={() => navigation.navigate('Login')}
                    style={styles.button}
                >
                    Log In
                </Button>
            </View>
        </ImageBackground>
    );
}

const styles = StyleSheet.create({
    background: {
        flex: 1,
        resizeMode: 'cover',
    },
    container: {
        flex: 1,
        justifyContent: 'center',
        alignItems: 'center',
    },
    logo: {
        width: 200,
        height: 200,
    },
    title: {
        fontSize: 30,
        color: '#17214a',
        fontWeight: 'bold',
        marginVertical: 20,
    },
    button: {
        marginVertical: 10,
        width: 200,
    },
});