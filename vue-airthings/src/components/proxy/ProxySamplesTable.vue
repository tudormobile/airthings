<script setup lang="ts">
import type { AirthingsSummarySample } from '../../types/proxy'

defineProps<{
	samples: AirthingsSummarySample[]
}>()
</script>

<template>
	<div class="samples-table-wrap">
		<table class="samples-table">
			<thead>
				<tr>
					<th>Home</th>
					<th>Name</th>
					<th>Radon</th>
					<th>Humidity</th>
					<th>Temperature</th>
					<th>Recorded</th>
					<th>Battery %</th>
				</tr>
			</thead>
			<tbody>
				<tr v-for="(sample, index) in samples" :key="`${sample.home}-${sample.name}-${index}`">
					<td>{{ sample.home }}</td>
					<td>{{ sample.name }}</td>
					<td>{{ sample.radon }}</td>
					<td>{{ sample.humidity }}</td>
					<td>{{ sample.temperature }}</td>
					<td>{{ sample.recorded.toISOString() }}</td>
					<td>{{ sample.batteryPercentage }}</td>
				</tr>
			</tbody>
		</table>
	</div>
</template>

<style scoped>
.samples-table-wrap {
	overflow-x: auto;
	margin: 0.75rem 0;
}

.samples-table {
	width: 100%;
	border-collapse: collapse;
	font-size: 0.95rem;
}

.samples-table th,
.samples-table td {
	padding: 0.5rem;
	border-bottom: 1px solid var(--color-border);
	text-align: left;
	white-space: nowrap;
}

.samples-table th {
	font-weight: 600;
}

@media (max-width: 760px) {
	.samples-table-wrap {
		display: none;
	}
}
</style>
