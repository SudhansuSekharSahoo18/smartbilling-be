import React, { useRef, useEffect, useState } from 'react';
import { AgGridReact } from 'ag-grid-react'; // React Data Grid Component
import "ag-grid-community/styles/ag-grid.css"; // Mandatory CSS required by the grid
import "ag-grid-community/styles/ag-theme-quartz.css";
import { GetAllBill, DeleteBill, ReturnBillItem } from '../../../APIEndpoints.js'
import { ReactToPrint } from 'react-to-print';
import Report from '../../Report/Report';
import { formatDate } from '../../../Helper/dateHelper.js'
import { getBackendUrl, getConfig, postRequest, readError } from '../../../Helper/apiHelper.js';
import { formatRupees } from '../../../Helper/format.js';

const PAYMENT_LABELS = { cash: 'Cash', upi: 'UPI', creditcard: 'Credit card', debitcard: 'Debit card' };

const Bills = (props) => {
  const gridRef = useRef(null);
  const componentRef = useRef(null);
  const reactToPrintRef = useRef();
  // The bill whose items are shown (double-clicked), or null for the list.
  const [detailBill, setDetailBill] = useState(null);
  const [returnBarcode, setReturnBarcode] = useState('');
  const [bill, setBill] = useState(null);
  const [shopName, setShopName] = useState('Shop Name Not Found');
  const [shopAddress, setShopAddress] = useState('Address not found');
  const [shopGstNumber, setShopGstNumber] = useState('GST number not found');
  const [selectedId, setSelectedId] = useState(0);
  const [rowData, setRowData] = useState([]);

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
    { field: "barcode", headerName: "Barcode", flex: 1 },
    { field: "itemName", headerName: "Item", flex: 2 },
    { field: "mrp", headerName: "MRP", flex: 1, valueFormatter: p => formatRupees(p.value) },
    { field: "quantity", headerName: "Qty", flex: 0.7 },
    { field: "discountPercentage", headerName: "Discount %", flex: 1 },
    { field: "amount", headerName: "Amount", flex: 1, valueFormatter: p => formatRupees(p.value) },
  ]);

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
    if (!window.confirm(`Delete bill ${selectedId}? Its items go back into stock.`)) {
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
      props.notify('success', `Bill ${selectedId} deleted`)
      setSelectedId(0);
      setBill(null);
    }
    else
      props.notify('error', await readError(response))
  }

  const onDoubleClick = (event) => {
    setDetailBill(event.data);
    setReturnBarcode('');
  }

  const OnReturnButtonClicked = async () => {
    if (returnBarcode.trim() === '') {
      props.notify('warning', 'Scan or type the barcode of the item being returned');
      return;
    }
    try {
      const response = await postRequest(props.ipAddress + ReturnBillItem, { billId: detailBill.id, barcode: returnBarcode.trim() });
      if (!response.ok) {
        props.notify('error', await readError(response));
        return;
      }
      const updated = await response.json();
      setDetailBill(updated);
      setRowData(rows => rows.map(row => row.id === updated.id ? updated : row));
      if (bill?.id === updated.id) setBill(updated);
      setReturnBarcode('');
      props.notify('success', `1 item returned. Bill ${updated.id} is now ${formatRupees(updated.totalAmount) || '₹0'}`);
    } catch (error) {
      props.notify('error', `Could not return the item: ${error.message}`);
    }
  }

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
            setRowData([]);
            props.notify('error', `Could not load bills from the server: ${error.message}`);
          });
      })
      .catch(error => props.notify('error', `Could not reach the server: ${error.message}`));
  }, []);

  return (
    <div>
      {!detailBill && <div className='card'>
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
            <Report bill={bill} shopName={shopName} shopAddress={shopAddress} shopGSTNumber={shopGstNumber} />
          </div>
        </div>
      </div>}
      {detailBill &&
        <div className='card'>
          <div className='toolbar'>
            <button className='btn-secondary' onClick={() => setDetailBill(null)}>← Back to bills</button>
            <span className='muted'>Bill {detailBill.id} · {formatRupees(detailBill.totalAmount) || '₹0'}</span>
            <span className='toolbar-spacer' />
            <label htmlFor="returnBarcode" className='muted'>Return item</label>
            <input id="returnBarcode" type="text" placeholder="Scan barcode" autoComplete="off" value={returnBarcode}
              onChange={(e) => setReturnBarcode(e.target.value)}
              onKeyDown={(e) => { if (e.key === 'Enter') OnReturnButtonClicked(); }} />
            <button className='btn-secondary' onClick={OnReturnButtonClicked}>Return 1</button>
          </div>

          <div className="ag-theme-quartz grid-card">
            <AgGridReact rowData={detailBill.billItems} columnDefs={colDefs2} rowSelection={'multiple'}
              pagination={pagination} paginationPageSize={paginationPageSize}
              paginationPageSizeSelector={paginationPageSizeSelector}
            />
          </div>

        </div>}
    </div>
  );
};

export default Bills;
