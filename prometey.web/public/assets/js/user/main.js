document.addEventListener('contextmenu', function(e) {
    e.preventDefault();
});

document.addEventListener('keydown', function(e) {
    if (e.key === 'F12' || (e.ctrlKey && e.shiftKey && e.key === 'I')) {
        e.preventDefault();
    }
});

window.addEventListener('DOMContentLoaded', async (event) => {
    const username = window.location.pathname.split('/').pop();

    const userId = await fetchUserData(username);
    console.log(userId);
    fetchPosts(userId, 1);
    fetchApplications(userId, 1);
});

let currentPostPage = 1;
let currentApplicationPage = 1;

async function fetchUserData(username) {
    try {
        const response = await fetch(`http://localhost:5205/api/General/get-user-details/${username}`);
        if (!response.ok) {
            throw new Error('Network response was not ok ' + response.statusText);
        }
        const userData = await response.json();

        const categories = [
            "Unknown", "Back-end Development", "Front-end Development",
            "Data Science", "DevOps", "Cybersecurity", "Design", "User"
        ];

        const categoryElement = document.getElementById('category');
        categoryElement.innerText = categories[userData.category] || "Unknown";

        const profilePhotoElement = document.getElementById('photo');
        const usernameElement = document.getElementById('username');
        const emailElement = document.getElementById('email');
        const nameElement = document.getElementById('name');
        const surnameElement = document.getElementById('surname');

        profilePhotoElement.src = userData.profilePhotoLink ? `${userData.profilePhotoLink}` : `../assets/images/profile-default-photo.png`;
        console.log(profilePhotoElement.src);
        usernameElement.innerText = userData.username;
        emailElement.innerText = userData.email;
        nameElement.innerText = userData.name;
        surnameElement.innerText = userData.surname;

        return userData.id;
    } catch (error) {
        console.error('There was a problem with the fetch operation:', error);
        window.location.href = '/';
    }
}

async function fetchPosts(userId, page) {
    try {
        const response = await fetch(`http://localhost:5205/api/General/get-posts-pagination-by-user-id/${userId}?page=${page}&pageSize=6`);
        if (!response.ok) {
            throw new Error('Network response was not ok ' + response.statusText);
        }
        const data = await response.json();
        renderPosts(data.posts);
        renderPagination(data.totalPages, page, 'posts');
    } catch (error) {
        console.error('There was a problem with the fetch operation:', error);
    }
}

async function fetchApplications(userId, page) {
    try {
        const response = await fetch(`http://localhost:5205/api/General/get-applications-pagination-by-user-id/${userId}?page=${page}&pageSize=1`);
        if (!response.ok) {
            throw new Error('Network response was not ok ' + response.statusText);
        }
        const data = await response.json();
        renderApplications(data.applications);
        renderPagination(data.totalPages, page, 'applications');
    } catch (error) {
        console.error('There was a problem with the fetch operation:', error);
    }
}

function renderPosts(posts) {
    const postsContainer = document.getElementById('posts');
    postsContainer.innerHTML = '';
    if (posts && posts.length > 0) {
        posts.forEach(post => {
            const categories = [
                "Unknown", "Back-end Development", "Front-end Development",
                "Data Science", "DevOps", "Cybersecurity", "Design", "User"
            ];

            const category = categories[post.category] || "Unknown";

            const postElement = document.createElement('article');
            postElement.innerHTML = `
            <div class="post-fix">
                <h2 class="blog-post-title">${post.title}</h2>
                <p class="blog-post-meta">${new Date(post.createdAt).toLocaleDateString()} Category: ${category}</p>
                <p>${post.description}</p>
                <a href="/post/${post.id}">Read</a>
                <hr>
            </div>
            `;
            postsContainer.appendChild(postElement);
        });
    } else {
        postsContainer.innerHTML = '<p>No posts available.</p>';
    }
}

function renderApplications(applications) {
    const applicationsContainer = document.getElementById('applications');
    applicationsContainer.innerHTML = '';
    if (applications && applications.length > 0) {
        applications.forEach(app => {
            const categories = [
                "Unknown", "Back-end Development", "Front-end Development",
                "Data Science", "DevOps", "Cybersecurity", "Design", "User"
            ];

            const category = categories[app.category] || "Unknown";

            const appElement = document.createElement('div');
            appElement.className = 'card flex-md-row mb-4 box-shadow h-md-250';
            appElement.innerHTML = `
                <div class="card-body card-fix d-flex flex-column align-items-start">
                    <strong class="d-inline-block mb-2 text-primary">${category}</strong>
                    <h3 class="mb-0">
                    <a class="text-dark" href="#">${app.name}</a>
                    </h3>
                    <p class="blog-post-meta">${new Date(app.created).toLocaleDateString()}</p>
                    <p class="card-text mb-auto">${app.description}</p>
                    <a href="${app.applicationFileUrl}">Download</a>
                </div>
                <img class="card-img-right flex-auto d-none d-md-block" style="border-radius: 15px;" src="${app.applicationPhotoLink}">
            `;
            applicationsContainer.appendChild(appElement);
        });
    } else {
        applicationsContainer.innerHTML = '<p>No applications available.</p>';
    }
}

function renderPagination(totalPages, currentPage, type) {
    const prevBtn = document.getElementById(`prev-${type}`);
    const nextBtn = document.getElementById(`next-${type}`);

    prevBtn.classList.toggle('disabled', currentPage === 1);
    nextBtn.classList.toggle('disabled', currentPage === totalPages);

    prevBtn.onclick = () => {
        if (type === 'posts') {
            currentPostPage = currentPage - 1;
            fetchPosts(window.location.pathname.split('/').pop(), currentPostPage);
        } else {
            currentApplicationPage = currentPage - 1;
            fetchApplications(currentApplicationPage);
        }
    };
    nextBtn.onclick = () => {
        if (type === 'posts') {
            currentPostPage = currentPage + 1;
            fetchPosts(window.location.pathname.split('/').pop(), currentPostPage);
        } else {
            currentApplicationPage = currentPage + 1;
            fetchApplications(currentApplicationPage);
        }
    };
}