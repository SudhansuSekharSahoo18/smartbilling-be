import './Billing.css';
import React, { useRef, useEffect, useState } from 'react';
import { ReactToPrint } from 'react-to-print';
import BillingTable from '../../TableContainer/BillingTable.jsx';
import Report from '../../Report/Report';
import CustomInput from '../../CustomInput/CustomInput';
import Dropdown from '../../Dropdown/Dropdown';
import { CreateBill, GetAllItems } from '../../../APIEndpoints.js'
import { getBackendUrl, getConfig, readError } from '../../../Helper/apiHelper.js';
import { calculateNetAmount, lineAmount, round2 } from './BillingManager.js'
import { formatRupees } from '../../../Helper/format.js';

const Billing = ({ notify }) => {
  const [ipAddress, setIpAddress] = useState(null);
  // const ipAddress = process.env.REACT_APP_backendUrl+"api/"
  const componentRef = useRef(null);
  const reactToPrintRef = useRef();
  const [inputValue, setInputValue] = useState('');
  const [products, setProducts] = useState([]);
  const [billItems, setBillItems] = useState([{
    // "id": maxId, 
    'itemId': 0, "barcode": '', "itemName": '',
    "mrp": null, "quantity": 1, "discountPercentage": 0
  }]);
  // The last bill saved by the server; it is what gets printed.
  const [printBill, setPrintBill] = useState(null);
  const [isSaving, setIsSaving] = useState(false);
  const [customerName, setCustomerName] = useState('');
  const [customerMobileNumber, setCustomerMobileNumber] = useState('');
  const [modeOfPayment, setModeOfPayment] = useState('cash');
  const [billDiscount, setBillDiscount] = useState();
  const [amountRecieved, setAmountRecieved] = useState();
  const [shopName, setShopName] = useState('Shop Name Not Found');
  const [shopAddress, setShopAddress] = useState('Address not found');
  const [shopGstNumber, setShopGstNumber] = useState('GST number not found');
  const shallPrintBill = useRef(false);
  const paymentOptions = [
    { value: 'cash', label: 'Cash' },
    { value: 'upi', label: 'UPI' },
    { value: 'creditcard', label: 'Credit Card' },
    { value: 'debitcard', label: 'Debit Card' },
  ];

  const handleSelectedPayment = (option) => {
    setModeOfPayment(option);
  };

  const handleKeyDown = (event) => {
    if (event.key === 'Enter') {
      onSubmitButtonClick();
    }
  };
  const onClearButtonClick = () => {
    const billItem = {
      // "id": 0, 
      'itemId': 0, "barcode": '', "itemName": '',
      "mrp": null, "quantity": 1, "discountPercentage": 0
    }
    setBillItems([billItem])
    setCustomerName('')
    setCustomerMobileNumber('')
  }

  const addBlankRow = () => {
    const billItem = {
      'itemId': 0, "barcode": '', "itemName": '',
      "mrp": null, "quantity": 1, "discountPercentage": 0
    }
    // setMaxId(maxId + 1);
    setBillItems([...billItems, billItem]);
  }

  const onSubmitButtonClick = () => {
    if (inputValue.trim() !== '') {
      let dbItem = products.find(x => x.barcode === inputValue)
      if (dbItem !== undefined) {
        let item = billItems.find(x => x.itemId === dbItem.id)
        if (item === undefined) {
          // const id = billItems.length + 1;
          // Replace the starting blank row (mrp is null until a price is entered).
          if (billItems.length === 1 && billItems[0].itemName === '' && !billItems[0].mrp) {
            const item = {
              "itemId": dbItem.id, "barcode": dbItem.barcode, "itemName": dbItem.itemName,
              "mrp": dbItem.mrp, "quantity": 1, "discountPercentage": dbItem.discountPercentage
            }
            setBillItems([item]);
          }
          else {
            const item = {
              "itemId": dbItem.id, "barcode": dbItem.barcode, "itemName": dbItem.itemName,
              "mrp": dbItem.mrp, "quantity": 1, "discountPercentage": dbItem.discountPercentage
            }
            setBillItems([item, ...billItems]);
          }
        }
        else {
          item.quantity += 1;
          setBillItems([...billItems])
        }

        setInputValue('');
      }
      else {
        alert('No item found');
      }
    }
  };

  const handleCtrlP = () => {
    // handlePrint();
    // createBill(billItems, 0, 'cash', customerName, '', true)
  };

  const handlePrint = () => {
    if (reactToPrintRef.current) {
      reactToPrintRef.current.handlePrint();
    }
  };

  useEffect(() => {
    if (shallPrintBill.current && printBill) {
      shallPrintBill.current = false;
      handlePrint();
    }
  }, [printBill]);

  useEffect(() => {
    const billAmount = calculateNetAmount(billItems)
    const numberValue = Number(billDiscount) || 0;
    setAmountRecieved(round2(billAmount - numberValue));
  }, [billDiscount]);

  useEffect(() => {
    const handleKeyDown = (event) => {
      if (event.ctrlKey && event.key === 'p') {
        event.preventDefault();
        handleCtrlP();
      }
    };

    window.addEventListener('keydown', handleKeyDown);

    return () => {
      window.removeEventListener('keydown', handleKeyDown);
    };
  }, []);

  useEffect(() => {
    Promise.all([getBackendUrl(), getConfig()])
      .then(([url, data]) => {
        setIpAddress(url);
        setShopName(data.shopName);
        setShopAddress(data.shopAddress);
        setShopGstNumber(data.shopGSTNumber);
        fetch(url + GetAllItems)
          .then(response => {
            if (!response.ok) {
              throw new Error('Network response was not ok');
            }
            return response.json();
          })
          .then(data => {
            setProducts(data);
            // console.log(process.env.REACT_APP_backendUrl)
          })
          .catch(error => {
            // Don't bill from made-up items: show the problem instead.
            setProducts([]);
            notify('error', `Could not load items from the server: ${error.message}`);
          });
      })
      .catch(error => notify('error', `Could not reach the server: ${error.message}`));
  }, []);

  const handleSubmit = async (bill) => {
    setIsSaving(true);
    try {
      const response = await fetch(ipAddress + CreateBill, {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json'
        },
        body: JSON.stringify(bill)
      });
      if (!response.ok) {
        notify('error', await readError(response));
        return;
      }
      const data = await response.json();
      notify('success', `Bill ${data.id} saved`);
      shallPrintBill.current = true;
      setPrintBill(data);
      // Start the next bill, so pressing the button again can't save this one twice.
      onClearButtonClick();
    } catch (error) {
      notify('error', `Could not save the bill: ${error.message}`);
    } finally {
      setIsSaving(false);
    }
  };

  const createBill = (billItemList, billDiscount, modeOfPayment, customerName, customerMobileNumber, customerAddress) => {
    // Skip the blank rows left for typing in an item.
    const list = billItemList.filter(ele => (ele.itemName ?? '').trim() !== '' || Number(ele.mrp) > 0);
    if (list.length === 0) {
      notify('warning', 'Add at least one item to the bill');
      return;
    }
    const invalid = list.find(ele => !(Number(ele.quantity) > 0) || !(Number(ele.mrp) >= 0) || (ele.itemName ?? '').trim() === '');
    if (invalid) {
      notify('warning', `Check "${invalid.itemName || 'the item without a name'}": it needs a name, a price and a quantity of at least 1`);
      return;
    }

    // The server works out all amounts again; these are sent for reference only.
    const billItemDto = list.map(ele => ({
      "itemId": ele.itemId,
      "billId": 0,
      "barcode": ele.barcode,
      "itemName": ele.itemName,
      "quantity": Number(ele.quantity),
      "mrp": Number(ele.mrp),
      "discountPercentage": Number(ele.discountPercentage) || 0,
      "amount": lineAmount(ele),
    }));
    const subTotal = calculateNetAmount(list);
    const discountAmount = Number(billDiscount) || 0;
    const bill = {
      "id": 0,
      "subTotal": subTotal,
      "discountAmount": discountAmount,
      "totalAmount": round2(subTotal - discountAmount),
      "modeOfPayment": modeOfPayment,
      "customerName": customerName,
      "customerMobileNumber": customerMobileNumber,
      "customerAddress": customerAddress,
      "billItems": billItemDto,
    }

    handleSubmit(bill)
  }

  return (
    <div>
      <ReactToPrint
        content={() => componentRef.current}
        ref={reactToPrintRef}
      />
      <div className='billingSection'>
        <div className='billingMain'>
          <div className='card scanBar'>
            <input id='barcodeInput' type="text" value={inputValue} onChange={(e) => setInputValue(e.target.value)} onKeyDown={handleKeyDown}
              placeholder="Scan or type a barcode and press Enter" autoFocus autoComplete="off"
            />
            <button onClick={onSubmitButtonClick}>Add</button>
            <button className='btn-secondary' onClick={onClearButtonClick}>Clear bill</button>
          </div>
          <BillingTable addBlankRow={addBlankRow} items={billItems} setBillItems={setBillItems} setBillDiscount={setBillDiscount} setAmountRecieved={setAmountRecieved}
          />
        </div>

        <aside className='card customerDetails'>
          <h2 className='card-title'>Customer</h2>
          <CustomInput label="Mobile" text={customerMobileNumber} setText={setCustomerMobileNumber} />
          <CustomInput label="Name" text={customerName} setText={setCustomerName} />
          <Dropdown label="Payment mode" options={paymentOptions} onSelect={handleSelectedPayment} />

          <div className='summaryDivider' />

          <CustomInput label="Bill discount (₹)" text={billDiscount} setText={setBillDiscount} type={'number'} />
          <div className='netAmount'>
            <span>Net amount</span>
            <strong>{formatRupees(amountRecieved) || '₹0'}</strong>
          </div>
          <button className='btn-lg btn-block'
            disabled={isSaving}
            onClick={() => createBill(billItems, billDiscount, modeOfPayment, customerName, customerMobileNumber, '')}>
            Create bill and print
          </button>
        </aside>
      </div>

      {/* print */}
      <div className='print-area'>
        <div ref={componentRef}>
          <Report bill={printBill}
            shopName={shopName} shopAddress={shopAddress} shopGSTNumber={shopGstNumber} />
        </div>
      </div>
    </div>

  );
};

export default Billing;
