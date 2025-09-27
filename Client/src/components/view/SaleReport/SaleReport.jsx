import React, { useRef, useEffect, useState } from 'react';
import { AgGridReact } from 'ag-grid-react'; // React Data Grid Component
import "ag-grid-community/styles/ag-grid.css"; // Mandatory CSS required by the grid
import "ag-grid-community/styles/ag-theme-quartz.css";
import Dropdown from '../../Dropdown/Dropdown';
import './SaleReport.css';
import { GenerateSaleReport } from '../../../APIEndpoints.js'


const SaleReport = (props) => {
  const [items, setItems] = useState([]);
  const [rowData, setRowData] = useState([
    { make: "Tesla", model: "Model Y", price: 64950, electric: true },
    { make: "Ford", model: "F-Series", price: 33850, electric: false },
    { make: "Toyota", model: "Corolla", price: 29600, electric: false },
  ]);

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

  const yearOptions = [
    { value: 2020, label: '2020' },
    { value: 2021, label: '2021' },
    { value: 2022, label: '2022' },
    { value: 2023, label: '2023' },
    { value: 2024, label: '2024' },
    { value: 2025, label: '2025' },
    { value: 2026, label: '2026' },
  ];

  const [selectedMonth, setSelectedMonth] = useState(monthOptions[0].value);
  const [selectedYear, setSelectedYear] = useState(yearOptions[0].value);

  const pagination = true;
  const paginationPageSize = 500;
  const paginationPageSizeSelector = [200, 500, 1000];

  const [colDefs, setColDefs] = useState([
    // { field: "button", cellRenderer: EditButton },
    // { field: "button", cellRenderer: DeleteButton },
    { field: "barcode", filter: true },
    { field: "title" },
    { field: "quantity" },
    // { field: "Unit" },
    { field: "sellPrice" },
    // { field: "tax", editable: true, cellEditor: 'agSelectCellEditor', cellEditorParams: { values: ['5%', '12%', '18%'], } },
  ]);


  const OnDownloadClicked = async () => {
    const filename = 'SaleReport_' + selectedMonth + '_' + selectedYear + '.csv'
    try {
      const response = await fetch(props.ipAddress + GenerateSaleReport + '?month=' + selectedMonth + '&year=' + selectedYear, {
        method: 'GET',
        headers: {
          'Content-Type': 'application/octet-stream',
        },
      })

      if (!response.ok) {
        throw new Error('Network response was not ok');
      }
      const blob = await response.blob();
      if (!blob || !(blob instanceof Blob)) {
        throw new Error('The fetched data is not a valid Blob');
      }
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
    }
  }

  const handleSelectMonth = (option) => {
    setSelectedMonth(option);
  };

  const handleSelectYear = (option) => {
    setSelectedYear(option);
  };

  useEffect(() => {
    const currentMonth = new Date().getMonth() + 1;
    // const currentYear = new Date().getFullYear();
    setSelectedMonth(monthOptions[new Date().getMonth()].value);
    
    // fetch('/config.json')
    // .then(response => response.json())
    // .then(data => {
    //     const url = data.backendUrl+'api/';
    //     fetch(url+'product')
    //     .then(response => {
    //     if (!response.ok) {
    //         throw new Error('Network response was not ok');
    //     }
    //     return response.json();
    //     })
    //     .then(data => {
    //         console.log(data)
    //         setItems(data);
    //     })
    //     .catch(error => {
    //     const defaultItems = [
    //         {id: 1, barcode: "101", itemName: 'Saree', price: 500},
    //         {id: 2, barcode: "102", itemName: 'Jeans', price: 1500},
    //         {id: 3, barcode: "103", itemName: 'Shirt', price: 400},
    //         {id: 4, barcode: "104", itemName: 'Socks', price: 150},
    //         {id: 5, barcode: "105", itemName: 'Lungi', price: 80},
    //         ];
    //     setItems(defaultItems)
    //     });
    // })
    // .catch(error => console.error('Error fetching config:', error));
  }, []);

  return (
    <div className="ag-theme-quartz" style={{ height: 500 }} >
      <Dropdown label={'Select Month'} options={monthOptions} onSelect={handleSelectMonth} />
      <Dropdown label={'Select Year'} options={yearOptions} onSelect={handleSelectYear} />
      <button onClick={() => OnDownloadClicked()}>Download</button>
      {/* <AgGridReact rowData={items} columnDefs={colDefs} rowSelection={'multiple'}
        pagination={pagination} paginationPageSize={paginationPageSize}
        paginationPageSizeSelector={paginationPageSizeSelector}
      /> */}
    </div>
  );
};

export default SaleReport;



