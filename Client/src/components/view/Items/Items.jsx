import React, { useRef, useEffect, useState } from 'react';
import { AgGridReact } from 'ag-grid-react'; // React Data Grid Component
import "ag-grid-community/styles/ag-grid.css"; // Mandatory CSS required by the grid
import "ag-grid-community/styles/ag-theme-quartz.css";
import CustomInput from '../../CustomInput/CustomInput';
import './Items.css';
import Dropdown from '../../Dropdown/Dropdown';
import { postRequest, getBackendUrl, readError } from '../../../Helper/apiHelper.js';
import CustomCheckBox from '../../CustomCheckBox/CustomCheckBox.jsx';
import { AddToBarcode, GetAllItems, CreateItem, UpdateItem, DeleteItem, Export, Import } from '../../../APIEndpoints.js'
import FileUpload from '../../FileUpload/FileUpload.jsx';
import { formatRupees } from '../../../Helper/format.js';

// Number boxes hold '' while empty (see CustomInput).
const isBlank = (value) => value === '' || value === undefined || value === null || Number.isNaN(value);
const toNumber = (value) => isBlank(value) ? 0 : Number(value);

const Items = (props) => {
  const gridRef = useRef(null);
  const [barcode, setBarcode] = useState('');
  const [itemName, setItemName] = useState('');
  const [hsnCode, setHsnCode] = useState('');
  const [quantity, setQuantity] = useState(1);
  const [selectedUnit, setSelectedUnit] = useState('Pieces');
  const [costPrice, setCostPrice] = useState();
  const [MRP, setMRP] = useState();
  const [discountPercentage, setDiscountPercentage] = useState();
  const [tax, setTax] = useState();
  const [isTaxInclusive, setIsTaxInclusive] = useState(true);
  const [selectedId, setSelectedId] = useState(0);
  const [isEditMode, setIsEditMode] = useState(false);

  const [items, setItems] = useState([]);
  // The unit is saved as its name, so value and label are the same.
  const unitOptions = [
    { value: 'Pieces', label: 'Pieces' },
    { value: 'Boxes', label: 'Boxes' },
    { value: 'Meters', label: 'Meters' },
  ];

  const handleSelectedUnit = (option) => {
    setSelectedUnit(option);
  };
  const pagination = true;
  const paginationPageSize = 10;
  const paginationPageSizeSelector = [10, 50, 100, 200];

  const [colDefs, setColDefs] = useState([
    { field: "id", headerName: "ID", flex: 0.6, filter: true },
    { field: "barcode", flex: 1, filter: true },
    { field: "itemName", headerName: "Item name", flex: 2, filter: true },
    { field: "hsnCode", headerName: "HSN code", flex: 1 },
    { field: "quantity", headerName: "Qty", flex: 0.8 },
    { field: "unit", flex: 1 },
    { field: "mrp", headerName: "MRP", flex: 1, valueFormatter: p => formatRupees(p.value) },
    { field: "discountPercentage", headerName: "Discount %", flex: 1 },
    { field: "tax", headerName: "Tax %", flex: 0.8, valueFormatter: p => isBlank(p.value) ? '' : `${p.value}%` },
  ]);

  const ValidateItem = () => {
    let isValid = true;
    let errorMessages = [];

    // validate ItemName
    if (itemName.trim() === '') {
      errorMessages.push('Please provide Item Name');
      isValid = false;
    }

    // validate cost price
    if (isBlank(costPrice) || costPrice <= 0) {
      errorMessages.push('Please provide Cost Price');
      isValid = false;
    }

    // validate MRP price
    if (isBlank(MRP) || MRP <= 0) {
      errorMessages.push('Please provide MRP');
      isValid = false;
    }

    // validate HSN Code
    if (hsnCode.trim() === '') {
      errorMessages.push('Please provide HSN Code');
      isValid = false;
    }

    // validate tax (0% is a valid GST rate)
    if (isBlank(tax) || tax < 0 || tax > 100) {
      errorMessages.push('Please provide Tax % (0 to 100)');
      isValid = false;
    }

    if (!isBlank(discountPercentage) && (discountPercentage < 0 || discountPercentage > 100)) {
      errorMessages.push('Discount % must be between 0 and 100');
      isValid = false;
    }

    DisplayErrorMessages(errorMessages);

    return isValid;
  }

  const DisplayErrorMessages = (errorMessages) => {
    for (let i = 0; i < errorMessages.length; i++) {
      props.notify('error', errorMessages[i])
    }
  }

  const itemDto = () => ({
    'barcode': barcode.trim(), 'itemName': itemName.trim(), 'hsnCode': hsnCode.trim(), 'quantity': toNumber(quantity), 'unit': selectedUnit,
    'costPrice': toNumber(costPrice), 'mrp': toNumber(MRP), 'discountPercentage': toNumber(discountPercentage), 'tax': toNumber(tax),
    'isTaxInclusive': isTaxInclusive,
  });

  const OnAddItemClicked = async () => {
    if (ValidateItem() === false) {
      return;
    }

    const response = await postRequest(props.ipAddress + CreateItem, itemDto())
    if (response.ok) {
      const data = await response.json();
      setItems([...items, data])
      OnClearButtonClicked()
      props.notify('success', `Item added with barcode ${data.barcode}`)
    }
    else
      props.notify('error', await readError(response))
  }

  const OnClearButtonClicked = () => {
    setIsEditMode(false)

    setBarcode('')
    setItemName('')
    setHsnCode('')
    setQuantity(1)
    setSelectedUnit('Pieces')
    setCostPrice('')
    setMRP('')
    setDiscountPercentage('')
    setTax('')
    setIsTaxInclusive(true)
  }


  const OnEditButtonClicked = async () => {
    if (isEditMode) {
      if (ValidateItem() === false) {
        return;
      }
      const response = await postRequest(props.ipAddress + UpdateItem, { 'id': selectedId, ...itemDto() })
      if (response.ok) {
        const saved = await response.json();
        setItems(items.map(row => row.id === saved.id ? saved : row));
        OnClearButtonClicked()
        props.notify('success', 'Item Updated')
      }
      else
        props.notify('error', await readError(response))
    }
  }

  const OnDeleteButtonClicked = async () => {
    if (selectedId === 0) {
      alert('Please select a record to delete')
      return;
    }
    const itemDto = { 'id': selectedId }
    const response = await postRequest(props.ipAddress + DeleteItem, itemDto)
    if (response.ok) {
      const newData = items.filter((item) => item.id !== selectedId);
      setItems(newData)
      setSelectedId(0);
      props.notify('success', 'Item Deleted')
    }
    else
      props.notify('error', await readError(response))
  }

  const OnAddToBarcodeButtonClicked = async () => {
    if (selectedId === 0) {
      alert('Please select a record to Add in Barcode')
      return;
    }

    const response = await postRequest(props.ipAddress + AddToBarcode, { 'id': selectedId })
    if (response.ok) {
      props.notify('success', 'Item added to Barcode')
    }
    else
      props.notify('error', await readError(response))
  }

  const OnExportButtonClicked = async () => {
    try {
      const response = await fetch(props.ipAddress + Export)
      if (!response.ok) {
        throw new Error(await readError(response));
      }
      // The server names the file (Items_<date>.xlsx).
      const disposition = response.headers.get('content-disposition') ?? '';
      const filename = /filename="?([^";]+)"?/.exec(disposition)?.[1] ?? 'Items.xlsx';
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
      props.notify('error', `Export failed: ${error.message}`);
    }
  }

  const loadItems = (url) =>
    fetch(url + GetAllItems)
      .then(response => {
        if (!response.ok) {
          throw new Error('Network response was not ok');
        }
        return response.json();
      })
      .then(data => {
        setItems(data);
      })
      .catch(error => {
        setItems([]);
        props.notify('error', `Could not load items from the server: ${error.message}`);
      });

  const onImported = (result) => {
    const summary = `Import finished: ${result.added} added, ${result.updated} updated`;
    if (result.errors.length > 0) {
      props.notify('warning', `${summary}, ${result.errors.length} row(s) skipped`);
      result.errors.slice(0, 5).forEach(error => props.notify('error', error));
    } else {
      props.notify('success', summary);
    }
    loadItems(props.ipAddress);
  }

  const onSelectionChanged = () => {
    const selectedNodes = gridRef.current.api.getSelectedNodes();
    if (selectedNodes.length > 0) {
      const selectedData = selectedNodes.map(node => node.data);
      const temp = selectedData[0];
      setSelectedId(temp['id']);
    }
  };

  const onRowDoubleClicked = (e) => {
    // Use the row that was double-clicked; selectedId state may not be updated yet.
    const item = e.data;
    setSelectedId(item.id);
    setBarcode(item.barcode ?? '')
    setItemName(item.itemName ?? '')
    setHsnCode(item.hsnCode ?? '')
    setQuantity(item.quantity)
    setSelectedUnit(unitOptions.some(x => x.value === item.unit) ? item.unit : 'Pieces')
    setCostPrice(item.costPrice)
    setMRP(item.mrp)
    setDiscountPercentage(item.discountPercentage)
    setTax(item.tax)
    setIsTaxInclusive(item.isTaxInclusive)

    setIsEditMode(true)
  };

  useEffect(() => {
    getBackendUrl()
      .then(url => loadItems(url))
      .catch(error => props.notify('error', `Could not reach the server: ${error.message}`));
  }, []);

  return (
    <div className='itemsPage'>
      <div className='card'>
        <div className='card-header'>
          <h2 className='card-title'>{isEditMode ? 'Edit item' : 'Add item'}</h2>
          {!isEditMode && <span className='muted'>Leave Barcode blank to number it automatically. Double-click a row to edit it.</span>}
        </div>
        <div className='form-grid'>
          <CustomInput label={'Barcode'} text={barcode} type={'text'} setText={setBarcode} placeholder='Auto' />
          <CustomInput label={'Item name'} text={itemName} type={'text'} setText={setItemName} />
          <CustomInput label={'HSN code'} text={hsnCode} type={'text'} setText={setHsnCode} />
          <CustomInput label={'Quantity'} text={quantity} type={'number'} setText={setQuantity} />
          <Dropdown label={'Unit'} options={unitOptions} onSelect={handleSelectedUnit} value={selectedUnit} />
          <CustomInput label={'Cost price'} type={'number'} text={costPrice} setText={setCostPrice} />
          <CustomInput label={'MRP'} text={MRP} type={'number'} setText={setMRP} />
          <CustomInput label={'Discount %'} type={'number'} text={discountPercentage} setText={setDiscountPercentage} />
          <CustomInput label={'Tax %'} text={tax} type={'number'} setText={setTax} />
          <CustomCheckBox label={'Tax inclusive'} isChecked={isTaxInclusive} setIsChecked={setIsTaxInclusive} />
        </div>
        <div className='toolbar itemsActions'>
          {!isEditMode && <button onClick={OnAddItemClicked}>Add item</button>}
          {isEditMode && <button onClick={OnEditButtonClicked}>Update item</button>}
          <button className='btn-secondary' onClick={OnClearButtonClicked}>{isEditMode ? 'Cancel' : 'Clear'}</button>
        </div>
      </div>

      {!isEditMode && <div className='card'>
        <div className='toolbar'>
          <button className='btn-secondary' onClick={OnAddToBarcodeButtonClicked}>Add to barcode</button>
          <button className='btn-danger' onClick={OnDeleteButtonClicked}>Delete</button>
          <span className='toolbar-spacer' />
          <button className='btn-secondary' onClick={OnExportButtonClicked}>Export</button>
          <span className='toolbar-divider' />
          <FileUpload url={props.ipAddress ? props.ipAddress + Import : null} accept='.xlsx' buttonText='Import'
            title='Add or update items from an Excel file laid out like the Export file'
            onUploaded={onImported} onError={(message) => props.notify('error', message)} />
        </div>
        <div className="ag-theme-quartz grid-card">
          <AgGridReact rowData={items} columnDefs={colDefs} embedFullWidthRows={true}
            ref={gridRef}
            rowSelection={'single'}
            onSelectionChanged={() => onSelectionChanged()}
            onRowDoubleClicked={(e) => onRowDoubleClicked(e)}
            pagination={pagination} paginationPageSize={paginationPageSize}
            paginationPageSizeSelector={paginationPageSizeSelector}
          />
        </div>
      </div>}
    </div>
  );
};

export default Items;
