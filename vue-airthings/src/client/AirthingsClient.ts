import type { TokenProvider } from '../types/provider'
import apiConfig from '../assets/api.json'

export class AirthingsClient {
	private readonly baseUrl: string

	constructor(private readonly tokenProvider: TokenProvider) {
		this.baseUrl = apiConfig.baseUrl
	}

	private async authorizedFetch(path: string): Promise<Response> {
		const { token } = await this.tokenProvider.getAccessToken()
		const res = await fetch(`${this.baseUrl}${path}`, {
			headers: { Authorization: `Bearer ${token}` },
		})
		if (!res.ok) throw new Error(`Airthings API error ${res.status}: ${path}`)
		return res
	}

	async getAccounts(): Promise<unknown> {
		const res = await this.authorizedFetch('/accounts')
		return res.json()
	}

	async getDevices(accountId: string): Promise<unknown> {
		const res = await this.authorizedFetch(`/accounts/${accountId}/devices`)
		return res.json()
	}

	async getSensors(accountId: string): Promise<unknown> {
		const res = await this.authorizedFetch(`/accounts/${accountId}/sensors`)
		return res.json()
	}
}
