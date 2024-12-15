document.addEventListener('contextmenu', function(e) {
    e.preventDefault();
});

document.addEventListener('keydown', function(e) {
    if (e.key === 'F12' || (e.ctrlKey && e.shiftKey && e.key === 'I')) {
        e.preventDefault();
    }
});

document.addEventListener('DOMContentLoaded', (event) => {
    // const loginDateStr = sessionStorage.getItem('loginDate');
    // if (loginDateStr) {
    //     const loginDate = new Date(loginDateStr);
    //     const currentTime = new Date();
    //     const timeDiff = currentTime - loginDate;
    //     const timeDiffMinutes = timeDiff / (1000 * 60);

    //     if (timeDiffMinutes >= 30) {
    //         sessionStorage.clear();
    //         window.location.href = '/login';
    //     } else {
    //         const remainingTime = (30 - timeDiffMinutes) * 60 * 1000;
    //         setTimeout(function() {
    //             sessionStorage.clear();
    //             window.location.href = '/login';
    //         }, remainingTime);
    //     }
    // }

    // const token = sessionStorage.getItem('jwtToken');
    // const userId = sessionStorage.getItem('userId');

    // if (!token || !userId) {
        window.location.href = '/';
        // return;
    // }

    
});