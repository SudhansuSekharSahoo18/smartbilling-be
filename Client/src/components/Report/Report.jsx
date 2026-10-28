import React from 'react';
import './Report.css';
import { calculateTotalMrp, gstBreakup, round2 } from '../view/Billing/BillingManager.js'
import { formatDate } from '../../Helper/dateHelper.js'

const money = (value) => round2(value).toFixed(2);

// Prints a saved bill (as returned by the server), so the printout always matches what was stored.
const Report = ({ bill, shopName, shopAddress, shopGSTNumber }) => {
    if (!bill) return null;
    const billItems = bill.billItems ?? [];
    const totalMrp = calculateTotalMrp(billItems);
    const itemDiscount = round2(totalMrp - bill.subTotal);
    const discount = round2(itemDiscount + (bill.discountAmount ?? 0));
    const gst = gstBreakup(bill);

    return (
        <div className='report'>
            <div style={{ fontSize: '50px', margin: '0px', textAlign: 'center' }}>{shopName}</div>
            <div style={{ height: '20px' }}></div>
            <div style={{ fontSize: '30px', fontWeight: 'bold', margin: '0px', textAlign: 'center' }}>{shopAddress}</div>
            <div style={{ fontSize: '30px', fontWeight: 'bold', margin: '0px', textAlign: 'center' }}>GSTIN: {shopGSTNumber}</div>
            <div style={{ height: '20px' }}></div>
            <div style={{ fontSize: '40px', margin: '0px', textAlign: 'center' }}>Retail Invoice</div>
            <div style={{ fontSize: '20px', fontWeight: 'bold', margin: '0px' }}>Bill Number: {bill.id}</div>
            <div style={{ fontSize: '20px', fontWeight: 'bold', margin: '0px' }}>Date: {formatDate(bill.createdDateTime)}</div>
            <div style={{ fontSize: '20px', fontWeight: 'bold', margin: '0px' }}>Name: {bill.customerName}</div>
            <table border="1">
                <thead>
                    <tr>
                        <th style={{ fontSize: '15px', fontWeight: 'bold' }}>ItemName</th>
                        <th style={{ fontSize: '15px', fontWeight: 'bold' }}>MRP</th>
                        <th style={{ fontSize: '15px', fontWeight: 'bold' }}>Qty</th>
                        <th style={{ fontSize: '15px', fontWeight: 'bold' }}>Discount</th>
                        <th style={{ fontSize: '15px', fontWeight: 'bold' }}>Amount</th>
                    </tr>
                </thead>
                <tbody>
                    {billItems.map((item, index) => (
                        <tr key={index}>
                            <td style={{ fontSize: '25px', fontWeight: 'bold' }}>{item.itemName}</td>
                            <td style={{ textAlign: 'right', fontSize: '25px', fontWeight: 'bold' }}>{money(item.mrp)}</td>
                            <td style={{ textAlign: 'right', fontSize: '25px', fontWeight: 'bold' }}>{item.quantity}</td>
                            <td style={{ textAlign: 'right', fontSize: '25px', fontWeight: 'bold' }}>{item.discountPercentage}%</td>
                            <td style={{ textAlign: 'right', fontSize: '25px', fontWeight: 'bold' }}>{money(item.amount)}</td>
                        </tr>
                    ))}
                    <tr>
                        <td style={{ textAlign: 'right', fontSize: '25px', fontWeight: 'bold' }}>Net amount</td>
                        <td></td>
                        <td></td>
                        <td></td>
                        <td style={{ textAlign: 'right', fontSize: '25px', fontWeight: 'bold' }}>{money(bill.totalAmount)}</td>
                    </tr>
                </tbody>
            </table>
            <table>
                <tbody>
                    {gst.map(({ rate, cgst, sgst }) => (
                        <React.Fragment key={rate}>
                            <tr style={{ fontSize: '20px', fontWeight: 'bold', margin: '0px' }}>
                                <td>CGST inclusive {rate / 2}%</td>
                                <td>{money(cgst)}</td>
                            </tr>
                            <tr style={{ fontSize: '20px', fontWeight: 'bold', margin: '0px' }}>
                                <td>SGST inclusive {rate / 2}%</td>
                                <td>{money(sgst)}</td>
                            </tr>
                        </React.Fragment>
                    ))}
                    <tr style={{ fontSize: '20px', fontWeight: 'bold', margin: '0px' }}>
                        <td>Total Amount</td>
                        <td>{money(totalMrp)}</td>
                    </tr>
                    <tr style={{ fontSize: '20px', fontWeight: 'bold', margin: '0px' }}>
                        <td>Discount</td>
                        <td>{money(discount)}</td>
                    </tr>
                    <tr style={{ fontSize: '20px', fontWeight: 'bold', margin: '0px' }}>
                        <td>Net Amount</td>
                        <td>{money(bill.totalAmount)}</td>
                    </tr>
                </tbody>
            </table>

            <div style={{ fontSize: '25px', fontWeight: 'bold', margin: '0px' }}>Terms & Conditions:</div>
            <div style={{ fontSize: '20px', fontWeight: 'bold', margin: '0px' }}>Subject to {shopName} No cash return No gurantee on the products</div>
            <div style={{ fontSize: '20px', fontWeight: 'bold', margin: '0px' }}>Exchange within 15 days with barcode sticker & bill</div>
            <div style={{ fontSize: '20px', fontWeight: 'bold', margin: '0px' }}>Thank you Shopping with us</div>
            <div style={{ height: '20px' }}></div>
            <div style={{ fontSize: '25px', fontWeight: 'bold', margin: '0px', textAlign: 'center' }}>Visit again!!!</div>
        </div>
    );
};

export default Report;
