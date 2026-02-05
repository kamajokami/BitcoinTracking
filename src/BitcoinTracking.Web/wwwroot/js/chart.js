// Chart for historical Bitcoin price development

(function () {
    'use strict';

    const canvas = document.getElementById('priceChart');
    if (!canvas) {
        console.warn('Canvas element #priceChart not found');
        return;
    }

    let chartInstance = null;
    let refreshTimerId = null;

    // Graf data from SavedDataController
    const API_ENDPOINT = '/SavedData/GetAllRecords';

    /**
     * Retrieves data from API and plots a graph
     */
    async function loadChart() {
        try {
            showChartLoading(true);
            hideChartError();

            const response = await fetch(API_ENDPOINT);
            if (!response.ok) {
                throw new Error(`HTTP ${response.status}: ${response.statusText}`);
            }

            const result = await response.json();

            // GetAllRecords returns { success: true, data: [...] }
            if (!result.success) {
                throw new Error(result.message || 'Nepodařilo se načíst data');
            }

            if (!result.data || !Array.isArray(result.data) || result.data.length === 0) {
                showEmptyState();
                return;
            }

            const data = result.data;

            // Sort from oldest to newest
            data.sort((a, b) => new Date(a.timestamp) - new Date(b.timestamp));

            renderChart(data);

        } catch (error) {
            console.error('Chyba při načítání grafu:', error);
            showChartError(error.message || 'Neznámá chyba při načítání grafu');
        } finally {
            showChartLoading(false);
        }
    }

    /**
     * Plots a graph with data
     */
    function renderChart(data) {
        // Preparing data for a chart
        const labels = data.map(item => {
            try {
                return new Date(item.timestamp).toLocaleString('cs-CZ', {
                    day: '2-digit',
                    month: '2-digit',
                    hour: '2-digit',
                    minute: '2-digit'
                });
            } catch (e) {
                return 'N/A';
            }
        });

        const pricesBtcCzk = data.map(item => item.priceBtcCzk || 0);
        const pricesBtcEur = data.map(item => item.priceBtcEur || 0);

        // Destroy old graph if it exists
        if (chartInstance) {
            try {
                chartInstance.destroy();
            } catch (e) {
                console.warn('Error destroying chart:', e);
            }
            chartInstance = null;
        }

        // Create new chart graf
        try {
            chartInstance = new Chart(canvas, {
                type: 'line',
                data: {
                    labels: labels,
                    datasets: [
                        {
                            label: 'BTC/CZK',
                            data: pricesBtcCzk,
                            borderColor: '#f7931a',
                            backgroundColor: 'rgba(247, 147, 26, 0.1)',
                            tension: 0.4,
                            fill: true,
                            yAxisID: 'y'
                        },
                        {
                            label: 'BTC/EUR',
                            data: pricesBtcEur,
                            borderColor: '#0d6efd',
                            backgroundColor: 'rgba(13, 110, 253, 0.1)',
                            tension: 0.4,
                            fill: true,
                            yAxisID: 'y1'
                        }
                    ]
                },
                options: {
                    responsive: true,
                    maintainAspectRatio: false,
                    interaction: {
                        mode: 'index',
                        intersect: false
                    },
                    plugins: {
                        legend: {
                            display: true,
                            position: 'top'
                        },
                        tooltip: {
                            callbacks: {
                                label: function (context) {
                                    const label = context.dataset.label || '';
                                    const value = context.parsed.y;
                                    return `${label}: ${value.toLocaleString('cs-CZ', {
                                        minimumFractionDigits: 2,
                                        maximumFractionDigits: 2
                                    })}`;
                                }
                            }
                        }
                    },
                    scales: {
                        y: {
                            type: 'linear',
                            display: true,
                            position: 'left',
                            title: {
                                display: true,
                                text: 'BTC/CZK'
                            },
                            ticks: {
                                callback: function (value) {
                                    return value.toLocaleString('cs-CZ') + ' Kč';
                                }
                            }
                        },
                        y1: {
                            type: 'linear',
                            display: true,
                            position: 'right',
                            title: {
                                display: true,
                                text: 'BTC/EUR'
                            },
                            grid: {
                                drawOnChartArea: false
                            },
                            ticks: {
                                callback: function (value) {
                                    return '€' + value.toLocaleString('cs-CZ');
                                }
                            }
                        },
                        x: {
                            ticks: {
                                maxRotation: 45,
                                minRotation: 45
                            }
                        }
                    }
                }
            });

        // Hide empty state
            hideEmptyState();

            console.log('The graph is plotted successfully with', data.length, 'records');

        } catch (error) {
            console.error('Error while plotting the graph:', error);
            showChartError('Chyba při vykreslování grafu: ' + error.message);
        }
    }

    /**
     * Displays load indicator
     */
    function showChartLoading(show) {
        const loadingEl = document.getElementById('chartLoading');
        if (loadingEl) {
            loadingEl.style.display = show ? 'block' : 'none';
        }
    }

    /**
     * Displays empty status (no data)
     */
    function showEmptyState() {
        const emptyEl = document.getElementById('chartEmpty');
        if (emptyEl) {
            emptyEl.style.display = 'block';
        }
        if (canvas) {
            canvas.style.display = 'none';
        }
        console.info('Chart: no data to display');
    }

    /**
     * Hides empty state
     */
    function hideEmptyState() {
        const emptyEl = document.getElementById('chartEmpty');
        if (emptyEl) {
            emptyEl.style.display = 'none';
        }
        if (canvas) {
            canvas.style.display = 'block';
        }
    }

    /**
     * Displays an error message
     */
    function showChartError(message) {
        const errorEl = document.getElementById('chartError');
        if (errorEl) {
            const messageEl = errorEl.querySelector('.error-message');
            if (messageEl) {
                messageEl.textContent = message;
            }
            errorEl.style.display = 'block';
        }
        if (canvas) {
            canvas.style.display = 'none';
        }
        console.error('Graf: error -', message);
    }

    /**
     * Hides error message
     */
    function hideChartError() {
        const errorEl = document.getElementById('chartError');
        if (errorEl) {
            errorEl.style.display = 'none';
        }
    }

    /**
    * Cleanup on page exit or reload
    */
    function cleanup() {
        console.log('Cleaning up chart resources...');

        // Stop timer
        if (refreshTimerId) {
            clearInterval(refreshTimerId);
            refreshTimerId = null;
        }

        // Destroy graph
        if (chartInstance) {
            try {
                chartInstance.destroy();
            } catch (e) {
                console.warn('Error during cleanup:', e);
            }
            chartInstance = null;
        }
    }

    // Initialization on page load
    console.log('Chart.js initialized - loading data from:', API_ENDPOINT);

    // Load data on startup on page load
    loadChart();

    // Automatic update every 30 seconds
    refreshTimerId = setInterval(loadChart, 30000);

    // Refresh button can also update the chart
    const refreshBtn = document.getElementById('btnRefresh');
    if (refreshBtn) {
        refreshBtn.addEventListener('click', function () {
            console.log('Refresh button clicked - reloading chart');
            loadChart();
        });
    }

    // Cleanup when leaving the page
    window.addEventListener('beforeunload', cleanup);

    // Cleanup on unload (Safari support)
    window.addEventListener('unload', cleanup);

})();