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

window.addEventListener('click', function (e) {
    if (e.target.classList.contains('modal')) {
        e.target.classList.remove('show');
    }
});

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

//menu state
const menuState = {
    categories: [
        { id: 'Burgers', name: 'Burgers' },
        { id: 'Drinks', name: 'Drinks' }
    ],
    items: [
        { id: 'i1', name: 'Classic Burger', categoryId: 'Burgers', price: 199, available: true, desc: '' },
        { id: 'i2', name: 'Iced Tea', categoryId: 'Drinks', price: 59, available: true, desc: '' }
    ],
    filter: 'ALL'
};

const genId = (p = 'i') => p + Math.random().toString(36).slice(2, 8);

//renders
function renderCategoryFilter() {
    const sel = document.getElementById('categoryFilter');
    if (!sel) return;
    // keep current selection
    const val = sel.value || 'ALL';
    sel.innerHTML = '<option value="ALL">All Categories</option>' +
        menuState.categories.map(c => `<option value="${c.id}">${c.name}</option>`).join('');
    sel.value = val;
}

function renderCategoryOptions() {
    // for item modal select
    const sel = document.getElementById('itemCategoryInput');
    if (!sel) return;
    sel.innerHTML = menuState.categories.map(c => `<option value="${c.id}">${c.name}</option>`).join('');
}

function renderCategoriesTable() {
    const tbody = document.getElementById('categoriesTbody');
    if (!tbody) return;
    tbody.innerHTML = menuState.categories.map(c => {
        const count = menuState.items.filter(i => i.categoryId === c.id).length;
        return `
      <tr data-category-id="${c.id}">
        <td class="px-4 py-3 font-medium">${c.name}</td>
        <td class="px-4 py-3">${count}</td>
        <td class="px-4 py-3"><span class="px-2 py-1 text-xs rounded-full bg-green-100 text-green-700">Yes</span></td>
        <td class="px-4 py-3">
          <div class="flex gap-2">
            <button type="button" class="px-2 py-1 rounded border hover:bg-gray-50" onclick="editCategory('${c.id}')">Edit</button>
            <button type="button" class="px-2 py-1 rounded border hover:bg-red-50 text-red-600" onclick="showDeleteCategory('${c.id}')">Delete</button>
          </div>
        </td>
      </tr>`;
    }).join('');
}

function renderItemsTable() {
    const tbody = document.getElementById('itemsTbody');
    if (!tbody) return;
    const rows = menuState.items
        .filter(i => menuState.filter === 'ALL' || i.categoryId === menuState.filter)
        .map(i => {
            const cat = menuState.categories.find(c => c.id === i.categoryId)?.name || i.categoryId;
            return `
      <tr data-id="${i.id}">
        <td class="px-4 py-3 font-medium">${i.name}</td>
        <td class="px-4 py-3">${cat}</td>
        <td class="px-4 py-3">₱${i.price.toFixed(2)}</td>
        <td class="px-4 py-3">
          <span class="px-2 py-1 text-xs rounded-full ${i.available ? 'bg-green-100 text-green-700' : 'bg-gray-100 text-gray-700'}">
            ${i.available ? 'Yes' : 'No'}
          </span>
        </td>
        <td class="px-4 py-3">
          <div class="flex gap-2">
            <button type="button" class="px-2 py-1 rounded border hover:bg-gray-50" onclick="showEditItem('${i.id}')">Edit</button>
            <button type="button" class="px-2 py-1 rounded border hover:bg-red-50 text-red-600" onclick="showDeleteItem('${i.id}')">Delete</button>
          </div>
        </td>
      </tr>`;
        }).join('');
    tbody.innerHTML = rows || `<tr><td class="px-4 py-6 text-gray-500" colspan="5">No items found.</td></tr>`;
}

function renderAllMenu() {
    renderCategoryFilter();
    renderCategoryOptions();
    renderCategoriesTable();
    renderItemsTable();
}

//filter
window.applyCategoryFilter = function (value) {
    menuState.filter = value || 'ALL';
    renderItemsTable();
};

//category
window.showAddCategory = () => openModal('addCategoryModal');

document.addEventListener('DOMContentLoaded', () => {
    // Add Category
    const addCatForm = document.getElementById('addCategoryForm');
    if (addCatForm) {
        addCatForm.addEventListener('submit', (e) => {
            e.preventDefault();
            const name = document.getElementById('categoryNameInput').value.trim();
            if (!name) return;
            const id = name;
            if (!menuState.categories.find(c => c.id === id)) {
                menuState.categories.push({ id, name });
            }
            closeModal('addCategoryModal');
            addCatForm.reset();
            renderAllMenu();
        });
    }

    // Edit Category
    const editCatForm = document.getElementById('editCategoryForm');
    if (editCatForm) {
        editCatForm.addEventListener('submit', (e) => {
            e.preventDefault();
            const id = document.getElementById('editCategoryId').value;
            const name = document.getElementById('editCategoryName').value.trim();
            if (!id || !name) return;
            const cat = menuState.categories.find(c => c.id === id);
            if (cat) {
                // update id and name consistently
                const oldId = cat.id;
                cat.id = name;
                cat.name = name;
                // migrate items to new category id
                menuState.items.forEach(i => { if (i.categoryId === oldId) i.categoryId = name; });
            }
            closeModal('editCategoryModal');
            renderAllMenu();
        });
    }

    // Confirm delete category
    const delCatBtn = document.getElementById('confirmDeleteCategoryBtn');
    if (delCatBtn) {
        delCatBtn.addEventListener('click', () => {
            const id = document.getElementById('deleteCategoryId').value;
            if (!id) return;
            // remove items in this category
            menuState.items = menuState.items.filter(i => i.categoryId !== id);
            // remove category
            menuState.categories = menuState.categories.filter(c => c.id !== id);
            closeModal('deleteCategoryModal');
            if (menuState.filter === id) menuState.filter = 'ALL';
            renderAllMenu();
        });
    }

    // Add / Edit Item (single form)
    const addItemForm = document.getElementById('addItemForm');
    if (addItemForm) {
        addItemForm.addEventListener('submit', (e) => {
            e.preventDefault();
            const editId = document.getElementById('editItemId').value;
            const name = document.getElementById('itemNameInput').value.trim();
            const categoryId = document.getElementById('itemCategoryInput').value;
            const price = parseFloat(document.getElementById('itemPriceInput').value || '0');
            const available = document.getElementById('itemAvailableInput').checked;
            const desc = document.getElementById('itemDescInput').value.trim();
            if (!name || !categoryId) return;

            if (editId) {
                const item = menuState.items.find(i => i.id === editId);
                if (item) {
                    item.name = name;
                    item.categoryId = categoryId;
                    item.price = price;
                    item.available = available;
                    item.desc = desc;
                }
            } else {
                menuState.items.push({ id: genId(), name, categoryId, price, available, desc });
            }

            closeModal('addItemModal');
            addItemForm.reset();
            document.getElementById('editItemId').value = '';
            document.getElementById('addItemTitle').textContent = 'Add Menu Item';
            document.getElementById('addItemSubmitBtn').textContent = 'Save Item';
            renderAllMenu();
        });
    }

    // Confirm delete item
    const delItemBtn = document.getElementById('confirmDeleteBtn');
    if (delItemBtn) {
        delItemBtn.addEventListener('click', () => {
            const id = document.getElementById('deleteItemId').value;
            if (!id) return;
            menuState.items = menuState.items.filter(i => i.id !== id);
            closeModal('deleteConfirmModal');
            renderItemsTable();
        });
    }

    // Initial render
    renderAllMenu();
});

// open edit category modal
window.editCategory = (id) => {
    const cat = menuState.categories.find(c => c.id === id);
    if (!cat) return;
    document.getElementById('editCategoryId').value = cat.id;
    document.getElementById('editCategoryName').value = cat.name;
    openModal('editCategoryModal');
};

// show delete category modal
window.showDeleteCategory = (id) => {
    document.getElementById('deleteCategoryId').value = id;
    openModal('deleteCategoryModal');
};

// show add item modal (empty)
window.showAddItem = () => {
    document.getElementById('editItemId').value = '';
    document.getElementById('addItemTitle').textContent = 'Add Menu Item';
    document.getElementById('addItemSubmitBtn').textContent = 'Save Item';
    // ensure category options are fresh
    renderCategoryOptions();
    openModal('addItemModal');
};

// show edit item modal
window.showEditItem = (id) => {
    const item = menuState.items.find(i => i.id === id);
    if (!item) return;
    document.getElementById('editItemId').value = item.id;
    document.getElementById('itemNameInput').value = item.name;
    renderCategoryOptions();
    document.getElementById('itemCategoryInput').value = item.categoryId;
    document.getElementById('itemPriceInput').value = item.price;
    document.getElementById('itemAvailableInput').checked = item.available;
    document.getElementById('itemDescInput').value = item.desc || '';
    document.getElementById('addItemTitle').textContent = 'Edit Menu Item';
    document.getElementById('addItemSubmitBtn').textContent = 'Update Item';
    openModal('addItemModal');
};

// show delete item modal
window.showDeleteItem = (id) => {
    document.getElementById('deleteItemId').value = id;
    openModal('deleteConfirmModal');
};

//dropdown menu
document.addEventListener('DOMContentLoaded', function () {
    const btn = document.getElementById('userMenuButton');
    const menu = document.getElementById('userMenu');
    if (!btn || !menu) return;

    function openMenu() {
        menu.classList.remove('hidden');
        btn.setAttribute('aria-expanded', 'true');
    }
    function closeMenu() {
        if (!menu.classList.contains('hidden')) {
            menu.classList.add('hidden');
            btn.setAttribute('aria-expanded', 'false');
        }
    }

    btn.addEventListener('click', function (e) {
        e.stopPropagation();
        menu.classList.contains('hidden') ? openMenu() : closeMenu();
    });

    // click outside
    document.addEventListener('click', function (e) {
        if (!menu.contains(e.target) && e.target !== btn) closeMenu();
    });

    // Esc to close
    document.addEventListener('keydown', function (e) {
        if (e.key === 'Escape') closeMenu();
    });
});
