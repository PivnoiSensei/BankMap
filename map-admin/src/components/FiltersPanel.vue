<script setup lang="ts">
import { useBranchStore } from '@/stores/branchStore';
import '@/assets/filters-panel.css'
const branchStore = useBranchStore();

</script>

<template>
    <div class="filters-panel">
        <h4>Фільтрація даних</h4>
        <div class="filters-section">
            <div class="filter-group">
                <label>Місто:</label>
                <select v-model="branchStore.filterCity">
                <option v-for="city in branchStore.uniqueCities" :key="city" :value="city">
                    {{ city === 'all' ? 'Усі міста' : city }}
                </option>
                </select>
            </div>

            <div class="filter-group">
                <label>Тип об'єкта:</label>
                <select v-model="branchStore.filterType">
                <option value="all">Усі типи</option>
                <option value="department">Відділення</option>
                <option value="atm">Банкомати</option>
                <option value="terminal">Термінали</option>
                </select>
            </div>
        </div>
    </div>
</template>