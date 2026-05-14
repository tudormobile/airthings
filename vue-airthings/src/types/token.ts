export type OAuthTokenType = 'Bearer' | 'bearer'

export interface AirthingsClientCredentialsTokenRequest {
	grant_type: 'client_credentials'
	scope?: string[]
	client_id?: string
	client_secret?: string
}

export interface AirthingsClientCredentialsToken {
	access_token: string
	token_type: OAuthTokenType
	expires_in: number
	scope?: string[]
}

