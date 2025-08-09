let crowdMarkers = {};
let filterLevel = null;

import {
    createBaseMap,
    addCrowdMarkers,
    addSuggestionMarkers,
    addTrafficMarkers
} from './modules/leafletLogic.js';

const map = createBaseMap('leafletMap');
addCrowdMarkers(map, crowdData);
addSuggestionMarkers(map, suggestions);
addTrafficMarkers(map, trafficEvents);

const crowdIcons = {
    0: L.divIcon({ className: 'crowd-icon level-0', html: '🟢' }),
    1: L.divIcon({ className: 'crowd-icon level-1', html: '🟡' }),
    2: L.divIcon({ className: 'crowd-icon level-2', html: '🟠' }),
    3: L.divIcon({ className: 'crowd-icon level-3', html: '🔴' }),
};

export function setFilterLevel(level) {
    filterLevel = level;
    updateVisibleMarkers();
}

function updateVisibleMarkers() {
    Object.values(crowdMarkers).forEach(m =>
        (filterLevel === null || m.level === filterLevel) ?
            m.marker.addTo(window.leafletMap) :
            window.leafletMap.removeLayer(m.marker)
    );
}

export function addOrUpdateCrowdMarker(id, lat, lng, level, info) {
    if (crowdMarkers[id]) {
        window.leafletMap.removeLayer(crowdMarkers[id].marker);
    }
    const marker = L.marker([lat, lng], {
        icon: crowdIcons[level] || crowdIcons[0]
    }).bindPopup(`<strong>${info.title}</strong><br/>${info.description}`);
    marker.addTo(window.leafletMap);
    blinkEffect(marker);
    crowdMarkers[id] = { marker, level };
}

function blinkEffect(marker) {
    const el = marker.getElement();
    if (!el) return;
    el.classList.add('fade-in', 'blink');
    setTimeout(() => el.classList.remove('blink'), 2000);
}

export function showSuggestions(suggestions) {
    suggestions.forEach(s => {
        const icon = L.icon({ iconUrl: "/icons/suggestion-marker.png", iconSize: [32, 32] });
        L.marker([s.latitude, s.longitude], { icon })
            .addTo(window.leafletMap)
            .bindPopup(`<strong>${s.title}</strong><br/>À ${s.distanceKm} km`);
    });
}

const iconCache = {};

// Color according to level
function getLevelColor(level) {
    switch (level) {
        case 1: return "green";    // Weak
        case 2: return "orange";   // Moderate
        case 3: return "red";      // Pupil
        default: return "gray";
    }
}

// Creating a custom icon with a pulsing effect
function getLevelIcon(level) {
    if (iconCache[level]) return iconCache[level];

    const color = getLevelColor(level);
    const icon = L.divIcon({
        className: 'traffic-icon',
        html: `<div class="pulse-${color}"></div>`,
        iconSize: [16, 16],
        iconAnchor: [8, 8],
        popupAnchor: [0, -10]
    });

    iconCache[level] = icon;
    return icon;
}

// Interop global
window.mapInterop = (() => {
    let map = null;
    let markers = [];

    function initMap(mapId = "leaflet-map", lat = 50.894966, lng = 4.341545, zoom = 13) {
        if (map) return;

        const mapElement = document.getElementById(mapId);
        if (!mapElement) {
            console.warn(`❌ Élément #${mapId} introuvable dans le DOM.`);
            return;
        }

        map = L.map(mapId).setView([lat, lng], zoom);
        L.tileLayer('https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png', {
            attribution: '&copy; OpenStreetMap contributors'
        }).addTo(map);
    }

    function updateTrafficMarkers(events = []) {
        if (!map) {
            console.warn("❌ La carte n'est pas encore initialisée.");
            return;
        }

        // Deleting old markers
        markers.forEach(marker => map.removeLayer(marker));
        markers = [];

        // Adding new markers
        events.forEach(e => {
            const marker = L.marker([e.latitude, e.longitude], {
                icon: getLevelIcon(e.level)
            }).bindPopup(`
                <strong>${e.description}</strong><br/>
                ${new Date(e.timestamp).toLocaleString()}
            `);

            marker.addTo(map);
            markers.push(marker);
        });
    }

    return {
        initMap,
        updateTrafficMarkers
    };
})();


































































































/*// Copyrigtht (c) 2025 Citizen Hackathon https://github.com/POLLESSI/Citizenhackathon2025V4.Blazor.Client. All rights reserved.*/