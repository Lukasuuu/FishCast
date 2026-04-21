// FishCast - JavaScript Utilities
// Este ficheiro contém apenas funcionalidades auxiliares e handlers globais

// ===================== TOAST =====================
function showToast(msg) {
    const t = document.getElementById('toast');
    if (!t) return;
    const toastMsg = document.getElementById('toastMsg');
    if (toastMsg) toastMsg.textContent = msg;
    t.classList.add('show');
    setTimeout(() => t.classList.remove('show'), 3000);
}

// ===================== MODAL =====================
function openModal(id) {
    const modal = document.getElementById('modal-' + id);
    if (modal) {
        modal.classList.add('show');
        document.body.style.overflow = 'hidden';
    }
}

function closeModal(id) {
    const modal = document.getElementById('modal-' + id);
    if (modal) {
        modal.classList.remove('show');
        document.body.style.overflow = '';
    }
}

// Close modals with ESC
document.addEventListener('keydown', e => {
    if (e.key === 'Escape') {
        document.querySelectorAll('.modal-fc.show').forEach(m => {
            m.classList.remove('show');
            document.body.style.overflow = '';
        });
    }
});

// ===================== IMAGE ERROR HANDLER =====================
// Handler global para imagens que falham ao carregar
document.addEventListener('DOMContentLoaded', () => {
    document.querySelectorAll('img').forEach(img => {
        img.addEventListener('error', function () {
            // Só substitui se ainda não é a imagem padrão
            if (!this.src.includes('default-') && !this.src.includes('data:image')) {
                const isAvatar = this.classList.contains('avatar-sm') ||
                    this.classList.contains('avatar-md') ||
                    this.classList.contains('avatar-lg');
                this.src = isAvatar ? '/images/default-avatar.svg' : '/images/default-captura.svg';
            }
        });
    });
});

// ===================== CONFIRMAÇÕES =====================
function confirmDelete(message) {
    return confirm(message || 'Tem certeza que deseja eliminar?');
}

// ===================== UTILIDADES =====================
// Formatadores
const formatters = {
    // Formata data para PT
    date: (dateStr) => {
        if (!dateStr) return '';
        const date = new Date(dateStr);
        return date.toLocaleDateString('pt-PT', {
            day: '2-digit',
            month: '2-digit',
            year: 'numeric'
        });
    },

    // Formata data e hora para PT
    datetime: (dateStr) => {
        if (!dateStr) return '';
        const date = new Date(dateStr);
        return date.toLocaleString('pt-PT', {
            day: '2-digit',
            month: '2-digit',
            year: 'numeric',
            hour: '2-digit',
            minute: '2-digit'
        });
    },

    // Formata peso
    peso: (valor) => {
        if (valor === null || valor === undefined) return '-';
        return parseFloat(valor).toFixed(2) + ' kg';
    }
};

// Expor formatadores globalmente
window.FishCast = { formatters, showToast, openModal, closeModal, confirmDelete };
