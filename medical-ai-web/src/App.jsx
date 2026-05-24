import { useState } from 'react'
import reactLogo from './assets/react.svg'
import viteLogo from './assets/vite.svg'
import heroImg from './assets/hero.png'
import './App.css'
import { BrowserRouter as Router, Routes, Route, Navigate } from 'react-router-dom';
import LoginPage from './pages/LoginPage';
import ClinicalForm from './pages/ClinicalForm';
import ResultDashboard from './pages/ResultDashboard';
import MainLayout from './components/MainLayout';

function App() {
  const isAuthenticated = !!localStorage.getItem('jwt_token');

  return (
    <Router>
      <Routes>
        <Route path="/login" element={<LoginPage />} />
        
        {/* Các trang cần đăng nhập sẽ nằm trong MainLayout (có Sidebar/Header) */}
        <Route path="/" element={isAuthenticated ? <MainLayout /> : <Navigate to="/login" />}>
          <Route index element={<Navigate to="/clinical-form" />} />
          <Route path="clinical-form" element={<ClinicalForm />} />
          <Route path="result/:checkupId" element={<ResultDashboard />} />
        </Route>
      </Routes>
    </Router>
  );
}

export default App;
