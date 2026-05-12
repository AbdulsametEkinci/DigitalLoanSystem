// src/pages/ApplyLoan.jsx
import { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { loanService } from '../services/loanService';

// Dashboard'da kullandığın ID'nin aynısı
const CUSTOMER_ID = "10b54c8e-ecb0-4eab-ad1a-5adc12f619c5";

export default function ApplyLoan() {
    const navigate = useNavigate(); // İşlem bitince ana sayfaya dönmek için
    
    // Form verilerini tutacağımız State'ler
    const [loanType, setLoanType] = useState(1); // 1: İhtiyaç, 2: Eğitim, 3: Taşıt
    const [principalAmount, setPrincipalAmount] = useState('');
    const [termInMonths, setTermInMonths] = useState('');
    
    // UI Durumları
    const [loading, setLoading] = useState(false);
    const [error, setError] = useState('');

    const handleSubmit = async (e) => {
        e.preventDefault(); // Sayfanın yenilenmesini engeller
        setLoading(true);
        setError('');

        // Backend'in beklediği DTO (CreateLoanRequestDto) formatı
        const requestDto = {
            customerId: CUSTOMER_ID,
            loanType: parseInt(loanType),
            principalAmount: parseFloat(principalAmount),
            termInMonths: parseInt(termInMonths)
        };

        try {
            await loanService.applyForLoan(requestDto);
            alert("Tebrikler! Krediniz onaylandı ve taksit planınız oluşturuldu.");
            navigate('/'); // Başarılı olunca Dashboard'a (Özet Ekranına) geri dön!
        } catch (err) {
            // Backend'den fırlattığımız "Kredi skoru yetersiz" vb. hatalar buraya düşer
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

                <button type="submit" disabled={loading} style={buttonStyle}>
                    {loading ? 'İşleniyor...' : 'Krediye Başvur'}
                </button>
            </form>
        </div>
    );
}

// Basit CSS Objesi
const labelStyle = { fontWeight: 'bold', marginBottom: '5px', display: 'block' };
const inputStyle = { width: '100%', padding: '10px', borderRadius: '4px', border: '1px solid #ccc', boxSizing: 'border-box' };
const buttonStyle = { backgroundColor: '#28a745', color: 'white', padding: '12px', border: 'none', borderRadius: '4px', cursor: 'pointer', fontWeight: 'bold', fontSize: '16px' };