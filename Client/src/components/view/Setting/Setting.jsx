import React, { useRef,useEffect,useState } from 'react';
import { AgGridReact } from 'ag-grid-react'; // React Data Grid Component
import "ag-grid-community/styles/ag-grid.css"; // Mandatory CSS required by the grid
import "ag-grid-community/styles/ag-theme-quartz.css";
import Dropdown from '../../Dropdown/Dropdown.jsx';
import './Setting.css';
import { GenerateSaleReport } from '../../../APIEndpoints.js'


const Setting = (props) => {
  
  useEffect(() => {
    
  }, []);

  return (
    <div style={{color: 'white'}} >
       Setting Page
    </div>
  );
};

export default Setting;



