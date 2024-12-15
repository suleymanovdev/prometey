document.addEventListener('contextmenu', function(e) {
    e.preventDefault();
});

document.addEventListener('keydown', function(e) {
    if (e.key === 'F12' || (e.ctrlKey && e.shiftKey && e.key === 'I')) {
        e.preventDefault();
    }
});  

document.addEventListener('DOMContentLoaded', function() {
    const loginDateStr = sessionStorage.getItem('loginDate');
    if (loginDateStr) {
        const loginDate = new Date(loginDateStr);
        const currentTime = new Date();
        const timeDiff = currentTime - loginDate;
        const timeDiffMinutes = timeDiff / (1000 * 60);

        if (timeDiffMinutes >= 30) {
            sessionStorage.clear();
            window.location.href = '/login';
        } else {
            const remainingTime = (30 - timeDiffMinutes) * 60 * 1000;
            setTimeout(function() {
                sessionStorage.clear();
                window.location.href = '/login';
            }, remainingTime);
        }

        if (!sessionStorage.getItem('jwtToken')) {
            sessionStorage.clear();
            window.location.href = '/';
        }

        const loginBtn = document.getElementById('login-btn');
        const signupBtn = document.getElementById('signup-btn');
        loginBtn.innerText = 'Logout';
        loginBtn.href = '#';
        signupBtn.innerText = 'Profile';
        signupBtn.href = '/profile';

        document.getElementById('profile-section').style.display = 'block';
        // document.getElementById('groups-section').style.display = 'block';

        loginBtn.addEventListener('click', function() {
            sessionStorage.clear();
            window.location.href = '/login';
        });

        fetch('http://localhost:5205/api/User/' + sessionStorage.getItem('userId'), {
            headers: {
                'Authorization': 'Bearer ' + sessionStorage.getItem('jwtToken')
            }
        })
        .then(response => {
            if (!response.ok) {
                sessionStorage.clear();
                window.location.href = '/';
                throw new Error('Network response was not ok ' + response.statusText);
            }
            return response.json();
        })
        .then(userData => {
            const profilePhoto = document.getElementById('photo');
            const name = document.getElementById('name');

            profilePhoto.src = userData.profilePhotoLink ? userData.profilePhotoLink : '../assets/images/profile-default-photo.png';
            name.innerText = sessionStorage.getItem('name');

            fetchPosts(1, userData);
            fetchApplications(1, userData);
            // fetchGroups();
        });
    } else {
        fetchPosts(1, null);
        fetchApplications(1, null);
    }

    const searchIcon = document.querySelector('.search-icon');
    const searchModal = document.getElementById('searchModal');
    const openSearchModal = document.getElementById('openSearchModal');
    const closeSearchModal = document.getElementById('closeSearchModal');
    const searchInput = document.getElementById('searchInput');
    const searchResults = document.getElementById('searchResults');

    openSearchModal.addEventListener('click', function() {
        searchModal.style.display = 'block';
        searchInput.value = '';
        searchResults.innerHTML = '';
        searchInput.focus();
    });

    closeSearchModal.addEventListener('click', function() {
        searchModal.style.display = 'none';
    });

    searchInput.addEventListener('input', async function() {
        const query = searchInput.value.trim();
        if (query.length < 1) {
            searchResults.innerHTML = '';
            return;
        }

        try {
            const response = await fetch('http://localhost:5205/api/General/search/' + query);
            const data = await response.json();

            searchResults.innerHTML = '';
            const posts = data.posts;
            const applications = data.applications;

            if (posts.length === 0 && applications.length === 0) {
                searchResults.innerHTML = '<p>No results found.</p>';
                return;
            }

            if (posts.length > 0) {
                const postResults = document.createElement('div');
                postResults.innerHTML = '<h6>Posts</h6>';
                posts.forEach(post => {
                    const postItem = document.createElement('div');
                    postItem.className = 'search-fix';
                    postItem.innerHTML = `
                        <a href="/post/${post.id}">
                            <h6>${post.title}</h6>
                        </a>
                        <hr>
                    `;
                    postResults.appendChild(postItem);
                });
                searchResults.appendChild(postResults);
            }

            if (applications.length > 0) {
                const appResults = document.createElement('div');
                appResults.innerHTML = '<h6>Applications</h6>';
                applications.forEach(app => {
                    const appItem = document.createElement('div');
                    appItem.className = 'search-fix';
                    appItem.innerHTML = `
                        <a href="/application/${app.id}">
                            <h6>${app.name}</h6>
                        </a>
                        <hr>
                    `;
                    appResults.appendChild(appItem);
                });
                searchResults.appendChild(appResults);
            }
        } catch (error) {
            searchResults.innerHTML = '<p>An error occurred while searching.</p>';
        }
    });
});

let currentPostPage = 1;
let currentApplicationPage = 1;

async function fetchPosts(page, userData) {
    const response = await fetch('http://localhost:5205/api/General/get-posts-pagination?page=' + page + '&pageSize=6');
    const data = await response.json();
    renderPosts(data.posts, userData);
    renderPagination(data.totalPages, page, 'posts');
}

async function fetchApplications(page, userData) {
    const response = await fetch('http://localhost:5205/api/General/get-applications-pagination?page=' + page + '&pageSize=1');
    const data = await response.json();
    renderApplications(data.applications, userData);
    renderPagination(data.totalPages, page, 'applications');
}

// async function fetchGroups() {
//     const response = await fetch('http://localhost:5205/api/User/' + sessionStorage.getItem('userId') + '/groups', {
//         headers: {
//             'Authorization': 'Bearer ' + sessionStorage.getItem('jwtToken')
//         }
//     });
//     const data = await response.json();
//     renderGroups(data);
// }

// function renderGroups(groups) {
//     const groupsOlList = document.getElementById('user-groups');
//     groupsOlList.innerHTML = '';
//     groups.forEach(group => {
//         const groupLi = document.createElement('li');
//         groupLi.innerHTML = `
//             <a href="/group/${group.domain}">${group.name}</a>
//         `;
//         groupsOlList.appendChild(groupLi);
//     });
// }

function renderPosts(posts, userData) {
    const postsContainer = document.getElementById('posts');
    postsContainer.innerHTML = '';
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
            <p class="blog-post-meta">${new Date(post.createdAt).toLocaleDateString()} Category: ${category} by <a href="/user/${post.authorUsername}">${post.author}</a></p>
            <p>${post.description}</p>
            <a href="/post/${post.id}">Read</a>
            <hr>
        </div>
        `;
        postsContainer.appendChild(postElement);
    });
}

function renderApplications(applications, userData) {
    const applicationsContainer = document.getElementById('applications');
    applicationsContainer.innerHTML = '';
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
                <p class="text-dark">${app.name}</p>
                </h3>
                <p class="blog-post-meta">${new Date(app.created).toLocaleDateString()} by <a href="/user/${app.authorUsername}">${app.author}</a></p>
                <p class="card-text mb-auto">${app.description}</p>
                <a href="/application/${app.id}" class="download-button">Download</a>
            </div>
            <img class="card-img-right flex-auto d-none d-md-block" style="border-radius: 15px;" src="${app.applicationPhotoLink}">
        `;
        applicationsContainer.appendChild(appElement);
    });
}

// function renderGroups(groups) {
//     const groupsOlList = document.getElementById('user-groups');
//     groupsOlList.innerHTML = '';
//     groups.forEach(group => {
//         const groupLi = document.createElement('li');
//         groupLi.innerHTML = `
//             <a href="/group/${group.domain}">${group.name}</a>
//         `;
//         groupsOlList.appendChild(groupLi);
//     });
// }

function renderPagination(totalPages, currentPage, type) {
    const prevBtn = document.getElementById('prev-' + type);
    const nextBtn = document.getElementById('next-' + type);

    prevBtn.classList.toggle('disabled', currentPage === 1);
    nextBtn.classList.toggle('disabled', currentPage === totalPages);

    prevBtn.onclick = () => {
        if (type === 'posts') {
            currentPostPage = currentPage - 1;
            fetchPosts(currentPostPage);
        } else {
            currentApplicationPage = currentPage - 1;
            fetchApplications(currentApplicationPage);
        }
    };
    nextBtn.onclick = () => {
        if (type === 'posts') {
            currentPostPage = currentPage + 1;
            fetchPosts(currentPostPage);
        } else {
            currentApplicationPage = currentPage + 1;
            fetchApplications(currentApplicationPage);
        }
    };
}