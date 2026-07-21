import React from 'react';
import { LogsPage } from './pages/LogsPage';
import { LoginPage } from './pages/LoginPage';
import './index.css';

const App: React.FC = () => {
  
  const token = localStorage.getItem('token');
  const isAuthenticated = !!token;

  return (
    <>
      {isAuthenticated ? <LogsPage /> : <LoginPage />}
    </>
  );
};

export default App;