document.addEventListener('DOMContentLoaded', function () {
    const modal = document.getElementById('confirmModal');
    if (!modal) return;

    const msg = document.getElementById('confirmMessage');
    const title = document.getElementById('confirmTitle');
    const hidden = document.getElementById('confirmHiddenInput');
    const form = document.getElementById('confirmForm');
    const cancel = document.getElementById('confirmCancel');

    function closeModal() {
        modal.classList.add('hidden');
        modal.classList.remove('modal-fade-in');
        const inner = modal.querySelector('.bg-white');
        if (inner) inner.classList.remove('modal-slide-in');
    }

    document.querySelectorAll('.confirm-btn').forEach(button => {
        button.addEventListener('click', function (e) {
            e.preventDefault();
            const itemId = this.getAttribute('data-item-id') || '';
            const itemName = this.getAttribute('data-item-name') || '';
            const url = this.getAttribute('data-url') || this.dataset.url || '';
            const hiddenName = this.getAttribute('data-hidden-name') || this.dataset.hiddenName || '';

            // Set modal content
            msg.textContent = itemName ? `Are you sure you want to remove "${itemName}" from your cart?` : 'Are you sure?';
            if (hiddenName) hidden.name = hiddenName;
            hidden.value = itemId || '';

            if (url) form.action = url;

            // Show modal with animation classes
            modal.classList.remove('hidden');
            modal.classList.add('modal-fade-in');
            const inner = modal.querySelector('.bg-white');
            if (inner) inner.classList.add('modal-slide-in');
        });
    });

    cancel.addEventListener('click', closeModal);

    modal.addEventListener('click', function (e) {
        if (e.target === modal) closeModal();
    });

    document.addEventListener('keydown', function (e) {
        if (e.key === 'Escape' && !modal.classList.contains('hidden')) closeModal();
    });
});