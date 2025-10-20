(function () {
    document.addEventListener('DOMContentLoaded', function () {
        initLoadingButtons();
    });

    function initLoadingButtons() {
        // Find all buttons with btn-text and btn-loading
        const buttons = document.querySelectorAll('button:has(.btn-text):has(.btn-loading)');

        buttons.forEach(button => {
            // For submit buttons inside forms, handle on form submit
            if (button.type === 'submit' && button.closest('form')) {
                const form = button.closest('form');
                form.addEventListener('submit', function (e) {
                    const isValid = form.checkValidity();
                    if (!isValid) {
                        button.querySelector('.btn-text').classList.remove('hidden');
                        button.querySelector('.btn-loading').classList.add('hidden');
                        button.disabled = false;
                        return;
                    }
                    button.querySelector('.btn-text').classList.add('hidden');
                    button.querySelector('.btn-loading').classList.remove('hidden');
                    button.disabled = true;
                });
            } else {
                // For non-submit buttons, handle on click if they have .btn-loading-trigger
                if (button.classList.contains('btn-loading-trigger')) {
                    button.addEventListener('click', function () {
                        button.querySelector('.btn-text').classList.add('hidden');
                        button.querySelector('.btn-loading').classList.remove('hidden');
                        button.disabled = true;
                    });
                }
            }
        });
    }
})();