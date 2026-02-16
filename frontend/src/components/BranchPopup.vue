<script setup lang="ts">
import { ref, computed, onMounted, onBeforeUnmount } from 'vue';
import type { BankMarker, WorkDay } from '@/stores/branchStore';

const props = defineProps<{
  branch: BankMarker;
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

const details = computed(() => props.branch.details);

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
    details.value?.TimeTables
      ?.find(t => t.Workstation === 'department')
      ?.WorkDays || []
);

const selectedDay = computed<WorkDay | undefined>(
  () => deptDays.value[selectedDayIndex.value]
);

//Cash in department
const cashDays = computed(
  () =>
    details.value?.TimeTables
      ?.find(t => t.Workstation === 'cashDepartment')
      ?.WorkDays || []
);

const selectedCashDay = computed<WorkDay | undefined>(() => {
  if (!selectedDay.value) return;
  return cashDays.value.find(
    d => d.WorkingDay === selectedDay.value!.WorkingDay
  );
});

//Separate cash departments
const cashDeps = computed(
  () => details.value?.CashDepartments || []
);

const cashDepsForDay = computed(() => {
  if (!selectedDay.value) return [];

  return cashDeps.value.map((dep, index) => {
    const day = dep.WorkDays.find(
      d => d.DayOfWeek === selectedDay.value!.WorkingDay
    );

    return {
      description: dep.CashDescription,
      day,
      index
    };
  }).filter(d => d.day);
});

//Helpers
function formatBreaks(day?: WorkDay) {
  if (!day?.Breaks?.length) return '-';

  return day.Breaks
    .map(b => `${b.BreakFrom}-${b.BreakTo}`)
    .join(', ');
}
</script>

<template>
  <div class="map-popup">

    <!-- HEADER -->
    <div class="popup-header">
      <strong class="popup-title">
        {{
          branch.name.includes(`в м. ${branch.baseCity}`)
            ? branch.name
            : `${branch.name} в м. ${branch.baseCity}`
        }}
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
        {{ branch.fullAddress }}
      </div>
      <div
        v-if="branch.details?.Address?.DetailedAddress"
        class="detailed-address"
      >
        {{
          branch.details.Address.DetailedAddress.charAt(0).toUpperCase()
          +
          branch.details.Address.DetailedAddress.slice(1)
        }}
      </div>
    </div>

    <!-- PHONE -->
    <div class="popup-body">
      <div class="popup-body-subtitle">
        Телефон:
      </div>
      <div>
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
          {{ selectedDay.WorkFrom }} - {{ selectedDay.WorkTo }}
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
          {{ selectedCashDay.WorkFrom }}
          -
          {{ selectedCashDay.WorkTo }}
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
                  ? `${cash.day.WorkFrom} - ${cash.day.WorkTo}`
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
