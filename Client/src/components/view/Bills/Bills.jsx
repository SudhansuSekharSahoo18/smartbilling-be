import React, { useRef, useEffect, useState } from 'react';
import { AgGridReact } from 'ag-grid-react'; // React Data Grid Component
import "ag-grid-community/styles/ag-grid.css"; // Mandatory CSS required by the grid
import "ag-grid-community/styles/ag-theme-quartz.css";
import { GetAllBill } from '../../../APIEndpoints.js'
import { ReactToPrint } from 'react-to-print';
import Report from '../../Report/Report';
import { formatDate } from '../../../Helper/dateHelper.js'
import { getBackendUrl, getConfig } from '../../../Helper/apiHelper.js';
import { DeleteBill } from '../../../APIEndpoints.js'
import { formatRupees } from '../../../Helper/format.js';

const PAYMENT_LABELS = { cash: 'Cash', upi: 'UPI', creditcard: 'Credit card', debitcard: 'Debit card' };

const Bills = (props) => {
  const gridRef = useRef(null);
  const componentRef = useRef(null);
  const reactToPrintRef = useRef();
  const shallPrintBill = useRef(false);
  const [showDetail, setShowDetail] = useState(false);
  const [billItems, setBillItems] = useState([]);
  const [bill, setBill] = useState(null);
  const [billNumber, setBillNumber] = useState();
  const [shopName, setShopName] = useState('Shop Name Not Found');
  const [shopAddress, setShopAddress] = useState('Address not found');
  const [shopGstNumber, setShopGstNumber] = useState('GST number not found');
  const [selectedId, setSelectedId] = useState(0);
  const [rowData, setRowData] = useState([
    { make: "Tesla", model: "Model Y", price: 64950, electric: true },
    { make: "Ford", model: "F-Series", price: 33850, electric: false },
    { make: "Toyota", model: "Corolla", price: 29600, electric: false },
  ]);

  const pagination = true;
  const paginationPageSize = 20;
  const paginationPageSizeSelector = [20, 50, 100];

  const [colDefs, setColDefs] = useState([
    { field: "id", headerName: "Bill no.", flex: 0.7, filter: true },
    { field: "createdDateTime", headerName: "Date", flex: 1.4, valueFormatter: p => p.value ? formatDate(p.value) : '' },
    { field: "customerName", headerName: "Customer", flex: 1.4, filter: true },
    { field: "modeOfPayment", headerName: "Payment", flex: 1, valueFormatter: p => PAYMENT_LABELS[p.value] ?? p.value ?? '' },
    { headerName: "Items", flex: 0.7, valueGetter: p => (p.data?.billItems ?? []).reduce((sum, item) => sum + item.quantity, 0) },
    { field: "totalAmount", headerName: "Total", flex: 1, valueFormatter: p => formatRupees(p.value) },
  ]);

  const [colDefs2, setColDefs2] = useState([
    { field: "itemName", headerName: "Item", flex: 2 },
    { field: "mrp", headerName: "MRP", flex: 1, valueFormatter: p => formatRupees(p.value) },
    { field: "quantity", headerName: "Qty", flex: 0.7 },
    { field: "discountPercentage", headerName: "Discount %", flex: 1 },
    { field: "amount", headerName: "Amount", flex: 1, valueFormatter: p => formatRupees(p.value) },
  ]);


  const EditButton = (props) => {
    return <button onClick={() => window.alert('clicked')}>Edit</button>;
  };

  const DeleteButton = (props) => {
    return <button onClick={() => window.alert('clicked')}>Delete</button>;
  };

  const OnSelectionChanged = (e) => {
    setBill(e.data);

    const selectedNodes = gridRef.current.api.getSelectedNodes();
    if (selectedNodes.length > 0) {
      const selectedData = selectedNodes.map(node => node.data);
      const temp = selectedData[0];
      setSelectedId(temp['id']);
    }
  }

  const OnDeleteButtonClicked = async () => {
    if (selectedId === 0) {
      alert('Please select a record to delete')
      return;
    }

    const response = await fetch(props.ipAddress + DeleteBill + selectedId, {
      method: 'DELETE',
      headers: {
        'Content-Type': 'application/json'
      },
    });

    if (response.ok) {
      const newData = rowData.filter((item) => item.id !== selectedId);
      setRowData(newData)
      setSelectedId(0);
      props.notify('success', 'Item Deleted')
    }
    else
      props.notify('error', response.status)
  }

  const onDoubleClick = (event) => {
    setBillItems(event.data.billItems);
    setShowDetail(true)
  }

  const handlePrint = () => {
    if (reactToPrintRef.current) {
      reactToPrintRef.current.handlePrint();
    }
  };
  useEffect(() => {
    if (shallPrintBill.current) {
      handlePrint();
    }
  }, [billNumber]);

  useEffect(() => {
    Promise.all([getBackendUrl(), getConfig()])
      .then(([url, data]) => {
        setShopName(data.shopName)
        setShopAddress(data.shopAddress)
        setShopGstNumber(data.shopGSTNumber)
        fetch(url + GetAllBill)
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
            const defaultItems = [
              { id: 1, barcode: "101", itemName: 'Saree', price: 500 },
              { id: 2, barcode: "102", itemName: 'Jeans', price: 1500 },
              { id: 3, barcode: "103", itemName: 'Shirt', price: 400 },
              { id: 4, barcode: "104", itemName: 'Socks', price: 150 },
              { id: 5, barcode: "105", itemName: 'Lungi', price: 80 },
            ];
            setRowData(defaultItems)
          });
      })
      .catch(error => console.error('Error fetching config:', error));
  }, []);

  return (
    <div>
      {!showDetail && <div className='card'>
        <div className='toolbar'>
          <ReactToPrint
            trigger={() => {
              return <button disabled={!bill}>Print bill</button>;
            }}
            content={() => componentRef.current}
            ref={reactToPrintRef}
          />
          <button className='btn-danger' onClick={OnDeleteButtonClicked}>Delete</button>
          <span className='toolbar-spacer' />
          <span className='muted'>{bill ? `Bill ${bill.id} selected` : 'Select a bill to print or delete it'}</span>
        </div>
        <div className="ag-theme-quartz grid-card">
          <AgGridReact rowData={rowData} columnDefs={colDefs} rowSelection={'multiple'}
            ref={gridRef}
            onRowDoubleClicked={(e) => onDoubleClick(e)}
            onRowClicked={(e) => OnSelectionChanged(e)}
            pagination={pagination} paginationPageSize={paginationPageSize}
            paginationPageSizeSelector={paginationPageSizeSelector}
          />
        </div>
        {/* print */}
        <div className='print-area'>
          <div ref={componentRef}>
            {bill && <Report billNumber={bill.id} customerName={bill.customerName} dateTime={formatDate(bill.createdDateTime)} billItems={bill.billItems}
              shopName={shopName} shopAddress={shopAddress} shopGSTNumber={shopGstNumber} />}
          </div>
        </div>
      </div>}
      {showDetail &&
        <div className='card'>
          <div className='toolbar'>
            <button className='btn-secondary' onClick={() => setShowDetail(false)}>← Back to bills</button>
            <span className='toolbar-spacer' />
            <label htmlFor="returnBarcode" className='muted'>Barcode</label>
            <input id="returnBarcode" type="text" />
            <button>Submit</button>
            <button className='btn-secondary'>Return</button>
          </div>

          <div className="ag-theme-quartz grid-card">
            <AgGridReact rowData={billItems} columnDefs={colDefs2} rowSelection={'multiple'}
              pagination={pagination} paginationPageSize={paginationPageSize}
              paginationPageSizeSelector={paginationPageSizeSelector}
            />
          </div>

        </div>}
    </div>
  );
};

export default Bills;