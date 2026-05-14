export interface AccessToken {
	token: string
	expiresAtEpochMs: number
	scope?: string[]
}

export interface TokenProvider {
	getAccessToken(): Promise<AccessToken>
}
