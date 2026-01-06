import api from './api'

export async function testConnection() {
    try {
        console.log('Testing API connection...')
        const response = await api.get('/health') // or any simple endpoint
        console.log('✅ Connection successful:', response.data)
        return true
    } catch (error) {
        console.error('❌ Connection failed:', error)
        return false
    }
}