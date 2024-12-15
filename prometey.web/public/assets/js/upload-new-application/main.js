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

    document.getElementById('application-image').addEventListener('change', function(event) {
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
        selector: '#application-description',
        height: 500,
        plugins: 'image code',
        toolbar: 'undo redo | formatselect | bold italic backcolor | alignleft aligncenter alignright alignjustify | bullist numlist outdent indent | image | code',
        content_style: 'body { font-family:Helvetica,Arial,sans-serif; font-size:14px }'
    });

    const uploadApplicationForm = document.getElementById('upload-application-form');
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

    uploadApplicationForm.addEventListener('submit', async (event) => {
        event.preventDefault();

        const userId = sessionStorage.getItem('userId');
        const name = document.getElementById('application-name').value;
        const description = tinymce.get('application-description').getContent();
        const category = document.getElementById('application-category').value;
        const zipFile = document.getElementById('application-zip').files[0];
        const logoFile = document.getElementById('application-image').files[0];

        if (name.length > 200) {
            hideSpinner();
            alert('Name must be less than 200 characters!');
            return;
        }

        if (description.length > 2000) {
            hideSpinner();
            alert('Description must be less than 1000+ characters!');
            return;
        }

        showSpinner();

        if (!name || !description || !category || !zipFile || !logoFile) {
            hideSpinner();
            alert('All fields are required!');
            return;
        }

        const formData = new FormData();
        formData.append('userId', userId);
        formData.append('name', name);
        formData.append('description', description);
        formData.append('category', category);
        formData.append('file', zipFile);

        const reader = new FileReader();
        reader.onloadend = async () => {
            const base64Logo = reader.result.split(',')[1];
            formData.append('base64ApplicationPhoto', base64Logo);

            try {
                const response = await fetch(`http://localhost:5205/api/User/create-application`, {
                    headers: {
                        Authorization: `Bearer ${token}`
                    },
                    method: 'POST',
                    body: formData
                });

                hideSpinner();

                if (!response.ok) {
                    const contentType = response.headers.get('content-type');
                    if (contentType && contentType.includes('application/json')) {
                        const errorData = await response.json();
                        if (errorData.message === "Application not created. CREATE_APPLICATION_REQUEST_EXCEPTION: File is not a zip archive.") {
                            alert("File is not a zip archive.");
                        } else {
                            alert('There was a problem with the fetch operation');
                        }
                    } else {
                        const errorText = await response.text();
                        alert('File is not a zip archive. Check everything and try again.');
                        console.error('There was a problem with the fetch operation:', errorText);
                    }
                    return;
                }

                hideSpinner();
                location.href = '/profile';
            } catch (error) {
                hideSpinner();
                alert('There was an error with the fetch operation: ' + error.message);
            }
        };
        reader.readAsDataURL(logoFile);
    });
});
