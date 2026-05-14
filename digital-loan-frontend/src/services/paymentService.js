import apiClient from '../api/axiosConfig';

export const paymentService = {
    pay: async (paymentData) => {
        const response = await apiClient.post('/payments', paymentData);
        return response.data;
    }
};