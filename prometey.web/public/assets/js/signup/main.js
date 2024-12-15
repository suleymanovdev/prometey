document.addEventListener('contextmenu', function(e) {
    e.preventDefault();
});

document.addEventListener('keydown', function(e) {
    if (e.key === 'F12' || (e.ctrlKey && e.shiftKey && e.key === 'I')) {
        e.preventDefault();
    }
});  

const signupForm = document.getElementById('signup-form');
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

signupForm.addEventListener('submit', async (event) => {
    event.preventDefault();

    const base64ProfilePhoto = document.getElementById('formFile').files[0];
    const email = document.getElementById('signup-email').value;
    const password = document.getElementById('signup-password').value;
    const confirmPassword = document.getElementById('signup-password-repeat').value;
    const name = document.getElementById('signup-name').value;
    const surname = document.getElementById('signup-surname').value;
    const username = document.getElementById('signup-username').value;
    const category = document.getElementById('signup-category').value;

    if (!base64ProfilePhoto) {
        alert('Profile photo is required!');
        return;
    }

    if (email === '' || password === '' || confirmPassword === '' || name === '' || surname === '' || username === '' || category === '') {
        alert('All fields are required!');
        return;
    }

    if (password !== confirmPassword) {
        alert('Passwords do not match!');
        return;
    }

    const formData = {
        base64ProfilePhoto: '',
        email,
        password,
        confirmPassword,
        name,
        surname,
        username,
        category: parseInt(category, 10),
    };

    showSpinner();

    if (base64ProfilePhoto) {
        const reader = new FileReader();
        reader.onloadend = async () => {
            formData.base64ProfilePhoto = reader.result.split(',')[1];
            await handleSignup(formData);
        };
        reader.readAsDataURL(base64ProfilePhoto);
    } else {
        await handleSignup(formData);
    }
});

async function handleSignup(formData) {
    try {
        const response = await fetch('http://localhost:5205/api/Auth/signup', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
            },
            body: JSON.stringify(formData),
        });

        if (!response.ok) {
            const errorText = await response.text();
            console.error('Error response text:', errorText);
            if (errorText.includes('SIGNUP_REQUEST_EXCEPTION: User with this email or username already exists.')) {
                alert('User with this email or username already exists. Please use a different email or username.');
            } else {
                throw new Error(`Error: ${response.statusText}, Details: ${errorText}`);
            }
            hideSpinner();
        } else {
            const data = await response.json();
            console.log('Success:', data);
            window.location.href = '/verify';
        }
    } catch (error) {
        console.error('Error:', error);
        alert('Signup failed! Please try again. Check your password. It should be at least 6 characters long and contain at least one uppercase letter, one lowercase letter, one number, and one special character.');
        hideSpinner();
    }
}