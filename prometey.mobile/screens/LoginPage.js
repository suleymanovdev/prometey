import React, { useState } from 'react';
import { View, StyleSheet, Alert } from 'react-native';
import { TextInput, Button, ActivityIndicator, useTheme } from 'react-native-paper';
import AsyncStorage from '@react-native-async-storage/async-storage';

export default function LoginPage({ navigation }) {
    const [email, setEmail] = useState('');
    const [password, setPassword] = useState('');
    const [loading, setLoading] = useState(false);
    const { colors } = useTheme();

    const login = async () => {
        setLoading(true);
        try {
            const response = await fetch('http://localhost:5205/api/Auth/login', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                },
                body: JSON.stringify({ email, password }),
            });

            const data = await response.json();
            setLoading(false);

            if (response.status === 200) {
                await saveCredentials(data.token, data.userId);
                navigation.reset({
                    index: 0,
                    routes: [{ name: 'MainTabs' }],
                });
            } else {
                Alert.alert('Login Failed', 'Please check your credentials');
            }
        } catch (error) {
            setLoading(false);
            Alert.alert('Error', 'An error occurred during login');
        }
    };

    const saveCredentials = async (token, userId) => {
        const expiryDate = new Date().getTime() + 30 * 60 * 1000;

        try {
            await AsyncStorage.multiSet([
                ['@auth_token', token],
                ['@auth_userId', userId],
                ['@auth_expiry', expiryDate.toString()],
            ]);

            console.log('Credentials saved successfully');
        } catch (e) {
            console.error('Failed to save the credentials', e);
        }
    };

    return (
        <View style={styles.container}>
            <TextInput
                label="Email"
                value={email}
                onChangeText={setEmail}
                style={styles.input}
                keyboardType="email-address"
                mode="outlined"
            />
            <TextInput
                label="Password"
                value={password}
                onChangeText={setPassword}
                style={styles.input}
                secureTextEntry
                mode="outlined"
            />
            <Button mode="contained" onPress={login} loading={loading} disabled={loading}>
                Log In
            </Button>
        </View>
    );
}

const styles = StyleSheet.create({
    container: {
        padding: 20,
        flex: 1,
        justifyContent: 'center',
    },
    input: {
        marginBottom: 20,
    },
});
