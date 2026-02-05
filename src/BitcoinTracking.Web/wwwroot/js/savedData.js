// SavedData page - CRUD operations with jQuery DataTables
(function () {
    'use strict';

    // STATE MANAGEMENT
    let dataTable = null;

    // Stores edited notes before saving
    let pendingChanges = {};

    // Stores IDs of selected rows (checkboxes)
    let selectedIds = [];

    // Modal windows delete
    let deleteModal;

    // DOM REFERENCES
    const btnDeleteSelected = document.getElementById('btnDeleteSelected');
    const btnSaveChanges = document.getElementById('btnSaveChanges');
    const btnRefresh = document.getElementById('btnRefresh');
    const btnConfirmDelete = document.getElementById('btnConfirmDelete');
    const selectAll = document.getElementById('selectAll');
    const tableElement = document.getElementById('savedDataTable');

    // INITIALIZATION
    document.addEventListener('DOMContentLoaded', () => {
        initializeDataTable();
        bindEvents();
        loadRecords();
    });

    // DATATABLE SETUP
    function initializeDataTable() {
        dataTable = $('#savedDataTable').DataTable({
            autoWidth: false,
            responsive: true,
            paging: true,
            pageLength: 15,
            lengthMenu: [10, 15, 25, 50, 100],
            ordering: true,
            order: [[2, 'desc']],
            searching: true,
            info: true,

            // Layout without custom DOM (for search)
            dom: '<"row"<"col-sm-12 col-md-6"l><"col-sm-12 col-md-6"f>>' +
                '<"row"<"col-sm-12"tr>>' +
                '<"row"<"col-sm-12 col-md-5"i><"col-sm-12 col-md-7"p>>',

            language: {
                processing: "Zpracovávám...",
                lengthMenu: "Zobrazit _MENU_ záznamů",
                zeroRecords: "Žádné záznamy nenalezeny",
                info: "Zobrazeno _START_ - _END_ z _TOTAL_ záznamů",
                infoEmpty: "Žádné záznamy",
                infoFiltered: "(filtrováno z _MAX_ záznamů)",
                search: "Hledání:",
                paginate: {
                    first: "První",
                    last: "Poslední",
                    next: "Další",
                    previous: "Předchozí"
                }
            },
            columns: [
                { data: null, searchable: false, orderable: false, render: renderCheckbox },
                { data: 'id' },
                { data: 'timestamp', render: renderTimestamp },
                { data: 'priceBtcEur', render: renderCurrencyEur },
                { data: 'exchangeRateEurCzk', render: renderExchangeRate },
                { data: 'priceBtcCzk', render: renderCurrencyCzk },
                { data: 'note', render: renderNoteInput, searchable: true },
                { data: null, orderable: false, searchable: false, render: renderActions }
            ],

            initComplete: function () {
                console.log('DataTable initialized with', this.api().data().count(), 'rows');
            }
        });

        dataTable.on('draw', syncSelectAllCheckbox);
    }

    // DATA LOADING
    function loadRecords() {

        showLoading(true, 'Načítám záznamy...');

        fetchJson('/SavedData/GetAllRecords')
            .then(result => {
                showLoading(false);

                if (!result.success) {
                    showError(result.message || 'Chyba při načítání dat');
                    return;
                }

                // Reset UI state
                pendingChanges = {};
                selectedIds = [];
                selectAll.checked = false;
                updateButtons();

                /* invalidate() = search index recalculation */
                dataTable.clear().rows.add(result.data).invalidate().draw(false); 
            })
            .catch(handleFetchError('Chyba při načítání záznamů'));
    }

    // EVENT BINDINGS
    function bindEvents() {
        // Select / Deselect all rows
        selectAll.addEventListener('change', function () {
            const checked = this.checked;
            document.querySelectorAll('.row-checkbox').forEach(cb => cb.checked = checked);
            updateSelectedIds();
            updateButtons();
        });

        // Refresh data
        btnRefresh.addEventListener('click', loadRecords);

        // Save edited notes
        btnSaveChanges.addEventListener('click', saveChanges);

        const deleteModalEl = document.getElementById('confirmDeleteModal');
        deleteModal = new bootstrap.Modal(deleteModalEl);

        // Delete selected records (open modal)
        btnDeleteSelected.addEventListener('click', () => {
            if (selectedIds.length === 0) {
                showError('Nevybrali jste žádné záznamy');
                return;
            }

            document.getElementById('deleteConfirmMessage').textContent =
                `Opravdu chcete smazat ${selectedIds.length} záznamů?`;

            deleteModal.show();
        });

        // Confirm bulk delete
        btnConfirmDelete.addEventListener('click', deleteSelectedRecords);

        // Delegated checkbox handling
        tableElement.addEventListener('change', function (e) {
            if (e.target.classList.contains('row-checkbox')) {
                updateSelectedIds();
                syncSelectAllCheckbox();
                updateButtons();
            }
        });

        // Delegated note editing
        tableElement.addEventListener('input', function (e) {
            if (!e.target.classList.contains('note-input')) return;

            const id = Number(e.target.dataset.id);
            const original = e.target.dataset.originalNote || '';
            const current = e.target.value.trim();

            if (current !== original) {
                pendingChanges[id] = current;
            } else {
                delete pendingChanges[id];
            }

            updateButtons();
        });

        // Delegated single delete
        tableElement.addEventListener('click', function (e) {
            const btn = e.target.closest('.btn-delete-single');
            if (!btn) return;

            const id = Number(btn.dataset.id);
            if (confirm('Chcete smazat tento záznam?')) {
                deleteSingleRecord(id);
            }
        });
    }

    // CRUD OPERATIONS
    function saveChanges() {
        const entries = Object.entries(pendingChanges);

        /*if (entries.length === 0) {*/
        if (!entries.length) {
            showError('Žádné změny k uložení');
            return;
        }

        for (const [, note] of entries) {
            if (!note) {
                showError('Poznámka nemůže být prázdná');
                return;
            }
        }

        btnSaveChanges.disabled = true;
        showLoading(true, 'Ukládám změny...');

        Promise.all(entries.map(([id, note]) =>
            fetchJson('/SavedData/UpdateNote', {
                method: 'POST',
                body: JSON.stringify({ id: Number(id), note })
            })
        ))
            .then(results => {
                showLoading(false);
                btnSaveChanges.disabled = false;

                if (results.every(r => r.success)) {
                    showSuccess(`Uloženo ${entries.length} změn`);
                    loadRecords();
                } else {
                    showError('Některé změny se nepodařilo uložit');
                }
            })
            .catch(handleFetchError('Chyba při ukládání změn'))
            .finally(() => btnSaveChanges.disabled = false);
    }

    function deleteSelectedRecords() {
        if (!selectedIds.length) return;

        showLoading(true, 'Smazávám záznamy...');

        fetchJson('/SavedData/DeleteSelected', {
            method: 'POST',
            body: JSON.stringify(selectedIds)
        })
            .then(result => {
                showLoading(false);

                if (result.success) {
                    showSuccess(result.message);
                    deleteModal.hide();
                    loadRecords();
                } else {
                    showError(result.message || 'Chyba při mazání');
                }
            })
            .catch(handleFetchError('Chyba při mazání záznamů'));
    }

    function deleteSingleRecord(id) {
        showLoading(true, 'Smazávám záznam...');

        fetchJson('/SavedData/DeleteRecord', {
            method: 'POST',
            body: JSON.stringify({ id })
        })
            .then(result => {
                showLoading(false);

                if (result.success) {
                    showSuccess('Záznam byl smazán');
                    loadRecords();
                } else {
                    showError(result.message || 'Chyba při mazání');
                }
            })
            .catch(handleFetchError('Chyba při mazání záznamu'));
    }

    // UI HELPERS
    function updateSelectedIds() {
        selectedIds = [...document.querySelectorAll('.row-checkbox:checked')]
            .map(cb => Number(cb.dataset.id));
    }

    function syncSelectAllCheckbox() {
        const all = document.querySelectorAll('.row-checkbox').length;
        const checked = document.querySelectorAll('.row-checkbox:checked').length;
        selectAll.checked = all > 0 && all === checked;
    }

    function updateButtons() {
        btnDeleteSelected.disabled = !selectedIds.length;
        btnSaveChanges.disabled = !Object.keys(pendingChanges).length;
    }


    // RENDERERS
    function renderCheckbox(_, type, row) {
        return `<input type="checkbox" class="form-check-input row-checkbox" data-id="${row.id}">`;
    }

    function renderTimestamp(data, type) {
        if (!data) return '';

        if (type === 'display' || type === 'filter') {
            return new Date(data).toLocaleString('cs-CZ');
        }

        // For SORT (ISO / timestamp)
        return data;
    }

    function renderCurrencyEur(data, type) {
        if (!data && data !== 0) return '';

        // Return number for sorting/filtering
        if (type === 'sort' || type === 'type') {
            return Number(data);
        }

        // Display and Filter: Formatted currency
        return new Intl.NumberFormat('cs-CZ', {
            style: 'currency',
            currency: 'EUR',
            minimumFractionDigits: 2,
            maximumFractionDigits: 2
        }).format(data);
    }

    function renderCurrencyCzk(data, type) {
        if (!data && data !== 0) return '';

        // Return number for sorting
        if (type === 'sort' || type === 'type') {
            return Number(data);
        }

        // Filter: Plain formatted number (without HTML tags for search)
        if (type === 'filter') {
            return new Intl.NumberFormat('cs-CZ', {
                style: 'currency',
                currency: 'CZK',
                minimumFractionDigits: 2,
                maximumFractionDigits: 2
            }).format(data);
        }

        // Display: Bold formatted currency
        return `<strong>${new Intl.NumberFormat('cs-CZ', {
            style: 'currency',
            currency: 'CZK',
            minimumFractionDigits: 2,
            maximumFractionDigits: 2
        }).format(data)}</strong>`;
    }

    function renderExchangeRate(data, type) {
        if (!data && data !== 0) return '';

        // Return number for sorting
        if (type === 'sort' || type === 'type') {
            return Number(data);
        }

        // Display and Filter: 4 decimal places
        return Number(data).toFixed(4);
    }

    function renderNoteInput(data, type, row) {
        const value = data ?? '';

        // For search and sort, return plain text
        if (type === 'filter' || type === 'sort' || type === 'type') {
            return value;
        }

        // Display: Editable input field
        const escapedValue = escapeHtml(value);

        return ` <input type="text"
           class="form-control form-control-sm note-input"
           data-id="${row.id}"
           data-original-note="${escapedValue}"
           value="${escapedValue}" />`;
    }

    function renderActions(_, type, row) {
        return `<button class="btn btn-sm btn-outline-danger btn-delete-single"
                    data-id="${row.id}"
                    title="Smazat záznam">
                <i class="bi bi-trash3"></i>
            </button>`;
    }

    // UTILITIES
    function fetchJson(url, options = {}) {
        const token = getAntiForgeryToken();

        return fetch(url, {
            headers: {
                'Content-Type': 'application/json',
                'RequestVerificationToken': token
            },
            ...options
        }).then(r => {
            if (!r.ok) throw new Error(`HTTP ${r.status}`);
            return r.json();
        });
    }

    function handleFetchError(prefix) {
        return err => {
            showLoading(false);
            showError(`${prefix}: ${err.message}`);
            console.error(err);
        };
    }

    // Get antiforgery token
    function getAntiForgeryToken() {
        const tokenInput = document.querySelector('input[name="__RequestVerificationToken"]');
        return tokenInput ? tokenInput.value : '';
    }

    // UI FEEDBACK
    function showLoading(show, text) {
        document.getElementById('loadingIndicator').style.display = show ? 'block' : 'none';
        if (text) document.getElementById('loadingText').textContent = text;
    }

    function showError(message) {
        showAlert('errorAlert', 'errorMessage', message);
    }

    function showSuccess(message) {
        showAlert('successAlert', 'successMessage', message);
    }

    function showAlert(containerId, messageId, message) {
        const alert = document.getElementById(containerId);
        document.getElementById(messageId).textContent = message;
        alert.style.display = 'block';
        setTimeout(() => alert.style.display = 'none', 5000);
    }

    // UTILITIES
    function escapeHtml(text) {
        const div = document.createElement('div');
        div.textContent = text;
        return div.innerHTML;
    }

    function formatDateTime(date) {
        return date.toLocaleString('cs-CZ', {
            day: '2-digit',
            month: '2-digit',
            year: 'numeric',
            hour: '2-digit',
            minute: '2-digit'
        });
    }

})();
