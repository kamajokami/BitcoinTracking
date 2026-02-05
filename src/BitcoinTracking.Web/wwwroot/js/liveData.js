/* LiveData page - Bitcoin rate fetching and saving */
(function () {
    'use strict';

    /** Holds the currently loaded Bitcoin rate */
    let currentRate = null;

    /* DOM elements (initialized on DOM ready) */
    let btnRefresh, btnSave, noteInput;
    let loadingIndicator, errorAlert, successAlert;
    let saveForm, rateRow;

    /* Initializes the page after DOM is fully loaded */
    
    document.addEventListener('DOMContentLoaded', () => {
        btnRefresh = document.getElementById('btnRefresh'); // Cache DOM elements
        btnSave = document.getElementById('btnSave');
        noteInput = document.getElementById('noteInput');
        loadingIndicator = document.getElementById('loadingIndicator');
        errorAlert = document.getElementById('errorAlert');
        successAlert = document.getElementById('successAlert');
        saveForm = document.getElementById('saveForm');
        rateRow = document.getElementById('rateRow');

        // Event bindings
        btnRefresh.addEventListener('click', fetchLiveRate);
        btnSave.addEventListener('click', saveRate);

        noteInput.addEventListener('keydown', e => {
            if (e.key === 'Enter') {
                e.preventDefault();
                saveRate();
            }
        });

        // Initial data load
        fetchLiveRate();
    });

    /* Data fetching*/
    /** Fetches live Bitcoin rate from MVC controller */
    function fetchLiveRate() {
        showLoading(true);
        hideAlerts();

        fetchJson('/LiveData/GetLiveRate')
            .then(result => {
                if (!result.success) {
                    throw new Error(result.message || 'Nepodařilo se načíst kurz');
                }

                currentRate = result.data;
                renderRate(currentRate);
            })
            .catch(err => showError(err.message))
            .finally(() => showLoading(false));
    }

    /** Renders Bitcoin rate into UI */
    function renderRate(rate) {
        rateRow.style.display = '';
        document.getElementById('timestamp').textContent =
            formatDateTime(new Date(rate.timestamp));

        document.getElementById('priceBtcEur').textContent =
            formatCurrency(rate.priceBtcEur, 'EUR');

        document.getElementById('exchangeRateEurCzk').textContent =
            formatNumber(rate.exchangeRateEurCzk, 4);

        document.getElementById('priceBtcCzk').textContent =
            formatCurrency(rate.priceBtcCzk, 'CZK');

        document.getElementById('source').textContent = rate.source;
        document.getElementById('lastUpdate').textContent =
            formatDateTime(new Date());

        saveForm.style.display = '';
    }

    /* Save*/
    /** Saves current rate to database */
    function saveRate() {
        const note = noteInput.value.trim();

        if (!note) {
            showError('Poznámka je povinná');
            noteInput.focus();
            return;
        }

        if (!currentRate) {
            showError('Nejprve načtěte aktuální kurz');
            return;
        }

        btnSave.disabled = true;
        showLoading(true);
        hideAlerts();

        const payload = {
            priceBtcEur: currentRate.priceBtcEur,
            exchangeRateEurCzk: currentRate.exchangeRateEurCzk,
            priceBtcCzk: currentRate.priceBtcCzk,
            note: note
        };

        fetchJson('/LiveData/SaveRate', {
            method: 'POST',
            body: JSON.stringify(payload),
            includeAntiForgery: true
        })
            .then(result => {
                if (!result.success) {
                    throw new Error(result.message || 'Uložení selhalo');
                }

                showSuccess('Záznam byl úspěšně uložen');
                noteInput.value = '';
            })
            .catch(err => showError(err.message))
            .finally(() => {
                btnSave.disabled = false;
                showLoading(false);
            });
    }


    /* Helper UI functions */
    /** Centralized fetch helper with antiforgery support */
    function fetchJson(url, options = {}) {
        const headers = {
            'Content-Type': 'application/json'
        };

        if (options.includeAntiForgery) {
            const token = getAntiForgeryToken();
            if (!token) {
                throw new Error('Chybí AntiForgery token');
            }
            headers['RequestVerificationToken'] = token;
        }

        return fetch(url, {
            headers,
            method: options.method || 'GET',
            body: options.body
        }).then(response => {
            if (!response.ok) {
                throw new Error(`HTTP ${response.status}`);
            }
            return response.json();
        });
    }

    /** Reads AntiForgery token from DOM */
    function getAntiForgeryToken() {
        const input = document.querySelector('input[name="__RequestVerificationToken"]');
        return input ? input.value : null;
    }

    /* UI helpers */
    function showLoading(show) {
        loadingIndicator.style.display = show ? 'block' : 'none';
        btnRefresh.disabled = show;
    }

    function hideAlerts() {
        errorAlert.style.display = 'none';
        successAlert.style.display = 'none';
    }

    function showError(message) {
        document.getElementById('errorMessage').textContent = message;
        errorAlert.style.display = 'block';
        setTimeout(() => errorAlert.style.display = 'none', 5000);
    }

    function showSuccess(message) {
        document.getElementById('successMessage').textContent = message;
        successAlert.style.display = 'block';
        setTimeout(() => successAlert.style.display = 'none', 5000);
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

    function formatCurrency(value, currency) {
        return new Intl.NumberFormat('cs-CZ', {
            style: 'currency',
            currency
        }).format(value);
    }

    function formatNumber(value, decimals) {
        return new Intl.NumberFormat('cs-CZ', {
            minimumFractionDigits: decimals,
            maximumFractionDigits: decimals
        }).format(value);
    }
})();

