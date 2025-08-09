export function createBaseMap(mapId, center = [50.89, 4.34], zoom = 13) {
    const el = document.getElementById(mapId);
    if (!el) throw new Error(`❌ Element #${mapId} not found.`);
    const map = L.map(mapId).setView(center, zoom);
    L.tileLayer('https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png', {
        attribution: '&copy; OpenStreetMap contributors'
    }).addTo(map);
    return map;
}

export function addCrowdMarkers(map, crowdEvents = []) {
    crowdEvents.forEach(e => {
        const marker = L.circleMarker([e.latitude, e.longitude], {
            radius: 8,
            color: getCrowdColor(e.level),
            fillOpacity: 0.5
        }).bindPopup(`<strong>${e.name}</strong><br/>${e.description}`);
        marker.addTo(map);
    });
}

function getCrowdColor(level) {
    switch (level) {
        case 1: return 'green';
        case 2: return 'orange';
        case 3: return 'red';
        default: return 'gray';
    }
}

export function addSuggestionMarkers(map, suggestions = []) {
    suggestions.forEach(s => {
        const marker = L.marker([s.latitude, s.longitude])
            .bindPopup(`<strong>${s.title}</strong><br/>${s.description}`);
        marker.addTo(map);
    });
}

const iconCache = {};

export function addTrafficMarkers(map, trafficEvents = []) {
    trafficEvents.forEach(e => {
        const marker = L.marker([e.latitude, e.longitude], {
            icon: getTrafficIcon(e.level)
        }).bindPopup(`<strong>${e.description}</strong><br/>${new Date(e.timestamp).toLocaleString()}`);
        marker.addTo(map);
    });
}

function getTrafficIcon(level) {
    if (iconCache[level]) return iconCache[level];
    const color = getTrafficColor(level);
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

function getTrafficColor(level) {
    switch (level) {
        case 1: return 'green';
        case 2: return 'orange';
        case 3: return 'red';
        default: return 'gray';
    }
}


















































































/*// Copyrigtht (c) 2025 Citizen Hackathon https://github.com/POLLESSI/Citizenhackathon2025V4.Blazor.Client. All rights reserved.*/
