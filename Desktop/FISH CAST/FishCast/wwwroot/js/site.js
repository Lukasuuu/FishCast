// Fallback para imagens que falham ao carregar
document.addEventListener('DOMContentLoaded', () => {
    document.querySelectorAll('img').forEach(img => {
        img.addEventListener('error', function () {
            if (!this.src.includes('default-') && !this.src.includes('data:image')) {
                const isAvatar = this.classList.contains('avatar-sm') ||
                    this.classList.contains('avatar-md') ||
                    this.classList.contains('avatar-lg');
                this.src = isAvatar ? '/images/default-avatar.svg' : '/images/default-captura.svg';
            }
        });
    });
});
