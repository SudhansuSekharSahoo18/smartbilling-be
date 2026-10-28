import React, { useLayoutEffect, useRef } from 'react';
import JsBarcode from 'jsbarcode';

// CODE128 barcode stretched to fill its box. The bars scale together, so it still scans;
// the human-readable code is a separate text element so it is not stretched with the bars.
const LabelBarcode = ({ value }) => {
  const svgRef = useRef(null);

  useLayoutEffect(() => {
    const svg = svgRef.current;
    while (svg.firstChild) svg.removeChild(svg.firstChild);
    if (!value) return;
    try {
      JsBarcode(svg, String(value), { format: 'CODE128', displayValue: false, margin: 0, width: 2, height: 100 });
    } catch {
      return; // Value cannot be encoded; leave the box empty.
    }
    const width = svg.getAttribute('width');
    const height = svg.getAttribute('height');
    svg.setAttribute('viewBox', `0 0 ${parseFloat(width)} ${parseFloat(height)}`);
    svg.setAttribute('width', '100%');
    svg.setAttribute('height', '100%');
    svg.setAttribute('preserveAspectRatio', 'none');
    svg.removeAttribute('style');
  }, [value]);

  return <svg ref={svgRef} style={{ display: 'block', width: '100%', height: '100%' }} />;
};

export default LabelBarcode;
