// leafletBundle.js
// ============================
// Low-level logic import
// ============================
import { initBaseMap, addTileLayer, setViewPosition, addCustomMarker } from './leafletLogic.js';

// ============================
// Module: Basic Leaflet Map (old leafletMap.js)
// ============================
export function initMainMap(containerId, options) {
    const map = initBaseMap(containerId, options);
    addTileLayer(map, options.tileUrl || 'https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png');
    return map;
}

// ============================
// Module : Alternative suggestions (old suggestionMap.js)
// ============================
export function displaySuggestions(map, suggestions) {
    if (!suggestions || !Array.isArray(suggestions)) return;

    suggestions.forEach(s => {
        addCustomMarker(map, s.lat, s.lng, {
            iconUrl: s.icon || 'icons/suggestion.png',
            popupText: `<b>${s.title}</b><br>${s.description}`
        });
    });
}

// ============================
// Module : Crowd Map (formerly crowdMap.js)
// ============================
export function updateCrowdLayers(map, crowdData) {
    if (!crowdData) return;

    crowdData.forEach(c => {
        let color = c.color || '#FF0000';
        addCustomMarker(map, c.lat, c.lng, {
            iconUrl: c.icon || 'icons/crowd.png',
            popupText: `<b>${c.name}</b><br>Density : ${c.crowdLevel}`,
            color
        });
    });
}

// ============================
// Module : Built-in charts (formerly crowdChart.js)
// ============================
export function renderCrowdChart(canvasId, data) {
    if (!data) return;

    const ctx = document.getElementById(canvasId).getContext('2d');
    new Chart(ctx, {
        type: 'bar',
        data: {
            labels: data.map(d => d.label),
            datasets: [{
                label: 'Crowd level',
                data: data.map(d => d.value),
                backgroundColor: data.map(d => d.color || '#FF0000')
            }]
        },
        options: {
            responsive: true,
            plugins: {
                legend: { position: 'top' },
                title: { display: true, text: 'Real-time traffic' }
            }
        }
    });
}


































































































/*// Copyrigtht (c) 2025 Citizen Hackathon https://github.com/POLLESSI/Citizenhackathon2025V4.Blazor.Client. All rights reserved.*/