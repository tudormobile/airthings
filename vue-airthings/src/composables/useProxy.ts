import { inject } from 'vue'
import type { ProxyClient } from '../client/ProxyClient'

export const PROXY_CLIENT_KEY = Symbol('ProxyClient')

export function useProxy(): ProxyClient {
	const client = inject<ProxyClient>(PROXY_CLIENT_KEY)
	if (!client) {
		throw new Error(
			'ProxyClient not provided. Set VITE_PROXY_API_KEY and VITE_PROXY_BASE_ADDRESS, then provide it in main.ts.',
		)
	}
	return client
}
