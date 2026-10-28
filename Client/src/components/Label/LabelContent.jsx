import React from 'react';
import LabelBarcode from './LabelBarcode';
import { elementText } from '../../Helper/labelDesign.js';

// One label drawn from a design. Styles are inline so the HTML prints the same when it is
// sent to the desktop app without this page's CSS.
//
// In the designer, pass onElementPointerDown to make elements draggable: it is called with
// (event, element, 'move' | 'resize').
const LabelContent = ({ design, item, shopName, selectedId, onElementPointerDown }) => {
  const interactive = !!onElementPointerDown;

  return (
    <div style={{
      position: 'relative',
      flex: 'none',
      width: `${design.widthMm}mm`,
      height: `${design.heightMm}mm`,
      overflow: 'hidden',
      background: 'white',
      color: 'black',
      fontFamily: 'Arial, sans-serif',
    }}>
      {design.elements.map(element => {
        const selected = interactive && element.id === selectedId;
        const boxStyle = {
          position: 'absolute',
          left: `${element.xMm}mm`,
          top: `${element.yMm}mm`,
          width: `${element.wMm}mm`,
          height: `${element.hMm}mm`,
          boxSizing: 'border-box',
        };
        if (interactive) {
          boxStyle.cursor = 'move';
          boxStyle.outline = selected ? '0.3mm solid #007bff' : '0.15mm dashed #9aa5b1';
        }

        return (
          <div
            key={element.id}
            style={boxStyle}
            onPointerDown={interactive ? (e) => onElementPointerDown(e, element, 'move') : undefined}
          >
            {element.type === 'barcode' ? (
              <LabelBarcode value={item.itemCode} />
            ) : (
              <div style={{
                width: '100%',
                height: '100%',
                overflow: 'hidden',
                whiteSpace: 'nowrap',
                fontSize: `${element.fontSizePt}pt`,
                lineHeight: `${element.hMm}mm`,
                fontWeight: element.bold ? 'bold' : 'normal',
                textAlign: element.align,
              }}>
                {elementText(element, item, shopName)}
              </div>
            )}
            {selected && (
              <div
                title="Drag to resize"
                style={{
                  position: 'absolute', right: '-0.8mm', bottom: '-0.8mm', width: '1.6mm', height: '1.6mm',
                  background: '#007bff', cursor: 'nwse-resize',
                }}
                onPointerDown={(e) => onElementPointerDown(e, element, 'resize')}
              />
            )}
          </div>
        );
      })}
    </div>
  );
};

export default LabelContent;
