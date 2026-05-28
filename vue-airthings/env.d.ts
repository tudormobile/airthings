/// <reference types="vite/client" />

interface ImportMetaEnv {
	readonly VITE_AIRTHINGS_CLIENT_ID?: string
	readonly VITE_AIRTHINGS_CLIENT_SECRET?: string
	readonly VITE_AIRTHINGS_SCOPES?: string
	readonly VITE_AIRTHINGS_DEBUG?: string
	readonly VITE_PROXY_API_KEY?: string
	readonly VITE_PROXY_BASE_ADDRESS?: string
	readonly VITE_PROXY_ADDITIONAL_HEADERS?: string
}

interface ImportMeta {
	readonly env: ImportMetaEnv
}
