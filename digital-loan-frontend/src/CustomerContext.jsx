import { createContext, useState, useCallback } from 'react';

export const CustomerContext = createContext();

export function CustomerProvider({ children }) {
  const [selectedCustomerId, setSelectedCustomerId] = useState(null);

  const selectCustomer = useCallback((customerId) => {
    setSelectedCustomerId(customerId);
  }, []);

  const clearCustomer = useCallback(() => {
    setSelectedCustomerId(null);
  }, []);

  const value = {
    selectedCustomerId,
    selectCustomer,
    clearCustomer
  };

  return (
    <CustomerContext.Provider value={value}>
      {children}
    </CustomerContext.Provider>
  );
}
