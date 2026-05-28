<script setup lang="ts">
import { ref } from 'vue'
import { useAirthingsProxy } from '../composables/useAirthingsProxy'
import { UnitsType, type AirthingsProxyResponse } from '../types/proxy'
import ProxyErrorMessage from '../components/proxy/ProxyErrorMessage.vue'
import ProxyResponseMeta from '../components/proxy/ProxyResponseMeta.vue'
import ProxySamplesTable from '../components/proxy/ProxySamplesTable.vue'
import ProxySamplesCards from '../components/proxy/ProxySamplesCards.vue'

const proxy = useAirthingsProxy()

const loading = ref(false)
const status = ref<AirthingsProxyResponse | null>(null)
const summary = ref<AirthingsProxyResponse | null>(null)
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
		<ProxyErrorMessage :message="error" />

		<section v-if="status" class="panel">
			<h2>Status Response</h2>
			<ProxyErrorMessage :message="status.message" />
			<ProxyResponseMeta
				v-if="!status.message"
				:version="status.version"
				:last-updated="status.lastUpdated"
				:sample-count="status.samples.length"
			/>
		</section>

		<section v-if="summary" class="panel">
			<h2>Summary Response</h2>
			<ProxyErrorMessage :message="summary.message" />
			<div v-if="!summary.message">
				<ProxyResponseMeta :version="summary.version" :last-updated="summary.lastUpdated" />
				<h3>Samples</h3>
				<p v-if="summary.samples.length === 0">No samples returned.</p>
				<template v-else>
					<ProxySamplesTable :samples="summary.samples" />
					<ProxySamplesCards :samples="summary.samples" />
				</template>

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

pre {
	overflow-x: auto;
	white-space: pre-wrap;
	word-break: break-word;
}
</style>
