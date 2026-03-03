<script setup lang="ts">
import { ref, computed, onMounted, onBeforeUnmount } from 'vue';
import type { Branch, WorkDay } from '@/stores/branchStore';

const props = defineProps<{
  branch: Branch;
}>();

const daysNames = [
  "Понеділок","Вівторок","Середа","Четвер",
  "П'ятниця","Субота","Неділя"
];

const today = new Date().getDay();
const defaultDay = today === 0 ? 6 : today - 1;

const selectedDayIndex = ref(defaultDay);
const isSelectOpen = ref(false);

function selectDay(index: number) {
  selectedDayIndex.value = index;
  isSelectOpen.value = false;
}

//Сlick outside to close
function handleClickOutside(e: MouseEvent) {
  const target = e.target as HTMLElement;
  if (!target.closest('.custom-select')) {
    isSelectOpen.value = false;
  }
}

onMounted(() => {
  document.addEventListener('click', handleClickOutside);
});

onBeforeUnmount(() => {
  document.removeEventListener('click', handleClickOutside);
});

//Department days
const deptDays = computed(
  () =>
    props.branch.schedules
      ?.find(t => t.workStation === 'department')
      ?.days || []
);

const selectedDay = computed<WorkDay | undefined>(
  () => deptDays.value[selectedDayIndex.value]
);

//Cash in department
const cashDays = computed(
  () =>
    props.branch.schedules
      ?.find(t => t.workStation === 'cashDepartment')
      ?.days || []
);

const selectedCashDay = computed<WorkDay | undefined>(() => {
  if (!selectedDay.value) return;
  return cashDays.value.find(
    d => d.dayOfWeek === selectedDay.value!.dayOfWeek
  );
});

//Separate cash departments
const cashDesks = computed(
  () => props.branch.cashDesks || []
);

const cashDepsForDay = computed(() => {
  if (!selectedDay.value) return [];

  return cashDesks.value.map((desk, index) => {
    const day = desk.workDays.find(
      d => d.dayOfWeek === selectedDay.value!.dayOfWeek
    );

    return {
      description: desk.description,
      day,
      index
    };
  }).filter(d => d.day);
});

//Helpers
function formatBreaks(day?: WorkDay) {
  if (!day?.breaks?.length) return '-';

  return day.breaks
    .map(b => `${b.from} - ${b.to}`)
    .join(', ');
}
</script>

<template>
  <div class="map-popup">

    <!-- HEADER -->
    <div class="popup-header">
      <strong class="popup-title">
        {{branch.name}}
      </strong>

      <div class="popup-day-select custom-select" :class="{ open: isSelectOpen }">
        <div class="custom-select-trigger" @click.stop="isSelectOpen = !isSelectOpen">
          {{ daysNames[selectedDayIndex] }}
        </div>

        <div class="custom-select-dropdown">
          <div
            v-for="(name, i) in daysNames"
            :key="i"
            class="popup-day-option"
            :class="{ selected: i === selectedDayIndex }"
            @click.stop="selectDay(i)"
          >
            {{ name }}
          </div>
        </div>
      </div>

    </div>

    <!-- REGULAR BADGE -->
    <div
      v-if="branch.isRegular"
      class="popup-body-subtitle regular-container"
    >
      <img src="/icon-regular.svg" class="regular-icon" />
      Чергове
    </div>

    <!-- ADDRESS -->
    <div class="popup-body">
      <div class="popup-body-subtitle">
        Адреса:
      </div>
      <div>
        {{ branch.address.fullAddress }}
      </div>
      <div
        v-if="branch.address.detailedAddress"
        class="detailed-address"
      >
        {{
          branch.address.detailedAddress.charAt(0).toUpperCase()
          +
          branch.address.detailedAddress.slice(1)
        }}
      </div>
    </div>

    <!-- PHONE -->
    <div class="popup-body">
      <div class="popup-body-subtitle">
        Телефон:
      </div>
      <div v-if="branch.phones[0]?.fullNumber !== ''">
        {{branch.phones[0]?.fullNumber}}
      </div>
      <div v-if="branch.phones.length == 0">
        0 800 30 70 10
      </div>
      <div style="margin-bottom: 4px;">
        Цілодобово і безкоштовно в межах України
      </div>
    </div>

    <!-- SCHEDULE TITLE -->
    <div class="popup-body-subtitle">
      Графік роботи:
    </div>

    <!-- SCHEDULE CONTENT -->
    <div class="popup-footer" :id="`popup-content-${branch.id}`">

      <template v-if="branch.isTemporaryClosed">
        <h4 class="popup-title">
          Тимчасово не працює
        </h4>
      </template>

      <template v-else-if="selectedDay">
        <!-- Department -->
        <div style="margin-bottom: 4px;">
          {{ selectedDay.from }} - {{ selectedDay.to }}
        </div>
        <template v-if="formatBreaks(selectedDay) !== '-'">
          <div class="popup-body-subtitle" style="margin-bottom: 4px;">
            Перерва:
          </div>
          <div style="margin-bottom: 4px; color: #000">
            {{ formatBreaks(selectedDay) }}
          </div>
        </template>

        <!-- Cash in department -->
        <div v-if="selectedCashDay" style="color: #28a745; margin-bottom: 4px;">
          Каса у відділенні:
          {{ selectedCashDay.from }}
          -
          {{ selectedCashDay.to }}
        </div>

        <!-- Separate cash departments -->
        <template v-if="cashDepsForDay.length">
          <div class="popup-body-subtitle">
            Окремі каси:
          </div>
          <template v-for="(cash, i) in cashDepsForDay" :key="i">
            <hr />
            <div class="cashdeps-container">
              <div style="margin-bottom: 4px; color: #000">
                {{ cash.description
                  ? cash.description + ':'
                  : '' }}
              </div>
              <div class="cashdeps-time">
                {{ cash.day
                  ? `${cash.day.from} - ${cash.day.to}`
                  : 'не працює' }}
              </div>
            </div>

            <div class="popup-body-subtitle" style="margin-bottom: 4px;">
              Перерва:
              <div class="cashdeps-time">
                {{ formatBreaks(cash.day) }}
              </div>
            </div>
          </template>
        </template>
      </template>
      <template v-else>
        <i>
          Вихідний
        </i>
      </template>
    </div>

  </div>
</template>
