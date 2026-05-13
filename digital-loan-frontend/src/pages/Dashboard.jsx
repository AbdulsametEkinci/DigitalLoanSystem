// src/pages/Dashboard.jsx
import { useEffect, useState, useContext } from 'react';
import { Link, useNavigate } from 'react-router-dom';
import { customerService } from '../services/customerService';
import { CustomerContext } from '../CustomerContext';

export default function Dashboard() {
    const navigate = useNavigate();
    const { selectedCustomerId, selectCustomer } = useContext(CustomerContext);
    const [summary, setSummary] = useState(null);
    const [loading, setLoading] = useState(false);
    const [error, setError] = useState('');

    const loadSummary = async () => {
        if (!selectedCustomerId) {
            setSummary(null);
            return;
        }
        
        setLoading(true);
        try {
            const data = await customerService.getSummary(selectedCustomerId);
            setSummary(data);
        } catch (err) {
            setError('Özet bilgileri çekilirken hata oluştu.');
            console.error(err);
        } finally {
            setLoading(false);
        }
    };

    useEffect(() => {
        loadSummary();
    }, [selectedCustomerId]);

    const getStatusColor = (status) => {
        if (status === 'Ödendi') return '#28a745'; 
        if (status === 'Gecikmiş') return '#d9534f'; 
        return '#6c757d'; 
    };

    const isPaymentDue = (dueDateString) => {
        const today = new Date();
        const dueDate = new Date(dueDateString);
        
        if (dueDate.getFullYear() < today.getFullYear()) return true;
        if (dueDate.getFullYear() === today.getFullYear() && dueDate.getMonth() <= today.getMonth()) {
            return true;
        }
        
        return false;
    };

    if (!selectedCustomerId) {
        return (
            <div style={{ textAlign: 'center', padding: '40px' }}>
                <h2>👥 Müşteri Seçiniz</h2>
                <p style={{ fontSize: '16px', color: '#666', marginBottom: '20px' }}>
                    Kredi işlemlerine başlamak için müşteri sayfasından bir müşteri seçiniz.
                </p>
                <button 
                    onClick={() => navigate('/customers')}
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
                    🔄 Müşteri Seç
                </button>
            </div>
        );
    }

    if (loading) return <h3>Yükleniyor... Lütfen bekleyin.</h3>;
    if (error) return <h3 style={{ color: 'red' }}>{error}</h3>;
    if (!summary) return <h3>Veri bulunamadı.</h3>;

    const allowedStatuses = ['Ödendi', 'Ödenmedi', 'Gecikmiş'];
    const displayInstallments = summary.installments?.filter(i => 
        allowedStatuses.includes(i.statusDisplay)
    ) || [];

    return (
        <div>
            <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginBottom: '20px' }}>
                <h2>Finansal Özetiniz</h2>
                <button 
                    onClick={() => navigate('/customers')}
                    style={{
                        padding: '8px 16px',
                        backgroundColor: '#6c757d',
                        color: 'white',
                        border: 'none',
                        borderRadius: '3px',
                        cursor: 'pointer'
                    }}
                >
                    🔄 Müşteri Değiştir
                </button>
            </div>
            
            <div style={{ display: 'flex', gap: '20px', marginBottom: '30px' }}>
                <div style={cardStyle}>
                    <h4>Toplam Kredi Borcu</h4>
                    <h2 style={{ color: '#d9534f' }}>₺{summary.totalRemainingDebt.toFixed(2)}</h2>
                </div>
                <div style={cardStyle}>
                    <h4>Kalan Anapara</h4>
                    <h2 style={{ color: '#5cb85c' }}>₺{summary.remainingPrincipal.toFixed(2)}</h2>
                </div>
                <div style={cardStyle}>
                    <h4>Gecikmiş Taksit Sayısı</h4>
                    <h2 style={{ color: summary.delayedInstallmentsCount > 0 ? 'red' : 'black' }}>
                        {summary.delayedInstallmentsCount} Adet
                    </h2>
                </div>
            </div>

            <h3>Taksit Planınız</h3>
            {displayInstallments.length > 0 ? (
                <table style={{ width: '100%', borderCollapse: 'collapse', textAlign: 'left' }}>
                    <thead>
                        <tr style={{ backgroundColor: '#f2f2f2' }}>
                            <th style={thTdStyle}>Taksit No</th>
                            <th style={thTdStyle}>Son Ödeme Tarihi</th>
                            <th style={thTdStyle}>Tutar</th>
                            <th style={thTdStyle}>Durum</th>
                            <th style={thTdStyle}>İşlem</th>
                        </tr>
                    </thead>
                    <tbody>
                        {displayInstallments.map((item, index) => {
                            const canBePaid = item.statusDisplay !== 'Ödendi' && isPaymentDue(item.dueDate);

                            return (
                                <tr key={index} style={{ borderBottom: '1px solid #ddd' }}>
                                    <td style={thTdStyle}>{item.installmentNumber}</td>
                                    <td style={thTdStyle}>{new Date(item.dueDate).toLocaleDateString('tr-TR')}</td>
                                    <td style={thTdStyle}><strong>₺{item.amount.toFixed(2)}</strong></td>
                                    <td style={thTdStyle}>
                                        <span style={{ 
                                            color: 'white', 
                                            backgroundColor: getStatusColor(item.statusDisplay),
                                            padding: '4px 8px',
                                            borderRadius: '4px',
                                            fontWeight: 'bold',
                                            fontSize: '13px'
                                        }}>
                                            {item.statusDisplay.toUpperCase()}
                                        </span>
                                    </td>
                                    <td style={thTdStyle}>
                                        {item.statusDisplay === 'Ödendi' ? (
                                            <span style={{ color: '#28a745', fontWeight: 'bold' }}>&#10003; Tamamlandı</span>
                                        ) : canBePaid ? (
                                            <Link 
                                                to="/payment" 
                                                state={{ installment: item }} 
                                                style={btnStyle}
                                            >
                                                Hemen Öde
                                            </Link>
                                        ) : (
                                            <span style={{ color: '#6c757d', fontSize: '12px', fontStyle: 'italic' }}>Günü Gelmedi</span>
                                        )}
                                    </td>
                                </tr>
                            );
                        })}
                    </tbody>
                </table>
            ) : (
                <p>Bekleyen taksitiniz bulunmamaktadır.</p>
            )}
        </div>
    );
}

const cardStyle = { border: '1px solid #ccc', padding: '15px', borderRadius: '8px', width: '30%', textAlign: 'center', backgroundColor: '#f9f9f9', boxShadow: '0 2px 4px rgba(0,0,0,0.1)' };
const thTdStyle = { padding: '12px', border: '1px solid #ddd' };
const btnStyle = { backgroundColor: '#007bff', color: 'white', padding: '8px 12px', textDecoration: 'none', borderRadius: '4px', fontSize: '14px', fontWeight: 'bold' };