<script setup lang="ts">
import { RouterView } from 'vue-router'
</script>

<template>
  <!-- RouterView подставит компонент TheMap, так как мы указали его для пути '/' -->
  <RouterView />
</template>

<style>
/* Обнуляем отступы, чтобы карта была на весь экран */
#app {
  width: 100%;
  height: 100vh;
}
</style>
