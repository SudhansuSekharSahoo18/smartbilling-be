import './App.css';
import React, { useEffect, useState } from 'react';
import { BrowserRouter as Router, Routes, Route, Link } from 'react-router-dom';
import Items from './components/view/Items/Items';
import Bills from './components/view/Bills/Bills';
import BarcodeView from './components/view/Barcode/BarcodeView';
import Billing from './components/view/Billing/Billing';
import SaleReport from './components/view/SaleReport/SaleReport';
import { ToastContainer, toast } from 'react-toastify';
import 'react-toastify/dist/ReactToastify.css';
import ErrorView from './components/view/Error/ErrorView';
import ErrorPageNotFoundView from './components/view/Error/ErrorPageNotFoundVew';
import Setting from './components/view/Setting/Setting';
import Dashboard from './components/view/Dashboard/Dashboard';

const App = () => {
  const [ipAddress, setIpAddress] = useState(null);
  const [barcodeGenerateFilePath, setBarcodeGenerateFilePath] = useState(null);
  const [selectedTab, setSelectedTab] = useState('');
  const notify = (type, message) => {
    if(type == 'error')
      toast.error(message)
    else if(type == 'warning')
      toast.warning(message)
    else if(type == 'success')
        toast.success(message)
    else
      toast.update(message)
  }
  // const history = useHistory();

  // const handleNavigate = (url) => {
  //   history.push(url);
  // };

  useEffect(() => {
    fetch('/config.json')
      .then(response => response.json())
      .then(data => {
        if (process.env.REACT_APP_ENVIRONMENT === 'DEVELOPMENT')
          setIpAddress(data.backendUrl + 'api/')
        else if (process.env.REACT_APP_ENVIRONMENT === 'PRODUCTION')
          setIpAddress(data.backendUrl_PROD + 'api/')
        setBarcodeGenerateFilePath(data.BarcodeGenerateFilePath);
      })
      .catch(error => {
        console.error('Error fetching config:', error)
      });
  }, []);

  return (
    <Router>
      <div className="app">
        <nav className="nav">
        <div>vs 1.0.3</div>
          <ul>
            <li className={selectedTab === '' ? 'selectedNav' : ''}>
              <Link to="/"><div onClick={() => setSelectedTab('')}>Dashboard</div></Link>
            </li>
            <li className={selectedTab === 'items' ? 'selectedNav' : ''}>
              <Link to="/items"><div onClick={() => setSelectedTab('items')}>Stock</div></Link>
            </li>
            <li className={selectedTab === 'pos' ? 'selectedNav' : ''}>
              <Link to="/pos"><div onClick={() => setSelectedTab('pos')}>Billing</div></Link>
            </li>
            <li className={selectedTab === 'bills' ? 'selectedNav' : ''}>
              <Link to="/bills"><div onClick={() => setSelectedTab('bills')}>Bills</div></Link>
            </li>
            <li className={selectedTab === 'generateBarcode' ? 'selectedNav' : ''}>
              <Link to="/generateBarcode"><div onClick={() => setSelectedTab('generateBarcode')}>Barcode</div></Link>
            </li>
            <li className={selectedTab === 'saleReport' ? 'selectedNav' : ''}>
              <Link to="/saleReport"><div onClick={() => setSelectedTab('saleReport')}>Sale Report</div></Link>
            </li>
            <li className={selectedTab === 'setting' ? 'selectedNav' : ''}>
              <Link to="/setting"><div onClick={() => setSelectedTab('setting')}>Setting</div></Link>
            </li>
          </ul>
        </nav>
        <ToastContainer />
        <div className='content'>
          <Routes>
            <Route path="/" element={<Dashboard notify={notify} ipAddress={ipAddress} />} />
            <Route path="/items" element={<Items notify={notify} ipAddress={ipAddress} />} />
            <Route path="/pos" element={<Billing notify={notify} ipAddress={ipAddress} />} />
            <Route path="/bills" element={<Bills notify={notify} ipAddress={ipAddress} />} />
            <Route path="/generateBarcode" element={<BarcodeView notify={notify} ipAddress={ipAddress}
              barcodeGenerateFilePath={barcodeGenerateFilePath} />} />
            <Route path="/saleReport" element={<SaleReport notify={notify} ipAddress={ipAddress} />} />
            <Route path="/setting" element={<Setting notify={notify} ipAddress={ipAddress} />} />
            <Route path="/error" element={<ErrorView message={'Not able to connect to server'} />} />
            <Route component={ErrorPageNotFoundView} />
          </Routes>
        </div>
      </div>
    </Router>
  );
};

export default App;
