import './Billing.css';
import React, { useRef, useEffect, useState } from 'react';
import { ReactToPrint } from 'react-to-print';
import BillingTable from '../../TableContainer/BillingTable.jsx';
import Report from '../../Report/Report';
import CustomInput from '../../CustomInput/CustomInput';
import Dropdown from '../../Dropdown/Dropdown';
import { formatDate } from '../../../Helper/dateHelper.js'
import { CreateBill, GetAllItems } from '../../../APIEndpoints.js'
import { calculateNetAmount } from './BillingManager.js'

const Billing = () => {
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
  const [billNumber, setBillNumber] = useState();
  const [customerName, setCustomerName] = useState('');
  const [customerMobileNumber, setCustomerMobileNumber] = useState('');
  const [modeOfPayment, setModeOfPayment] = useState('cash');
  const [dateTime, setDateTime] = useState('');
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
          if (billItems.length === 1 && billItems[0].itemName === '' && billItems[0].mrp === 0) {
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
    if (shallPrintBill.current) {
      handlePrint();
    }
  }, [billNumber]);

  useEffect(() => {
    const billAmount = calculateNetAmount(billItems)
    const numberValue = isNaN(billDiscount) ? 0 : Number(billDiscount);
    setAmountRecieved(billAmount - numberValue);
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
    fetch('/config.json')
      .then(response => response.json())
      .then(data => {
        setIpAddress(data.backendUrl + 'api/')
        setShopName(data.shopName)
        setShopAddress(data.shopAddress)
        setShopGstNumber(data.shopGSTNumber)
        const url = data.backendUrl + 'api/';
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
            // alert(error);
            const defaultItems = [
              { id: 1, barcode: "101", itemName: 'Saree', mrp: 500 },
              { id: 2, barcode: "102", itemName: 'Jeans', mrp: 1500 },
              { id: 3, barcode: "103", itemName: 'Shirt', mrp: 400 },
              { id: 4, barcode: "104", itemName: 'Socks', mrp: 150 },
              { id: 5, barcode: "105", itemName: 'Lungi', mrp: 80 },
            ];
            setProducts(defaultItems)
            // setLoading(false);
          });
      })
      .catch(error => console.error('Error fetching config:', error));
  }, []);

  const handleSubmit = async (bill) => {
    try {
      const response = await fetch(ipAddress + CreateBill, {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json'
        },
        body: JSON.stringify(bill)
      });
      const data = await response.json();
      setCustomerName(data.customerName);
      setDateTime(formatDate(data.createdDateTime));
      shallPrintBill.current = true;
      setBillNumber(data.id);
    } catch (error) {
      alert(error)
    }
  };

  const createBill = (billItemList, discountPercentage, modeOfPayment, customerName, customerMobileNumber, customerAddress, isPrintBillEnable) => {
    try {
      const billItemDto = [];
      const list = billItemList.filter((element, i) => (element.mrp !== null));
      list.forEach(ele => {
        billItemDto.push({
          "itemId": ele.itemId,
          "billId": 0,
          "barcode": ele.barcode,
          "itemName": ele.itemName,
          "quantity": ele.quantity,
          "mrp": ele.mrp,
          "discountPercentage": ele.discountPercentage,
          "amount": (100 - ele.discountPercentage) * 0.01 * ele.mrp * ele.quantity,
        });
      });

      let gst = 0;
      let subTotal = 0;
      let totalAmount = 0;
      billItemDto.forEach(x => subTotal += x.amount);
      totalAmount = subTotal - discountPercentage + gst;
      const bill = {
        "id": 0,
        "subTotal": subTotal,
        "discountPercentage": discountPercentage,
        "totalAmount": totalAmount,
        "modeOfPayment": modeOfPayment,
        "customerName": customerName,
        "customerMobileNumber": customerMobileNumber,
        "customerAddress": customerAddress,
        "billItems": billItemDto,
      }

      // POST request
      handleSubmit(bill)
    } catch (e) {
      // console.log(e)
    }
  }

  return (
    <div>
      <div id='barcodeControls' className='barcodeControls'>
        <input id='barcodeInput' type="text" value={inputValue} onChange={(e) => setInputValue(e.target.value)} onKeyDown={handleKeyDown} placeholder="Enter barcode"
        />
        <button onClick={onSubmitButtonClick}>Submit</button>
        <button onClick={onClearButtonClick}>Clear</button>
        <button onClick={() => createBill(billItems, billDiscount, modeOfPayment, customerName, customerMobileNumber, '', true)}>Create Bill</button>
        <ReactToPrint
          // trigger={() => {
          // return <button>Pri t Bill</button>;
          // }}
          content={() => componentRef.current}
          ref={reactToPrintRef}
        />
        {/* <div>Customer Details</div> */}

        <div className='billingSection'>
          <BillingTable addBlankRow={addBlankRow} items={billItems} setBillItems={setBillItems} setBillDiscount={setBillDiscount} setAmountRecieved={setAmountRecieved}
          />
          <div className='customerDetails'>
            <CustomInput className='customInput' label="Mobile" text={customerMobileNumber} setText={setCustomerMobileNumber} />
            <CustomInput className='customInput' label="Name" text={customerName} setText={setCustomerName} />
            <Dropdown label="Payment Mode" options={paymentOptions} onSelect={handleSelectedPayment} />
          </div>
        </div>
        <div>
          <CustomInput className='customInput' label="Bill Discount" text={billDiscount} setText={setBillDiscount} type={'number'} />
          <CustomInput className='customInput' label="Amount Recieved" text={amountRecieved} setText={setAmountRecieved} isReadyOnly={true} />
        </div>
      </div>

      {/* print */}
      <div className='print-area'>
        <div ref={componentRef}>
          <Report billNumber={billNumber} customerName={customerName} dateTime={dateTime} billItems={billItems}
            shopName={shopName} shopAddress={shopAddress} shopGSTNumber={shopGstNumber} />
        </div>
      </div>
    </div>

  );
};

export default Billing;
