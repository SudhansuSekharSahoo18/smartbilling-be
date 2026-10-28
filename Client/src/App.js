import './App.css';
import React, { useEffect, useState } from 'react';
import { BrowserRouter as Router, Routes, Route, NavLink, useLocation } from 'react-router-dom';
import Items from './components/view/Items/Items';
import Bills from './components/view/Bills/Bills';
import BarcodeView from './components/view/Barcode/BarcodeView';
import LabelDesigner from './components/view/LabelDesign/LabelDesigner';
import Billing from './components/view/Billing/Billing';
import SaleReport from './components/view/SaleReport/SaleReport';
import { ToastContainer, toast } from 'react-toastify';
import 'react-toastify/dist/ReactToastify.css';
import ErrorView from './components/view/Error/ErrorView';
import ErrorPageNotFoundView from './components/view/Error/ErrorPageNotFoundVew';
import Setting from './components/view/Setting/Setting';
import SetupView from './components/view/Setting/SetupView';
import Dashboard from './components/view/Dashboard/Dashboard';
import Staff from './components/view/Staff/Staff';
import {
  DashboardIcon, StockIcon, BillingIcon, BillsIcon, BarcodeIcon, LabelIcon, ReportIcon, StaffIcon, SettingIcon,
} from './components/Icons';
import LoginView from './components/view/Login/LoginView';
import { getBackendUrl, getConfig } from './Helper/apiHelper.js';
import { AUTH_REQUIRED_EVENT, getAuthStatus, signOut } from './Helper/auth.js';

const APP_VERSION = '1.0.3';

const NAV_ITEMS = [
  { path: '/', label: 'Dashboard', icon: DashboardIcon, subtitle: 'Sales at a glance' },
  { path: '/items', label: 'Stock', icon: StockIcon, subtitle: 'Add, edit, import and export items' },
  { path: '/pos', label: 'Billing', icon: BillingIcon, subtitle: 'Scan items and create a bill' },
  { path: '/bills', label: 'Bills', icon: BillsIcon, subtitle: 'Saved bills. Double-click a bill to see its items' },
  { path: '/generateBarcode', label: 'Barcode', icon: BarcodeIcon, subtitle: 'Items waiting for barcode labels' },
  { path: '/labelDesign', label: 'Label Design', icon: LabelIcon, subtitle: 'What goes on a barcode label' },
  { path: '/saleReport', label: 'Sale Report', icon: ReportIcon, subtitle: 'Monthly sale register and tax summary' },
  { path: '/staff', label: 'Staff', icon: StaffIcon, subtitle: 'Accounts that can sign in', adminOnly: true },
  { path: '/setting', label: 'Setting', icon: SettingIcon, subtitle: 'Shop details, label printer and health checks' },
];

const PageHeader = () => {
  const { pathname } = useLocation();
  const page = NAV_ITEMS.find(item => item.path === pathname);
  if (!page) return null;
  return (
    <header className="page-header">
      <h1 className="page-title">{page.label}</h1>
      <p className="page-subtitle">{page.subtitle}</p>
    </header>
  );
};

const App = () => {
  const [ipAddress, setIpAddress] = useState(null);
  // 'checking' | 'firstStart' | 'signedOut' | 'signedIn' | 'offline'
  const [authState, setAuthState] = useState('checking');
  const [userName, setUserName] = useState('');
  const [isAdmin, setIsAdmin] = useState(false);
  const [shopName, setShopName] = useState('');
  const [needsSetup, setNeedsSetup] = useState(false);
  const notify = (type, message) => {
    if (type === 'error')
      toast.error(message)
    else if (type === 'warning')
      toast.warning(message)
    else if (type === 'success')
      toast.success(message)
    else
      toast.info(message)
  }

  const applyConfig = (data) => {
    setShopName(data.shopName ?? '');
  };

  const checkSignIn = () => {
    setAuthState('checking');
    getAuthStatus()
      .then(status => {
        setUserName(status.userName ?? '');
        setIsAdmin(!!status.isAdmin);
        setAuthState(status.signedIn ? 'signedIn' : status.needsAdminPassword ? 'firstStart' : 'signedOut');
      })
      .catch(error => {
        console.error('Error checking sign-in:', error);
        setAuthState('offline');
      });
  };

  useEffect(() => {
    checkSignIn();
    // A 401 from any page means the session has ended.
    const onAuthRequired = () => setAuthState(state => (state === 'signedIn' ? 'signedOut' : state));
    window.addEventListener(AUTH_REQUIRED_EVENT, onAuthRequired);
    return () => window.removeEventListener(AUTH_REQUIRED_EVENT, onAuthRequired);
  }, []);

  const onSignOutClicked = () => {
    signOut()
      .catch(error => console.error('Error signing out:', error))
      .finally(() => setAuthState('signedOut'));
  };

  useEffect(() => {
    if (authState !== 'signedIn') return;
    Promise.all([getBackendUrl(), getConfig()])
      .then(([url, data]) => {
        setIpAddress(url);
        applyConfig(data);
        setNeedsSetup(!data.isSetupComplete);
      })
      .catch(error => {
        console.error('Error fetching config:', error)
      });
  }, [authState]);

  if (authState === 'checking') {
    return <div className="app app--setup" />;
  }

  if (authState === 'offline') {
    return (
      <div className="app app--setup">
        <div className="setup-page">
          <div className="card setup-card login-card">
            <h2 className="card-title">Can't reach the Smart Billing server</h2>
            <p className="card-subtitle">Check that the server is running, then try again.</p>
            <button onClick={checkSignIn}>Try again</button>
          </div>
        </div>
      </div>
    );
  }

  if (authState === 'firstStart' || authState === 'signedOut') {
    return (
      <div className="app app--setup">
        <LoginView isFirstStart={authState === 'firstStart'} onSignedIn={(user) => {
          setUserName(user.userName);
          setIsAdmin(!!user.isAdmin);
          setAuthState('signedIn');
        }} />
      </div>
    );
  }

  if (needsSetup) {
    return (
      <div className="app app--setup">
        <ToastContainer position="bottom-right" />
        <SetupView notify={notify} onComplete={(data) => {
          applyConfig(data);
          setNeedsSetup(false);
        }} />
      </div>
    );
  }

  return (
    <Router>
      <div className="app">
        <aside className="sidebar">
          <div className="brand">
            <span className="brand-mark">SB</span>
            <div>
              <div className="brand-name">Smart Billing</div>
              {shopName && <div className="brand-shop" title={shopName}>{shopName}</div>}
            </div>
          </div>
          <nav className="sidebar-nav">
            {NAV_ITEMS.filter(item => isAdmin || !item.adminOnly).map(({ path, label, icon: Icon }) => (
              <NavLink key={path} to={path} end className="sidebar-link">
                <Icon />
                <span>{label}</span>
              </NavLink>
            ))}
          </nav>
          <div className="sidebar-footer">
            <div className="sidebar-user">
              <span title={userName}>{userName}</span>
              <button className="sidebar-signout" onClick={onSignOutClicked}>Sign out</button>
            </div>
            Version {APP_VERSION}
          </div>
        </aside>
        <ToastContainer position="bottom-right" />
        <main className="main">
          <PageHeader />
          <div className="content">
            <Routes>
              <Route path="/" element={<Dashboard notify={notify} ipAddress={ipAddress} />} />
              <Route path="/items" element={<Items notify={notify} ipAddress={ipAddress} />} />
              <Route path="/pos" element={<Billing notify={notify} ipAddress={ipAddress} />} />
              <Route path="/bills" element={<Bills notify={notify} ipAddress={ipAddress} />} />
              <Route path="/generateBarcode" element={<BarcodeView notify={notify} ipAddress={ipAddress} />} />
              <Route path="/labelDesign" element={<LabelDesigner notify={notify} />} />
              <Route path="/saleReport" element={<SaleReport notify={notify} ipAddress={ipAddress} />} />
              {isAdmin && <Route path="/staff" element={<Staff notify={notify} currentUserName={userName} />} />}
              <Route path="/setting" element={<Setting notify={notify} ipAddress={ipAddress}
                onSettingsSaved={applyConfig} />} />
              <Route path="/error" element={<ErrorView message={'Not able to connect to server'} />} />
              <Route path="*" element={<ErrorPageNotFoundView />} />
            </Routes>
          </div>
        </main>
      </div>
    </Router>
  );
};

export default App;
