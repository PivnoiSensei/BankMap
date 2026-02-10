<script setup lang="ts">
import SettingsPanel from './SettingsPanel.vue';
import FiltersPanel from './FiltersPanel.vue';
import { onMounted, ref, shallowRef, watch } from 'vue';
import maplibregl from 'maplibre-gl';
import { useBranchStore, type BankMarker, type WorkDay } from '@/stores/branchStore';
import type { FeatureCollection, Point } from 'geojson';
import 'maplibre-gl/dist/maplibre-gl.css';
import '@/assets/map-styles.css';

// Строгий интерфейс свойств маркера для GeoJSON (плоская структура)
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

const createPopupHtml = (branch: BankMarker): string => {
    const d = branch.details;
    // Находим основной график и график кассы
    const deptTable = d?.TimeTables?.find(t => t.Workstation === 'department');
    const cashTable = d?.TimeTables?.find(t => t.Workstation === 'cashDepartment');
    
    const workDays = deptTable?.WorkDays || [];

    const today = new Date().getDay(); 
    const currentDayIndex = today === 0 ? 6 : today - 1;

    const currentDayData = workDays[currentDayIndex];
    const currentCashDay = cashTable?.WorkDays.find(cd => cd.WorkingDay === currentDayData?.WorkingDay);

    const daysNames = ["Понеділок", "Вівторок", "Середа", "Четвер", "П'ятниця", "Субота", "Неділя"];

    const daysOptions = daysNames.map((name, i) => {
        const isSelected = i === currentDayIndex ? 'selected' : '';
        return `<option value="${i}" ${isSelected}>${name}</option>`;
    }).join('');


    return `
    <div class="map-popup">
        <div class="popup-header" style="display: flex; justify-content: space-between; align-items: center; gap: 10px; margin-bottom: 10px;">
            <strong style="font-size: 1.1em; color: #007bff;">${branch.name}</strong>
            <select class="popup-day-select" data-branch-id="${branch.id}" style="padding: 2px; border-radius: 4px;">
                 ${daysOptions}
            </select>
        </div>

        <div class="popup-body" style="margin-bottom: 10px; font-size: 0.9em;">
            <div><b>Місто:</b> ${branch.baseCity}</div>
            <div><b>Адреса:</b> ${branch.fullAddress}</div>
            ${d?.Address?.DetailedAddress ? `<div><b>ℹ️ Помітка:</b> ${d.Address.DetailedAddress}</div>` : ''}
        </div>

         <div class="popup-footer" id="popup-content-${branch.id}" style="background: #f8f9fa; padding: 8px; border-radius: 4px; font-size: 0.85em;">
            ${currentDayData ? renderWorkTime(currentDayData, currentCashDay) : '<i>Графік на сьогодні відсутній</i>'}
        </div>
    </div>
    `;
};

const renderWorkTime = (day: WorkDay, cashDay?: WorkDay) => {
    const breaks = day.Breaks?.length 
        ? day.Breaks.map(b => `${b.BreakFrom}-${b.BreakTo}`).join(', ') 
        : '—';

    return `
        <div style="margin-bottom: 4px;"><b>Відділення:</b> ${day.WorkFrom} - ${day.WorkTo}</div>
        <div style="margin-bottom: 4px;"><b>Перерва:</b> ${breaks}</div>
        <div style="color: #28a745;"><b>Каса:</b> ${cashDay ? `${cashDay.WorkFrom} - ${cashDay.WorkTo}` : 'не працює'}</div>
    `;
};

const zoomToCity = (city: string) => {
    const activeMap = map.value;
    if (!activeMap || city === 'all') return;

    const cityBranches = branchStore.rawBranches.filter(b => b.baseCity === city);
    if (!cityBranches.length) return;

    const bounds = new maplibregl.LngLatBounds();

    cityBranches.forEach(b => {
        bounds.extend([b.longitude, b.latitude]);
    });

    activeMap.fitBounds(bounds, {
        padding: 80,
        maxZoom: 13,
        duration: 800
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


            const popup = new maplibregl.Popup({ offset: 15 })
                .setLngLat([branch.longitude, branch.latitude])
                .setHTML(createPopupHtml(branch))
                .addTo(map.value);
            activePopups.value.push(popup);
        }
    });

    m.on('contextmenu', () => {
        branchStore.selectedBranch = null;
        activePopups.value.forEach(p => p.remove());
        activePopups.value = [];
    });

    mapContainer.value?.addEventListener('change', (event) => {

        const target = event.target as HTMLSelectElement;
        
        if (target && target.classList.contains('popup-day-select')) {
            const branchId = Number(target.dataset.branchId);
            const dayIndex = Number(target.value);
            
            const branch = branchStore.rawBranches.find(b => b.id === branchId);
            const displayDiv = document.getElementById(`popup-content-${branchId}`);
            
            if (branch && displayDiv) {
                const d = branch.details;
                const deptDays = d?.TimeTables?.find(t => t.Workstation === 'department')?.WorkDays || [];
                const cashDays = d?.TimeTables?.find(t => t.Workstation === 'cashDepartment')?.WorkDays || [];
                
                const selectedDay = deptDays[dayIndex];
                const selectedCashDay = cashDays.find(cd => cd.WorkingDay === selectedDay?.WorkingDay);
                
                if (selectedDay) {
                    displayDiv.innerHTML = renderWorkTime(selectedDay, selectedCashDay);
                }else{
                    displayDiv.innerHTML = 'Вихідний';
                }
            }
        }
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
        <SettingsPanel @toggle-districts="handleDistrictToggle" />
        <FiltersPanel/>
    </div>
    <div ref="mapContainer" class="map-container"></div>
  </div>
</template>