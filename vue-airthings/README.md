# vue-airthings

This template should help get you started developing with Vue 3 in Vite.

## Recommended IDE Setup

[VS Code](https://code.visualstudio.com/) + [Vue (Official)](https://marketplace.visualstudio.com/items?itemName=Vue.volar) (and disable Vetur).

## Recommended Browser Setup

- Chromium-based browsers (Chrome, Edge, Brave, etc.):
  - [Vue.js devtools](https://chromewebstore.google.com/detail/vuejs-devtools/nhdogjmejiglipccpnnnanhbledajbpd)
  - [Turn on Custom Object Formatter in Chrome DevTools](http://bit.ly/object-formatters)
- Firefox:
  - [Vue.js devtools](https://addons.mozilla.org/en-US/firefox/addon/vue-js-devtools/)
  - [Turn on Custom Object Formatter in Firefox DevTools](https://fxdx.dev/firefox-devtools-custom-object-formatters/)

## Type Support for `.vue` Imports in TS

TypeScript cannot handle type information for `.vue` imports by default, so we replace the `tsc` CLI with `vue-tsc` for type checking. In editors, we need [Volar](https://marketplace.visualstudio.com/items?itemName=Vue.volar) to make the TypeScript language service aware of `.vue` types.

## Customize configuration

See [Vite Configuration Reference](https://vite.dev/config/).

## Project Setup

```sh
npm install
```

### Compile and Hot-Reload for Development

```sh
npm run dev
```

### Type-Check, Compile and Minify for Production

```sh
npm run build
```

## Proxy Client Setup

The app can provide a proxy client for the route at `/proxy` and for use in your own components.

Add these Vite environment variables:

```env
VITE_PROXY_API_KEY=your_proxy_api_key
VITE_PROXY_BASE_ADDRESS=https://your-proxy-host
VITE_PROXY_ADDITIONAL_HEADERS={"X-Organization-Id":"your-org-id"}
```

Recommended file placement:

- Put real local values in `.env.local` (git-ignored).
- Keep only placeholders in `.env.example` (committed).

Notes:

- `VITE_PROXY_BASE_ADDRESS` should be the host/root URL; the client appends `/home/airthings/v1` internally.
- `VITE_PROXY_ADDITIONAL_HEADERS` is optional JSON for extra headers. Example: `{"X-Organization-Id":"your-org-id"}`.
- Values in `VITE_PROXY_ADDITIONAL_HEADERS` must be strings.
- If either variable is missing, the proxy client is not provided to Vue DI.

Use in components:

```ts
import { useProxy } from './composables/useProxy'
import { UnitsType } from './types/proxy'

const proxy = useProxy()
const status = await proxy.readStatus()
const summary = await proxy.readSummary(UnitsType.Metric)
```
