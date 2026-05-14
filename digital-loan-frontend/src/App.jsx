import { BrowserRouter as Router, Routes, Route, Link } from 'react-router-dom';
import { CustomerProvider } from './CustomerContext';
import Dashboard from './pages/Dashboard';
import Customers from './pages/Customers';
import CustomerDetail from './pages/CustomerDetail';
import ApplyLoan from './pages/ApplyLoan';
import PaymentPage from './pages/PaymentPage';

function App() {
  return (
    <CustomerProvider>
      <Router>
        <div style={{ maxWidth: '900px', margin: '0 auto', fontFamily: 'Arial, sans-serif' }}>
          
          {/* Üst Menü (Navbar) */}
          <nav style={{ padding: '15px', backgroundColor: '#004085', color: 'white', marginBottom: '20px', borderRadius: '5px' }}>
            <h1 style={{ margin: '0 0 10px 0' }}>Dijital Kredi Sistemi</h1>
            <div style={{ display: 'flex', gap: '15px', flexWrap: 'wrap' }}>
              <Link to="/" style={linkStyle}>📊 Ana Sayfa</Link>
              <Link to="/customers" style={linkStyle}>👥 Müşteriler</Link>
              <Link to="/apply" style={linkStyle}>📝 Kredi Başvurusu</Link>
            </div>
          </nav>

          {/* Sayfaların Yükleneceği Alan */}
          <main style={{ padding: '20px', border: '1px solid #ddd', borderRadius: '5px' }}>
            <Routes>
              <Route path="/" element={<Dashboard />} />
              <Route path="/customers" element={<Customers />} />
              <Route path="/customer-detail" element={<CustomerDetail />} />
              <Route path="/apply" element={<ApplyLoan />} />
              <Route path="/payment" element={<PaymentPage />} />
            </Routes>
          </main>

        </div>
      </Router>
    </CustomerProvider>
  );
}

const linkStyle = {
  color: 'white',
  textDecoration: 'none',
  fontWeight: 'bold',
  padding: '8px 12px',
  borderRadius: '3px',
  transition: 'background-color 0.2s',
};

export default App;
