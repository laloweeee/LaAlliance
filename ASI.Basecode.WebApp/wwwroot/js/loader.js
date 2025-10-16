function initFormLoader(formId, buttonId, buttonTextId, buttonLoaderId) {
    const form = document.getElementById(formId);

    if (!form) return;

    form.addEventListener('submit', function (e) {
        const button = document.getElementById(buttonId);
        const buttonText = document.getElementById(buttonTextId);
        const buttonLoader = document.getElementById(buttonLoaderId);

        if (button && buttonText && buttonLoader) {
            button.disabled = true;
            buttonText.classList.add('hidden');
            buttonLoader.classList.remove('hidden');
        }
    });
}