<!-- Leaflet CSS -->
<link
  rel="stylesheet"
  href="https://unpkg.com/leaflet@1.9.4/dist/leaflet.css"
  integrity="sha256-p4NxAoJBhIIN+hmNHrzRCf9tD/miZyoHS5obTRR9BMY="
  crossorigin=""
/>

<!-- Chart.js -->
<script src="https://cdn.jsdelivr.net/npm/chart.js@4.4.3/dist/chart.umd.min.js" defer></script>

<!-- Leaflet JS -->
<script
  src="https://unpkg.com/leaflet@1.9.4/dist/leaflet.js"
  integrity="sha256-20nQCchB9co0qIjJZRGuk2/Z9VM+kNiyxNV1lvTlZBo="
  crossorigin=""
  defer
></script>

<style>
  /* Responsive 3x3 grid with the map in the center */
  .aq-grid {
    display: grid;
    grid-template-columns: 1fr minmax(420px, 1.4fr) 1fr; /* left, center (map), right */
    grid-template-rows: auto 520px auto;                  /* top charts, map row, bottom charts */
    gap: 1rem;
  }
  .aq-card {
    background: #ffffff;
    border-radius: 12px;
    box-shadow: 0 2px 16px rgba(0,0,0,0.08);
    padding: 12px;
    min-height: 180px;
  }
  #map {
    width: 100%;
    height: 100%;
    min-height: 420px;
    border-radius: 12px;
  }
  /* Mobile: stack everything, put map first */
  @media (max-width: 900px) {
    .aq-grid {
      grid-template-columns: 1fr;
      grid-template-rows: auto;
    }
  }
</style>

