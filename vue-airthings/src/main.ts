import './assets/main.css'

import { createApp } from 'vue'
import App from './App.vue'
import router from './router'
import { AirthingsClient } from './client/AirthingsClient'
import { BrokerTokenProvider } from './client/BrokerTokenProvider'
import { AIRTHINGS_CLIENT_KEY } from './composables/useAirthings'

const tokenProvider = new BrokerTokenProvider('/api/token')
const airthingsClient = new AirthingsClient(tokenProvider)

const app = createApp(App)

app.use(router)
app.provide(AIRTHINGS_CLIENT_KEY, airthingsClient)

app.mount('#app')
