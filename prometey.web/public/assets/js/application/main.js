document.addEventListener('contextmenu', function(e) {
    e.preventDefault();
});

document.addEventListener('keydown', function(e) {
    if (e.key === 'F12' || (e.ctrlKey && e.shiftKey && e.key === 'I')) {
        e.preventDefault();
    }
});  

window.addEventListener('DOMContentLoaded', (event) => {
    const applicationId = window.location.pathname.split('/').pop();

    const application = document.getElementById('application');

    fetch(`http://localhost:5205/api/General/get-application-by-id/${applicationId}`)
        .then(response => {
            if (!response.ok) {
                throw new Error('Network response was not ok ' + response.statusText);
            }
            return response.json();
        })
        .then(applicationData => {
            const categories = [
                "Unknown", "Back-end Development", "Front-end Development",
                "Data Science", "DevOps", "Cybersecurity", "Design", "User"
            ];
            const applicationCategory = categories[applicationData.category] || "Unknown";
            const applicationElement = document.createElement('article');
            const imageUrl = applicationData.applicationPhotoLink ? `${applicationData.applicationPhotoLink}` : '../assets/images/prometey.png';
            applicationElement.innerHTML = `
            <div class="card-fix">
                <img src="${imageUrl}" alt="Image preview" class="side-image">
                <h2 class="blog-post-title">${applicationData.name}</h2>
                <p class="blog-post-meta">${new Date(applicationData.created).toLocaleDateString()} Category: ${applicationCategory} by <a href="/user/${applicationData.authorUsername}">${applicationData.author}</a></p>
                <p>${applicationData.description}</p>
                <hr>
                <a href="${applicationData.applicationFileUrl}">Download</a>
            </div>
            `;
            application.appendChild(applicationElement);
        })
        .catch(error => {
            console.error('There was a problem with the fetch operation:', error);
        });
});