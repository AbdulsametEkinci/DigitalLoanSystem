// src/pages/ApplyLoan.jsx
import { useState, useContext } from 'react';
import { useNavigate } from 'react-router-dom';
import { loanService } from '../services/loanService';
import { CustomerContext } from '../CustomerContext';

export default function ApplyLoan() {
    const navigate = useNavigate();
    const { selectedCustomerId } = useContext(CustomerContext);
    
    // Form verilerini tutacağımız State'ler
    const [loanType, setLoanType] = useState(1);
    const [principalAmount, setPrincipalAmount] = useState('');
    const [termInMonths, setTermInMonths] = useState('');
    
    // UI Durumları
    const [loading, setLoading] = useState(false);
    const [error, setError] = useState('');

    const handleSubmit = async (e) => {
        e.preventDefault();
        
        if (!selectedCustomerId) {
            setError('Lütfen müşteri seçiniz!');
            return;
        }

        setLoading(true);
        setError('');

        const requestDto = {
            customerId: selectedCustomerId,
            loanType: parseInt(loanType),
            principalAmount: parseFloat(principalAmount),
            termInMonths: parseInt(termInMonths)
        };

        try {
            await loanService.applyForLoan(requestDto);
            alert("Tebrikler! Krediniz onaylandı ve taksit planınız oluşturuldu.");
            navigate('/customer-detail');
        } catch (err) {
            const errorMsg = err.response?.data?.error || "Bir hata oluştu.";
            setError(errorMsg);
            console.error(err);
        } finally {
            setLoading(false);
        }
    };

    return (
        <div style={{ maxWidth: '500px', margin: '0 auto' }}>
            <h2>Yeni Kredi Başvurusu</h2>
            <p>Size uygun kredi türünü ve tutarını seçin. Güncel faiz oranları otomatik hesaplanacaktır.</p>

            {error && <div style={{ color: 'white', backgroundColor: '#d9534f', padding: '10px', marginBottom: '15px', borderRadius: '5px' }}>{error}</div>}
            {!selectedCustomerId && (
                <div style={{ color: 'white', backgroundColor: '#ffc107', padding: '10px', marginBottom: '15px', borderRadius: '5px' }}>
                    ⚠️ Müşteri seçiniz
                </div>
            )}

            <form onSubmit={handleSubmit} style={{ display: 'flex', flexDirection: 'column', gap: '15px' }}>
                
                <div>
                    <label style={labelStyle}>Kredi Türü:</label>
                    <select value={loanType} onChange={(e) => setLoanType(e.target.value)} style={inputStyle} required>
                        <option value={1}>İhtiyaç Kredisi</option>
                        <option value={2}>Eğitim Kredisi</option>
                        <option value={3}>Taşıt Kredisi</option>
                    </select>
                </div>

                <div>
                    <label style={labelStyle}>İstediğiniz Tutar (TL):</label>
                    <input 
                        type="number" 
                        min="1000" 
                        step="0.01"
                        value={principalAmount} 
                        onChange={(e) => setPrincipalAmount(e.target.value)} 
                        style={inputStyle} 
                        placeholder="Örn: 50000"
                        required 
                    />
                </div>

                <div>
                    <label style={labelStyle}>Vade (Ay):</label>
                    <input 
                        type="number" 
                        min="1" 
                        max="120"
                        value={termInMonths} 
                        onChange={(e) => setTermInMonths(e.target.value)} 
                        style={inputStyle} 
                        placeholder="Örn: 12"
                        required 
                    />
                </div>

                <button type="submit" disabled={loading || !selectedCustomerId} style={buttonStyle}>
                    {loading ? 'İşleniyor...' : 'Krediye Başvur'}
                </button>

                <button 
                    type="button" 
                    onClick={() => navigate('/customer-detail')} 
                    style={{ ...buttonStyle, backgroundColor: '#6c757d' }}
                >
                    ← Geri Dön
                </button>
            </form>
        </div>
    );
}

const labelStyle = { fontWeight: 'bold', marginBottom: '5px', display: 'block' };
const inputStyle = { width: '100%', padding: '10px', borderRadius: '4px', border: '1px solid #ccc', boxSizing: 'border-box' };
const buttonStyle = { backgroundColor: '#28a745', color: 'white', padding: '12px', border: 'none', borderRadius: '4px', cursor: 'pointer', fontWeight: 'bold', fontSize: '16px' };