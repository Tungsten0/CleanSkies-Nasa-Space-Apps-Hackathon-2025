// --- Map ---
const map = L.map('map').setView([56.1304, -106.3468], 4);

// Limit movement & zoom to Canada
const canadaBounds = [
  [41.7, -141.0],
  [83.1, -52.6]
];
map.setMaxBounds(canadaBounds);
map.on('drag', () => map.panInsideBounds(canadaBounds, { animate: false }));
map.setMinZoom(3);
map.setMaxZoom(12);

// Tile layer
L.tileLayer('https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png', {
  attribution: '&copy; OpenStreetMap contributors'
}).addTo(map);




// --- Sample stations (replace with real data later) ---
const stations = [
  { city: "Windsor Downtown", lat: 42.314, lon: -83.04, aqi: 55,  temp: 22, wind: "15 km/h" },
  { city: "West Windsor",     lat: 42.290, lon: -83.08, aqi: 120, temp: 21, wind: "12 km/h" },
  { city: "Tecumseh",         lat: 42.320, lon: -82.90, aqi: 38,  temp: 23, wind: "10 km/h" }
];

// --- Helpers for AQI ---
function aqiCategory(aqi) {
  if (aqi <= 50)  return { label: "Good",                         color: "#86efac" }; // green-300
  if (aqi <= 100) return { label: "Moderate",                     color: "#fde68a" }; // yellow-300
  if (aqi <= 150) return { label: "Unhealthy (Sensitive Groups)", color: "#fdba74" }; // orange-300
  if (aqi <= 200) return { label: "Unhealthy",                    color: "#f87171" }; // red-400
  return             { label: "Very Unhealthy",                   color: "#c4b5fd" }; // purple-300
}
function adviceFor(aqi) {
  if (aqi <= 50)  return "Air quality is good — enjoy outdoor activities.";
  if (aqi <= 100) return "Moderate — sensitive people should limit prolonged exertion.";
  if (aqi <= 150) return "Sensitive groups: reduce prolonged or heavy exertion outdoors.";
  if (aqi <= 200) return "Unhealthy — everyone should reduce prolonged outdoor exertion.";
  return "Very Unhealthy — avoid outdoor activity if possible.";
}

// --- Sidebar refs ---
const sidebar = document.getElementById('sidebar');
const closeBtn = document.getElementById('closeSidebar');
closeBtn?.addEventListener('click', () => sidebar.style.transform = 'translateX(-100%)');

// --- Add colored markers + wire click handler ---
stations.forEach(s => {
  const cat = aqiCategory(s.aqi);
  const marker = L.circleMarker([s.lat, s.lon], {
    radius: 10,
    color: cat.color,
    fillColor: cat.color,
    fillOpacity: 0.9
  }).addTo(map);

  marker.bindPopup(`${s.city}<br/>AQI: ${s.aqi} (${cat.label})`);

  marker.on('click', () => {
    // Fill sidebar
    document.getElementById('cityName').textContent = s.city;
    document.getElementById('aqiValue').textContent = s.aqi;
    document.getElementById('aqiCat').textContent = cat.label;
    document.getElementById('aqiBox').style.background = cat.color;
    document.getElementById('advice').textContent = adviceFor(s.aqi);
    document.getElementById('temp').textContent = `${s.temp}°C`;
    document.getElementById('wind').textContent = s.wind;
    document.getElementById('source').textContent = 'OpenAQ (sample)';
    document.getElementById('updated').textContent = `Updated: ${new Date().toLocaleTimeString()}`;
``
    // Slide sidebar in
    sidebar.style.transform = 'translateX(0)';
  });
});
