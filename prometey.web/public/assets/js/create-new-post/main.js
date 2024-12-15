document.addEventListener('contextmenu', function(e) {
    e.preventDefault();
});

document.addEventListener('keydown', function(e) {
    if (e.key === 'F12' || (e.ctrlKey && e.shiftKey && e.key === 'I')) {
        e.preventDefault();
    }
});  

window.addEventListener('DOMContentLoaded', (event) => {
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

    document.getElementById('post-image').addEventListener('change', function(event) {
        const file = event.target.files[0];
        if (file) {
            const reader = new FileReader();
            reader.onload = function(e) {
                document.getElementById('preview').src = e.target.result;
            };
            reader.readAsDataURL(file);
        }
    });

    tinymce.init({
        selector: '#post-content',
        height: 500,
        plugins: 'image code',
        toolbar: 'undo redo | formatselect | bold italic backcolor | alignleft aligncenter alignright alignjustify | bullist numlist outdent indent | image | code',
        content_style: 'body { font-family:Helvetica,Arial,sans-serif; font-size:14px }'
    });

    const createPostForm = document.getElementById('create-post-form');
    const submitButton = document.getElementById('submit-button');

    function showSpinner() {
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
            spinner.replaceWith(submitButton);
        }
    }

    createPostForm.addEventListener('submit', async (event) => {
        event.preventDefault();
        const userId = sessionStorage.getItem('userId');
        const title = document.getElementById('post-title').value;
        const description = document.getElementById('post-description').value;
        const content = document.getElementById('post-content').value;
        const category = document.getElementById('post-category').value;
        const base64PostPhoto = document.getElementById('post-image').files[0];

        const formData = {
            base64PostPhoto: '',
            title,
            description,
            content,
            category: parseInt(category, 10),
        };
        
        if (title.length > 100) {
            alert('Title must be 100 characters or less.');
            return;
        }

        if (description.length > 500) {
            alert('Description must be 500 characters or less.');
            return;
        }

        if (content.length > 50000) {
            alert('Content must be 50000 characters or less.');
            return;
        }

        showSpinner();

        if (!title || !description || !content) {
            hideSpinner();
            alert('All fields are required!');
            return;
        }

        if (base64PostPhoto) {
            const reader = new FileReader();
            reader.onloadend = async () => {
                formData.base64PostPhoto = reader.result.split(',')[1];
                await handleCreatePost(formData);
            };
            reader.readAsDataURL(base64PostPhoto);
        } else {
            await handleCreatePost(formData);
        }
    });

    async function handleCreatePost(formData) {
        const response = await fetch(`http://localhost:5205/api/User/${userId}/create-post`, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
                'Authorization': `Bearer ${token}`
            },
            body: JSON.stringify(formData)
        });

        if (!response.ok) {
            hideSpinner();
            alert('Post must have Image, also post must be 5000 characters or less. Please try again.');
            // res message
            console.log(response.json());
            return;
        }

        hideSpinner();
        location.href = '/profile';
    }
});
