import apiClient from '../api/axiosConfig';

export const installmentService = {
    getInstallments: async (loanId) => {
        const response = await apiClient.get(`/installments?loanId=${loanId}`);
        return response.data;
    }
};