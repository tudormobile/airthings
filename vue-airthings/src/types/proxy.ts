export enum UnitsType {
	Metric = 'metric',
	Imperial = 'imperial',
}

export interface ApiResponse {
	message?: string
}

export interface AirthingsSummarySample {
	home: string
	name: string
	radon: number
	humidity: number
	temperature: number
	recorded: Date
	batteryPercentage: number
}

export interface AirthingsProxyResponse extends ApiResponse {
	lastUpdated: Date
	version: string
	samples: AirthingsSummarySample[]
}
