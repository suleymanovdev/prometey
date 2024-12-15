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
    } else {
        window.location.href = '/login';
    }

    const jwtToken = sessionStorage.getItem('jwtToken');
    const dynamicContent = document.getElementById('dynamic-content');
    const pendingContent = document.getElementById('pending-content');
    const backToDashboardBtn = document.createElement('button');
    backToDashboardBtn.id = 'back-to-dashboard';
    backToDashboardBtn.textContent = 'Back to Dashboard';
    backToDashboardBtn.classList.add('btn', 'btn-secondary');
    backToDashboardBtn.addEventListener('click', showDashboard);
    document.getElementById('admin-content').insertBefore(backToDashboardBtn, dynamicContent);

    document.getElementById('users-btn').addEventListener('click', () => {
        loadUsers(1);
    });

    document.getElementById('applications-btn').addEventListener('click', () => {
        loadApplications(1);
    });

    document.getElementById('posts-btn').addEventListener('click', () => {
        loadPosts(1);
    });

    function showDashboard() {
        backToDashboardBtn.style.display = 'none';
        pendingContent.style.display = 'none';
        fetchDashboardStatistics();
    }

    function fetchDashboardStatistics() {
        fetch('http://localhost:5205/api/Admin/get-statistics', {
            method: 'GET',
            headers: { 'Authorization': `Bearer ${jwtToken}` }
        })
        .then(response => response.json())
        .then(data => {
            renderDashboardChart(data);
        })
        .catch(error => console.error('Error fetching statistics:', error));
    }

    function loadUsers(page) {
        fetch(`http://localhost:5205/api/Admin/get-users?page=${page}`, {
            method: 'GET',
            headers: { 'Authorization': `Bearer ${jwtToken}` }
        })
        .then(response => response.json())
        .then(data => {
            dynamicContent.innerHTML = generateTableHTML(data.items, 'user');
            renderPaginationControls(data.totalPages, page, loadUsers);
            backToDashboardBtn.style.display = 'block';
        })
        .catch(error => console.error('Error fetching users:', error));
    }

    function loadApplications(page) {
        fetch(`http://localhost:5205/api/Admin/get-applications?page=${page}`, {
            method: 'GET',
            headers: { 'Authorization': `Bearer ${jwtToken}` }
        })
        .then(response => response.json())
        .then(data => {
            dynamicContent.innerHTML = generateTableHTML(data.items, 'application');
            pendingContent.innerHTML = generatePendingTableHTML(data.items.filter(item => !item.isVerified), 'application');
            renderPaginationControls(data.totalPages, page, loadApplications);
            backToDashboardBtn.style.display = 'block';
        })
        .catch(error => console.error('Error fetching applications:', error));
    }

    function loadPosts(page) {
        fetch(`http://localhost:5205/api/Admin/get-posts?page=${page}`, {
            method: 'GET',
            headers: { 'Authorization': `Bearer ${jwtToken}` }
        })
        .then(response => response.json())
        .then(data => {
            dynamicContent.innerHTML = generateTableHTML(data.items, 'post');
            renderPaginationControls(data.totalPages, page, loadPosts);
            backToDashboardBtn.style.display = 'block';
        })
        .catch(error => console.error('Error fetching posts:', error));
    }

    function generateTableHTML(data, type) {
        const keysToShow = {
            user: ['id', 'profilePhotoLink', 'name', 'surname', 'email', 'username', 'category', 'status', 'role', 'isVerified', 'registrationDate'],
            post: ['id', 'postPhotoLink', 'title', 'description', 'content', 'category', 'author', 'createdAt', 'isVerified'],
            application: ['id', 'applicationPhotoLink', 'name', 'description', 'category', 'author', 'content', 'created', 'isVerified']
        };

        let tableHTML = `<div class="table-container"><table class="table table-striped"><thead><tr>`;
        keysToShow[type].forEach(key => {
            tableHTML += `<th>${key}</th>`;
        });
        tableHTML += `<th>Actions</th></tr></thead><tbody>`;
        data.forEach(item => {
            tableHTML += `<tr data-type="${type}">`;
            keysToShow[type].forEach(key => {
                const value = item[key] !== undefined ? item[key] : '';
                if (key === 'profilePhotoLink' || key === 'postPhotoLink' || key === 'applicationPhotoLink') {
                    tableHTML += `<td><img src="${value}" class="img-thumbnail" style="max-width: 100px; max-height: 100px;"></td>`;
                } else if (key === 'content' && type === 'application') {
                    tableHTML += `<td><a href="${item.applicationFileUrl}">Download</a></td>`;
                } else if (key === 'content' && type === 'post') {
                    tableHTML += `<td class="truncate" title="${value}">${value}</td>`;
                } else if (key === 'category' || key === 'status' || key === 'role') {
                    tableHTML += `<td>${translateEnum(key, value)}</td>`;
                } else {
                    tableHTML += `<td class="truncate" title="${value}">${value}</td>`;
                }
            });

            tableHTML += `<td>
                <button class="btn btn-sm btn-danger">Delete</button>
            </td>`;
            tableHTML += `</tr>`;
        });
        tableHTML += `</tbody></table></div>`;
        return tableHTML;
    }

    function generatePendingTableHTML(data, type) {
        const keysToShow = {
            application: ['id', 'applicationPhotoLink', 'name', 'description', 'category', 'author', 'content', 'created']
        };

        let tableHTML = `<h3>Pending Applications</h3>`;
        tableHTML += `<div class="table-container"><table class="table table-striped"><thead><tr>`;
        keysToShow[type].forEach(key => {
            tableHTML += `<th>${key}</th>`;
        });
        tableHTML += `<th>Actions</th></tr></thead><tbody>`;
        data.forEach(item => {
            tableHTML += `<tr data-type="${type}">`;
            keysToShow[type].forEach(key => {
                const value = item[key] !== undefined ? item[key] : '';
                if (key === 'applicationPhotoLink') {
                    tableHTML += `<td><img src="${value}" class="img-thumbnail" style="max-width: 100px; max-height: 100px;"></td>`;
                } else if (key === 'content' && type === 'application') {
                    tableHTML += `<td><a href="${item.applicationFileUrl}">Download</a></td>`;
                } else if (key === 'content') {
                    tableHTML += `<td class="truncate" title="${value}">${value}</td>`;
                } else if (key === 'category' || key === 'status' || key === 'role') {
                    tableHTML += `<td>${translateEnum(key, value)}</td>`;
                } else {
                    tableHTML += `<td class="truncate" title="${value}">${value}</td>`;
                }
            });

            const approveFunction = 'approveApplication';
            tableHTML += `<td>
                <button class="btn btn-sm btn-primary" onclick="${approveFunction}('${item.id}')">Approve</button>
            </td>`;
            tableHTML += `</tr>`;
        });
        tableHTML += `</tbody></table></div>`;
        return tableHTML;
    }

    async function approveApplication(applicationId) {
        try {
            const response = await fetch(`http://localhost:5205/api/Admin/approve-application?applicationId=${applicationId}`, {
                method: 'POST',
                headers: {
                    'Authorization': `Bearer ${jwtToken}`,
                    'Content-Type': 'application/json'
                }
            });

            if (response.ok) {
                alert('Application approved successfully.');
                fetchDashboardStatistics();
                loadApplications(1);
            } else {
                const errorData = await response.json();
                alert(`Error approving application: ${errorData.error}`);
            }
        } catch (error) {
            console.error('Error approving application:', error);
            alert('Error approving application.');
        }
    }

    window.approveApplication = approveApplication;

    function renderDashboardChart(data) {
        const ctx = document.createElement('canvas');
        dynamicContent.innerHTML = '';
        dynamicContent.appendChild(ctx);

        new Chart(ctx, {
            type: 'bar',
            data: {
                labels: ['Users', 'Applications', 'Posts'],
                datasets: [{
                    label: '# of Entries',
                    data: [data.usersCount, data.applicationsCount, data.postsCount],
                    backgroundColor: [
                        'rgba(75, 192, 192, 0.2)',
                        'rgba(54, 162, 235, 0.2)',
                        'rgba(255, 206, 86, 0.2)',
                        'rgba(153, 102, 255, 0.2)',
                        'rgba(255, 159, 64, 0.2)'
                    ],
                    borderColor: [
                        'rgba(75, 192, 192, 1)',
                        'rgba(54, 162, 235, 1)',
                        'rgba(255, 206, 86, 1)',
                        'rgba(153, 102, 255, 1)',
                        'rgba(255, 159, 64, 1)'
                    ],
                    borderWidth: 1
                }]
            },
            options: {
                scales: {
                    y: {
                        beginAtZero: true
                    }
                }
            }
        });
    }

    function renderPaginationControls(totalPages, currentPage, loadFunction) {
        const paginationWrapper = document.createElement('div');
        paginationWrapper.className = 'pagination-wrapper';

        for (let i = 1; i <= totalPages; i++) {
            const pageButton = document.createElement('button');
            pageButton.textContent = i;
            pageButton.className = 'btn btn-secondary';
            pageButton.disabled = (i === currentPage);
            pageButton.addEventListener('click', () => loadFunction(i));
            paginationWrapper.appendChild(pageButton);
        }

        dynamicContent.appendChild(paginationWrapper);
    }

    function translateEnum(key, value) {
        const categories = {
            0: 'NONE',
            1: 'BackEndDevelopment',
            2: 'FrontEndDevelopment',
            3: 'DataScience',
            4: 'DevOps',
            5: 'Cybersecurity',
            6: 'Design',
            7: 'WithoutCategory'
        };
        const roles = {
            0: 'USER',
            1: 'ADMIN'
        };
        const statuses = {
            0: 'BASIC',
            1: 'PREMIUM'
        };
        switch (key) {
            case 'category':
                return categories[value];
            case 'role':
                return roles[value];
            case 'status':
                return statuses[value];
            default:
                return value;
        }
    }

    fetchDashboardStatistics();

    dynamicContent.addEventListener('click', function(event) {
        if (event.target.tagName === 'TD' && event.target.parentNode.childNodes.length > 1) {
            const rowData = Array.from(event.target.parentNode.children).map(td => td.textContent);
            showModal(rowData);
        }
    });

    function showModal(data) {
        const modalContent = document.getElementById('modal-content');
        modalContent.innerHTML = '';
        const table = document.createElement('table');
        table.className = 'table table-striped';
        data.forEach((item, index) => {
            if (item && item.trim() !== '' && !item.includes('Delete') && !item.includes('Download')) {
                const row = table.insertRow();
                const cellKey = row.insertCell(0);
                const cellValue = row.insertCell(1);
                cellValue.textContent = item;
            }
        });
        modalContent.appendChild(table);
        $('#myModal').modal('show');
    }

    document.getElementById('dynamic-content').addEventListener('click', function(event) {
        if (event.target.classList.contains('btn-danger')) {
            const row = event.target.closest('tr');
            const id = row.firstElementChild.textContent;
            const type = row.dataset.type;

            if (type === 'user') {
                deleteUser(id);
            } else if (type === 'post') {
                deletePost(id);
            } else if (type === 'application') {
                deleteApplication(id);
            }
        }
    });

    async function deleteUser(userId) {
        try {
            const response = await fetch(`http://localhost:5205/api/Admin/delete-user?userId=${userId}`, {
                method: 'DELETE',
                headers: {
                    'Authorization': `Bearer ${jwtToken}`,
                    'Content-Type': 'application/json'
                }
            });

            if (response.ok) {
                alert('User deleted successfully.');
                loadUsers(1);
            } else {
                const errorData = await response.json();
                alert(`Error deleting user: ${errorData.Error}`);
            }
        } catch (error) {
            console.error('Error deleting user:', error);
            alert('Error deleting user.');
        }
    }

    async function deletePost(postId) {
        try {
            const response = await fetch(`http://localhost:5205/api/Admin/delete-post?postId=${postId}`, {
                method: 'DELETE',
                headers: {
                    'Authorization': `Bearer ${jwtToken}`,
                    'Content-Type': 'application/json'
                }
            });

            if (response.ok) {
                alert('Post deleted successfully.');
                loadPosts(1);
            } else {
                const errorData = await response.json();
                alert(`Error deleting post: ${errorData.Error}`);
            }
        } catch (error) {
            console.error('Error deleting post:', error);
            alert('Error deleting post.');
        }
    }

    async function deleteApplication(applicationId) {
        try {
            const response = await fetch(`http://localhost:5205/api/Admin/delete-application?applicationId=${applicationId}`, {
                method: 'DELETE',
                headers: {
                    'Authorization': `Bearer ${jwtToken}`,
                    'Content-Type': 'application/json'
                }
            });

            if (response.ok) {
                alert('Application deleted successfully.');
                loadApplications(1);
            } else {
                const errorData = await response.json();
                alert(`Error deleting application: ${errorData.Error}`);
            }
        } catch (error) {
            console.error('Error deleting application:', error);
            alert('Error deleting application.');
        }
    }
});
