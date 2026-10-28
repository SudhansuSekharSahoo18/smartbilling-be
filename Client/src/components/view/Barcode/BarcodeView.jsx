import React, { useRef, useEffect, useState } from 'react';
import { AgGridReact } from 'ag-grid-react';
import "ag-grid-community/styles/ag-grid.css";
import "ag-grid-community/styles/ag-theme-quartz.css";
import { ReactToPrint } from 'react-to-print';
import BarcodeReport from '../../Report/BarcodeReport';
import { DeleteBarcode, GetBarcodeList } from '../../../APIEndpoints.js'
import FolderChooser from '../../FolderChooser/FolderChooser.jsx';
import { GenerateBarcode } from '../../../APIEndpoints.js'
import { getBackendUrl, getConfig } from '../../../Helper/apiHelper.js';
import { isDesktop, sendToDesktop } from '../../../Helper/desktopBridge.js';
import { DEFAULT_DESIGN, buildPrintJob, loadLabelDesign, pageWidthMm } from '../../../Helper/labelDesign.js';
import { formatRupees } from '../../../Helper/format.js';
import './BarcodeView.css';


const BarcodeView = ({ notify, ipAddress }) => {
  const gridRef = useRef(null);
  const [data, setData] = useState(null);
  const [error, setError] = useState(null);
  const componentRef = useRef(null);
  const reactToPrintRef = useRef();
  const [shopName, setShopName] = useState('')
  const [labelDesign, setLabelDesign] = useState(DEFAULT_DESIGN)
  const [ip, setIp] = useState(null)
  const [selectedId, setSelectedId] = useState(0)

  const [rowData, setRowData] = useState([]);

  const [colDefs, setColDefs] = useState([
    { field: "itemCode", headerName: "Item code", flex: 1, editable: true, filter: true },
    { field: "itemName", headerName: "Item name", flex: 2, editable: true },
    { field: "price", flex: 1, editable: true, valueFormatter: p => formatRupees(p.value) },
    { field: "quantity", headerName: "Labels", flex: 1, editable: true },
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
        body: JSON.stringify({ 'barcodeList': rowData })
      });

      if (!response.ok) {
        throw new Error('Network response was not ok');
      }

      const responseData = await response.json();
      onClearItemButtonClicked();
      notify('success', `Barcode file created: ${responseData.filePath}`)
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

  const onPrintLabelsClicked = async () => {
    if (rowData.length === 0) {
      notify('warning', 'Add items to print labels');
      return;
    }
    if (!isDesktop()) {
      handlePrint();
      return;
    }
    try {
      const result = await sendToDesktop({
        type: 'printLabels',
        job: buildPrintJob(labelDesign, rowData, shopName),
      });
      if (result.ok) notify('success', result.message);
      else notify('error', result.message);
    } catch (error) {
      notify('error', error.message);
    }
  };

  const onChooseLabelPrinterClicked = async () => {
    try {
      const result = await sendToDesktop({ type: 'chooseLabelPrinter' });
      if (result.ok) notify('success', `Label printer: ${result.printer}`);
    } catch (error) {
      notify('error', error.message);
    }
  };

  useEffect(() => {
    Promise.all([getBackendUrl(), getConfig()])
      .then(([url, data]) => {
        setIp(url);
        setShopName(data.shopName ?? '');
        loadLabelDesign(url)
          .then(setLabelDesign)
          .catch(error => notify('error', `${error.message}. Using the default label design.`));
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
          .catch(error => notify('error', `Could not load the label list: ${error.message}`));
      })
      .catch(error => notify('error', `Could not reach the server: ${error.message}`));
  }, []);

  return (
    <div>
      <div className='card'>
        <div className='toolbar'>
          <button onClick={() => onPrintLabelsClicked()}>Print labels</button>
          {isDesktop() && <button className='btn-secondary' onClick={() => onChooseLabelPrinterClicked()}>Label printer…</button>}
          <span className='toolbar-divider' />
          <button className='btn-secondary' onClick={() => onAddItemButtonClicked()}>Add row</button>
          <button className='btn-danger' onClick={() => OnDeleteButtonClicked()}>Delete</button>
          <button className='btn-secondary' onClick={() => onClearItemButtonClicked()}>Clear all</button>
          <span className='toolbar-spacer' />
          <button className='btn-secondary' onClick={() => OnGenerateBarcodeClicked()}>Export CSV</button>
        </div>
        <ReactToPrint
          trigger={() => <span hidden />}
          content={() => componentRef.current}
          pageStyle={`@page { size: ${pageWidthMm(labelDesign)}mm ${labelDesign.heightMm}mm; margin: 0 } body { margin: 0 }`}
          ref={reactToPrintRef}
        />
        <div className="ag-theme-quartz barcodeGrid">
          <AgGridReact rowData={rowData} columnDefs={colDefs}
            ref={gridRef}
            rowSelection={'single'}
            onSelectionChanged={() => onSelectionChanged()}
          />
        </div>
      </div>
      <div className='card'>
        <h2 className='card-title'>Label preview</h2>
        <p className='card-subtitle'>One label per unit of quantity, as set on the Label Design page.</p>
        <div className='labelPreview'>
          <div ref={componentRef}>
            <BarcodeReport barcodeList={rowData} shopName={shopName} design={labelDesign} />
          </div>
          {rowData.length === 0 && <div className='empty-state'>No labels to print. Add items from Stock with "Add to barcode", or add a row here.</div>}
        </div>
      </div>
    </div>
  );
};

export default BarcodeView;