import React, { useEffect, useState } from 'react';
import { 
    View, 
    Text, 
    StyleSheet, 
    FlatList, 
    ActivityIndicator,
    TouchableOpacity
} from 'react-native';
import AsyncStorage from '@react-native-async-storage/async-storage';

export default function MainPage({ navigation }) {
    const [posts, setPosts] = useState([]);
    const [loading, setLoading] = useState(true);
    const categories = [
        "Unknown", "Back-end Development", "Front-end Development",
        "Data Science", "DevOps", "Cybersecurity", "Design", "User"
    ];

    const fetchPosts = async () => {
        try {
            const token = await AsyncStorage.getItem('@auth_token');

            const response = await fetch('http://localhost:5205/api/General/get-posts-pagination?page=1&pageSize=10');

            if (response.ok) {
                const data = await response.json();
                setPosts(data.posts);
            } else {
                throw new Error('Error fetching posts');
            }
        } catch (error) {
            console.error('Error fetching posts:', error);
        } finally {
            setLoading(false);
        }
    };

    useEffect(() => {
        fetchPosts();
    }, []);

    const handlePostPress = (postId) => {
        navigation.navigate('PostDetail', { postId });
    };

    const renderPost = ({ item }) => (
        <TouchableOpacity 
            style={styles.postContainer}
            onPress={() => handlePostPress(item.id)}
        >
            <Text style={styles.postTitle}>{item.title}</Text>
            <Text style={styles.postMeta}>Category: {categories[item.category]} by {item.author}</Text>
            <Text style={styles.postDescription}>{item.description}</Text>
        </TouchableOpacity>
    );

    if (loading) {
        return (
            <View style={styles.loader}>
                <ActivityIndicator size="large" color="#17214a" />
            </View>
        );
    }

    return (
        <View style={styles.container}>
            <FlatList
                data={posts}
                renderItem={renderPost}
                keyExtractor={(item) => item.id.toString()}
            />
        </View>
    );
}

const styles = StyleSheet.create({
    container: {
        flex: 1,
        padding: 16,
        backgroundColor: '#fdfdfd',
    },
    postContainer: {
        backgroundColor: '#fff',
        borderRadius: 10,
        padding: 16,
        marginBottom: 16,
        shadowColor: '#000',
        shadowOffset: { width: 0, height: 2 },
        shadowOpacity: 0.3,
        shadowRadius: 3,
        elevation: 3,
    },
    postTitle: {
        fontSize: 18,
        fontWeight: 'bold',
        marginBottom: 8,
        color: '#17214a',
    },
    postMeta: {
        fontSize: 14,
        color: '#555',
        marginBottom: 8,
    },
    postDescription: {
        fontSize: 16,
        color: '#333',
    },
    loader: {
        flex: 1,
        justifyContent: 'center',
        alignItems: 'center',
    },
});