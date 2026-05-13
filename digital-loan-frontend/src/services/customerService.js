// src/services/customerService.js
import apiClient from '../api/axiosConfig';

export const customerService = {
  // Tüm müşterileri getir
  getAllCustomers: async () => {
    const response = await apiClient.get('/customers');
    return response.data;
  },

  // Müşteri oluştur
  createCustomer: async (createCustomerDto) => {
    const response = await apiClient.post('/customers', createCustomerDto);
    return response.data;
  },

  // Müşteri detayları getir
  getCustomer: async (customerId) => {
    const response = await apiClient.get(`/customers/${customerId}`);
    return response.data;
  },

  // Müşteri bilgilerini güncelle
  updateCustomer: async (customerId, updateCustomerDto) => {
    const response = await apiClient.put(`/customers/${customerId}`, updateCustomerDto);
    return response.data;
  },

  // Müşteri sil
  deleteCustomer: async (customerId) => {
    await apiClient.delete(`/customers/${customerId}`);
    return true;
  },

  // Müşteri özetini getir (borç, gecikme vb)
  getSummary: async (customerId) => {
    const response = await apiClient.get(`/customers/${customerId}/summary`);
    return response.data;
  }
};