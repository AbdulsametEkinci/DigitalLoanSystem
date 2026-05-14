// src/services/loanService.js
import apiClient from '../api/axiosConfig';

export const loanService = {
    // Yeni kredi başvurusu yapar (UC-2)
    applyForLoan: async (loanData) => {
        const response = await apiClient.post('/loans', loanData);
        return response.data;
    }
};