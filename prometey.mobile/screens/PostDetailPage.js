import React, { useEffect, useState } from 'react';
import { View, Text, StyleSheet, ScrollView, Image, ActivityIndicator, useWindowDimensions } from 'react-native';
import RenderHtml from 'react-native-render-html';
import he from 'he';

export default function PostDetailPage({ route }) {
    const { postId } = route.params;
    const [post, setPost] = useState(null);
    const [loading, setLoading] = useState(true);
    const { width } = useWindowDimensions();

    useEffect(() => {
        const fetchPost = async () => {
            try {
                const response = await fetch(`http://localhost:5205/api/General/get-post-by-id/${postId}`);
                const data = await response.json();
                setPost(data);
            } catch (error) {
                console.error('Error fetching post data:', error);
            } finally {
                setLoading(false);
            }
        };

        fetchPost();
    }, [postId]);

    if (loading) {
        return (
            <View style={styles.loader}>
                <ActivityIndicator size="large" color="#17214a" />
            </View>
        );
    }

    if (!post) {
        return (
            <View style={styles.errorContainer}>
                <Text style={styles.errorText}>Error loading post.</Text>
            </View>
        );
    }

    const categories = [
        "Unknown", "Back-end Development", "Front-end Development",
        "Data Science", "DevOps", "Cybersecurity", "Design", "User"
    ];

    const postCategory = categories[post.category] || "Unknown";

    // Настройки для HTML рендеринга
    const tagsStyles = {
        body: {
            fontSize: 16,
            lineHeight: 24,
            color: '#333',
            fontFamily: 'System',
        },
        p: {
            marginBottom: 10,
        },
        a: {
            color: '#17214a',
            textDecorationLine: 'underline',
        },
        br: {
            height: 16, // Добавляем высоту для переносов строк
        }
    };

    // Базовый HTML стиль для контента
    const baseStyle = {
        fontSize: 16,
        lineHeight: 24,
        color: '#333',
    };

    // Преобразуем HTML-сущности
    const decodedContent = he.decode(post.content); // Using he to decode HTML entities

    return (
        <ScrollView style={styles.container}>
            <Image
                source={{ uri: post.postPhotoLink || 'https://via.placeholder.com/150' }}
                style={styles.image}
            />
            <View style={styles.contentContainer}>
                <Text style={styles.title}>{post.title}</Text>
                <Text style={styles.meta}>
                    {new Date(post.createdAt).toLocaleDateString()} | Category: {postCategory} | by {post.author}
                </Text>
                <RenderHtml
                    contentWidth={width - 32}
                    source={{ html: decodedContent }}
                    tagsStyles={tagsStyles}
                    baseStyle={baseStyle}
                    renderersProps={{
                        a: {
                            onPress: (event, href) => {
                                console.log('Clicked link:', href);
                            }
                        }
                    }}
                />
            </View>
        </ScrollView>
    );
}

const styles = StyleSheet.create({
    container: {
        flex: 1,
        backgroundColor: '#fff',
    },
    contentContainer: {
        padding: 16,
    },
    image: {
        width: '100%',
        height: 200,
        marginBottom: 16,
    },
    title: {
        fontSize: 24,
        fontWeight: 'bold',
        marginBottom: 8,
        color: '#17214a',
    },
    meta: {
        fontSize: 14,
        color: '#555',
        marginBottom: 16,
    },
    loader: {
        flex: 1,
        justifyContent: 'center',
        alignItems: 'center',
    },
    errorContainer: {
        flex: 1,
        justifyContent: 'center',
        alignItems: 'center',
    },
    errorText: {
        fontSize: 18,
        color: 'red',
    },
});
