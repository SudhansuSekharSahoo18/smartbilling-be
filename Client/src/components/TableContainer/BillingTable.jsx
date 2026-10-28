import React, { useState, useEffect } from 'react';
import './BillingTable.css';
import { calculateNetAmount, lineAmount } from '../view/Billing/BillingManager.js'
import { formatRupees } from '../../Helper/format.js';

const BillingTable = ({ addBlankRow, items, setBillItems, setBillDiscount, setAmountRecieved }) => {
  const [flag, setFlag] = useState(false);
  const [focusedIndex, setFocusedIndex] = useState([0, 0]);
  const [totalBillAmount, setTotalBillAmount] = useState(0);

  const handleKeyDown = (event, row, col) => {
    if (event.key === 'ArrowRight') {
      const nextIndex = col + 1;
      const nextInput = document.getElementById(`input-${row}-${nextIndex}`);
      if (nextInput) {
        nextInput.focus();
      }
    }
    else if (event.key === 'ArrowLeft') {
      const nextIndex = col - 1;
      const nextInput = document.getElementById(`input-${row}-${nextIndex}`);
      if (nextInput) {
        nextInput.focus();
      }
    }
    else if (event.key === 'ArrowUp') {
      const nextIndex = row - 1;
      const nextInput = document.getElementById(`input-${nextIndex}-${col}`);
      if (nextInput) {
        nextInput.focus();
      }
    }
    else if (event.key === 'ArrowDown' || event.key === 'Enter') {
      if (items.length - 1 === row) {
        addBlankRow();
        setFocusedIndex([row, col])
        setFlag(true)
        return;
      }

      onDownArrowPressed(row, col)
    }
  };

  const onDownArrowPressed = (row, col) => {
    const nextIndex = row + 1;
    const nextInput = document.getElementById(`input-${nextIndex}-${col}`);
    if (nextInput) {
      nextInput.focus();
    }
  }

  const handleInputChange = (e, index, field) => {
    for (let i = 0; i < items.length; i++) {
      if (i === index) {
        if (field === 'itemName') {
          items[i].itemName = e.target.value
        } else if (field === 'mrp') {
          items[i].mrp = Number(e.target.value)
        } else if (field === 'quantity') {
          items[i].quantity = Number(e.target.value)
        } else if (field === 'discountPercentage') {
          items[i].discountPercentage = Number(e.target.value)
        }
      }
    }

    setBillItems([...items]);
  };

  const handleDelete = (index) => {
    const newDate = items.filter((item, i) => i !== index);
    setBillItems(newDate);
  };

  useEffect(() => {
    if (flag) {
      setFlag(false)
      onDownArrowPressed(focusedIndex[0], focusedIndex[1])
    }
    const totalAmount = calculateNetAmount(items);
    setTotalBillAmount(totalAmount);
    setBillDiscount(0);
    setAmountRecieved(totalAmount)
  }, [items]);

  return (
    <div className="table-container card">
      {(
        <div className='data-table'>
          <div className="table-content">
            <table>
              <thead>
                <tr>
                  <th className='col-remove'></th>
                  <th className='col-barcode'>Barcode</th>
                  <th>Item</th>
                  <th className='num'>Price</th>
                  <th className='num'>Qty</th>
                  <th className='num'>Discount %</th>
                  <th className='num'>Amount</th>
                </tr>
              </thead>
              <tbody>
                {items.length > 0 && items.map((item, index) => (
                  <tr key={index}>
                    <td className='col-remove'>
                      <button className='btn-remove' title='Remove item' aria-label='Remove item' onClick={() => handleDelete(index)}>
                        ×
                      </button>
                    </td>
                    <td className='col-barcode'>{item.barcode}</td>
                    <td>
                      <input type="text"
                        id={`input-${index}-${2}`}
                        onChange={(e) => handleInputChange(e, index, 'itemName')}
                        value={item.itemName}
                        onKeyDown={(e) => handleKeyDown(e, index, 2)}
                      />
                    </td>
                    <td>
                      <input className='numberInput'
                        style={{ textAlign: 'right' }}
                        id={`input-${index}-${3}`}
                        autoComplete="off"
                        onChange={(e) => handleInputChange(e, index, 'mrp')}
                        value={item.mrp}
                        onKeyDown={(e) => handleKeyDown(e, index, 3)}
                      />
                    </td>
                    <td>
                      <input className='numberInput'
                        type="number  no-spinner"
                        style={{ textAlign: 'right' }}
                        id={`input-${index}-${4}`}
                        onChange={(e) => handleInputChange(e, index, 'quantity')}
                        value={item.quantity}
                        onKeyDown={(e) => handleKeyDown(e, index, 4)}
                      />
                    </td>
                    <td>
                      <input className='numberInput'
                        style={{ textAlign: 'right' }}
                        id={`input-${index}-${5}`}
                        onChange={(e) => handleInputChange(e, index, 'discountPercentage')}
                        value={item.discountPercentage}
                        onKeyDown={(e) => handleKeyDown(e, index, 5)}
                      />
                    </td>
                    <td className='num amount'>
                      {formatRupees(lineAmount(item))}
                    </td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>
          <div className='table-total'>
            <span>{items.filter(item => item.mrp).length} item(s)</span>
            <span>Total <strong>{formatRupees(totalBillAmount) || '₹0'}</strong></span>
          </div>

        </div>
      )}
    </div>
  );
};

export default BillingTable;
