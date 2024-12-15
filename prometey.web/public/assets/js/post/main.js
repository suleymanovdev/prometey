document.addEventListener('contextmenu', function(e) {
    e.preventDefault();
});

document.addEventListener('keydown', function(e) {
    if (e.key === 'F12' || (e.ctrlKey && e.shiftKey && e.key === 'I')) {
        e.preventDefault();
    }
});  

window.addEventListener('DOMContentLoaded', (event) => {
    const postId = window.location.pathname.split('/').pop();
    const post = document.getElementById('post');

    fetch(`http://localhost:5205/api/General/get-post-by-id/${postId}`)
        .then(response => {
            if (!response.ok) {
                throw new Error('Network response was not ok ' + response.statusText);
            }
            return response.json();
        })
        .then(postData => {
            const categories = [
                "Unknown", "Back-end Development", "Front-end Development",
                "Data Science", "DevOps", "Cybersecurity", "Design", "User"
            ];
            const postCategory = categories[postData.category] || "Unknown";

            const postElement = document.createElement('article');
            const imageUrl = postData.postPhotoLink ? `${postData.postPhotoLink}` : '../assets/images/prometey.png';
            postElement.innerHTML = `
            <div class="post-fix" id="postContent">
                <img src="${imageUrl}" alt="Image preview" class="side-image">
                <h2 class="blog-post-title">${postData.title}</h2>
                <p class="blog-post-meta">${new Date(postData.createdAt).toLocaleDateString()} Category: ${postCategory} by <a href="/user/${postData.authorUsername}">${postData.author}</a></p>
                <div>${postData.content}</div>
                <hr>
            </div>
            `;
            post.appendChild(postElement);

            document.getElementById('printButton').addEventListener('click', function () {
                const { jsPDF } = window.jspdf;

                html2canvas(document.getElementById('postContent')).then(canvas => {
                    const doc = new jsPDF('p', 'mm', 'a4');

                    const imgData = canvas.toDataURL('image/png');
                    const imgWidth = 210;
                    const imgHeight = (canvas.height * imgWidth) / canvas.width;

                    doc.addImage(imgData, 'PNG', 0, 0, imgWidth, imgHeight);
                    doc.save('article.pdf');
                });
            });
        })
        .catch(error => {
            console.error('There was a problem with the fetch operation:', error);
            window.location.href = '/';
        });
});