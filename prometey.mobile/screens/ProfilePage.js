import React, { useEffect, useState } from 'react';
import { View, StyleSheet, Image, Text } from 'react-native';
import AsyncStorage from '@react-native-async-storage/async-storage';
import { Button } from 'react-native-paper';

export default function ProfilePage({ navigation }) {
    const [userData, setUserData] = useState({});

    const categories = [
        "Unknown", "Back-end Development", "Front-end Development",
        "Data Science", "DevOps", "Cybersecurity", "Design", "User"
    ];

    const getUserData = async () => {
        try {
            const token = await AsyncStorage.getItem('@auth_token');
            const userId = await AsyncStorage.getItem('@auth_userId');
            const expiryDate = await AsyncStorage.getItem('@auth_expiry');

            const currentTime = new Date().getTime();
            if (!token || !userId || currentTime > parseInt(expiryDate)) {
                await AsyncStorage.clear();
                navigation.navigate('Login');
                return;
            }

            const response = await fetch(`http://localhost:5205/api/User/${userId}`, {
                headers: {
                    'Authorization': `Bearer ${token}`,
                },
            });

            if (response.ok) {
                const data = await response.json();
                setUserData(data);
            } else {
                throw new Error('Failed to fetch user data');
            }
        } catch (error) {
            console.error('Error fetching user data:', error);
            navigation.navigate('Login');
        }
    };

    useEffect(() => {
        getUserData();
    }, []);

    return (
        <View style={styles.container}>
            <Image
                style={styles.profileImage}
                source={{
                    uri: userData.profilePhotoLink || 'https://via.placeholder.com/150',
                }}
            />
            <Text style={styles.title}>{userData.name} {userData.surname}</Text>
            <Text style={styles.info}>Username: {userData.username}</Text>
            <Text style={styles.info}>Email: {userData.email}</Text>
            <Text style={styles.info}>Category: {categories[userData.category]}</Text>
        </View>
    );
}

const styles = StyleSheet.create({
    container: {
        flex: 1,
        alignItems: 'center',
        padding: 20,
        backgroundColor: '#fff',
    },
    profileImage: {
        width: 150,
        height: 150,
        borderRadius: 75,
        marginBottom: 20,
    },
    title: {
        fontSize: 24,
        fontWeight: 'bold',
    },
    info: {
        fontSize: 18,
        marginVertical: 5,
    },
    logoutButton: {
        marginTop: 20,
        width: '80%',
    },
});
