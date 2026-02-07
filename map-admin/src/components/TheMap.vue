<script setup lang="ts">
import SettingsPanel from './SettingsPanel.vue';
import { onMounted, ref, shallowRef, watch } from 'vue';
import maplibregl from 'maplibre-gl';
import { useBranchStore } from '@/stores/branchStore';
import type { FeatureCollection, Point } from 'geojson'; // Добавьте типы
import 'maplibre-gl/dist/maplibre-gl.css';
import '@/assets/map-styles.css';

// Типизируем свойства ваших точек
interface BranchProperties {
    id: number;
    name: string;
    address: string;
    status: string;
    cluster?: boolean;
    cluster_id?: number;
    point_count?: number;
}

const mapContainer = ref<HTMLDivElement | null>(null);
const map = shallowRef<maplibregl.Map | null>(null);
const draftMarker = shallowRef<maplibregl.Marker | null>(null);
const branchStore = useBranchStore();
const isSourceReady = ref(false);

const handleDistrictToggle = (isVisible: boolean) => {
    const activeMap = map.value;
    if (!activeMap) return;
    
    const visibility = isVisible ? 'visible' : 'none';
    const districtLayers = ['place_suburb', 'place_neighbourhood', 'place_village', 'place_hamlet'];
    districtLayers.forEach(id => {
        if (activeMap.getLayer(id)) activeMap.setLayoutProperty(id, 'visibility', visibility);
    });
};

const updateDraftMarker = (lng: number, lat: number) => {
    const activeMap = map.value;
    if (!activeMap) return;

    if (!draftMarker.value) {
        draftMarker.value = new maplibregl.Marker({ color: '#aaaaaa', draggable: true })
            .setLngLat([lng, lat])
            .addTo(activeMap);
            
        draftMarker.value.on('dragend', () => {
            const pos = draftMarker.value?.getLngLat();
            if (pos) {
                branchStore.draftCoords = { lat: Number(pos.lat.toFixed(7)), lng: Number(pos.lng.toFixed(7)) };
            }
        });
    } else {
        draftMarker.value.setLngLat([lng, lat]);
    }
};

const renderMarkers = () => {
    const activeMap = map.value;
    if (!activeMap || !isSourceReady.value) return;

    const geojsonData: FeatureCollection<Point, BranchProperties> = {
        type: 'FeatureCollection',
        features: branchStore.branches.map(b => ({
            type: 'Feature',
            geometry: { type: 'Point', coordinates: [b.longitude, b.latitude] },
            properties: { id: b.id, name: b.name, address: b.address, status: b.status }
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
        zoom: 6
    });
    map.value = m;

    m.addControl(new maplibregl.NavigationControl(), 'top-right');
    
    m.on('load', async () => {
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
                    'match', ['get', 'status'],
                    'Active', '#28a745', 'Reconstruction', '#fd7e14', 'Closed', '#dc3545', '#6c757d'
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
        
        // 1. Извлекаем первый элемент в константу
        const clusterFeature = features[0];

        // 2. Делаем проверку на существование и наличие свойств
        if (!clusterFeature?.properties || !clusterFeature.geometry) return;

        // 3. Теперь TS уверен, что свойства есть
        const clusterId = clusterFeature.properties.cluster_id as number;
        const source = m.getSource('branches-source') as maplibregl.GeoJSONSource;

        if (source && typeof clusterId === 'number') {
            const zoom = await source.getClusterExpansionZoom(clusterId);
            const geometry = clusterFeature.geometry as Point;
            
            m.easeTo({ 
                center: geometry.coordinates as [number, number], 
                zoom 
            });
        }
    });

    m.on('click', 'unclustered-point', (e) => {
        // Извлекаем первый элемент массива из события
        const pointFeature = e.features?.[0];

        // Проверяем наличие объекта и его свойств
        if (!pointFeature?.properties) return;

        // Приводим свойства к вашему интерфейсу
        const props = pointFeature.properties as BranchProperties;
        const branch = branchStore.branches.find(b => b.id === props.id);
        
        if (branch) {
            branchStore.selectedBranch = branch;
            new maplibregl.Popup({ offset: 15 })
                .setLngLat(e.lngLat)
                .setHTML(`<b>${props.name}</b><br>${props.address}`)
                .addTo(m);
        }
    });

    m.on('click', (e) => {
        const features = m.queryRenderedFeatures(e.point, { layers: ['unclustered-point', 'clusters'] });
        if (features.length > 0) return;

        branchStore.selectedBranch = null;
        branchStore.draftCoords = { lat: Number(e.lngLat.lat.toFixed(7)), lng: Number(e.lngLat.lng.toFixed(7)) };
        updateDraftMarker(e.lngLat.lng, e.lngLat.lat);
    });

    m.on('contextmenu', () => {
        if (draftMarker.value) { draftMarker.value.remove(); draftMarker.value = null; }
        branchStore.selectedBranch = null;
        branchStore.draftCoords = { lat: 0, lng: 0 };
    });

    watch(() => branchStore.branches, () => {
        if (draftMarker.value) { draftMarker.value.remove(); draftMarker.value = null; }
        renderMarkers();
    }, { deep: true });

    watch(() => branchStore.selectedBranch, (val) => {
        if (val && draftMarker.value) { draftMarker.value.remove(); draftMarker.value = null; }
    });
});
</script>

<template>
  <div class="map-wrap">
    <SettingsPanel @toggle-districts="handleDistrictToggle" />
    <div ref="mapContainer" class="map-container"></div>
  </div>
</template>