<script setup lang="ts">
import { onMounted, ref, shallowRef, watch } from 'vue';
import maplibregl from 'maplibre-gl';
import { useBranchStore } from '@/stores/branchStore';
import type { FeatureCollection, Point } from 'geojson';
import 'maplibre-gl/dist/maplibre-gl.css';
import '@/assets/map-styles.css';

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
const branchStore = useBranchStore();
const isSourceReady = ref(false);

//Render markers from pinia storage
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

    //Define new Map instance + Load map style and data from tile.openstreetmap source
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

    //Filter unwanted districts
        m.setFilter('place_other', [
            "all",
            [
                "in",
                "class",
                "continent",
                "hamlet",
                "isolated_dwelling",
            ],
            [
                "==",
                "$type",
                "Point"
            ]
        ])

        //Clusterisation
        m.addSource('branches-source', {
            type: 'geojson',
            data: { type: 'FeatureCollection', features: [] },
            cluster: true,
            clusterMaxZoom: 14,
            clusterRadius: 50
        });

        //Create Clusters
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

        //Branches
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

        //After layers creation fetch branches
        isSourceReady.value = true;
        await branchStore.fetchBranches();
        renderMarkers();
    });

    //Zoom on Clusters
    m.on('click', 'clusters', async (e) => {
        const features = m.queryRenderedFeatures(e.point, { layers: ['clusters'] });
    
        const clusterFeature = features[0];

        if (!clusterFeature?.properties || !clusterFeature.geometry) return;

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

    //Select branch for popup
    m.on('click', 'unclustered-point', (e) => {
        const pointFeature = e.features?.[0];

        if (!pointFeature?.properties) return;

        const props = pointFeature.properties as BranchProperties;
        const branch = branchStore.branches.find(b => b.id === props.id);
        
        if (branch) {
            branchStore.selectedBranch = branch;
            
            new maplibregl.Popup({ offset: 15 })
                .setLngLat([branchStore.selectedBranch.longitude, branchStore.selectedBranch.latitude])
                .setHTML(`<b>${props.name}</b><br>${props.address}`)
                .addTo(m);
        }
    });

    //Right click on map canvas
    // m.on('contextmenu', () => {
    //     branchStore.selectedBranch = null;
    //     activePopups.value.forEach(p => p.remove());
    //     activePopups.value = [];
    // });

    //Call markers render after branches data was fetched 
    watch(() => branchStore.branches, () => {
        renderMarkers();
    }, { deep: true });

});
</script>

<template>
  <div class="map-wrap">
    <div ref="mapContainer" class="map-container"></div>
  </div>
</template>