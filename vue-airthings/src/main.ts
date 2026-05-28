import './assets/main.css'

import { createApp } from 'vue'
import App from './App.vue'
import router from './router'
import { ProxyClient } from './client/ProxyClient'
import { PROXY_CLIENT_KEY } from './composables/useProxy'

// Proxy client setup
const proxyApiKey = import.meta.env.VITE_PROXY_API_KEY
const proxyBaseAddress = import.meta.env.VITE_PROXY_BASE_ADDRESS
const proxyAdditionalHeaders = parseProxyAdditionalHeaders(import.meta.env.VITE_PROXY_ADDITIONAL_HEADERS)
const proxyClient =
	proxyApiKey?.trim() && proxyBaseAddress?.trim()
		? new ProxyClient(proxyApiKey, proxyBaseAddress, undefined, proxyAdditionalHeaders)
		: null

const app = createApp(App)

app.use(router)
if (proxyClient) {
	app.provide(PROXY_CLIENT_KEY, proxyClient)
}
app.mount('#app')

function parseProxyAdditionalHeaders(rawHeaders: string | undefined): Record<string, string> {
	if (!rawHeaders?.trim()) return {}

	let parsedHeaders: unknown
	try {
		parsedHeaders = JSON.parse(rawHeaders)
	} catch (error) {
		const message = error instanceof Error ? error.message : String(error)
		throw new Error(`VITE_PROXY_ADDITIONAL_HEADERS must be valid JSON. ${message}`)
	}

	if (parsedHeaders === null || typeof parsedHeaders !== 'object' || Array.isArray(parsedHeaders)) {
		throw new Error('VITE_PROXY_ADDITIONAL_HEADERS must be a JSON object of header name/value pairs.')
	}

	const headers: Record<string, string> = {}
	for (const [key, value] of Object.entries(parsedHeaders as Record<string, unknown>)) {
		if (typeof value !== 'string') {
			throw new Error(`VITE_PROXY_ADDITIONAL_HEADERS value for "${key}" must be a string.`)
		}
		headers[key] = value
	}

	return headers
}

