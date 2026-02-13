<script setup lang="ts">
import { ref, watch } from 'vue';
import { useBranchStore } from '@/stores/branchStore';
import '@/assets/settings-panel.css';

const emit = defineEmits(['toggle-districts']);
const branchStore = useBranchStore();
const isEditing = ref(false);
const fileInput = ref<HTMLInputElement | null>(null);

const tempClosed = ref(false);

watch(() => branchStore.selectedBranch, (selected) => {
    if (selected) {
        tempClosed.value = selected.isTemporaryClosed;
        isEditing.value = true;
    } else {
        isEditing.value = false;
    }
});

//Update status
const saveStatus = async () => {
    if (branchStore.selectedBranch) {
        const success = await branchStore.updateBranchStatus(
            branchStore.selectedBranch.id, 
            tempClosed.value
        );
        if (success) {
            alert('Статус оновлено!');
            branchStore.selectedBranch = null;
        }
    }
};

const handleJsonUpload = async (event: Event) => {
    const target = event.target as HTMLInputElement;
    if (target.files && target.files[0]) {
        const success = await branchStore.importJson(target.files[0]);
        if (success) {
            alert('Дані успішно оновлено!');
        } else {
            alert('Помилка при імпорті JSON.');
        }
        if (fileInput.value) fileInput.value.value = '';
    }
}
</script>

<template>
  <div class="settings-panel">
    <h3>Керування картою</h3>

    <hr />

    <div class="section">
      <h4>Візуалізація</h4>
      <label class="switch">
        <input type="checkbox" class="settings-checkbox" @change="(e: any) => emit('toggle-districts', e.target.checked)">
        Показувати мікрорайони
      </label>
    </div>

    <hr />

    <div class="section">
        <div v-if="isEditing" class="section edit-section">
          <h4>Редагування: {{ branchStore.selectedBranch?.name }}</h4>
          <label class="switch">
              <input type="checkbox" class="settings-checkbox" v-model="tempClosed">
              Тимчасово зачинено
          </label>
          <div class="button-group">
              <button type="button" class="update-btn" @click="saveStatus">Зберегти статус</button>
              <button type="button" class="cancel-btn" @click="branchStore.selectedBranch = null">Скасувати</button>
          </div>
      </div>

      <hr />

      <h4>Оновлення бази</h4>
      <p style="font-size: 0.8em; color: #666; margin-bottom: 10px;">
        Завантажте актуальний JSON файл для повного оновлення точок на карті.
      </p>
      <input 
        type="file" 
        ref="fileInput" 
        style="display: none" 
        accept=".json" 
        @change="handleJsonUpload" 
      />
      <button type="button" class="import-btn" @click="fileInput?.click()">
          Оновити через JSON
      </button>
    </div>

    <div class="stats-footer">
      <small>Відображено точок: {{ branchStore.filteredBranches.length }}</small>
    </div>
  </div>
</template>
