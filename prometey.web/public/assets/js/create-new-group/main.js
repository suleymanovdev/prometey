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

    document.getElementById('group-logo').addEventListener('change', function(event) {
        const file = event.target.files[0];
        if (file) {
            const reader = new FileReader();
            reader.onload = function(e) {
                document.getElementById('preview').src = e.target.result;
            };
            reader.readAsDataURL(file);
        }
    });

    const createGroupForm = document.getElementById('create-group-form');
    const submitButton = document.getElementById('submit-button');
    const domainInput = document.getElementById('group-domain');

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

    domainInput.addEventListener('input', (event) => {
        let domainValue = domainInput.value;
        domainValue = domainValue.replace(/\s+/g, '-').toLowerCase();
        domainValue = domainValue.replace(/[^a-z0-9-]/g, '');
        domainInput.value = domainValue;
    });

    createGroupForm.addEventListener('submit', async (event) => {
        event.preventDefault();
        
        const logo = document.getElementById('group-logo').files[0];
        const name = document.getElementById('group-name').value;
        const domain = domainInput.value;
        const description = document.getElementById('group-description').value;
        
        if (!name || !domain || !description) {
            alert('All fields are required!');
            return;
        }

        showSpinner();
        
        const formData = {
            logo: '',
            domain,
            name,
            description
        };

        if (logo) {
            const reader = new FileReader();
            reader.onloadend = async () => {
                formData.logo = reader.result.split(',')[1];
                await handleCreateGroup(formData);
            };
            reader.readAsDataURL(logo);
        } else {
            await handleCreateGroup(formData);
        }
    });

    async function handleCreateGroup(formData) {
        try {
            const response = await fetch(`http://localhost:5205/api/User/${userId}/create-group`, {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                    'Authorization': `Bearer ${token}`
                },
                body: JSON.stringify(formData)
            });

            if (!response.ok) {
                hideSpinner();

                const errorText = await response.text();
                if (errorText.includes('Group with this domain already exists.')) {
                    alert('Group with this domain already exists!');
                } else if (errorText.includes('Request is null.')) {
                    alert('Request is null!');
                } else if (errorText.includes('Name is null or empty.')) {
                    alert('Name is null or empty!');
                }
                else if (errorText.includes('Description is null or empty.'))
                {
                    alert('Description is null or empty!');
                } else if (errorText.includes('Domain is null or empty.')) {
                    alert('Domain is null or empty!');  
                } else if (errorText.includes('Logo is null or empty.')) {
                    alert('Logo is null or empty!');
                } else {
                    alert('Failed to create group!');
                }
                return;
            }

            hideSpinner();
            window.location.href = '/profile';
        } catch (error) {
            hideSpinner();
            alert('An error occurred: ' + error.message);
        }
    }
});
