import React, { useRef, useEffect, useState } from 'react';
import { AgGridReact } from 'ag-grid-react'; // React Data Grid Component
import "ag-grid-community/styles/ag-grid.css"; // Mandatory CSS required by the grid
import "ag-grid-community/styles/ag-theme-quartz.css";
import CustomInput from '../../CustomInput/CustomInput.jsx';
import './Dashboard.css';
import Dropdown from '../../Dropdown/Dropdown.jsx';
import { postRequest, patchRequest, getBackendUrl } from '../../../Helper/apiHelper.js';
import CustomCheckBox from '../../CustomCheckBox/CustomCheckBox.jsx';
import { GetTotalSaleByDate, GetMonthlySales, GetDailySales } from '../../../APIEndpoints.js'
import {
  BarChart, Bar, XAxis, YAxis, CartesianGrid, Tooltip, ResponsiveContainer
} from 'recharts';


const Dashboard = (props) => {
  const now = new Date();
  const toMonthValue = (d) => `${d.getFullYear()}-${String(d.getMonth() + 1).padStart(2, '0')}`;

  const [totalSale, setTotalSale] = useState(0);
  const [monthlySales, setMonthlySales] = useState([]);
  const [selectedMonth, setSelectedMonth] = useState(toMonthValue(now));
  const [dailySales, setDailySales] = useState([]);
  const [isDailyLoading, setIsDailyLoading] = useState(false);
  const [dailyError, setDailyError] = useState(null);


  useEffect(() => {
    getBackendUrl()
      .then(url => {
        fetch(url + GetTotalSaleByDate)
          .then(response => {
            if (!response.ok) throw new Error('Network response was not ok');
            return response.json();
          })
          .then(data => setTotalSale(data))
          .catch(error => console.log('error - > ' + error));

        fetch(url + GetMonthlySales)
          .then(response => {
            if (!response.ok) throw new Error('Network response was not ok');
            return response.json();
          })
          .then(data => setMonthlySales(data))
          .catch(error => console.log('error - > ' + error));
      })
      .catch(error => console.error('Error fetching config:', error));
  }, []);

  useEffect(() => {
    const [year, month] = selectedMonth.split('-');
    setIsDailyLoading(true);
    setDailyError(null);
    getBackendUrl()
      .then(url => fetch(`${url}${GetDailySales}?month=${parseInt(month)}&year=${parseInt(year)}`))
      .then(response => {
        if (!response.ok) throw new Error(`Server error: ${response.status}`);
        return response.json();
      })
      .then(data => {
        setDailySales(data);
        setIsDailyLoading(false);
      })
      .catch(error => {
        console.log('Daily sales fetch error:', error);
        setDailyError(error.message);
        setIsDailyLoading(false);
      });
  }, [selectedMonth]);

  return (
    <div className='dashboard'>
        <div>Today's Total Sales: {totalSale}</div>
        <div>Payments </div>
        <div>UPI:</div>
        <div>Cash: </div>
        <div>Card Payment: </div>

        <div style={{ marginTop: '2rem' }}>
          <div style={{ display: 'flex', alignItems: 'center', gap: '1rem', marginBottom: '0.5rem' }}>
            <h3 style={{ margin: 0 }}>Daily Sales</h3>
            <input
              type="month"
              value={selectedMonth}
              onChange={e => setSelectedMonth(e.target.value)}
              style={{ padding: '4px 8px', borderRadius: '4px', border: '1px solid #555', backgroundColor: '#333', color: '#fff' }}
            />
          </div>
          {isDailyLoading && <div style={{ color: '#ccc', padding: '1rem' }}>Loading...</div>}
          {dailyError && <div style={{ color: '#f88', padding: '1rem' }}>Failed to load data: {dailyError}. Make sure the server is running and restarted.</div>}
          {!isDailyLoading && !dailyError && (
          <ResponsiveContainer width="100%" height={300}>
            <BarChart data={dailySales} margin={{ top: 10, right: 30, left: 10, bottom: 0 }}>
              <CartesianGrid strokeDasharray="3 3" stroke="#555" />
              <XAxis dataKey="day" tick={{ fill: '#ccc' }} axisLine={{ stroke: '#777' }} />
              <YAxis tick={{ fill: '#ccc' }} axisLine={{ stroke: '#777' }} />
              <Tooltip
                contentStyle={{ backgroundColor: '#333', border: '1px solid #555', color: '#fff' }}
                formatter={(value) => [`₹${value.toFixed(2)}`, 'Total Sale']}
                labelFormatter={(label) => `Day ${label}`}
              />
              <Bar dataKey="totalSale" fill="#f7884f" radius={[4, 4, 0, 0]} />
            </BarChart>
          </ResponsiveContainer>
          )}
        </div>

        <div style={{ marginTop: '2rem' }}>
          <h3>Monthly Sales (FY {new Date().getMonth() >= 3 ? new Date().getFullYear() : new Date().getFullYear() - 1}-{String(new Date().getMonth() >= 3 ? new Date().getFullYear() + 1 : new Date().getFullYear()).slice(-2)})</h3>
          <ResponsiveContainer width="100%" height={300}>
            <BarChart data={monthlySales} margin={{ top: 10, right: 30, left: 10, bottom: 0 }}>
              <CartesianGrid strokeDasharray="3 3" stroke="#555" />
              <XAxis dataKey="month" tick={{ fill: '#ccc' }} axisLine={{ stroke: '#777' }} />
              <YAxis tick={{ fill: '#ccc' }} axisLine={{ stroke: '#777' }} />
              <Tooltip
                contentStyle={{ backgroundColor: '#333', border: '1px solid #555', color: '#fff' }}
                formatter={(value) => [`₹${value.toFixed(2)}`, 'Total Sale']}
              />
              <Bar dataKey="totalSale" fill="#4f8ef7" radius={[4, 4, 0, 0]} />
            </BarChart>
          </ResponsiveContainer>
        </div>
    </div>
  );
};

export default Dashboard;



