<script setup lang="ts">
const props = defineProps<{
	version: string
	lastUpdated: Date
	sampleCount?: number
}>()
</script>

<template>
	<ul class="meta-list">
		<li><strong>Version:</strong> {{ props.version }}</li>
		<li><strong>Last Updated:</strong> {{ props.lastUpdated.toISOString() }}</li>
		<li v-if="props.sampleCount !== undefined"><strong>Sample Count:</strong> {{ props.sampleCount }}</li>
	</ul>
</template>

<style scoped>
.meta-list {
	margin: 0;
	padding-left: 1.25rem;
}

.meta-list li {
	margin: 0.3rem 0;
}
</style>
