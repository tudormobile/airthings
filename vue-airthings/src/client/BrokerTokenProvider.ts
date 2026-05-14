import type { AccessToken, TokenProvider } from '../types/provider'

/**
 * Fetches tokens from your own backend token-broker endpoint.
 * The broker holds the client_secret safely server-side.
 * The frontend only ever receives a short-lived access token.
 */
export class BrokerTokenProvider implements TokenProvider {
	private cached?: AccessToken

	constructor(private readonly brokerUrl: string = '/api/token') {}

	async getAccessToken(): Promise<AccessToken> {
		const now = Date.now()

		if (this.cached && now < this.cached.expiresAtEpochMs - 30_000) {
			return this.cached
		}

		const res = await fetch(this.brokerUrl, { method: 'POST' })
		if (!res.ok) throw new Error(`Token broker error: ${res.status}`)

		const json = (await res.json()) as { token: string; expiresInSeconds: number; scope?: string[] }

		this.cached = {
			token: json.token,
			expiresAtEpochMs: now + json.expiresInSeconds * 1000,
			scope: json.scope,
		}

		return this.cached
	}
}
