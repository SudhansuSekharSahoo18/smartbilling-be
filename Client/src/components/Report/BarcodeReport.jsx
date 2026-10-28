import React from 'react';
import './Report.css';
import LabelContent from '../Label/LabelContent';
import { expandByQuantity, pageWidthMm } from '../../Helper/labelDesign.js';

// Printable label sheet: one page per row of labels on the roll (design.columns labels per row).
const BarcodeReport = ({ barcodeList, shopName, design }) => {
    const labels = expandByQuantity(barcodeList);
    const pages = [];
    for (let i = 0; i < labels.length; i += design.columns) {
        pages.push(labels.slice(i, i + design.columns));
    }

    return (
        <div className='barcodeReport'>
            {pages.map((page, index) => (
                <div key={index} style={{
                    display: 'flex',
                    columnGap: `${design.columnGapMm}mm`,
                    width: `${pageWidthMm(design)}mm`,
                    height: `${design.heightMm}mm`,
                    overflow: 'hidden',
                    pageBreakAfter: index < pages.length - 1 ? 'always' : 'auto',
                }}>
                    {page.map((item, column) => (
                        <LabelContent key={column} design={design} item={item} shopName={shopName} />
                    ))}
                </div>
            ))}
        </div>
    );
};

export default BarcodeReport;
