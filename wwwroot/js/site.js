/* ═══════════════════════════════════════════════════
   LOGISTICS COMMAND CENTER — CLIENT JAVASCRIPT
   ═══════════════════════════════════════════════════ */

// ── MODAL ──────────────────────────────────────────────

//function openModal() {
//    document.getElementById('modal-overlay').classList.remove('hidden');
//    document.body.style.overflow = 'hidden';
//}

//function closeModal() {
//    document.getElementById('modal-overlay').classList.add('hidden');
//    document.getElementById('modal-content').innerHTML = '';
//    document.body.style.overflow = '';
//}

//// Close on Escape key
//document.addEventListener('keydown', function (e) {
//    if (e.key === 'Escape') closeModal();
//});

// ── BULK SELECTION ─────────────────────────────────────

function toggleSelectAll(masterCb) {
    document.querySelectorAll('.row-checkbox').forEach(cb => {
        cb.checked = masterCb.checked;
    });
    updateBulkBar();
}

function updateBulkBar() {
    console.log("inside bulk bar");
    const checked = document.querySelectorAll('.row-checkbox:checked');
    const bar = document.getElementById('bulk-bar');
    const counter = document.getElementById('selected-count');

    if (checked.length > 0) {
        bar.style.display = 'flex';
        counter.textContent = `${checked.length} selected`;
    } else {
        bar.style.display = 'none';
    }

    // Update master checkbox indeterminate state
    const all = document.querySelectorAll('.row-checkbox');
    const masterCb = document.getElementById('select-all');
    if (masterCb) {
        masterCb.checked = checked.length === all.length && all.length > 0;
        masterCb.indeterminate = checked.length > 0 && checked.length < all.length;
    }
}

function clearSelection() {
    document.querySelectorAll('.row-checkbox').forEach(cb => cb.checked = false);
    const masterCb = document.getElementById('select-all');
    if (masterCb) { masterCb.checked = false; masterCb.indeterminate = false; }
    updateBulkBar();
}

// ── DELETE ANTI-FORGERY ────────────────────────────────

//function addAntiForgeryToDelete(btn) {
//    const token = document.querySelector('input[name="__RequestVerificationToken"]');
//    if (token) {
//        btn.setAttribute('hx-headers',
//            JSON.stringify({ 'RequestVerificationToken': token.value }));
//        htmx.process(btn); // re-process so HTMX picks up the new header
//    }
//}

//// ── TOAST NOTIFICATIONS ────────────────────────────────

//function showToast(message, type = 'info') {
//    const container = document.getElementById('toast-container');
//    const toast = document.createElement('div');
//    toast.className = `toast toast-${type}`;
//    const icons = { success: 'fa-check-circle', error: 'fa-circle-xmark', info: 'fa-circle-info' };
//    toast.innerHTML = `<i class="fas ${icons[type] || icons.info}"></i><span>${message}</span>`;
//    container.appendChild(toast);
//    setTimeout(() => {
//        toast.style.transition = 'opacity 0.4s, transform 0.4s';
//        toast.style.opacity = '0';
//        toast.style.transform = 'translateX(20px)';
//        setTimeout(() => toast.remove(), 400);
//    }, 3500);
//}

//// ── HTMX EVENT LISTENERS ──────────────────────────────

//// After bulk update — reset selection state
//document.body.addEventListener('htmx:afterSwap', function (evt) {
//    if (evt.detail.target && evt.detail.target.id === 'product-table-body') {
//        clearSelection();
//        const masterCb = document.getElementById('select-all');
//        if (masterCb) { masterCb.checked = false; masterCb.indeterminate = false; }
//        // Re-attach checkbox listeners to new rows
//        attachCheckboxListeners();
//    }
//});

//function attachCheckboxListeners() {
//    document.querySelectorAll('.row-checkbox').forEach(cb => {
//        cb.removeEventListener('change', updateBulkBar);
//        cb.addEventListener('change', updateBulkBar);
//    });
//}

//// After any HTMX request completing — handle success/error toasts
//document.body.addEventListener('htmx:afterRequest', function (evt) {
//    const method = evt.detail.requestConfig?.verb?.toUpperCase();
//    const status = evt.detail.xhr?.status;

//    if (!evt.detail.successful) {
//        const errText = evt.detail.xhr?.responseText;
//        if (status === 409) {
//            showToast('Concurrency conflict — another user modified this record. Please reload.', 'error');
//        } else if (status >= 500) {
//            showToast('Server error. Please try again.', 'error');
//        } else if (status === 404) {
//            showToast('Item not found.', 'error');
//        }
//        return;
//    }

//    if (method === 'DELETE') {
//        showToast('Product deleted successfully.', 'success');
//    } else if (method === 'POST' && evt.detail.pathInfo?.requestPath?.includes('BulkUpdate')) {
//        showToast('Bulk status update complete.', 'success');
//    } else if (method === 'POST' && evt.detail.pathInfo?.requestPath?.includes('Create')) {
//        showToast('Product created.', 'success');
//    } else if (method === 'PUT' && evt.detail.pathInfo?.requestPath?.includes('Edit')) {
//        showToast('Product updated.', 'success');
//    }
//});

//// ── ANTIFORGERY FOR DELETE BUTTONS ─────────────────────
//// HTMX doesn't send AntiForgery by default for non-POST.
//// We'll inject it from meta tag or hidden input.
//document.body.addEventListener('htmx:configRequest', function (evt) {
//    const method = evt.detail.verb?.toUpperCase();
//    if (method === 'DELETE' || method === 'PUT') {
//        const token = document.querySelector('input[name="__RequestVerificationToken"]');
//        if (token) {
//            evt.detail.headers['RequestVerificationToken'] = token.value;
//        }
//    }
//});

//// ── INIT ──────────────────────────────────────────────

//document.addEventListener('DOMContentLoaded', function () {
//    attachCheckboxListeners();

//    // Inject a hidden antiforgery input into the page if none exists (for HTMX usage)
//    if (!document.querySelector('input[name="__RequestVerificationToken"]')) {
//        const form = document.createElement('form');
//        form.style.display = 'none';
//        document.body.appendChild(form);
//    }
//});

// Expose for inline handlers
//window.openModal = openModal;
//window.closeModal = closeModal;
window.toggleSelectAll = toggleSelectAll;
window.updateBulkBar = updateBulkBar;
window.clearSelection = clearSelection;
//window.addAntiForgeryToDelete = addAntiForgeryToDelete;
