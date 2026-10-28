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
  LineChart, Line, XAxis, YAxis, CartesianGrid, Tooltip, ResponsiveContainer
} from 'recharts';
import { formatRupees } from '../../../Helper/format.js';

// Matches the --sb-* tokens in index.css (recharts needs literal colours).
const CHART = {
  daily: '#2563eb',
  monthly: '#f97316',
  grid: '#e2e8f0',
  axis: '#64748b',
  cursor: '#cbd5e1',
  tooltip: { backgroundColor: '#fff', border: '1px solid #e2e8f0', borderRadius: 8, boxShadow: '0 4px 12px rgba(15,23,42,0.08)', color: '#0f172a' },
};

// Axis ticks: ₹950, ₹1.2k, ₹3.4L
const shortRupees = (value) => {
  if (value >= 100000) return `₹${(value / 100000).toFixed(1).replace(/\.0$/, '')}L`;
  if (value >= 1000) return `₹${(value / 1000).toFixed(1).replace(/\.0$/, '')}k`;
  return `₹${value}`;
};


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

  const fyStartYear = now.getMonth() >= 3 ? now.getFullYear() : now.getFullYear() - 1;
  const fyLabel = `FY ${fyStartYear}-${String(fyStartYear + 1).slice(-2)}`;
  const monthTotal = dailySales.reduce((sum, d) => sum + (d.totalSale ?? 0), 0);
  const fyTotal = monthlySales.reduce((sum, m) => sum + (m.totalSale ?? 0), 0);
  const selectedMonthLabel = new Date(`${selectedMonth}-01T00:00:00`).toLocaleString('en-IN', { month: 'long', year: 'numeric' });

  return (
    <div className='dashboard'>
      <div className='stat-grid'>
        <div className='card stat-card'>
          <p className='card-subtitle'>Today's sales</p>
          <div className='stat-value'>{formatRupees(totalSale) || '₹0'}</div>
        </div>
        <div className='card stat-card'>
          <p className='card-subtitle'>{selectedMonthLabel}</p>
          <div className='stat-value'>{isDailyLoading || dailyError ? '–' : formatRupees(monthTotal) || '₹0'}</div>
        </div>
        <div className='card stat-card'>
          <p className='card-subtitle'>{fyLabel}</p>
          <div className='stat-value'>{formatRupees(fyTotal) || '₹0'}</div>
        </div>
      </div>

      <div className='card'>
        <div className='card-header'>
          <h2 className='card-title'>Daily sales</h2>
          <input
            type="month"
            aria-label="Month"
            value={selectedMonth}
            onChange={e => setSelectedMonth(e.target.value)}
          />
        </div>
        {isDailyLoading && <div className='empty-state'>Loading…</div>}
        {dailyError && <div className='empty-state chart-error'>Failed to load data: {dailyError}. Make sure the server is running.</div>}
        {!isDailyLoading && !dailyError && (
          <ResponsiveContainer width="100%" height={280}>
            <LineChart data={dailySales} margin={{ top: 8, right: 16, left: 8, bottom: 0 }}>
              <CartesianGrid strokeDasharray="3 3" stroke={CHART.grid} vertical={false} />
              <XAxis dataKey="day" tick={{ fill: CHART.axis, fontSize: 12 }} axisLine={{ stroke: CHART.grid }} tickLine={false} />
              <YAxis tick={{ fill: CHART.axis, fontSize: 12 }} axisLine={false} tickLine={false} tickFormatter={shortRupees} width={56} />
              <Tooltip
                cursor={{ stroke: CHART.cursor, strokeWidth: 1 }}
                contentStyle={CHART.tooltip}
                formatter={(value) => [formatRupees(value), 'Sales']}
                labelFormatter={(label) => `Day ${label}`}
              />
              <Line type="monotone" dataKey="totalSale" stroke={CHART.daily} strokeWidth={2.5}
                dot={{ r: 3, fill: CHART.daily, strokeWidth: 0 }} activeDot={{ r: 5 }} />
            </LineChart>
          </ResponsiveContainer>
        )}
      </div>

      <div className='card'>
        <div className='card-header'>
          <h2 className='card-title'>Monthly sales ({fyLabel})</h2>
        </div>
        <ResponsiveContainer width="100%" height={280}>
          <LineChart data={monthlySales} margin={{ top: 8, right: 16, left: 8, bottom: 0 }}>
            <CartesianGrid strokeDasharray="3 3" stroke={CHART.grid} vertical={false} />
            <XAxis dataKey="month" tick={{ fill: CHART.axis, fontSize: 12 }} axisLine={{ stroke: CHART.grid }} tickLine={false} />
            <YAxis tick={{ fill: CHART.axis, fontSize: 12 }} axisLine={false} tickLine={false} tickFormatter={shortRupees} width={56} />
            <Tooltip
              cursor={{ stroke: CHART.cursor, strokeWidth: 1 }}
              contentStyle={CHART.tooltip}
              formatter={(value) => [formatRupees(value), 'Sales']}
            />
            <Line type="monotone" dataKey="totalSale" stroke={CHART.monthly} strokeWidth={2.5}
              dot={{ r: 4, fill: CHART.monthly, strokeWidth: 0 }} activeDot={{ r: 6 }} />
          </LineChart>
        </ResponsiveContainer>
      </div>
    </div>
  );
};

export default Dashboard;



