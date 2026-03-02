<script setup lang="ts">
import { useBranchStore } from '@/stores/branchStore';
import { computed } from 'vue';
import '@/assets/filters-panel.css'
const branchStore = useBranchStore();

const sortedCities = computed(() => {
    return [...branchStore.uniqueCities].sort((a, b) => {
        if (a === 'all') return -1;
        if (b === 'all') return 1;
        
        return a.localeCompare(b);
    })
}) 

</script>

<template>
    <div class="filters-panel">
        <h4>Фільтрація даних</h4>
        <div class="filters-section">
            <div class="filter-group">
                <label>Місто:</label>
                <select v-model="branchStore.filterCity">
                <option v-for="city in sortedCities" :key="city" :value="city" >
                    {{ city === 'all' ? 'Усі міста' : city }}
                </option>
                </select>
            </div>

            <div class="filter-group">
                <label>Тип об'єкта:</label>
                <select v-model="branchStore.filterType">
                <option value="all">Усі типи</option>
                <option value="Department">Відділення</option>
                <option value="Atm">Банкомати</option>
                <option value="Terminal">Термінали</option>
                </select>
            </div>
        </div>
    </div>
</template>