// src/pages/Dashboard.jsx
import { useEffect, useState } from 'react';
import { Link } from 'react-router-dom';
import { customerService } from '../services/customerService';

const CUSTOMER_ID = "10b54c8e-ecb0-4eab-ad1a-5adc12f619c5"; // Kendi ID'ni unutma

export default function Dashboard() {
    const [summary, setSummary] = useState(null);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState('');

    const loadSummary = async () => {
        try {
            const data = await customerService.getSummary(CUSTOMER_ID);
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
    }, []);

    const getStatusColor = (status) => {
        if (status === 'Ödendi') return '#28a745'; 
        if (status === 'Gecikmiş') return '#d9534f'; 
        return '#6c757d'; 
    };

    // Yeni İş Kuralı: Taksitin günü gelmiş mi?
    // İçinde bulunduğumuz ay/yıl ile DueDate ay/yıl kıyaslanır.
    const isPaymentDue = (dueDateString) => {
        const today = new Date();
        const dueDate = new Date(dueDateString);
        
        // Eğer DueDate yılı bugünün yılından küçükse kesin ödenmeli
        if (dueDate.getFullYear() < today.getFullYear()) return true;
        
        // Eğer Yıl aynıysa ve Ay bugünün ayına küçük-eşitse ödenmeli
        if (dueDate.getFullYear() === today.getFullYear() && dueDate.getMonth() <= today.getMonth()) {
            return true;
        }
        
        return false; // Gelecek yıl veya gelecek aylarsa buton kapalı!
    };

    if (loading) return <h3>Yükleniyor... Lütfen bekleyin.</h3>;
    if (error) return <h3 style={{ color: 'red' }}>{error}</h3>;
    if (!summary) return <h3>Veri bulunamadı.</h3>;

    const allowedStatuses = ['Ödendi', 'Ödenmedi', 'Gecikmiş'];
    const displayInstallments = summary.installments?.filter(i => 
        allowedStatuses.includes(i.statusDisplay)
    ) || [];

    return (
        <div>
            <h2>Finansal Özetiniz</h2>
            
            <div style={{ display: 'flex', gap: '20px', marginBottom: '30px' }}>
                <div style={cardStyle}>
                    <h4>Toplam Kredi Borcu</h4>
                    <h2 style={{ color: '#d9534f' }}>{summary.totalRemainingDebt} ₺</h2>
                </div>
                <div style={cardStyle}>
                    <h4>Kalan Anapara</h4>
                    <h2 style={{ color: '#5cb85c' }}>{summary.remainingPrincipal} ₺</h2>
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
                            // Bu satır için ödenebilir mi kontrolünü yapıyoruz
                            const canBePaid = item.statusDisplay !== 'Ödendi' && isPaymentDue(item.dueDate);

                            return (
                                <tr key={index} style={{ borderBottom: '1px solid #ddd' }}>
                                    <td style={thTdStyle}>{item.installmentNumber}</td>
                                    <td style={thTdStyle}>{new Date(item.dueDate).toLocaleDateString('tr-TR')}</td>
                                    <td style={thTdStyle}><strong>{item.amount} ₺</strong></td>
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