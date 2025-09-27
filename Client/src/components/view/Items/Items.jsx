import React, { useRef, useEffect, useState } from 'react';
import { AgGridReact } from 'ag-grid-react'; // React Data Grid Component
import "ag-grid-community/styles/ag-grid.css"; // Mandatory CSS required by the grid
import "ag-grid-community/styles/ag-theme-quartz.css";
import CustomInput from '../../CustomInput/CustomInput';
import './Items.css';
import Dropdown from '../../Dropdown/Dropdown';
import { postRequest } from '../../../Helper/apiHelper.js';
import CustomCheckBox from '../../CustomCheckBox/CustomCheckBox.jsx';
import { AddToBarcode, GetAllItems, CreateItem, UpdateItem, DeleteItem, Export, Import } from '../../../APIEndpoints.js'
import FileUpload from '../../FileUpload/FileUpload.jsx';


const Items = (props) => {
  const gridRef = useRef(null);
  const [ip, setIp] = useState('');
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
  const unitOptions = [
    { value: 1, label: 'Pieces' },
    { value: 2, label: 'Boxes' },
    { value: 3, label: 'Meters' },
  ];

  const handleSelectedUnit = (option) => {
    setSelectedUnit(option);
  };
  const pagination = true;
  const paginationPageSize = 10;
  const paginationPageSizeSelector = [10, 50, 100, 200];

  // const EditButton = (props) => {
  //   return <button onClick={() => window.alert('clicked')}>Edit</button>;
  // };

  // const DeleteButton = (props) => {
  //   return <button className='deleteButton' onClick={() => {
  //     console.log(props.id)
  //     // const userConfirmed = window.confirm('Are you sure you want to delete this item?');
  //     // if (userConfirmed) {
  //     //   const newData = props.items.filter((item, i) => i !== index);
  //     //   setItems(newData)
  //     // }
  //   }}>Delete</button>;
  // };

  const [colDefs, setColDefs] = useState([
    // { field: "button", flex: 1, cellRendererFramework: p => <DeleteButton id={p.id} />, },
    // { field: "", flex: 1, cellRenderer: EditButton },
    { field: "id", flex: 1, filter: true },
    { field: "barcode", flex: 1, filter: true },
    { field: "itemName", flex: 2 },
    { field: "hsnCode", flex: 1 },
    { field: "quantity", flex: 1 },
    { field: "Unit", flex: 1 },
    { field: "mrp", flex: 1 },
    { field: "discountPercentage", flex: 1 },
    { field: "tax", flex: 1, editable: true, cellEditor: 'agSelectCellEditor', cellEditorParams: { values: ['5%', '12%', '18%'], } },
    // { field: "isTaxInclusive", flex: 1 },
  ]);

  const ValidateItem = () => {
    let isValid = true;
    let errorMessages = [];

    // validate ItemName
    if (itemName === '' || itemName === undefined) {
      const message = 'Please provide Item Name';
      errorMessages.push(message);
      isValid = false;
    }

    // validate cost price
    if (costPrice === 0 || costPrice === undefined) {
      const message = 'Please provide Cost Price';
      errorMessages.push(message);
      isValid = false;
    }

    // validate MRP price
    if (MRP === 0 || MRP === undefined) {
      const message = 'Please provide MRP';
      errorMessages.push(message);
      isValid = false;
    }

    // validate tax
    if (tax === 0 || tax === undefined) {
      const message = 'Please provide Tax';
      errorMessages.push(message);
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

  const OnAddItemClicked = async () => {
    if (ValidateItem() === false) {
      return;
    }

    const itemDto = {
      'barcode': barcode, 'itemName': itemName, 'hsnCode': hsnCode, 'quantity': quantity, 'unit': selectedUnit,
      'costPrice': costPrice, 'mrp': MRP, 'discountPercentage': discountPercentage, 'tax': tax, 'isTaxInclusive': isTaxInclusive,
    };
    const response = await postRequest(props.ipAddress + CreateItem, itemDto)
    if (response.ok) {
      const data = await response.json();
      setItems([...items, data])
      OnClearButtonClicked()
      props.notify('success', 'Item added')
    }
    else
      props.notify('error', response.status)
  }

  const OnClearButtonClicked = () => {
    setIsEditMode(false)

    setBarcode('')
    setItemName('')
    setHsnCode('')
    setQuantity(1)
    setCostPrice(0)
    setMRP(0)
    setDiscountPercentage(0)
    setTax(0)
    setIsTaxInclusive(true)
  }


  const OnEditButtonClicked = async () => {
    if (isEditMode) {
      const itemDto = {
        'id': selectedId, 'barcode': barcode, 'itemName': itemName, 'hsnCode': hsnCode, 'quantity': quantity, 'unit': selectedUnit,
        'costPrice': costPrice, 'mrp': MRP, 'discountPercentage': discountPercentage, 'tax': tax,
        'isTaxInclusive': isTaxInclusive,
      };
      const response = await postRequest(props.ipAddress + UpdateItem, itemDto)
      if (response.ok) {
        const updatedData = items.map(row =>
          row.id === selectedId ? {
            ...row, barcode: barcode, itemName: itemName, hsnCode: hsnCode, quantity: quantity, costPrice: costPrice,
            mrp: MRP, discountPercentage: discountPercentage, tax: tax, isTaxInclusive: isTaxInclusive
          } : row
        );
        setItems(updatedData);
        OnClearButtonClicked()
        setIsEditMode(!isEditMode)
        props.notify('success', 'Item Updated')
      }
      else
        props.notify('error', response.status)
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
      props.notify('error', response.status)
  }

  const OnAddToBarcodeButtonClicked = async () => {
    if (selectedId === 0) {
      alert('Please select a record to Add in Barcode')
      return;
    }

    const response = await postRequest(props.ipAddress + AddToBarcode, { 'id': selectedId })
    if (response.ok) {
      // const data = await response.json();
      // setItems([...items, data])
      // OnClearButtonClicked()
      props.notify('success', 'Item added to Barcode')
    }
    else
      props.notify('error', response.status)
  }

  const OnExportButtonClicked = async () => {
    const filename = 'items.xlsx'
    try {
      const response = await fetch(props.ipAddress + Export, {
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
  
  const OnImportButtonClicked = async () => {
    const filename = 'items.csv'
    try {
      const response = await fetch(props.ipAddress + Import, {
        method: 'POST',
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

  const onSelectionChanged = () => {
    const selectedNodes = gridRef.current.api.getSelectedNodes();
    if (selectedNodes.length > 0) {
      const selectedData = selectedNodes.map(node => node.data);
      const temp = selectedData[0];
      setSelectedId(temp['id']);
    }
  };

  const onRowDoubleClicked = (e) => {
    setSelectedId(e.data['id']);
    const item = items.filter((item) => item.id === selectedId)[0];
    setBarcode(item.barcode)
    setItemName(item.itemName)
    setHsnCode(item.hsnCode)
    setQuantity(item.quantity)
    setCostPrice(item.costPrice)
    setMRP(item.mrp)
    setDiscountPercentage(item.discountPercentage)
    setTax(item.tax)
    setIsTaxInclusive(item.isTaxInclusive)

    setIsEditMode(!isEditMode)
  };

  useEffect(() => {
    // setItemName('test6');
    // setQuantity(5);
    // setCostPrice(100);
    // setSellPrice(200);
    // setTax(0);

    fetch('/config.json')
      .then(response => response.json())
      .then(data => {
        const url = data.backendUrl + 'api/';
        setIp(data.backendUrl);

        fetch(url + GetAllItems)
          .then(response => {
            if (!response.ok) {
              throw new Error('Network response was not ok');
            }
            return response.json();
          })
          .then(data => {
            // data.length = 5
            setItems(data);
          })
          .catch(error => {
            const defaultItems = [
              { id: 1, barcode: "101", itemName: 'Invalid', price: 500 },
              { id: 2, barcode: "102", itemName: 'Invalid', price: 1500 },
              { id: 3, barcode: "103", itemName: 'Invalid', price: 400 },
              { id: 4, barcode: "104", itemName: 'Invalid', price: 150 },
              { id: 5, barcode: "105", itemName: 'Invalid', price: 80 },
            ];
            setItems(defaultItems)
          });
      })
      .catch(error => console.error('Error fetching config:', error));
  }, []);

  return (
    <div className='itemsPage'>
      <div className='inputForm'>
        <CustomInput className='customInput' label={'Barcode'} text={barcode} type={'text'} setText={setBarcode} />
        <CustomInput className='customInput' label={'ItemName'} text={itemName} type={'text'} setText={setItemName} />
        <CustomInput className='customInput' label={'Quantity'} text={quantity} type={'number'} setText={setQuantity} />
        <Dropdown label={'Select Unit'} options={unitOptions} onSelect={handleSelectedUnit} />
        <CustomInput className='customInput' label={'CostPrice'} type={'number'} text={costPrice} setText={setCostPrice} />
        <CustomInput className='customInput' label={'MRP'} text={MRP} type={'number'} setText={setMRP} />
        <CustomInput className='customInput' label={'Discount Percentage'} type={'number'} text={discountPercentage} setText={setDiscountPercentage} />
        <CustomInput className='customInput' label={'Tax'} text={tax} type={'number'} setText={setTax} />
        <label>%</label>
        <CustomCheckBox className='customInput' label={'Tax Inclusive'} isChecked={isTaxInclusive} setIsChecked={setIsTaxInclusive} />
        <CustomInput className='customInput' label={'HSN Code'} text={hsnCode} type={'text'} setText={setHsnCode} />

      </div>
      {!isEditMode && <button onClick={OnAddItemClicked}>Add Item</button>}
      {isEditMode && <button onClick={OnEditButtonClicked}>Update</button>}
      <button onClick={OnClearButtonClicked}>{isEditMode ? 'Cancel' : 'Clear'}</button>
      {!isEditMode && <button onClick={OnDeleteButtonClicked}>Delete</button>}
      {!isEditMode && <button onClick={OnAddToBarcodeButtonClicked}>Add to Barcode</button>}
      {!isEditMode && <button onClick={OnExportButtonClicked}>Export</button>}
      {!isEditMode && <button onClick={OnImportButtonClicked}>Import</button>}
      {<FileUpload ipAddress={ip} />}
      {!isEditMode && <div className="ag-theme-quartz" >
        <AgGridReact rowData={items} columnDefs={colDefs} embedFullWidthRows={true}
          ref={gridRef}
          rowSelection={'single'}
          onSelectionChanged={() => onSelectionChanged()}
          onRowDoubleClicked={(e) => onRowDoubleClicked(e)}
          pagination={pagination} paginationPageSize={paginationPageSize}
          paginationPageSizeSelector={paginationPageSizeSelector}
        />
      </div>}
    </div>
  );
};

export default Items;



