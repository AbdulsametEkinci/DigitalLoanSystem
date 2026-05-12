// src/pages/PaymentPage.jsx
import { useState } from 'react';
import { useLocation, useNavigate, Link } from 'react-router-dom';
import { paymentService } from '../services/paymentService';

export default function PaymentPage() {
    const location = useLocation();
    const navigate = useNavigate();
    
    // Dashboard'dan Link ile gönderdiğimiz taksit verisini alıyoruz
    const installment = location.state?.installment;

    const [cardNumber, setCardNumber] = useState('');
    const [expiryDate, setExpiryDate] = useState('');
    const [loading, setLoading] = useState(false);
    const [error, setError] = useState('');

    // Sayfaya direkt link yazarak girilirse (verisiz) geri atalım
    if (!installment) {
        return <h3>Geçersiz işlem. Lütfen Ana Sayfadan bir taksit seçin.</h3>;
    }

    const handlePayment = async (e) => {
        e.preventDefault();
        setLoading(true);
        setError('');

        const requestDto = {
            installmentId: installment.installmentId || installment.id, // DTO'dan gelen ID
            cardNumber: cardNumber,
            expiryDate: expiryDate
        };

        try {
            await paymentService.pay(requestDto);
            alert("Ödeme başarıyla alındı!");
            navigate('/'); // Başarılı olunca Dashboard'a dön
        } catch (err) {
            const errorMsg = err.response?.data?.error || "Ödeme işlemi başarısız oldu.";
            setError(errorMsg);
        } finally {
            setLoading(false);
        }
    };

    return (
        <div style={{ maxWidth: '400px', margin: '0 auto' }}>
            <h2>Güvenli Ödeme Ekranı</h2>
            <Link to="/" style={{ textDecoration: 'none', color: '#004085', fontWeight: 'bold' }}>&larr; Geri Dön</Link>
            
            <div style={{ backgroundColor: '#f8f9fa', padding: '15px', borderRadius: '8px', marginTop: '20px', border: '1px solid #ddd' }}>
                <h3 style={{ margin: '0 0 10px 0' }}>Taksit No: {installment.installmentNumber}</h3>
                <h1 style={{ margin: '0', color: '#28a745', fontSize: '24px' }}>Ödenecek Tutar: {installment.amount} ₺</h1>
            </div>

            <br />
            {error && <div style={{ color: 'white', backgroundColor: '#d9534f', padding: '10px', marginBottom: '15px', borderRadius: '5px' }}>{error}</div>}

            <form onSubmit={handlePayment} style={{ display: 'flex', flexDirection: 'column', gap: '15px' }}>
                <div>
                    <label style={{ fontWeight: 'bold', display: 'block', marginBottom: '5px' }}>Kart Numarası (4000 yazılırsa reddedilir):</label>
                    <input 
                        type="text" 
                        value={cardNumber} 
                        onChange={(e) => setCardNumber(e.target.value)} 
                        style={inputStyle} 
                        placeholder="Örn: 5000 1234 5678 9010"
                        maxLength="16"
                        required 
                    />
                </div>

                <div>
                    <label style={{ fontWeight: 'bold', display: 'block', marginBottom: '5px' }}>Son Kullanma Tarihi:</label>
                    <input 
                        type="text" 
                        value={expiryDate} 
                        onChange={(e) => setExpiryDate(e.target.value)} 
                        style={inputStyle} 
                        placeholder="AA/YY"
                        maxLength="5"
                        required 
                    />
                </div>

                <button type="submit" disabled={loading} style={{ backgroundColor: '#007bff', color: 'white', padding: '12px', border: 'none', borderRadius: '4px', cursor: 'pointer', fontWeight: 'bold', fontSize: '16px' }}>
                    {loading ? 'İşleniyor...' : 'Ödemeyi Tamamla'}
                </button>
            </form>
        </div>
    );
}

const inputStyle = { width: '100%', padding: '10px', borderRadius: '4px', border: '1px solid #ccc', boxSizing: 'border-box' };