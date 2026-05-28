<script setup lang="ts">
import type { AirthingsSummarySample } from '../../types/proxy'

defineProps<{
	samples: AirthingsSummarySample[]
}>()
</script>

<template>
	<div class="samples-cards">
		<article
			v-for="(sample, index) in samples"
			:key="`${sample.home}-${sample.name}-${index}`"
			class="sample-card"
		>
			<h4>{{ sample.name }}</h4>
			<p><strong>Home:</strong> {{ sample.home }}</p>
			<p><strong>Radon:</strong> {{ sample.radon }}</p>
			<p><strong>Humidity:</strong> {{ sample.humidity }}</p>
			<p><strong>Temperature:</strong> {{ sample.temperature }}</p>
			<p><strong>Recorded:</strong> {{ sample.recorded.toISOString() }}</p>
			<p><strong>Battery %:</strong> {{ sample.batteryPercentage }}</p>
		</article>
	</div>
</template>

<style scoped>
.samples-cards {
	display: none;
	gap: 0.75rem;
	margin: 0.75rem 0;
}

.sample-card {
	padding: 0.75rem;
	border: 1px solid var(--color-border);
	border-radius: 8px;
	background: color-mix(in srgb, var(--color-background) 92%, var(--color-border) 8%);
}

.sample-card h4 {
	margin: 0 0 0.5rem 0;
}

.sample-card p {
	margin: 0.25rem 0;
}

@media (max-width: 760px) {
	.samples-cards {
		display: grid;
	}
}
</style>
