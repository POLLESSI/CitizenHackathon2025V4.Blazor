import {
    createBaseMap,
    addCrowdMarkers as addInitialCrowdMarkers,
    addSuggestionMarkers as addInitialSuggestionMarkers,
    addTrafficMarkers
} from './app/leafletLogic.js';

let map;
let crowdMarkers = {};
let filterLevel = null;
let initialized = false;

const crowdIcons = {
    0: L.divIcon({ className: 'crowd-icon level-0', html: '🟢' }),
    1: L.divIcon({ className: 'crowd-icon level-1', html: '🟡' }),
    2: L.divIcon({ className: 'crowd-icon level-2', html: '🟠' }),
    3: L.divIcon({ className: 'crowd-icon level-3', html: '🔴' }),
};

// 📌 Initializes the map and initial markers
export function initOutzenMap(mapId, crowdData, suggestions, trafficEvents, center = [50.89, 4.34], zoom = 13) {
    map = createBaseMap(mapId, center, zoom);
    window.leafletMap = map; // for compatibility with other modules (eg SignalR)

    addInitialCrowdMarkers(map, crowdData);
    addInitialSuggestionMarkers(map, suggestions);
    addTrafficMarkers(map, trafficEvents);

    initialized = true;
}

// 🎯 Adds or updates a traffic marker (SignalR)
export function addOrUpdateCrowdMarker(id, lat, lng, level, info) {
    if (!map) return;

    if (crowdMarkers[id]) {
        map.removeLayer(crowdMarkers[id].marker);
    }

    const marker = L.marker([lat, lng], {

        icon: crowdIcons[level] || crowdIcons[0]
    }).bindPopup(`< strong >${info.title}</ strong >< br />${info.description}`);

    marker.addTo(map);
    blinkEffect(marker);
    crowdMarkers[id] = { marker, level }
        ;

    updateVisibleMarkers();
}

// 💥 Flashing new markers
function blinkEffect(marker) {
    const el = marker.getElement();
    if (!el) return;
    el.classList.add('fade-in', 'blink');
    setTimeout(() => el.classList.remove('blink'), 2000);
}

// 🧽 Deletes a marker (optional)
export function removeCrowdMarker(id) {
    if (crowdMarkers[id]) {
        map.removeLayer(crowdMarkers[id].marker);
        delete crowdMarkers[id];
    }
}

// 🔍 Applies a crowd filter (level 0 to 3)
export function setFilterLevel(level) {
    filterLevel = level;
    updateVisibleMarkers();
}

// 🔄 Show/hide markers according to the filter
function updateVisibleMarkers() {
    if (!map) return;

    Object.values(crowdMarkers).forEach(({ marker, level }) => {
        if (filterLevel === null || level === filterLevel) {
            marker.addTo(map);
        }
        else {
            map.removeLayer(marker);
        }
    });
}

// 🌟 Adds suggestions (can be called dynamically)
export function showSuggestions(suggestions) {
    if (!map) return;

    suggestions.forEach(s => {
        const icon = L.icon({
            iconUrl: "/icons/suggestion-marker.png",
            iconSize: [32, 32]
        });

        const marker = L.marker([s.latitude, s.longitude], { icon })
            .addTo(map)
            .bindPopup(`< strong >${s.title}</ strong >< br /> À ${s.distanceKm}
km`);
    });
}

// 📉 Initialize the chart (Chart.js)
export function initCrowdChart() {
    const ctx = document.getElementById('crowdChart')?.getContext('2d');
    if (!ctx) {
        console.warn("📉 Élément #crowdChart introuvable.");
        return;
    }

    window.crowdChart = new Chart(ctx, {
        type: 'line',
        data: {
            labels: [],
            datasets:
                [{
                    label: 'Crowd Level (%)',
                    data: [],
                    borderColor: '#FFD700',
                    backgroundColor: 'rgba(255,215,0,0.3)'
                }]
        },
        options:
        {
            scales:
            {
                y: { beginAtZero: true, max: 100 }
            }
        }
    });
}

// 🔁 Updates the chart in real time
export function updateCrowdChart(value) {
    const chart = window.crowdChart;
    if (!chart) return;

    const now = new Date().toLocaleTimeString();
    chart.data.labels.push(now);
    chart.data.datasets[0].data.push(parseInt(value));

    if (chart.data.labels.length > 20) {
        chart.data.labels.shift();
        chart.data.datasets[0].data.shift();
    }

    chart.update();
}








































































































/*// Copyrigtht (c) 2025 Citizen Hackathon https://github.com/POLLESSI/Citizenhackathon2025V4.Blazor.Client. All rights reserved.*/