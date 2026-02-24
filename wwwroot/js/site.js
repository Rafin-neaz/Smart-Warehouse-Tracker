/* ═══════════════════════════════════════════════════
   LOGISTICS COMMAND CENTER — CLIENT JAVASCRIPT
   ═══════════════════════════════════════════════════ */


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

function showToast(message, duration = 2500) {
    const container = document.getElementById("toast-container");
    if (!container) return;

    const toast = document.createElement("div");
    toast.className = "toast";
    toast.textContent = message;

    container.appendChild(toast);

    // Trigger animation
    requestAnimationFrame(() => {
        toast.classList.add("show");
    });

    // Auto-remove after duration
    setTimeout(() => {
        toast.classList.remove("show");
        setTimeout(() => toast.remove(), 400); // match CSS transition
    }, duration);
}

document.body.addEventListener("showToast", (e) => {
    showToast(e.detail.value);
});
document.body.addEventListener('bulk-update-success', function (evt) {
    // Hide the bulk bar
    const bar = document.getElementById('bulk-bar');
    const counter = document.getElementById('selected-count');
    if (bar) bar.style.display = 'none';
    if (counter) counter.textContent = '0 selected';

    // Optionally uncheck all checkboxes
    document.querySelectorAll('.row-checkbox').forEach(cb => cb.checked = false);

    // Reset master checkbox
    const masterCb = document.getElementById('select-all');
    if (masterCb) {
        masterCb.checked = false;
        masterCb.indeterminate = false;
    }
    showToast("Updated Successfully", 2000);
});
document.querySelectorAll('#tab-nav .tab').forEach(tab => {
    tab.addEventListener('click', function () {
        // Remove 'active' from all tabs
        document.querySelectorAll('#tab-nav .tab').forEach(t => t.classList.remove('active'));
        // Add 'active' to the clicked tab
        this.classList.add('active');
    });
});

function updateActiveTab() {
    const tabs = document.querySelectorAll('#tab-nav .tab');
    const urlParams = new URLSearchParams(window.location.search);
    const currentTab = urlParams.get('tab') || 'all';

    tabs.forEach(tab => {
        //Remove old active class
        tab.classList.remove('active');

        //Add active if tab matches currentTab
        const tabName = tab.getAttribute('hx-vals')
            ? JSON.parse(tab.getAttribute('hx-vals')).tab
            : tab.dataset.tab;

        if (tabName === currentTab) {
            tab.classList.add('active');
        }
    });
}

// Run on page load
document.addEventListener('DOMContentLoaded', updateActiveTab);

// Run on HTMX navigation (forward/back)
document.body.addEventListener('htmx:afterSwap', updateActiveTab);
document.body.addEventListener('htmx:popstate', updateActiveTab);
let currentPage = 1;
function getNextPage() {
    currentPage++; // Increment the local state
    return currentPage;
}

// Reset page to 1 whenever search or tab changes
document.body.addEventListener('htmx:beforeRequest', function (evt) {
    const triggerElt = evt.detail.elt;
    // Just reset the variable; the configRequest will handle the rest
    if (triggerElt.id === 'search-box' || triggerElt.classList.contains('tab')) {
        currentPage = 1;
    }
});

document.body.addEventListener("htmx:configRequest", function (e) {

    const token = document
        .querySelector('meta[name="csrf-token"]')
        ?.getAttribute("content");
    if (token) {
        e.detail.headers['RequestVerificationToken'] = token;
    }

    const searchVal = document.getElementById('search-box').value;
    const activeTab = document.querySelector(".tab.active").getAttribute("data-tab-value");

    e.detail.parameters['search'] = searchVal;
    e.detail.parameters['tab'] = activeTab;

    // FIX START: Call the increment function here
    if (e.detail.elt.getAttribute('hx-trigger') === 'revealed') {
        e.detail.parameters['page'] = getNextPage(); // Use the function to increment!
    }
    else if (e.detail.elt.getAttribute('hx-post') === "/Product/BulkUpdate") {
        e.detail.parameters['page'] = currentPage;
    }
    else {
        currentPage = 1; // Sync local variable
        e.detail.parameters['page'] = 1;
    }
    // FIX END

    console.log("Request sent:", e.detail.parameters); // Check your console!
});


window.toggleSelectAll = toggleSelectAll;
window.updateBulkBar = updateBulkBar;
window.clearSelection = clearSelection;

