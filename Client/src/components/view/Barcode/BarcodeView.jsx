import React, { useRef, useEffect, useState } from 'react';
import { AgGridReact } from 'ag-grid-react';
import "ag-grid-community/styles/ag-grid.css";
import "ag-grid-community/styles/ag-theme-quartz.css";
import { ReactToPrint } from 'react-to-print';
import BarcodeReport from '../../Report/BarcodeReport';
import { DeleteBarcode, GetBarcodeList } from '../../../APIEndpoints.js'
import FolderChooser from '../../FolderChooser/FolderChooser.jsx';
import { GenerateBarcode } from '../../../APIEndpoints.js'


const BarcodeView = ({ notify, ipAddress, barcodeGenerateFilePath }) => {
  const gridRef = useRef(null);
  const [data, setData] = useState(null);
  const [error, setError] = useState(null);
  const componentRef = useRef(null);
  const reactToPrintRef = useRef();
  const [shopName, setShopName] = useState('Nandini Fashion')
  const [ip, setIp] = useState(null)
  const [barcodeFilePath, setBarcodeFilePath] = useState(null)
  const [selectedId, setSelectedId] = useState(0)

  const [rowData, setRowData] = useState([
    { itemCode: "101", itemName: "Jeans", price: 1299, quantity: 10 },
    { itemCode: "102", itemName: "Shirt", price: 800, quantity: 10 },
    // { barcode: "103", itemName: "Jacket", price: 3000, quantity: 10 },
  ]);

  const [colDefs, setColDefs] = useState([
    { field: "itemCode", editable: true, filter: true },
    { field: "itemName", editable: true },
    { field: "price", editable: true },
    { field: "quantity", editable: true },
  ]);

  const OnDeleteButtonClicked = async () => {
    if (selectedId === 0) {
      alert('Please select a record to delete')
      return;
    }
    const deletedBarcode = rowData.filter((item) => item.id === selectedId);
    if (deletedBarcode[0].id !== undefined) {
      try {
        const url = ip + DeleteBarcode + deletedBarcode[0].id ;
        const response = await fetch(url, {
          method: 'DELETE',
          headers: {
            'Content-Type': 'application/json'
          },
        });
        
        if (response.ok) {
          notify('Barcode removed')
          const newData = rowData.filter((item) => item.id !== selectedId);
          setRowData(newData)
          setSelectedId(0);
        }
        else {
          throw new Error('Network response was not ok');
        }
      } catch (error) {
        notify('error', error.message);
      }
    }
  }


  const onSelectionChanged = () => {
    const selectedNodes = gridRef.current.api.getSelectedNodes();
    if (selectedNodes.length > 0) {
      const selectedData = selectedNodes.map(node => node.data);
      const temp = selectedData[0];
      setSelectedId(temp['id']);
    }
  };

  const OnGenerateBarcodeClicked = async () => {
    const url = ip + GenerateBarcode;

    try {
      const response = await fetch(url, {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json'
        },
        body: JSON.stringify({ 'filePath': barcodeFilePath, 'barcodeList': rowData })
      });

      if (!response.ok) {
        throw new Error('Network response was not ok');
      }

      const responseData = await response.json();
      onClearItemButtonClicked();
      notify('Barcode file created')
    } catch (error) {
      setError(error.message);
    }
  }

  const onAddItemButtonClicked = () => {
    const item = { itemCode: "", itemName: "", price: 0, quantity: 0 }
    setRowData([...rowData, item]);
    // console.log(rowData)
  };

  const onClearItemButtonClicked = () => {
    setRowData([]);
  };


  const handlePrint = () => {
    if (reactToPrintRef.current) {
      reactToPrintRef.current.handlePrint();
    }
  };

  useEffect(() => {
    fetch('/config.json')
      .then(response => response.json())
      .then(data => {
        const url = data.backendUrl + 'api/';
        setIp(url)
        setBarcodeFilePath(data.BarcodeGenerateFilePath)
        fetch(url + GetBarcodeList)
          .then(response => {
            if (!response.ok) {
              throw new Error('Network response was not ok');
            }
            return response.json();
          })
          .then(data => {
            setRowData(data);
          })
          .catch(error => {
            throw error;
          });
      })
      .catch(error => console.error('Error fetching config:', error));
  }, []);

  return (
    <div className="ag-theme-quartz" style={{ height: 500 }} >
      {/* <FolderChooser /> */}
      <button onClick={() => OnGenerateBarcodeClicked()}>Export</button>
      <button onClick={() => onAddItemButtonClicked()}>Add Item</button>
      <button onClick={() => onClearItemButtonClicked()}>Clear</button>
      <button onClick={() => OnDeleteButtonClicked()}>Delete</button>
      {/* <ReactToPrint
        trigger={() => {
          return <button>Print Barcode</button>;
        }}
        content={() => componentRef.current}
        ref={reactToPrintRef}
      /> */}
      <AgGridReact rowData={rowData} columnDefs={colDefs}
        ref={gridRef}
        rowSelection={'single'}
        onSelectionChanged={() => onSelectionChanged()}
      />
      <div ref={componentRef}>
        <BarcodeReport barcodeList={rowData} shopName={shopName} />
      </div>
    </div>
  );
};

export default BarcodeView;