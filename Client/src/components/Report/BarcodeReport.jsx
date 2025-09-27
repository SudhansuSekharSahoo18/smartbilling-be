import React, { useState } from 'react';
import './Report.css';
import Barcode from 'react-barcode';

// https://www.youtube.com/watch?v=E3oKoxctZd0&ab_channel=Bakhriddin%40%24
const BarcodeReport = ({ barcodeList, shopName }) => {
    return (
        <div className='barcodeReport' style={{ width: '2in', height: '1in', margin: '0' }}>
            {barcodeList.length > 0 && barcodeList.map((item, index) => (
                <div key={index} style={{ pageBreakAfter: 'always' }}>
                    <div style={{ fontSize: '10px', margin: '0px', textAlign: 'center' }}>{shopName}</div>
                    <Barcode width={1} height={40} value={item.itemCode} fontSize={10} />
                    <div style={{ fontSize: '10px', margin: '0px' }}>{item.itemName}</div>
                    <div style={{ fontSize: '10px', margin: '0px' }}>Fixed Price: {item.price}</div>
                </div>
            ))}
        </div>
    );
};

export default BarcodeReport;
