document.addEventListener('contextmenu', function(e) {
    e.preventDefault();
});

document.addEventListener('keydown', function(e) {
    if (e.key === 'F12' || (e.ctrlKey && e.shiftKey && e.key === 'I')) {
        e.preventDefault();
    }
});  

const loginForm = document.getElementById('login-form');
const loginSubmitButton = document.getElementById('submit-button');

function showLoginSpinner() {
    const spinner = document.createElement('div');
    spinner.classList.add('spinner-border', 'text-primary');
    spinner.setAttribute('role', 'status');

    const spinnerText = document.createElement('span');
    spinnerText.classList.add('sr-only');
    spinnerText.textContent = 'Loading...';

    spinner.appendChild(spinnerText);
    spinner.id = 'login-loading-spinner';
    loginSubmitButton.replaceWith(spinner);
}

function hideLoginSpinner() {
    const spinner = document.getElementById('login-loading-spinner');
    if (spinner) {
        spinner.replaceWith(loginSubmitButton);
    }
}

loginForm.addEventListener('submit', async (event) => {
    event.preventDefault();

    const email = document.getElementById('login-email').value;
    const password = document.getElementById('login-password').value;

    if (email === '' || password === '') {
        alert('All fields are required!');
        return;
    }

    const formData = {
        email: email,
        password: password,
    };

    showLoginSpinner();

    try {
        const response = await fetch('http://localhost:5205/api/Auth/login', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
            },
            body: JSON.stringify(formData),
        });

        if (!response.ok) {
            if (response.error === 'LOGIN_REQUEST_EXCEPTION: Email not verified. Register again. Please check the email or password and try again.')
            {
                alert('Email not verified. Register again.');
            }
            else
            {
                alert('Login failed. Please try again.');
            }

            hideLoginSpinner();
        } else {
            const data = await response.json();
            const token = data.token;
            const userId = data.userId;

            if (userId === "ADMIN")
            {
                sessionStorage.setItem('jwtToken', token);
                sessionStorage.setItem('userId', userId);
                sessionStorage.setItem('loginDate', new Date().toISOString());
                window.location.href = '/admin';
                return;
            }

            sessionStorage.setItem('jwtToken', token);
            sessionStorage.setItem('userId', userId);
            sessionStorage.setItem('loginDate', new Date().toISOString());

            window.location.href = '/profile';
        }
    } catch (error) {
        console.error('Error:', error);
        alert('Login failed!');
        hideLoginSpinner();
    }
});
