<script lang="ts" setup>
import config from '../assets/config.json'
</script>

<template>
    <div class="wrapper">
        <h2>{{ config.Name }}</h2>
        <p>{{ config.Description }}</p>
    </div>
</template>

<style scoped>
.wrapper {
    margin: 0 auto;
}
</style>