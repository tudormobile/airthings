import { inject } from 'vue'
import type { AirthingsClient } from '../client/AirthingsClient'

export const AIRTHINGS_CLIENT_KEY = Symbol('AirthingsClient')

export function useAirthings(): AirthingsClient {
	const client = inject<AirthingsClient>(AIRTHINGS_CLIENT_KEY)
	if (!client) throw new Error('AirthingsClient not provided. Did you call app.provide() in main.ts?')
	return client
}
