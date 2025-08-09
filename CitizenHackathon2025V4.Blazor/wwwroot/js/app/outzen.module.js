// ===========================
// OUTZEN.MODULE.JS - MAP, ANIMATIONS & SIGNALR
// ===========================

// merged imports
import {
    createBaseMap,
    addCrowdMarkers,
    addSuggestionMarkers,
    addTrafficMarkers
} from './leafletLogic.js';

let mapInstance = null;

export function init(elementId = 'leafletMap', lat = 50.8950, lng = 4.3415, zoom = 13) {
    mapInstance = createBaseMap(elementId, [lat, lng], zoom);
    initAnimatedBackground();
    initScrollAndParallax();
}

export function updateCrowdData(crowdData) {
    if (mapInstance) addCrowdMarkers(mapInstance, crowdData);
}

export function updateSuggestions(suggestions) {
    if (mapInstance) addSuggestionMarkers(mapInstance, suggestions);
}

export function updateTraffic(trafficEvents) {
    if (mapInstance) addTrafficMarkers(mapInstance, trafficEvents);
}

export function updateWeatherForecast(data) {
    console.log("📡 Weather update", data);
    // TODO: apply weather changes if required
}

export function initAnimatedBackground(svgSelector = ".svg-background svg", stop1Id = "stop1", stop2Id = "stop2") {
    const svg = document.querySelector(svgSelector);
    const stop1 = document.getElementById(stop1Id);
    const stop2 = document.getElementById(stop2Id);
    if (!svg || !stop1 || !stop2) return;

    let hue = 0;
    setInterval(() => {
        hue = (hue + 1) % 360;
        stop1.setAttribute("stop-color", `hsl(${hue}, 100%, 60%)`);
        stop2.setAttribute("stop-color", `hsl(${(hue + 120) % 360}, 100%, 60%)`);
    }, 50);

    document.addEventListener("mousemove", e => {
        const x = (e.clientX / window.innerWidth - 0.5) * 20;
        const y = (e.clientY / window.innerHeight - 0.5) * 20;
        svg.style.transform = `rotateX(${y}deg) rotateY(${x}deg) scale(1.05)`;
    });

    document.addEventListener("mouseleave", () => {
        svg.style.transform = "rotateX(0deg) rotateY(0deg)";
    });

    document.addEventListener("scroll", () => {
        const scrollY = window.scrollY;
        const intensity = Math.min(scrollY / 1000, 1);
        svg.style.transform += ` scale(${1 + intensity * 0.05})`;
    });

    document.body.style.background = "linear-gradient(135deg, #000428, #004e92)";
}

export function initScrollAndParallax() {
    const sections = document.querySelectorAll("section.animate-on-scroll, .scroll-reveal");
    const observer = new IntersectionObserver(entries => {
        entries.forEach(entry => {
            if (entry.isIntersecting) {
                entry.target.classList.add("visible");
                observer.unobserve(entry.target);
            }
        });
    }, { threshold: 0.1 });
    sections.forEach(s => observer.observe(s));

    const bg = document.querySelector('.parallax-bg');
    if (bg) {
        document.addEventListener('mousemove', e => {
            const x = (e.clientX / window.innerWidth - 0.5) * 20;
            const y = (e.clientY / window.innerHeight - 0.5) * 20;
            bg.style.transform = `translate(${x}px, ${y}px)`;
        });
    }
}

export async function startSignalR(hubUrl, onDataReceived = {}) {
    const connection = new signalR.HubConnectionBuilder()
        .withUrl(hubUrl, { transport: signalR.HttpTransportType.WebSockets })
        .withAutomaticReconnect()
        .configureLogging(signalR.LogLevel.Information)
        .build();

    if (onDataReceived.crowd)
        connection.on("ReceiveCrowdInfo", onDataReceived.crowd);

    if (onDataReceived.traffic)
        connection.on("ReceiveTrafficInfo", onDataReceived.traffic);

    if (onDataReceived.suggestions)
        connection.on("ReceiveSuggestions", onDataReceived.suggestions);

    if (onDataReceived.weatherForecasts)
        connection.on("ReceiveWeatherForecasts", onDataReceived.weatherForecasts);

    connection.onclose(err => console.warn("SignalR disconnected:", err));

    try {
        await connection.start();
        console.log("✅ SignalR connected.");
    } catch (err) {
        console.error("❌ SignalR connection failed:", err);
        setTimeout(() => startSignalR(hubUrl, onDataReceived), 5000);
    }

    window.outzenConnection = connection;
}
let crowdMarkers = {};
let filterLevel = null;

import {
    createBaseMap,
    addCrowdMarkers,
    addSuggestionMarkers,
    addTrafficMarkers
} from './app/leafletLogic.js/index.js';

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

/*++++++++++++leafletMap.js+++++++++++++++*/
/*+++++++++++++++++++++++++++++++++++++++++*/
let crowdMarkers = {};
let filterLevel = null;

import {
    createBaseMap,
    addCrowdMarkers,
    addSuggestionMarkers,
    addTrafficMarkers
} from './app/leafletLogic.js/index.js';

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
/*++++++++++++++outzenMap.js+++++++++++++++*/
//++++++++++++++++++++++++++++++++++++++++++
import {
    createBaseMap,
    addCrowdMarkers as addInitialCrowdMarkers,
    addSuggestionMarkers as addInitialSuggestionMarkers,
    addTrafficMarkers
} from './utils/leafletLogic.js';

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
    }).bindPopup(`<strong>${info.title}</strong><br/>${info.description}`);

    marker.addTo(map);
    blinkEffect(marker);
    crowdMarkers[id] = { marker, level };

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
        } else {
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
            .bindPopup(`<strong>${s.title}</strong><br/>À ${s.distanceKm} km`);
    });
}

// 📉 Initialize the chart (Chart.js)
export function initCrowdChart() {
    const ctx = document.getElementById('crowdChart')?.getContext('2d');
    if (!ctx) {
        console.warn("📉 Element #crowdChart not found.");
        return;
    }

    window.crowdChart = new Chart(ctx, {
        type: 'line',
        data: {
            labels: [],
            datasets: [{
                label: 'Crowd Level (%)',
                data: [],
                borderColor: '#FFD700',
                backgroundColor: 'rgba(255,215,0,0.3)'
            }]
        },
        options: {
            scales: {
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
//++++++++++++++trafficInterop.js++++++++++++++
/*+++++++++++++++++++++++++++++++++++++++++++++*/
const iconCache = {};

function getLevelColor(level) {
    switch (level) {
        case 1: return "green";
        case 2: return "orange";
        case 3: return "red";
        default: return "gray";
    }
}

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

window.trafficInterop = (() => {
    let map = null;
    let markers = [];

    function initMap(mapId = "leaflet-map", lat = 50.894966, lng = 4.341545, zoom = 13) {
        if (map) return;
        const mapElement = document.getElementById(mapId);
        if (!mapElement) return console.warn(`❌ Element #${mapId} not found.`);
        map = L.map(mapId).setView([lat, lng], zoom);
        L.tileLayer('https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png', {
            attribution: '&copy; OpenStreetMap contributors'
        }).addTo(map);
    }

    function updateTrafficMarkers(events = []) {
        if (!map) return console.warn("❌ The card is not initialized.");
        markers.forEach(marker => map.removeLayer(marker));
        markers = [];
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
//+++++++++++++++++++ mapInterop.js++++++++++++++++++++
/*+++++++++++++++++++++++++++++++++++++++++++++++++++++*/
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
            console.warn(`❌ Element #${mapId} not found in the DOM.`);
            return;
        }

        map = L.map(mapId).setView([lat, lng], zoom);
        L.tileLayer('https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png', {
            attribution: '&copy; OpenStreetMap contributors'
        }).addTo(map);
    }

    function updateTrafficMarkers(events = []) {
        if (!map) {
            console.warn("❌ The card is not yet initialized.");
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
/*++++++++++++++++++SignalRInterop.js++++++++++++++++++++*/
/*+++++++++++++++++++++++++++++++++++++++++++++++++++++++*/
window.signalrInterop = {
    startConnection: async (hubUrl) => {
        const connection = new signalR.HubConnectionBuilder()
            .withUrl("https://localhost:7254/hub/outzen", {
                withCredentials: true
            })
            .withAutomaticReconnect()
            .build();

        connection.on("NewCrowdEvent", (data) => {
            const shape = {
                type: data.type,
                x: data.x,
                y: data.y,
                size: data.size || 60,
                radius: data.radius || 40,
                height: data.height || 60
            };

            if (window.GeometryCanvas) {
                window.GeometryCanvas.addShape(shape);
            }
        });

        try {
            await connection.start();
            console.log(`✅ SignalR connected to ${hubUrl}`);
        } catch (err) {
            console.error("❌ SignalR error:", err);
        }
    }
};
/*+++++++++++++++V2+++++++++++++++++*/
/*++++++++++++++++++++++++++++++++++*/
export async function startSignalR(connection) {
    await connection.start();

    console.log("✅ SignalR connected to OutZenHub");
}
/*+++++++++++++++++++Simulator.js++++++++++++++++++++*/
/*+++++++++++++++++++++++++++++++++++++++++++++++++++*/
// SignalR Automatic Event Simulator

function simulateCrowdEvent() {
    const fakeCrowdData = [
        {
            latitude: 50.895 + (Math.random() - 0.5) * 0.01,
            longitude: 4.3415 + (Math.random() - 0.5) * 0.01,
            level: Math.floor(Math.random() * 3) + 1,
            name: "📍 Tourist area",
            description: "Crowd detected"
        }
    ];
    console.log("👥 Crowd simulation:", fakeCrowdData);
    if (window.crowdInterop) {
        crowdInterop.init();
        crowdInterop.update(fakeCrowdData);
    }
}

function simulateTrafficEvent() {
    const fakeTrafficData = [
        {
            latitude: 50.895 + (Math.random() - 0.5) * 0.01,
            longitude: 4.3415 + (Math.random() - 0.5) * 0.01,
            level: Math.floor(Math.random() * 3) + 1,
            description: "🚦 Simulated traffic",
            timestamp: new Date().toISOString()
        }
    ];
    console.log("🚦 Traffic Simulation:", fakeTrafficData);
    if (window.trafficInterop) {
        trafficInterop.updateTrafficMarkers(fakeTrafficData);
    }
}

function simulateWeatherForecast() {
    const fakeWeatherData = [
        {
            location: "Brussels",
            temperature: Math.floor(Math.random() * 30) + 10,
            condition: "Sunny",
            timestamp: new Date().toISOString()
        }
    ];
    console.log("🌤️ Weather Simulation:", fakeWeatherData);
    if (window.weatherInterop) {
        weatherInterop.updateWeatherForecasts(fakeWeatherData);
    }
}

function simulateSuggestions() {
    const fakeSuggestions = [
        {
            title: "Improve public transport",
            description: "Increase bus frequency during peak hours",
            latitude: 50.895 + (Math.random() - 0.5) * 0.01,
            longitude: 4.3415 + (Math.random() - 0.5) * 0.01
        }
    ];
    console.log("💡 Suggestions Simulation:", fakeSuggestions);
    if (window.suggestionsInterop) {
        suggestionsInterop.updateSuggestions(fakeSuggestions);
    }
}

function simulateAllEvents() {
    simulateCrowdEvent();
    simulateTrafficEvent();
    simulateWeatherForecast();
    simulateSuggestions();
}

// Launches simulators every 5 seconds
setInterval(() => {
    simulateCrowdEvent();
    simulateTrafficEvent();
}, 5000);

setInterval(() => {
    simulateWeatherForecast();
    simulateSuggestions();
}, 10000);

// Initializing the animated background

function initAnimatedBackground(svgSelector = ".svg-background svg", stop1Id = "stop1", stop2Id = "stop2") {
    const svg = document.querySelector(svgSelector);
    const stop1 = document.getElementById(stop1Id);
    const stop2 = document.getElementById(stop2Id);
    if (!svg || !stop1 || !stop2) return;
    let hue = 0;
    setInterval(() => {
        hue = (hue + 1) % 360;
        stop1.setAttribute("stop-color", `hsl(${hue}, 100%, 60%)`);
        stop2.setAttribute("stop-color", `hsl(${(hue + 120) % 360}, 100%, 60%)`);
    }, 50);
    document.addEventListener("mousemove", e => {
        const x = (e.clientX / window.innerWidth - 0.5) * 20;
        const y = (e.clientY / window.innerHeight - 0.5) * 20;
        svg.style.transform = `rotateX(${y}deg) rotateY(${x}deg) scale(1.05)`;
    });
    document.addEventListener("mouseleave", () => {
        svg.style.transform = "rotateX(0deg) rotateY(0deg)";
    });
    document.addEventListener("scroll", () => {
        const scrollY = window.scrollY;
        const intensity = Math.min(scrollY / 1000, 1);
        svg.style.transform += ` scale(${1 + intensity * 0.05})`;
    });
    document.body.style.background = "linear-gradient(135deg, #000428, #004e92)";
}

//++++++++++++++main.js+++++++++++++++++++++
/*+++++++++++++++++++++++++++++++++++++++++*/
import { ThemeManager } from './utils/theme-manager.js/index.js';
import * as Outzen from './app/outzen.module.js/index.js';
import "core-js/stable";
import "regenerator-runtime/runtime";

// Initializing the theme
ThemeManager.init();

// Initializing the main board
Outzen.init();

// Initializing visual effects
Outzen.initAnimatedBackground();
Outzen.initScrollAndParallax();

// Connecting to SignalR
Outzen.startSignalR("/hub/outzen", {
    crowd: data => Outzen.updateCrowdData(data),
    traffic: data => Outzen.updateTraffic(data),
    suggestions: data => Outzen.updateSuggestions(data),
    weatherForecasts: data => Outzen.updateWeatherForecast(data)
});
/*++++++++++++++++++init.js++++++++++++++++++++*/
/*+++++++++++++++++++++++++++++++++++++++++++++*/
import {
    initMap,
    startSignalR,
    updateCrowdData,
    updateTraffic,
    updateSuggestions,
    initAnimatedBackground,
    initScrollAndParallax
} from './app/outzen.module.js/index.js';

import { ThemeManager } from './utils/theme-manager.js/index.js';
ThemeManager.init();

// Initialize the Leaflet map
const map = initMap('leafletMap');

// Initialize visual effects
initAnimatedBackground();
initScrollAndParallax();

// SignalR connection with specific callbacks
startSignalR("https://localhost:7254/hub/outzen", {
    crowd: data => {
        console.log("👥 Crowd Update :", data);
        updateCrowdData(data);
    },
    traffic: data => {
        console.log("🚦 Traffic Update :", data);
        updateTraffic(data);
    },
    suggestions: data => {
        console.log("💡 Update suggestions :", data);
        updateSuggestions(data);
    },
    weatherforecast: data => {
        console.log("Weather forecast received:", data);
        if (window.GeometryCanvas) {
            window.GeometryCanvas.addWeatherForecast(data);
        }
    });















































































/*// Copyrigtht (c) 2025 Citizen Hackathon https://github.com/POLLESSI/Citizenhackathon2025V4.Blazor.Client. All rights reserved.*/
