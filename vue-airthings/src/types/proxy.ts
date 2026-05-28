export enum UnitsType {
	Metric = 'metric',
	Imperial = 'imperial',
}

export interface ApiResponse {
	message?: string
}

export interface SummarySample {
	home: string
	name: string
	radon: number
	humidity: number
	temperature: number
	recorded: Date
	batteryPercentage: number
}

export interface ProxyResponse extends ApiResponse {
	lastUpdated: Date
	version: string
	samples: SummarySample[]
}
