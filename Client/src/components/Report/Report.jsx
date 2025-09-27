import React from 'react';
import './Report.css';
import { calculateTotalMrp, calculateNetAmount, calculateTotalDiscount } from '../view/Billing/BillingManager.js'

const Report = ({ billNumber, customerName, dateTime, billItems, shopName, shopAddress, shopGSTNumber }) => {

    return (
        <div className='report'>
            <div style={{ fontSize: '50px', margin: '0px', textAlign: 'center' }}>{shopName}</div>
            <div style={{ height: '20px' }}></div>
            <div style={{ fontSize: '30px', fontWeight: 'bold', margin: '0px', textAlign: 'center' }}>{shopAddress}</div>
            <div style={{ fontSize: '30px', fontWeight: 'bold', margin: '0px', textAlign: 'center' }}>GSTIN: {shopGSTNumber}</div>
            <div style={{ height: '20px' }}></div>
            <div style={{ fontSize: '40px', margin: '0px', textAlign: 'center' }}>Retail Invoice</div>
            <div style={{ fontSize: '20px', fontWeight: 'bold', margin: '0px' }}>Bill Number: {billNumber}</div>
            <div style={{ fontSize: '20px', fontWeight: 'bold', margin: '0px' }}>Date: {dateTime}</div>
            <div style={{ fontSize: '20px', fontWeight: 'bold', margin: '0px' }}>Name: {customerName}</div>
            <table border="1">
                <thead>
                    <tr>
                        {/* <th>Barcode</th> */}
                        <th style={{ fontSize: '15px', fontWeight: 'bold' }}>ItemName</th>
                        <th style={{ fontSize: '15px', fontWeight: 'bold' }}>MRP</th>
                        <th style={{ fontSize: '15px', fontWeight: 'bold' }}>Qty</th>
                        <th style={{ fontSize: '15px', fontWeight: 'bold' }}>Discount</th>
                        <th style={{ fontSize: '15px', fontWeight: 'bold' }}>Amount</th>
                    </tr>
                </thead>
                <tbody>
                    {billItems.length > 0 && billItems.map((item, index) => (

                        <tr key={index}>
                            <td style={{ fontSize: '25px', fontWeight: 'bold' }}>{item.itemName}</td>
                            <td style={{ textAlign: 'right', fontSize: '25px', fontWeight: 'bold' }}>{item.mrp}</td>
                            <td style={{ textAlign: 'right', fontSize: '25px', fontWeight: 'bold' }}>{item.quantity}</td>
                            <td style={{ textAlign: 'right', fontSize: '25px', fontWeight: 'bold' }}>{item.discountPercentage}</td>
                            <td style={{ textAlign: 'right', fontSize: '25px', fontWeight: 'bold' }}>{parseFloat((100 - item.discountPercentage) * 0.01 * item.mrp * item.quantity).toFixed(2)}</td>
                        </tr>
                    ))}
                    <div style={{ height: '20px' }}></div>
                    <tr>
                        {/* <td></td> */}
                        <td style={{ textAlign: 'right', fontSize: '25px', fontWeight: 'bold' }}>Net amount</td>
                        <td></td>
                        <td></td>
                        <td></td>
                        <td style={{ textAlign: 'right', fontSize: '25px', fontWeight: 'bold' }}>{calculateNetAmount(billItems)}</td>
                    </tr>
                </tbody>
            </table>
            <table>
                <tbody>
                    <tr style={{ fontSize: '20px', fontWeight: 'bold', margin: '0px' }}>
                        <td>CGST inclusive 2.5%</td>
                        <td>{parseFloat(calculateNetAmount(billItems) * 0.025).toFixed(2)}</td>
                    </tr>
                    <tr style={{ fontSize: '20px', fontWeight: 'bold', margin: '0px' }}>
                        <td>SGST inclusive 2.5%</td>
                        <td>{parseFloat(calculateNetAmount(billItems) * 0.025).toFixed(2)}</td>
                    </tr>
                    <tr style={{ fontSize: '20px', fontWeight: 'bold', margin: '0px' }}>
                        <td>Total Amount</td>
                        <td>{calculateTotalMrp(billItems)}</td>
                    </tr>
                    <tr style={{ fontSize: '20px', fontWeight: 'bold', margin: '0px' }}>
                        <td>Discount</td>
                        <td>{calculateTotalDiscount(billItems)}</td>
                    </tr>
                    <tr style={{ fontSize: '20px', fontWeight: 'bold', margin: '0px' }}>
                        <td>Net Amount</td>
                        <td>{calculateNetAmount(billItems)}</td>
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
