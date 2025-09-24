// Simple modal helpers
window.openModal = function (id) {
    document.getElementById(id)?.classList.remove('hidden');
};
window.closeModal = function (id) {
    document.getElementById(id)?.classList.add('hidden');
};

// Item flow
window.showAddItem = (name) => {
    const modal = document.getElementById('addItemModal');
    const input = document.getElementById('itemNameInput');
    if (input) input.value = name || '';
    const file = document.getElementById('itemImageInput');
    const img = document.getElementById('itemImagePreview');
    const ph = document.getElementById('itemImagePlaceholder');
    if (file && img && ph) {
        file.onchange = () => {
            const f = file.files[0];
            if (!f) { img.classList.add('hidden'); ph.classList.remove('hidden'); return; }
            const reader = new FileReader();
            reader.onload = e => { img.src = e.target.result; img.classList.remove('hidden'); ph.classList.add('hidden'); };
            reader.readAsDataURL(f);
        };
    }
    openModal('addItemModal');
};

// Category flow
window.showAddCategory = () => openModal('addCategoryModal');
window.editCategory = (id) => {
    document.getElementById('editCategoryId').value = id;
    document.getElementById('editCategoryName').value = (id === 'burgers' ? 'Burgers' : id);
    openModal('editCategoryModal');
};
window.deleteCategory = (id) => openModal('deleteCategoryModal');

// Optional: wireMenu placeholder for future interactive features
window.wireMenu = function () {
    // place any dynamic wiring here later
};

// ---- Promotions helpers ----
window.showCreateCampaign = () => openModal('createCampaignModal');
window.showCreateEvent = () => openModal('createEventModal');
window.showCreateDiscount = () => openModal('createDiscountModal');
window.showManageTrending = () => openModal('manageTrendingModal');

window.initPromoTabs = function () {
    const buttons = document.querySelectorAll('.promo-tab-btn');
    const sections = {
        campaigns: document.getElementById('tab-campaigns'),
        events: document.getElementById('tab-events'),
        discounts: document.getElementById('tab-discounts'),
        trending: document.getElementById('tab-trending'),
        analytics: document.getElementById('tab-analytics')
    };
    // default active
    let active = 'campaigns';
    function activate(name) {
        active = name;
        for (const [k, el] of Object.entries(sections)) {
            if (!el) continue;
            el.classList.toggle('hidden', k !== name);
            el.classList.toggle('active', k === name);
        }
        buttons.forEach(b => {
            const on = b.dataset.tab === name;
            b.classList.toggle('border-emerald-600', on);
            b.classList.toggle('text-emerald-700', on);
            b.classList.toggle('border-b-2', on);
        });
        if (name === 'analytics' && window.initPromoAnalytics) window.initPromoAnalytics();
    }
    buttons.forEach(b => b.addEventListener('click', () => activate(b.dataset.tab)));
    activate(active);
};

window.initPromoAnalytics = function () {
    const ctx = document.getElementById('promoPerfChart');
    if (!ctx || ctx.dataset.initialized) return;
    ctx.dataset.initialized = '1';
    new Chart(ctx, {
        type: 'line',
        data: {
            labels: ['Week 1', 'Week 2', 'Week 3', 'Week 4'],
            datasets: [{ label: 'Clicks', data: [120, 180, 160, 240] }]
        },
        options: { responsive: true, maintainAspectRatio: false }
    });
};

// =====================================================
// Exact JavaScript from sampleAdminPortal.html (after // Navigation)
// =====================================================

// Navigation
function showSection(sectionId) {
    document.querySelectorAll('.section').forEach(section => {
        section.classList.add('hidden');
    });
    const activeSection = document.getElementById(sectionId);
    if (activeSection) activeSection.classList.remove('hidden');

    // update sidebar active state
    document.querySelectorAll('.nav-item').forEach(btn => {
        btn.classList.remove('active');
    });
    const activeBtn = document.querySelector(`.nav-item[data-section="${sectionId}"]`);
    if (activeBtn) activeBtn.classList.add('active');
}

// Dashboard charts (REAL sample init; replaces your old initCharts stub)
if (document.getElementById('salesTrendChart')) {
    new Chart(document.getElementById('salesTrendChart'), {
        type: 'line',
        data: {
            labels: ['Mon', 'Tue', 'Wed', 'Thu', 'Fri', 'Sat', 'Sun'],
            datasets: [{
                label: 'Orders',
                data: [120, 150, 170, 140, 200, 250, 220],
                borderColor: '#10b981',
                backgroundColor: 'rgba(16,185,129,0.2)',
                fill: true,
                tension: 0.4
            }]
        },
        options: { responsive: true, maintainAspectRatio: false }
    });
}

if (document.getElementById('peakHoursChart')) {
    new Chart(document.getElementById('peakHoursChart'), {
        type: 'bar',
        data: {
            labels: ['9 AM', '12 PM', '3 PM', '6 PM', '9 PM'],
            datasets: [{
                label: 'Orders',
                data: [50, 120, 90, 180, 60],
                backgroundColor: '#3b82f6'
            }]
        },
        options: { responsive: true, maintainAspectRatio: false }
    });
}

// Menu category filter
function applyCategoryFilter(value) {
    const rows = document.querySelectorAll('#itemsTbody tr');
    rows.forEach(r => {
        const cat = r.getAttribute('data-category');
        r.style.display = (value === 'ALL' || value === cat) ? '' : 'none';
    });
}

// Reports demo actions
function downloadReportPdf() {
    alert('Pretend: generating PDF…');
}
function emailReport() {
    alert('Pretend: emailing report…');
}

// Staff search
function filterStaff() {
    const q = (document.getElementById('staffSearch')?.value || '').toLowerCase();
    const rows = document.querySelectorAll('#staffTable tbody tr');
    rows.forEach(r => {
        r.style.display = r.innerText.toLowerCase().includes(q) ? '' : 'none';
    });
}
