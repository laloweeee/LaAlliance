function showToast() {
    const toast = document.querySelector('.animate-fade-in');
    
    if (toast) {
        toast.style.opacity = '0';
        toast.style.transform = 'translateY(-10px)';
        setTimeout(() => toast.remove(), 300);
    }
}