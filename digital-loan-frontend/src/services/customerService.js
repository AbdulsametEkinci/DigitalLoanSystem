// src/services/customerService.js
import apiClient from '../api/axiosConfig';

export const customerService = {
    getSummary: async (customerId) => {
        const response = await apiClient.get(`/Customers/${customerId}/summary`);
        return response.data;
    }
};