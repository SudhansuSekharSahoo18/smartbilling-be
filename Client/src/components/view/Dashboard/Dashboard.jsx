import React, { useRef, useEffect, useState } from 'react';
import { AgGridReact } from 'ag-grid-react'; // React Data Grid Component
import "ag-grid-community/styles/ag-grid.css"; // Mandatory CSS required by the grid
import "ag-grid-community/styles/ag-theme-quartz.css";
import CustomInput from '../../CustomInput/CustomInput.jsx';
import './Dashboard.css';
import Dropdown from '../../Dropdown/Dropdown.jsx';
import { postRequest, patchRequest, getBackendUrl } from '../../../Helper/apiHelper.js';
import CustomCheckBox from '../../CustomCheckBox/CustomCheckBox.jsx';
import { GetTotalSaleByDate } from '../../../APIEndpoints.js'


const Dashboard = (props) => {
  const [totalSale, setTotalSale] = useState(0);


  useEffect(() => {
    getBackendUrl()
      .then(url => fetch(url + GetTotalSaleByDate))
      .then(response => {
        if (!response.ok) {
          throw new Error('Network response was not ok');
        }
        return response.json();
      })
      .then(data => {
        setTotalSale(data);
      })
      .catch(error => {
        console.log('error - > ' + error);
      });
  }, []);

  return (
    <div className='dashboard'>
        <div>Today's Total Sales: {totalSale}</div>
        <div>Payments </div>
        <div>UPI:</div>
        <div>Cash: </div>
        <div>Card Payment: </div>
    </div>
  );
};

export default Dashboard;



