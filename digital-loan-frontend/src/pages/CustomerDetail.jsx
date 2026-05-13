import { useState, useEffect, useContext } from 'react';
import { useNavigate } from 'react-router-dom';
import { CustomerContext } from '../CustomerContext';
import { customerService } from '../services/customerService';
import { loanService } from '../services/loanService';

export default function CustomerDetail() {
  const navigate = useNavigate();
  const { selectedCustomerId, clearCustomer } = useContext(CustomerContext);

  const [customer, setCustomer] = useState(null);
  const [summary, setSummary] = useState(null);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState('');

  // Müşteri verisini yükle
  useEffect(() => {
    if (!selectedCustomerId) {
      navigate('/customers');
      return;
    }
    loadCustomerData();
  }, [selectedCustomerId]);

  const loadCustomerData = async () => {
    setLoading(true);
    setError('');
    try {
      const [customerData, summaryData] = await Promise.all([
        customerService.getCustomer(selectedCustomerId),
        customerService.getSummary(selectedCustomerId)
      ]);
      setCustomer(customerData);
      setSummary(summaryData);
    } catch (err) {
      setError(err.response?.data?.Error || 'Veri yükleme hatası');
    } finally {
      setLoading(false);
    }
  };

  const handleCreateLoan = () => {
    navigate('/apply');
  };

  const handleMakePayment = (installmentId) => {
    navigate('/payment', { state: { installmentId, customerId: selectedCustomerId } });
  };

  const handleBackToCustomers = () => {
    clearCustomer();
    navigate('/customers');
  };

  if (loading) {
    return <div style={{ padding: '20px', textAlign: 'center' }}>Yükleniyor...</div>;
  }

  if (error) {
    return (
      <div style={{ padding: '20px' }}>
        <div style={{ padding: '10px', backgroundColor: '#f8d7da', color: '#721c24', borderRadius: '5px', marginBottom: '10px' }}>{error}</div>
        <button onClick={handleBackToCustomers} style={{ padding: '10px 20px', backgroundColor: '#007bff', color: 'white', border: 'none', borderRadius: '3px', cursor: 'pointer' }}>
          ← Geri Dön
        </button>
      </div>
    );
  }

  if (!customer || !summary) {
    return <div style={{ padding: '20px' }}>Müşteri bulunamadı</div>;
  }

  return (
    <div style={{ maxWidth: '1000px', margin: '0 auto', padding: '20px' }}>
      {/* Üst Bilgi */}
      <div style={{ backgroundColor: '#f9f9f9', padding: '20px', borderRadius: '5px', marginBottom: '20px' }}>
        <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginBottom: '15px' }}>
          <div>
            <h2 style={{ margin: '0 0 10px 0' }}>{customer.fullName}</h2>
            <div style={{ fontSize: '14px', color: '#666' }}>
              <p style={{ margin: '5px 0' }}>TCKN: {customer.identityNumber}</p>
              <p style={{ margin: '5px 0' }}>Email: {customer.email}</p>
              <p style={{ margin: '5px 0' }}>Telefon: {customer.phoneNumber}</p>
            </div>
          </div>
          <button onClick={handleBackToCustomers} style={{ padding: '10px 20px', backgroundColor: '#6c757d', color: 'white', border: 'none', borderRadius: '3px', cursor: 'pointer' }}>
            ← Geri Dön
          </button>
        </div>
      </div>

      {/* Finansal Özet */}
      <div style={{ display: 'grid', gridTemplateColumns: '1fr 1fr 1fr', gap: '15px', marginBottom: '20px' }}>
        <div style={{ backgroundColor: '#e7f3ff', padding: '15px', borderRadius: '5px', textAlign: 'center' }}>
          <div style={{ fontSize: '12px', color: '#004085' }}>Toplam Borç</div>
          <div style={{ fontSize: '24px', fontWeight: 'bold', color: '#004085' }}>₺{summary.totalRemainingDebt.toFixed(2)}</div>
        </div>
        <div style={{ backgroundColor: '#fff3cd', padding: '15px', borderRadius: '5px', textAlign: 'center' }}>
          <div style={{ fontSize: '12px', color: '#856404' }}>Kalan Anapara</div>
          <div style={{ fontSize: '24px', fontWeight: 'bold', color: '#856404' }}>₺{summary.remainingPrincipal.toFixed(2)}</div>
        </div>
        <div style={{ backgroundColor: '#f8d7da', padding: '15px', borderRadius: '5px', textAlign: 'center' }}>
          <div style={{ fontSize: '12px', color: '#721c24' }}>Gecikmiş Taksit</div>
          <div style={{ fontSize: '24px', fontWeight: 'bold', color: '#721c24' }}>{summary.delayedInstallmentsCount}</div>
        </div>
      </div>

      {/* Kredi İşlemleri */}
      <div style={{ marginBottom: '20px' }}>
        <button
          onClick={handleCreateLoan}
          style={{
            padding: '12px 24px',
            backgroundColor: '#28a745',
            color: 'white',
            border: 'none',
            borderRadius: '3px',
            cursor: 'pointer',
            fontSize: '16px',
            fontWeight: 'bold'
          }}
        >
          ➕ Yeni Kredi Çek
        </button>
      </div>

      {/* Taksitler */}
      <div style={{ marginBottom: '20px' }}>
        <h3>📊 Aktif Taksitler</h3>
        {summary.installments.length === 0 ? (
          <p style={{ color: '#666' }}>Aktif kredi bulunmamaktadır.</p>
        ) : (
          <div style={{ display: 'grid', gap: '10px' }}>
            {summary.installments.map((installment, index) => (
              <div key={index} style={{
                padding: '15px',
                backgroundColor: installment.statusDisplay === 'Ödendi' ? '#d4edda' : '#f8f9fa',
                borderRadius: '5px',
                border: `1px solid ${installment.statusDisplay === 'Ödendi' ? '#c3e6cb' : '#dee2e6'}`,
                display: 'flex',
                justifyContent: 'space-between',
                alignItems: 'center'
              }}>
                <div>
                  <strong>Taksit #{installment.installmentNumber}</strong>
                  <div style={{ fontSize: '12px', color: '#666' }}>
                    Tutar: ₺{installment.amount.toFixed(2)} | Vade: {new Date(installment.dueDate).toLocaleDateString('tr-TR')}
                  </div>
                  <div style={{ fontSize: '12px', color: '#666' }}>
                    Durum: <span style={{
                      backgroundColor: installment.statusDisplay === 'Ödendi' ? '#28a745' :
                        installment.statusDisplay === 'Gecikmiş' ? '#dc3545' : '#ffc107',
                      color: installment.statusDisplay === 'Ödendi' ? 'white' : 'black',
                      padding: '2px 8px',
                      borderRadius: '3px'
                    }}>
                      {installment.statusDisplay}
                    </span>
                  </div>
                </div>
                {installment.statusDisplay !== 'Ödendi' && (
                  <button
                    onClick={() => handleMakePayment(installment.id)}
                    style={{
                      padding: '8px 16px',
                      backgroundColor: '#007bff',
                      color: 'white',
                      border: 'none',
                      borderRadius: '3px',
                      cursor: 'pointer',
                      fontSize: '12px'
                    }}
                  >
                    💳 Öde
                  </button>
                )}
              </div>
            ))}
          </div>
        )}
      </div>

      {/* Ödenmemiş Taksitler */}
      {summary.unpaidInstallments.length > 0 && (
        <div>
          <h3>⚠️ Ödenmemiş Taksitler ({summary.unpaidInstallments.length})</h3>
          <div style={{ display: 'grid', gap: '10px' }}>
            {summary.unpaidInstallments.map((installment, index) => (
              <div key={index} style={{
                padding: '15px',
                backgroundColor: installment.isDelayed ? '#f8d7da' : '#fff3cd',
                borderRadius: '5px',
                border: `1px solid ${installment.isDelayed ? '#f5c6cb' : '#ffeaa7'}`,
                display: 'flex',
                justifyContent: 'space-between',
                alignItems: 'center'
              }}>
                <div>
                  <strong>Taksit #{installment.installmentNumber}</strong>
                  <div style={{ fontSize: '12px', color: '#666' }}>
                    Tutar: ₺{installment.amount.toFixed(2)} | Vade: {new Date(installment.dueDate).toLocaleDateString('tr-TR')}
                  </div>
                  {installment.isDelayed && (
                    <div style={{ fontSize: '12px', color: '#dc3545', fontWeight: 'bold' }}>
                      ⚠️ GECİKMİŞ
                    </div>
                  )}
                </div>
                <button
                  onClick={() => handleMakePayment(installment.id)}
                  style={{
                    padding: '8px 16px',
                    backgroundColor: '#dc3545',
                    color: 'white',
                    border: 'none',
                    borderRadius: '3px',
                    cursor: 'pointer',
                    fontSize: '12px'
                  }}
                >
                  💳 Öde
                </button>
              </div>
            ))}
          </div>
        </div>
      )}
    </div>
  );
}
