import { inject } from 'vue'
import type { AirthingsProxyClient } from '../client/AirthingsProxyClient'

export const AIRTHINGS_PROXY_CLIENT_KEY = Symbol('AirthingsProxyClient')

export function useAirthingsProxy(): AirthingsProxyClient {
	const client = inject<AirthingsProxyClient>(AIRTHINGS_PROXY_CLIENT_KEY)
	if (!client) {
		throw new Error(
			'AirthingsProxyClient not provided. Set VITE_AIRTHINGS_PROXY_API_KEY and VITE_AIRTHINGS_PROXY_BASE_ADDRESS, then provide it in main.ts.',
		)
	}
	return client
}
