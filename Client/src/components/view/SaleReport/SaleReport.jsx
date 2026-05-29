import React, { useEffect, useState } from 'react';
import Dropdown from '../../Dropdown/Dropdown';
import './SaleReport.css';
import { getBackendUrl } from '../../../Helper/apiHelper.js';
import { GenerateSaleReport, GetTaxSummaryReport } from '../../../APIEndpoints.js'


const SaleReport = (props) => {
  const monthOptions = [
    { value: 1, label: 'Jan' },
    { value: 2, label: 'Feb' },
    { value: 3, label: 'Mar' },
    { value: 4, label: 'April' },
    { value: 5, label: 'May' },
    { value: 6, label: 'June' },
    { value: 7, label: 'July' },
    { value: 8, label: 'Aug' },
    { value: 9, label: 'Sept' },
    { value: 10, label: 'Oct' },
    { value: 11, label: 'Nov' },
    { value: 12, label: 'Dec' },
  ];

  const currentYear = new Date().getFullYear();
  const yearOptions = Array.from({ length: 7 }, (_, i) => {
    const y = currentYear - 6 + i;
    return { value: y, label: String(y) };
  });

  const [selectedMonth, setSelectedMonth] = useState(new Date().getMonth() + 1);
  const [selectedYear, setSelectedYear] = useState(currentYear);

  const handleSelectMonth = (option) => setSelectedMonth(option);
  const handleSelectYear = (option) => setSelectedYear(option);

  const downloadFile = async (url, filename) => {
    try {
      const response = await fetch(url, { method: 'GET' });
      if (!response.ok) throw new Error(`Server error: ${response.status}`);
      const blob = await response.blob();
      const urlBlob = window.URL.createObjectURL(blob);
      const a = document.createElement('a');
      a.href = urlBlob;
      a.download = filename;
      document.body.appendChild(a);
      a.click();
      a.remove();
      window.URL.revokeObjectURL(urlBlob);
    } catch (error) {
      console.error('Error downloading file:', error);
      props.notify?.('error', 'Download failed: ' + error.message);
    }
  };

  const OnDownloadSaleReportClicked = async () => {
    const url = await getBackendUrl();
    const filename = `SaleReport_${selectedMonth}_${selectedYear}.csv`;
    await downloadFile(`${url}${GenerateSaleReport}?month=${selectedMonth}&year=${selectedYear}`, filename);
  };

  const OnDownloadTaxSummaryClicked = async () => {
    const url = await getBackendUrl();
    const filename = `TaxSummary_${selectedMonth}_${selectedYear}.xlsx`;
    await downloadFile(`${url}${GetTaxSummaryReport}?month=${selectedMonth}&year=${selectedYear}`, filename);
  };

  return (
    <div className='saleReport'>
      <div style={{ display: 'flex', alignItems: 'center', gap: '1rem', flexWrap: 'wrap' }}>
        <Dropdown label={'Select Month'} options={monthOptions} onSelect={handleSelectMonth} value={selectedMonth} />
        <Dropdown label={'Select Year'} options={yearOptions} onSelect={handleSelectYear} value={selectedYear} />
      </div>
      <div style={{ marginTop: '1rem', display: 'flex', gap: '0.75rem' }}>
        <button onClick={OnDownloadSaleReportClicked}>Download Sale Report (CSV)</button>
        <button onClick={OnDownloadTaxSummaryClicked}>Download Tax Summary (Excel)</button>
      </div>
    </div>
  );
};

export default SaleReport;



