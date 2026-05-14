import axios from 'axios';

const apiClient = axios.create({
    baseURL: 'https://localhost:7113/api', // Sonda / (slash) olmamalı!
    headers: {
        'Content-Type': 'application/json'
    }
});

export default apiClient;