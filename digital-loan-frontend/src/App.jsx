import { BrowserRouter as Router, Routes, Route, Link } from 'react-router-dom';
import Dashboard from './pages/Dashboard';
import ApplyLoan from './pages/ApplyLoan';
import PaymentPage from './pages/PaymentPage';

function App() {
  return (
    <Router>
      <div style={{ maxWidth: '900px', margin: '0 auto', fontFamily: 'Arial, sans-serif' }}>
        
        {/* Üst Menü (Navbar) */}
        <nav style={{ padding: '15px', backgroundColor: '#004085', color: 'white', marginBottom: '20px', borderRadius: '5px' }}>
          <h1 style={{ margin: '0 0 10px 0' }}>Dijital Kredi Sistemi</h1>
          <div>
            <Link to="/" style={linkStyle}>Ana Sayfa (Özet)</Link>
            <Link to="/apply" style={linkStyle}>Kredi Başvurusu</Link>
          </div>
        </nav>

        {/* Sayfaların Yükleneceği Alan */}
        <main style={{ padding: '20px', border: '1px solid #ddd', borderRadius: '5px' }}>
          <Routes>
            <Route path="/" element={<Dashboard />} />
            <Route path="/apply" element={<ApplyLoan />} />
            <Route path="/payment" element={<PaymentPage />} />
          </Routes>
        </main>

      </div>
    </Router>
  );
}

const linkStyle = {
  color: 'white',
  textDecoration: 'none',
  marginRight: '20px',
  fontWeight: 'bold'
};

export default App;
