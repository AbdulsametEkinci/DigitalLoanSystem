import { useState, useContext, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import { customerService } from '../services/customerService';
import { CustomerContext } from '../CustomerContext';

export default function Customers() {
  const navigate = useNavigate();
  const { selectCustomer } = useContext(CustomerContext);

  // Form State
  const [formData, setFormData] = useState({
    identityNumber: '',
    fullName: '',
    email: '',
    phoneNumber: ''
  });
  const [editingId, setEditingId] = useState(null);
  const [customers, setCustomers] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');
  const [success, setSuccess] = useState('');

  // Sayfa açıldığında müşterileri yükle
  useEffect(() => {
    loadCustomers();
  }, []);

  // Müşteri listesini getir
  const loadCustomers = async () => {
    setLoading(true);
    try {
      const data = await customerService.getAllCustomers();
      setCustomers(data);
    } catch (err) {
      console.error('Müşteri yükleme hatası:', err);
      setError('Müşteriler yüklenirken hata oluştu');
    } finally {
      setLoading(false);
    }
  };

  // Form alanları değişince
  const handleInputChange = (e) => {
    const { name, value } = e.target;
    setFormData(prev => ({
      ...prev,
      [name]: value
    }));
  };

  // Müşteri oluştur veya güncelle
  const handleSubmit = async (e) => {
    e.preventDefault();
    setLoading(true);
    setError('');
    setSuccess('');

    try {
      if (editingId) {
        // Güncelleme
        const updateDto = {
          fullName: formData.fullName,
          email: formData.email,
          phoneNumber: formData.phoneNumber
        };
        const updated = await customerService.updateCustomer(editingId, updateDto);
        setCustomers(prev => prev.map(c => c.id === editingId ? updated : c));
        setSuccess('Müşteri başarıyla güncellendi!');
        setEditingId(null);
      } else {
        // Oluştur
        const created = await customerService.createCustomer(formData);
        setCustomers(prev => [...prev, created]);
        setSuccess('Müşteri başarıyla oluşturuldu!');
      }
      setFormData({ identityNumber: '', fullName: '', email: '', phoneNumber: '' });
      
      // Başarı mesajını 3 saniye sonra temizle
      setTimeout(() => setSuccess(''), 3000);
    } catch (err) {
      const errorMsg = err.response?.data?.Error || 'Bir hata oluştu';
      setError(errorMsg);
      console.error('İşlem hatası:', err);
    } finally {
      setLoading(false);
    }
  };

  // Müşteri seç ve detay sayfasına git
  const handleSelectCustomer = (customerId) => {
    selectCustomer(customerId);
    navigate('/customer-detail');
  };

  // Müşteri düzenle
  const handleEditCustomer = async (customer) => {
    setEditingId(customer.id);
    setFormData({
      identityNumber: customer.identityNumber,
      fullName: customer.fullName,
      email: customer.email,
      phoneNumber: customer.phoneNumber
    });
  };

  // Müşteri sil
  const handleDeleteCustomer = async (customerId) => {
    if (window.confirm('Bu müşteri silinecek. Emin misiniz?')) {
      setLoading(true);
      try {
        await customerService.deleteCustomer(customerId);
        setCustomers(prev => prev.filter(c => c.id !== customerId));
        setSuccess('Müşteri silindi!');
      } catch (err) {
        setError(err.response?.data?.Error || 'Silme işleminde hata');
      } finally {
        setLoading(false);
      }
    }
  };

  // İptal
  const handleCancel = () => {
    setEditingId(null);
    setFormData({ identityNumber: '', fullName: '', email: '', phoneNumber: '' });
  };

  return (
    <div style={{ maxWidth: '1000px', margin: '0 auto', padding: '20px' }}>
      <h2>👥 Müşteri Yönetimi</h2>

      {/* Hata/Başarı Mesajları */}
      {error && <div style={{ padding: '10px', backgroundColor: '#f8d7da', color: '#721c24', borderRadius: '5px', marginBottom: '10px' }}>{error}</div>}
      {success && <div style={{ padding: '10px', backgroundColor: '#d4edda', color: '#155724', borderRadius: '5px', marginBottom: '10px' }}>{success}</div>}

      {/* Form */}
      <form onSubmit={handleSubmit} style={{ backgroundColor: '#f9f9f9', padding: '20px', borderRadius: '5px', marginBottom: '20px' }}>
        <h3>{editingId ? '✏️ Müşteri Güncelle' : '➕ Yeni Müşteri Oluştur'}</h3>

        <div style={{ marginBottom: '15px' }}>
          <label style={{ display: 'block', marginBottom: '5px', fontWeight: 'bold' }}>TCKN:</label>
          <input
            type="text"
            name="identityNumber"
            value={formData.identityNumber}
            onChange={handleInputChange}
            disabled={editingId}
            placeholder="Kimlik numarası"
            required
            style={{ width: '100%', padding: '8px', fontSize: '14px', borderRadius: '3px', border: '1px solid #ccc' }}
          />
        </div>

        <div style={{ marginBottom: '15px' }}>
          <label style={{ display: 'block', marginBottom: '5px', fontWeight: 'bold' }}>Ad Soyad:</label>
          <input
            type="text"
            name="fullName"
            value={formData.fullName}
            onChange={handleInputChange}
            placeholder="Adı Soyadı"
            required
            style={{ width: '100%', padding: '8px', fontSize: '14px', borderRadius: '3px', border: '1px solid #ccc' }}
          />
        </div>

        <div style={{ marginBottom: '15px' }}>
          <label style={{ display: 'block', marginBottom: '5px', fontWeight: 'bold' }}>Email:</label>
          <input
            type="email"
            name="email"
            value={formData.email}
            onChange={handleInputChange}
            placeholder="example@mail.com"
            required
            style={{ width: '100%', padding: '8px', fontSize: '14px', borderRadius: '3px', border: '1px solid #ccc' }}
          />
        </div>

        <div style={{ marginBottom: '15px' }}>
          <label style={{ display: 'block', marginBottom: '5px', fontWeight: 'bold' }}>Telefon:</label>
          <input
            type="tel"
            name="phoneNumber"
            value={formData.phoneNumber}
            onChange={handleInputChange}
            placeholder="+905551234567"
            required
            style={{ width: '100%', padding: '8px', fontSize: '14px', borderRadius: '3px', border: '1px solid #ccc' }}
          />
        </div>

        <div style={{ display: 'flex', gap: '10px' }}>
          <button type="submit" disabled={loading} style={{ padding: '10px 20px', backgroundColor: '#28a745', color: 'white', border: 'none', borderRadius: '3px', cursor: 'pointer' }}>
            {loading ? 'İşleniyor...' : (editingId ? 'Güncelle' : 'Oluştur')}
          </button>
          {editingId && (
            <button type="button" onClick={handleCancel} style={{ padding: '10px 20px', backgroundColor: '#6c757d', color: 'white', border: 'none', borderRadius: '3px', cursor: 'pointer' }}>
              İptal
            </button>
          )}
        </div>
      </form>

      {/* Müşteri Listesi */}
      <div>
        <h3>📋 Müşteri Listesi</h3>
        {loading && customers.length === 0 ? (
          <p style={{ color: '#666', textAlign: 'center', padding: '20px' }}>Müşteriler yükleniyor...</p>
        ) : customers.length === 0 ? (
          <p style={{ color: '#666' }}>Henüz müşteri eklenmedi.</p>
        ) : (
          <div style={{ display: 'grid', gap: '10px' }}>
            {customers.map(customer => (
              <div key={customer.id} style={{ padding: '15px', backgroundColor: '#f0f0f0', borderRadius: '5px', border: '1px solid #ddd' }}>
                <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center' }}>
                  <div>
                    <strong>{customer.fullName}</strong>
                    <div style={{ fontSize: '12px', color: '#666' }}>
                      TCKN: {customer.identityNumber} | {customer.email} | {customer.phoneNumber}
                    </div>
                  </div>
                  <div style={{ display: 'flex', gap: '10px' }}>
                    <button
                      onClick={() => handleSelectCustomer(customer.id)}
                      style={{ padding: '8px 12px', backgroundColor: '#007bff', color: 'white', border: 'none', borderRadius: '3px', cursor: 'pointer', fontSize: '12px' }}
                    >
                      📊 Detay
                    </button>
                    <button
                      onClick={() => handleEditCustomer(customer)}
                      style={{ padding: '8px 12px', backgroundColor: '#ffc107', color: 'black', border: 'none', borderRadius: '3px', cursor: 'pointer', fontSize: '12px' }}
                    >
                      ✏️ Düzenle
                    </button>
                    <button
                      onClick={() => handleDeleteCustomer(customer.id)}
                      style={{ padding: '8px 12px', backgroundColor: '#dc3545', color: 'white', border: 'none', borderRadius: '3px', cursor: 'pointer', fontSize: '12px' }}
                    >
                      🗑️ Sil
                    </button>
                  </div>
                </div>
              </div>
            ))}
          </div>
        )}
      </div>
    </div>
  );
}
