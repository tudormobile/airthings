<script setup lang="ts">
import { ref } from 'vue'
import { useProxy } from '../composables/useProxy'
import { UnitsType, type ProxyResponse } from '../types/proxy'

const proxy = useProxy()

const loading = ref(false)
const status = ref<ProxyResponse | null>(null)
const summary = ref<ProxyResponse | null>(null)
const error = ref('')

async function readStatus(): Promise<void> {
	loading.value = true
	error.value = ''
	try {
		status.value = await proxy.readStatus()
	} catch (e) {
		error.value = e instanceof Error ? e.message : String(e)
	} finally {
		loading.value = false
	}
}

async function readSummary(units: UnitsType): Promise<void> {
	loading.value = true
	error.value = ''
	try {
		summary.value = await proxy.readSummary(units)
	} catch (e) {
		error.value = e instanceof Error ? e.message : String(e)
	} finally {
		loading.value = false
	}
}
</script>

<template>
	<main class="proxy-page">
		<h1>Proxy Demo</h1>
		<p>Read status and summary data from the Airthings proxy service.</p>

		<section class="actions">
			<button type="button" :disabled="loading" @click="readStatus">Read Status</button>
			<button type="button" :disabled="loading" @click="readSummary(UnitsType.Metric)">Read Summary (Metric)</button>
			<button type="button" :disabled="loading" @click="readSummary(UnitsType.Imperial)">Read Summary (Imperial)</button>
		</section>

		<p v-if="loading">Loading...</p>
		<p v-if="error" class="error">{{ error }}</p>

		<section v-if="status" class="panel">
			<h2>Status Response</h2>
			<p v-if="status.message" class="error">{{ status.message }}</p>
			<ul v-else>
				<li><strong>Version:</strong> {{ status.version }}</li>
				<li><strong>Last Updated:</strong> {{ status.lastUpdated.toISOString() }}</li>
				<li><strong>Sample Count:</strong> {{ status.samples.length }}</li>
			</ul>
		</section>

		<section v-if="summary" class="panel">
			<h2>Summary Response</h2>
			<p v-if="summary.message" class="error">{{ summary.message }}</p>
			<div v-else>
				<p><strong>Version:</strong> {{ summary.version }}</p>
				<p><strong>Last Updated:</strong> {{ summary.lastUpdated.toISOString() }}</p>
				<h3>Samples</h3>
				<p v-if="summary.samples.length === 0">No samples returned.</p>
				<div v-else class="samples-table-wrap">
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
							<tr v-for="(sample, index) in summary.samples" :key="`${sample.home}-${sample.name}-${index}`">
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

				<div v-if="summary.samples.length > 0" class="samples-cards">
					<article
						v-for="(sample, index) in summary.samples"
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

				<h3>Raw JSON</h3>
				<pre>{{ JSON.stringify(summary.samples, null, 2) }}</pre>
			</div>
		</section>
	</main>
</template>

<style scoped>
.proxy-page {
	max-width: 900px;
	margin: 0 auto;
	padding: 1rem;
}

.actions {
	display: flex;
	gap: 0.5rem;
	flex-wrap: wrap;
	margin: 1rem 0;
}

.panel {
	margin-top: 1rem;
	padding: 1rem;
	border: 1px solid var(--color-border);
	border-radius: 8px;
}

.error {
	color: #b00020;
}

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

pre {
	overflow-x: auto;
	white-space: pre-wrap;
	word-break: break-word;
}

@media (max-width: 760px) {
	.samples-table-wrap {
		display: none;
	}

	.samples-cards {
		display: grid;
	}
}
</style>
