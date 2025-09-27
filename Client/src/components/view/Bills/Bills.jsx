import React, { useRef, useEffect, useState } from 'react';
import { AgGridReact } from 'ag-grid-react'; // React Data Grid Component
import "ag-grid-community/styles/ag-grid.css"; // Mandatory CSS required by the grid
import "ag-grid-community/styles/ag-theme-quartz.css";
import { GetAllBill } from '../../../APIEndpoints.js'
import { ReactToPrint } from 'react-to-print';
import Report from '../../Report/Report';
import { formatDate } from '../../../Helper/dateHelper.js'
import { DeleteBill } from '../../../APIEndpoints.js'

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
    { field: "id", filter: true },
    { field: "createdDateTime" },
    { field: "totalAmount" },
    { field: "quantity" },
    { field: "tax", editable: true, cellEditor: 'agSelectCellEditor', cellEditorParams: { values: ['5%', '12%', '18%'], } },
  ]);

  const [colDefs2, setColDefs2] = useState([
    { field: "id", filter: true },
    { field: "itemName" },
    { field: "amount" },
    { field: "quantity" },
    { field: "price", editable: true, cellEditor: 'agSelectCellEditor', cellEditorParams: { values: ['5%', '12%', '18%'], } },
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
    fetch('/config.json')
      .then(response => response.json())
      .then(data => {
        const url = data.backendUrl + 'api/';
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
    <div  >
      {!showDetail && <div className="ag-theme-quartz" style={{ height: 500 }} >
        <ReactToPrint
          trigger={() => {
            return <button>Print Bill</button>;
          }}
          content={() => componentRef.current}
          ref={reactToPrintRef}
        />
        <button onClick={OnDeleteButtonClicked}>Delete</button>
        <AgGridReact rowData={rowData} columnDefs={colDefs} rowSelection={'multiple'}
          ref={gridRef}
          onRowDoubleClicked={(e) => onDoubleClick(e)}
          onRowClicked={(e) => OnSelectionChanged(e)}
          pagination={pagination} paginationPageSize={paginationPageSize}
          paginationPageSizeSelector={paginationPageSizeSelector}
        />
        {/* print */}
        <div className='print-area'>
          <div ref={componentRef}>
            {bill && <Report billNumber={bill.id} customerName={bill.customerName} dateTime={formatDate(bill.createdDateTime)} billItems={bill.billItems}
              shopName={shopName} shopAddress={shopAddress} shopGSTNumber={shopGstNumber} />}
          </div>
        </div>
      </div>}
      {showDetail &&
        <div>
          <button onClick={() => setShowDetail(false)}>Back</button>
          <label htmlFor="">Barcode </label>
          <input type="text" enterKeyHint='hello' />
          <button>Submit</button>
          <button>Return</button>

          <div className="ag-theme-quartz" style={{ height: 500 }} >
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