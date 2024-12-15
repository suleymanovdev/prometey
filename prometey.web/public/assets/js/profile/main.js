document.addEventListener('contextmenu', function(e) {
    e.preventDefault();
});

document.addEventListener('keydown', function(e) {
    if (e.key === 'F12' || (e.ctrlKey && e.shiftKey && e.key === 'I')) {
        e.preventDefault();
    }
});  

document.addEventListener('DOMContentLoaded', (event) => {
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
    }

    const token = sessionStorage.getItem('jwtToken');
    const userId = sessionStorage.getItem('userId');

    if (!token || !userId) {
        window.location.href = '/';
        return;
    }

    fetch(`http://localhost:5205/api/User/${userId}`, {
        headers: {
            'Authorization': `Bearer ${token}`
        }
    })
    .then(response => {
        if (!response.ok) {
            throw new Error('Network response was not ok ' + response.statusText);
        }
        return response.json();
    })
    .then(userData => {
        const categoryElement = document.getElementById('category');
        const categories = [
            "Unknown", "Back-end Development", "Front-end Development",
            "Data Science", "DevOps", "Cybersecurity", "Design", "User"
        ];
        categoryElement.innerText = categories[userData.category] || "Unknown";

        const profilePhotoElement = document.getElementById('photo');
        const usernameElement = document.getElementById('username');
        const emailElement = document.getElementById('email');
        const nameElement = document.getElementById('name');
        const surnameElement = document.getElementById('surname');

        sessionStorage.setItem('name', userData.name + ' ' + userData.surname);

        profilePhotoElement.src = userData.profilePhotoLink ? `${userData.profilePhotoLink}` : `../assets/images/profile-default-photo.png`;
        usernameElement.innerText = userData.username;
        emailElement.innerText = userData.email;
        nameElement.innerText = userData.name;
        surnameElement.innerText = userData.surname;

        fetchPosts(1);
        fetchApplications(1);
        // fetchGroups();
    })
    .catch(error => {
        console.error('There was a problem with the fetch operation:', error);
        window.location.href = '/';
    });
});

document.getElementById('editProfileForm').addEventListener('submit', async function(event) {
    event.preventDefault();
    showSpinner();

    const token = sessionStorage.getItem('jwtToken');
    const userId = sessionStorage.getItem('userId');

    const formData = new FormData(event.target);
    const base64ProfilePhoto = formData.get('profilePhoto');

    const data = {
        base64ProfilePhoto: '',
        name: formData.get('name'),
        surname: formData.get('surname'),
        email: formData.get('email'),
        username: formData.get('username'),
        category: parseInt(formData.get('category'), 10),
    };

    if (base64ProfilePhoto && base64ProfilePhoto.size > 0) {
        const reader = new FileReader();
        reader.onloadend = async () => {
            data.base64ProfilePhoto = reader.result.split(',')[1];
            await updateProfile(data, token, userId);
        };
        reader.readAsDataURL(base64ProfilePhoto);
    } else {
        await updateProfile(data, token, userId);
    }
});

document.getElementById('deleteUserForm').addEventListener('submit', function(event) {
    event.preventDefault();

    const userId = sessionStorage.getItem('userId');
    const token = sessionStorage.getItem('jwtToken');

    fetch(`http://localhost:5205/api/User/${userId}`, {
        method: 'DELETE',
        headers: {
            'Authorization': `Bearer ${token}`,
            'Content-Type': 'application/json'
        }
    })

    
});

function showEditProfileModal() {
    const userId = sessionStorage.getItem('userId');
    const token = sessionStorage.getItem('jwtToken');
    
    fetch(`http://localhost:5205/api/User/${userId}`, {
        headers: {
            'Authorization': `Bearer ${token}`
        }
    })
    .then(response => {
        if (!response.ok) {
            throw new Error('Network response was not ok ' + response.statusText);
        }
        return response.json();
    })
    .then(userData => {
        document.getElementById('editName').value = userData.name;
        document.getElementById('editSurname').value = userData.surname;
        document.getElementById('editEmail').value = userData.email;
        document.getElementById('editUsername').value = userData.username;
        document.getElementById('editCategory').value = userData.category;
        
        $('#editProfileModal').modal('show');
    })
    .catch(error => {
        console.error('There was a problem with the fetch operation:', error);
    });
}

function deleteProfile() {
    $('#deleteUser').modal('show');
}

async function updateProfile(data, token, userId) {
    try {
        const response = await fetch(`http://localhost:5205/api/User/${userId}`, {
            method: 'PUT',
            headers: {
                'Authorization': `Bearer ${token}`,
                'Content-Type': 'application/json'
            },
            body: JSON.stringify(data)
        });

        if (!response.ok) {
            const errorText = await response.text();
            console.error('Error response text:', errorText);
            throw new Error('Network response was not ok ' + response.statusText);
        }

        const contentType = response.headers.get('content-type');
        let result;
        if (contentType && contentType.indexOf('application/json') !== -1) {
            result = await response.json();
        } else {
            result = await response.text();
        }

        hideSpinner();
        $('#editProfileModal').modal('hide');
        location.reload();
    } catch (error) {
        console.error('There was a problem with the fetch operation:', error);
        hideSpinner();
    }
}

function showSpinner() {
    const submitButton = document.getElementById('submitButton');
    const spinner = document.createElement('div');
    spinner.classList.add('spinner-border', 'text-primary');
    spinner.setAttribute('role', 'status');
    const spinnerText = document.createElement('span');
    spinnerText.classList.add('sr-only');
    spinnerText.textContent = 'Loading...';
    spinner.appendChild(spinnerText);
    spinner.id = 'loading-spinner';
    submitButton.replaceWith(spinner);
}

function hideSpinner() {
    const spinner = document.getElementById('loading-spinner');
    if (spinner) {
        spinner.replaceWith(document.getElementById('submitButton'));
    }
}

function createNewPost() {
    var newPostWindow = window.open("../create-new-post/index.html", "", "width=1000,height=1140");
    newPostWindow.addEventListener('beforeunload', function() {
        location.reload();
    });
}

function uploadNewApplication() {
    var newApplicationWindow = window.open("../upload-new-application/index.html", "", "width=1000,height=1230");
    newApplicationWindow.addEventListener('beforeunload', function() {
        location.reload();
    });
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

async function fetchPosts(page) {
    try {
        const response = await fetch(`http://localhost:5205/api/User/${sessionStorage.getItem('userId')}/posts?page=${page}&pageSize=6`, {
            headers: { 'Authorization': `Bearer ${sessionStorage.getItem('jwtToken')}` }
        });
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

async function fetchApplications(page) {
    try {
        const response = await fetch(`http://localhost:5205/api/User/${sessionStorage.getItem('userId')}/applications?page=${page}&pageSize=1`, {
            headers: { 'Authorization': `Bearer ${sessionStorage.getItem('jwtToken')}` }
        });
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
                <a href="/delete-post/${post.id}">Delete</a>
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
                    <p class="text-dark">${app.name}</p>
                    </h3>
                    <p class="blog-post-meta">${new Date(app.created).toLocaleDateString()}</p>
                    <p class="card-text mb-auto">${app.description}</p>
                    <a href="${app.applicationFileUrl}">Download</a>
                    <a href="/delete-application/${app.id}">Delete</a>
                    ${!app.isVerified ? '<span class="badge badge-warning">Under Review (it takes max 1 day)</span>' : ''}
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