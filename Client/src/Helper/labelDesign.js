import JsBarcode from 'jsbarcode';
import { GetLabelDesign, SaveLabelDesign } from '../APIEndpoints.js';

// A label design: the label size, how many labels sit side by side on the roll, and the
// elements placed on one label. All positions and sizes are in millimetres from the label's
// top-left corner, so the same design renders identically on screen and on the printer.

export const DESIGN_VERSION = 1;
export const SNAP_MM = 0.5;

export const TEXT_FIELDS = [
  { value: 'shopName', label: 'Shop name' },
  { value: 'itemName', label: 'Item name' },
  { value: 'itemCode', label: 'Item code' },
  { value: 'price', label: 'Price' },
  { value: 'custom', label: 'Custom text' },
];

export const SIZE_PRESETS = [
  { value: '50x25', label: '50 × 25 mm (2 × 1 in)', widthMm: 50, heightMm: 25, columns: 1, columnGapMm: 0 },
  { value: '2up-50x25', label: '2 per row, 50 × 25 mm', widthMm: 50, heightMm: 25, columns: 2, columnGapMm: 3 },
  { value: '2up-38x25', label: '2 per row, 38 × 25 mm', widthMm: 38, heightMm: 25, columns: 2, columnGapMm: 2 },
];

export const DEFAULT_DESIGN = {
  version: DESIGN_VERSION,
  widthMm: 50,
  heightMm: 25,
  columns: 1,
  columnGapMm: 0,
  elements: [
    { id: 'shop', type: 'text', field: 'shopName', prefix: '', text: '', xMm: 0, yMm: 0.5, wMm: 50, hMm: 3.5, fontSizePt: 7, bold: true, align: 'center' },
    { id: 'barcode', type: 'barcode', field: 'itemCode', xMm: 4, yMm: 4.5, wMm: 42, hMm: 9 },
    { id: 'code', type: 'text', field: 'itemCode', prefix: '', text: '', xMm: 0, yMm: 13.5, wMm: 50, hMm: 3, fontSizePt: 7, bold: false, align: 'center' },
    { id: 'name', type: 'text', field: 'itemName', prefix: '', text: '', xMm: 0, yMm: 17, wMm: 50, hMm: 3.5, fontSizePt: 7, bold: false, align: 'center' },
    { id: 'price', type: 'text', field: 'price', prefix: 'Fixed Price: ', text: '', xMm: 0, yMm: 20.5, wMm: 50, hMm: 4, fontSizePt: 8, bold: true, align: 'center' },
  ],
};

const clamp = (value, min, max) => Math.min(max, Math.max(min, value));
const toNumber = (value, fallback) => (Number.isFinite(Number(value)) ? Number(value) : fallback);

export const snapMm = (value) => Math.round(value / SNAP_MM) * SNAP_MM;

export const pageWidthMm = (design) =>
  design.columns * design.widthMm + (design.columns - 1) * design.columnGapMm;

export const findPreset = (design) =>
  SIZE_PRESETS.find(p =>
    p.widthMm === design.widthMm && p.heightMm === design.heightMm &&
    p.columns === design.columns && p.columnGapMm === design.columnGapMm)?.value ?? 'custom';

// Fill gaps and fix out-of-range values so a hand-edited or older saved design still renders.
export const normalizeDesign = (raw) => {
  if (!raw || typeof raw !== 'object' || !Array.isArray(raw.elements)) return structuredClone(DEFAULT_DESIGN);
  const widthMm = clamp(toNumber(raw.widthMm, 50), 10, 200);
  const heightMm = clamp(toNumber(raw.heightMm, 25), 10, 200);
  return {
    version: DESIGN_VERSION,
    widthMm,
    heightMm,
    columns: clamp(Math.round(toNumber(raw.columns, 1)), 1, 4),
    columnGapMm: clamp(toNumber(raw.columnGapMm, 0), 0, 20),
    elements: raw.elements
      .filter(el => el && (el.type === 'text' || el.type === 'barcode'))
      .map((el, index) => ({
        ...el,
        id: el.id ? String(el.id) : `el${index}`,
        field: el.type === 'barcode' ? 'itemCode' : (TEXT_FIELDS.some(f => f.value === el.field) ? el.field : 'custom'),
        prefix: el.prefix ?? '',
        text: el.text ?? '',
        xMm: clamp(toNumber(el.xMm, 0), 0, widthMm),
        yMm: clamp(toNumber(el.yMm, 0), 0, heightMm),
        wMm: clamp(toNumber(el.wMm, 10), 1, widthMm),
        hMm: clamp(toNumber(el.hMm, 3), 1, heightMm),
        fontSizePt: clamp(toNumber(el.fontSizePt, 7), 4, 48),
        bold: !!el.bold,
        align: ['left', 'center', 'right'].includes(el.align) ? el.align : 'center',
      })),
  };
};

// Scale element positions and sizes when the label size changes, so the layout keeps its shape.
export const resizeDesign = (design, widthMm, heightMm) => {
  const sx = widthMm / design.widthMm;
  const sy = heightMm / design.heightMm;
  return {
    ...design,
    widthMm,
    heightMm,
    elements: design.elements.map(el => ({
      ...el,
      xMm: snapMm(el.xMm * sx),
      yMm: snapMm(el.yMm * sy),
      wMm: Math.max(1, snapMm(el.wMm * sx)),
      hMm: Math.max(1, snapMm(el.hMm * sy)),
    })),
  };
};

export const elementText = (element, item, shopName) => {
  const values = {
    shopName,
    itemName: item.itemName,
    itemCode: item.itemCode,
    price: item.price,
    custom: element.text,
  };
  const value = values[element.field] ?? '';
  return element.field === 'custom' ? String(value) : `${element.prefix ?? ''}${value ?? ''}`;
};

// One label per unit of quantity; a blank or zero quantity still prints one label.
export const expandByQuantity = (items) =>
  items.flatMap(item => {
    const copies = Math.max(1, parseInt(item.quantity, 10) || 0);
    return Array.from({ length: copies }, () => item);
  });

// CODE128 bar pattern as a string of modules ('1' = bar, '0' = space), from the same encoder the
// on-screen barcode uses. Empty when the value cannot be encoded.
export const barcodeModules = (value) => {
  if (!value) return '';
  try {
    const encoded = {};
    JsBarcode(encoded, String(value), { format: 'CODE128' });
    return encoded.encodings.map(e => e.data).join('');
  } catch {
    return '';
  }
};

// Print job for the desktop app, which draws the labels itself with Windows printing.
// (Printing HTML through WebView2 rotates pages that are wider than tall, which labels are.)
// Each page is one row of labels; element positions are in mm from the page's top-left corner.
export const buildPrintJob = (design, items, shopName) => {
  const labels = expandByQuantity(items);
  const pages = [];
  for (let i = 0; i < labels.length; i += design.columns) {
    const elements = [];
    labels.slice(i, i + design.columns).forEach((item, column) => {
      const offsetMm = column * (design.widthMm + design.columnGapMm);
      design.elements.forEach(element => {
        const box = { type: element.type, xMm: offsetMm + element.xMm, yMm: element.yMm, wMm: element.wMm, hMm: element.hMm };
        elements.push(element.type === 'barcode'
          ? { ...box, modules: barcodeModules(item.itemCode) }
          : { ...box, text: elementText(element, item, shopName), fontSizePt: element.fontSizePt, bold: element.bold, align: element.align });
      });
    });
    pages.push(elements);
  }
  return { pageWidthMm: pageWidthMm(design), pageHeightMm: design.heightMm, labelCount: labels.length, pages };
};

export const loadLabelDesign = async (baseUrl) => {
  const response = await fetch(baseUrl + GetLabelDesign);
  if (response.status === 204) return structuredClone(DEFAULT_DESIGN);
  if (!response.ok) throw new Error('Could not load the label design');
  return normalizeDesign(await response.json());
};

export const saveLabelDesign = async (baseUrl, design) => {
  const response = await fetch(baseUrl + SaveLabelDesign, {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify(design),
  });
  if (!response.ok) throw new Error(await response.text() || 'Could not save the label design');
};
