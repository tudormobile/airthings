import type { AirthingsProxyResponse, AirthingsSummarySample } from '../types/proxy'
import { UnitsType } from '../types/proxy'

const PROXY_SERVICE_PATH = '/home/airthings/v1'

interface ProxyEnvelope {
	isSuccess?: unknown
	data?: unknown
}

export class AirthingsProxyClient {
	private readonly apiKey: string
	private readonly baseUrl: URL
	private readonly fetchImpl: typeof fetch
	private readonly additionalHeaders: Record<string, string>

	constructor(
		apiKey: string,
		baseAddress: string,
		fetchImpl: typeof fetch = (...args) => globalThis.fetch(...args),
		additionalHeaders: Record<string, string> = {},
	) {
		if (!apiKey?.trim()) throw new Error('apiKey must be provided.')
		if (!baseAddress?.trim()) throw new Error('baseAddress must be provided.')

		let parsedBaseAddress: URL
		try {
			parsedBaseAddress = new URL(baseAddress)
		} catch {
			throw new Error('baseAddress must be a well-formed absolute URL.')
		}

		parsedBaseAddress.pathname = PROXY_SERVICE_PATH
		this.apiKey = apiKey
		this.baseUrl = parsedBaseAddress
		this.fetchImpl = fetchImpl
		this.additionalHeaders = additionalHeaders
	}

	readStatus(): Promise<AirthingsProxyResponse> {
		return this.apiRequest('status')
	}

	readSummary(unitsType: UnitsType = UnitsType.Metric): Promise<AirthingsProxyResponse> {
		return this.apiRequest(`summary/${unitsType}`)
	}

	private async apiRequest(path: string): Promise<AirthingsProxyResponse> {
		const url = new URL(path.replace(/^\/+/, ''), `${this.baseUrl.toString().replace(/\/+$/, '')}/`)

		try {
			const response = await this.fetchImpl(url.toString(), {
				method: 'GET',
				headers: {
					...this.additionalHeaders,
					ApiKey: this.apiKey,
				},
			})

			if (!response.ok) {
				throw new Error(`HTTP ${response.status} ${response.statusText}`.trim())
			}

			const text = await response.text()
			let payload: ProxyEnvelope
			try {
				payload = JSON.parse(text) as ProxyEnvelope
			} catch (error) {
				const message = error instanceof Error ? error.message : String(error)
				return this.errorResponse(`Invalid JSON: ${message}`)
			}

			if (payload.isSuccess !== true) {
				const message = typeof payload.data === 'string' ? payload.data : 'Request failed.'
				return this.errorResponse(message)
			}

			const data = payload.data
			if (!isObject(data)) {
				return this.errorResponse('Invalid JSON: data payload was not an object.')
			}

			const rawVersion = data.version
			if (typeof rawVersion === 'string') {
				return {
					version: rawVersion,
					lastUpdated: new Date(),
					samples: [],
				}
			}

			const version = isObject(rawVersion) && typeof rawVersion.version === 'string' ? rawVersion.version : ''
			const lastUpdated = parseDate(data.lastUpdated) ?? new Date()
			const samples = parseSamples(data.samples)

			return {
				version,
				lastUpdated,
				samples,
			}
		} catch (error) {
			const message = error instanceof Error ? error.message : String(error)
			return this.errorResponse(`Network error: ${message}`)
		}
	}

	private errorResponse(message: string): AirthingsProxyResponse {
		return {
			message,
			version: '',
			lastUpdated: new Date(),
			samples: [],
		}
	}
}

function parseSamples(value: unknown): AirthingsSummarySample[] {
	if (!Array.isArray(value)) return []

	return value
		.filter(isObject)
		.map((sample) => ({
			home: asString(sample.home),
			name: asString(sample.name),
			radon: asNumber(sample.radon),
			humidity: asNumber(sample.humidity),
			temperature: asNumber(sample.temperature),
			recorded: parseDate(sample.recorded) ?? new Date(0),
			batteryPercentage: asInteger(sample.batteryPercentage),
		}))
}

function parseDate(value: unknown): Date | null {
	if (typeof value !== 'string') return null
	const date = new Date(value)
	return Number.isNaN(date.getTime()) ? null : date
}

function asString(value: unknown): string {
	return typeof value === 'string' ? value : ''
}

function asNumber(value: unknown): number {
	return typeof value === 'number' ? value : 0
}

function asInteger(value: unknown): number {
	return Number.isInteger(value) ? (value as number) : 0
}

function isObject(value: unknown): value is Record<string, unknown> {
	return typeof value === 'object' && value !== null
}
