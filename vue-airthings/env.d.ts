/// <reference types="vite/client" />

interface ImportMetaEnv {
	readonly VITE_AIRTHINGS_PROXY_API_KEY?: string
	readonly VITE_AIRTHINGS_PROXY_BASE_ADDRESS?: string
	readonly VITE_AIRTHINGS_PROXY_ADDITIONAL_HEADERS?: string
}

interface ImportMeta {
	readonly env: ImportMetaEnv
}
