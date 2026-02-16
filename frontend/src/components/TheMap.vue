<script setup lang="ts">
import SettingsPanel from './SettingsPanel.vue';
import FiltersPanel from './FiltersPanel.vue';
import BranchPopup from './BranchPopup.vue';

import { onMounted, ref, shallowRef, watch, computed, createApp } from 'vue';
import { useRoute } from 'vue-router';
import maplibregl from 'maplibre-gl';

import { useBranchStore } from '@/stores/branchStore';
import type { FeatureCollection, Point } from 'geojson';
import 'maplibre-gl/dist/maplibre-gl.css';
import '@/assets/map-styles.css';
import '@/assets/customSelect.css'

const route = useRoute();
const isAdmin = computed(() => route.meta.isAdmin);

interface MapMarkerProperties {
    id: number;
    name: string;
    type: string; 
    isClosed: boolean;
    cluster?: boolean;
    cluster_id?: number;
    point_count?: number;
}

const mapContainer = ref<HTMLDivElement | null>(null);
const map = shallowRef<maplibregl.Map | null>(null);
const activePopups = shallowRef<maplibregl.Popup[]>([]);
const branchStore = useBranchStore();
const isSourceReady = ref(false);

const zoomToCity = (city: string) => {
    const activeMap = map.value;
    if (!activeMap || city === 'all') return;

    const cityBranches = branchStore.rawBranches.filter(b => b.baseCity === city);
    if (!cityBranches.length) return;

    activeMap.stop(); 
    activeMap.setPadding({ top: 0, bottom: 0, left: 0, right: 0 });

    const bounds = new maplibregl.LngLatBounds();

    cityBranches.forEach(b => {
        bounds.extend([b.longitude, b.latitude]);
    });

    activeMap.fitBounds(bounds, {
        padding: { top: 80, bottom: 80, left: 80, right: 80 }, 
        minZoom: 12,
        maxZoom: 13,
        duration: 800,
        essential: true 
    });
};

const handleDistrictToggle = (isVisible: boolean) => {
    const activeMap = map.value;
    if (!activeMap) return;
    const classes = ["continent", "hamlet", "isolated_dwelling"];
    if (isVisible) classes.push("neighbourhood");
    activeMap.setFilter('place_other', ["all", ["in", "class", ...classes], ["==", "$type", "Point"]]);
};

const renderMarkers = () => {
    const activeMap = map.value;
    if (!activeMap || !isSourceReady.value) return;

    const geojsonData: FeatureCollection<Point, MapMarkerProperties> = {
        type: 'FeatureCollection',
        features: branchStore.filteredBranches.map(b => ({
            type: 'Feature',
            geometry: { 
                type: 'Point', 
                coordinates: [b.longitude, b.latitude] as [number, number] 
            },
            properties: { 
                id: b.id, 
                name: b.name, 
                type: b.departmentType, 
                isClosed: b.isTemporaryClosed 
            }
        }))
    };

    const source = activeMap.getSource('branches-source') as maplibregl.GeoJSONSource;
    if (source) source.setData(geojsonData);
};

onMounted(async () => {
    if (!mapContainer.value) return;

    const m = new maplibregl.Map({
        container: mapContainer.value,
        style: 'https://tile.openstreetmap.org.ua/styles/positron-gl-style/style.json',
        center: [30.5234, 50.4501],
        zoom: 6,
        minZoom: 5.8
    });
    map.value = m;

    m.addControl(new maplibregl.NavigationControl(), 'top-right');
    
    m.on('load', async () => {
        handleDistrictToggle(false);

        m.addSource('branches-source', {
            type: 'geojson',
            data: { type: 'FeatureCollection', features: [] },
            cluster: true,
            clusterMaxZoom: 14,
            clusterRadius: 50
        });

        m.addLayer({
            id: 'clusters',
            type: 'circle',
            source: 'branches-source',
            filter: ['has', 'point_count'],
            paint: {
                'circle-color': ['step', ['get', 'point_count'], '#51bbd6', 10, '#f1f075', 50, '#f28cb1'],
                'circle-radius': ['step', ['get', 'point_count'], 20, 10, 30, 50, 40]
            }
        });

        m.addLayer({
            id: 'cluster-count',
            type: 'symbol',
            source: 'branches-source',
            filter: ['has', 'point_count'],
            layout: { 'text-field': ['get', 'point_count_abbreviated'], 'text-size': 12 }
        });

        m.addLayer({
            id: 'unclustered-point',
            type: 'circle',
            source: 'branches-source',
            filter: ['!', ['has', 'point_count']],
            paint: {
                'circle-color': [
                    'case',
                    ['boolean', ['get', 'isClosed'], false], '#6c757d',
                    ['match', ['get', 'type'],
                        'department', '#007bff',
                        'atm', '#28a745',
                        'terminal', '#ffc107',
                        '#333'
                    ]
                ],
                'circle-radius': 8,
                'circle-stroke-width': 2,
                'circle-stroke-color': '#fff'
            }
        });

        isSourceReady.value = true;
        await branchStore.fetchBranches();
        renderMarkers();
    });

    m.on('click', 'clusters', async (e) => {
        const features = m.queryRenderedFeatures(e.point, { layers: ['clusters'] });
        const clusterFeature = features[0];
        if (!clusterFeature?.properties || !clusterFeature.geometry) return;

        const clusterId = clusterFeature.properties.cluster_id;
        const source = m.getSource('branches-source') as maplibregl.GeoJSONSource;

        if (source && typeof clusterId === 'number') {
            const zoom = await source.getClusterExpansionZoom(clusterId);
            m.easeTo({ center: (clusterFeature.geometry as Point).coordinates as [number, number], zoom });
        }
    });

    m.on('click', 'unclustered-point', (e) => {
        const pointFeature = e.features?.[0];
        if (!pointFeature?.properties) return;

        const props = pointFeature.properties as MapMarkerProperties;
        const branch = branchStore.rawBranches.find(b => b.id === props.id);
        branchStore.selectedBranch = branch || null;

        if (branch && map.value) {
            activePopups.value.forEach(p => p.remove());
            activePopups.value = [];

            //Mount BranchPopup
            const container = document.createElement('div');

            const app = createApp(BranchPopup, { branch });
            app.mount(container);

            const popup = new maplibregl.Popup({
                offset: 15,
                maxWidth: 'none',
                anchor: 'bottom'
            })
                .setLngLat([branch.longitude, branch.latitude])
                .setDOMContent(container)
                .addTo(map.value);

            popup.on('close', () => {
                app.unmount();
            });

            activePopups.value.push(popup);

            m.flyTo({
                center: [branch.longitude, branch.latitude],
                padding: { top: 600, bottom: 0, left: 0, right: 0 },
                speed: 0.8,
                zoom: 14
            });
        }
    });

    m.on('contextmenu', () => {
        branchStore.selectedBranch = null;
        activePopups.value.forEach(p => p.remove());
        activePopups.value = [];
    });

    watch(() => branchStore.filterCity,(city) => {
            activePopups.value.forEach(p => p.remove());
            activePopups.value = [];

            renderMarkers();
            zoomToCity(city);
        }
    );
    
    watch([() => branchStore.filterType, () => branchStore.rawBranches], 
        () => {
            activePopups.value.forEach(p => p.remove());
            activePopups.value = [];
            renderMarkers();
        }, 
        { deep: true }
    );
    
    watch(() => branchStore.selectedBranch, (newSelected) => {
        if (!newSelected) {
            activePopups.value.forEach(p => p.remove());
            activePopups.value = [];
        }
    })
});
</script>

<template>
  <div class="map-wrap">
    <div class="tools-container">
        <SettingsPanel @toggle-districts="handleDistrictToggle" v-if="isAdmin" />
        <FiltersPanel/>
    </div>
    <div ref="mapContainer" class="map-container"></div>
  </div>
</template>