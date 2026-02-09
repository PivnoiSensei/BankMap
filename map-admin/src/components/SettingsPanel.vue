<script setup lang="ts">
import { ref, watch } from 'vue';
import { useBranchStore, type Branch } from '@/stores/branchStore';
import '@/assets/settings-panel.css';

const emit = defineEmits(['toggle-districts']);
const branchStore = useBranchStore();
const isEditing = ref(false);
const fileInput = ref<HTMLInputElement | null>(null);

const currentBranch = ref<Partial<Branch>>({
    name: '',
    address: '',
    status: 'Active'
});

//Fill settings form with selected branch data
watch(() => branchStore.selectedBranch, (selected) => {
    if (selected) {
        currentBranch.value = { ...selected };
        isEditing.value = true;
    } else {
        resetForm();
    }
});

const resetForm = () => {
    isEditing.value = false;
    branchStore.selectedBranch = null;
    currentBranch.value = { name: '', address: '', status: 'Active' };
};

const submitForm = async () => {
    if (isEditing.value && currentBranch.value.id) { //Update
        const success = await branchStore.updateBranch(currentBranch.value as Branch);
        if (success) resetForm();
    } else { //Create
        const branchToSave: Omit<Branch, 'id'> = {
            name: currentBranch.value.name || '',
            address: currentBranch.value.address || '',
            status: currentBranch.value.status || 'Active',
            latitude: branchStore.draftCoords.lat,
            longitude: branchStore.draftCoords.lng
        };
        const success = await branchStore.addBranch(branchToSave);
        if (success) resetForm();
    }
}

const removeBranch = async () => {
    if (isEditing.value && currentBranch.value.id){
        if (confirm('Ви впевнені, що хочете видалити це відділення?')) {
            const success = await branchStore.deleteBranch(currentBranch.value.id)
            if(success) resetForm();
        }
    }
}

const handleCsvUpload = async (event: Event) => {
    const target = event.target as HTMLInputElement;
    if (target.files && target.files[0]) {
        const success = await branchStore.importCsv(target.files[0]);
        if (success) {
            alert('Імпорт успішно завершено!');
        } else {
            alert('Помилка при завантаженні CSV.');
        }
        if (fileInput.value) fileInput.value.value = '';
    }
}
</script>

<template>
  <div class="settings-panel">
    <h3>Керування картою</h3>
    
    <div class="section">
      <h4>Шари</h4>
      <label class="switch">
        <input type="checkbox" class="district-checkbox" checked @change="(e: any) => emit('toggle-districts', e.target.checked)">
        Показувати мікрорайони
      </label>
    </div>

    <hr />

    <div class="section">
      <h4>{{ isEditing ? 'Редагування' : 'Нове відділення' }}</h4>
      <form @submit.prevent="submitForm">
        <input v-model="currentBranch.name" placeholder="Назва відділення" required />
        <input v-model="currentBranch.address" placeholder="Адреса" required />
        
        <div class="coords">
          <template v-if="isEditing">
            <div class="coord-input">
              <label>Широта</label>
              <input v-model.number="currentBranch.latitude" type="number" step="any" />
            </div>
            <div class="coord-input">
              <label>Довгота</label>
              <input v-model.number="currentBranch.longitude" type="number" step="any" />
            </div>
          </template>

          <template v-else>
            <div class="coord-input">
              <label>Широта</label>
              <input v-model.number="branchStore.draftCoords.lat" type="number" step="any" />
            </div>
            <div class="coord-input">
              <label>Довгота</label>
              <input v-model.number="branchStore.draftCoords.lng" type="number" step="any" />
            </div>
          </template>
        </div>

        <select v-model="currentBranch.status">
          <option value="Active">🟢 Працює</option>
          <option value="Reconstruction">🟠 Реконструкція</option>
          <option value="Closed">🔴 Зачинено</option>
        </select>

        <button type="submit" :class="{ 'update-btn': isEditing }">
            {{ isEditing ? 'Зберегти зміни' : 'Додати на карту' }}
        </button>
        <button v-if="isEditing" type="button" class="delete-btn" @click="removeBranch">
            Видалити маркер
        </button>
        <button v-if="isEditing" type="button" @click="resetForm" class="cancel-btn">Скасувати</button>
      </form>
    </div>

    <hr v-if="!isEditing" />

    <div class="section" v-if="!isEditing">
      <h4>Масовий імпорт</h4>
      <input type="file" ref="fileInput" style="display: none" accept=".csv" @change="handleCsvUpload" />
      <button type="button" class="import-btn" @click="fileInput?.click()">
          Завантажити CSV
      </button>
    </div>
  </div>
</template>