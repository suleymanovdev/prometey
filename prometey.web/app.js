const express = require('express');
const path = require('path');
const app = express();

app.use(express.static(path.join(__dirname, 'public')));
app.use(express.json());

app.get('/', (req, res) => {
    res.sendFile(path.join(__dirname, 'public', 'index.html'));
});

app.get('/user/:username', (req, res) => {
    res.sendFile(path.join(__dirname, 'public', '/user/index.html'));
});

app.get('/post/:postId', (req, res) => {
    res.sendFile(path.join(__dirname, 'public', '/post/index.html'));
});

app.get('/application/:applicationId', (req, res) => {
    res.sendFile(path.join(__dirname, 'public', '/application/index.html'));
});

app.get('/delete-post/:postId', (req, res) => {
    res.sendFile(path.join(__dirname, 'public', '/delete-post/index.html'));
});

app.get('/delete-application/:applicationId', (req, res) => {
    res.sendFile(path.join(__dirname, 'public', '/delete-application/index.html'));
});

app.get('/group/:domain', (req, res) => {
    res.sendFile(path.join(__dirname, 'public', '/group/index.html'));
});

app.get('/assets/*', (req, res) => {
    res.sendFile(path.join(__dirname, 'public', '/'));
});

app.use((req, res) => {
    res.status(404).sendFile(path.join(__dirname, 'public', '404.html'));
});

app.use((err, req, res, next) => {
    if (err.code === 'ENOENT') {
        res.status(404).sendFile(path.join(__dirname, 'public', '404.html'));
    } else {
        next(err);
    }
});

const port = 3000;
app.listen(port, () => {
    console.log(`Server is running at http://localhost:${port}`);
});
