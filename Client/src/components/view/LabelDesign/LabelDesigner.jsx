import React, { useCallback, useEffect, useRef, useState } from 'react';
import './LabelDesigner.css';
import LabelContent from '../../Label/LabelContent';
import { getBackendUrl, getConfig } from '../../../Helper/apiHelper.js';
import {
  DEFAULT_DESIGN, SIZE_PRESETS, TEXT_FIELDS, SNAP_MM,
  findPreset, loadLabelDesign, normalizeDesign, resizeDesign, saveLabelDesign, snapMm,
} from '../../../Helper/labelDesign.js';

const PX_PER_MM = 96 / 25.4;
const ZOOM_LEVELS = [2, 4, 6, 8];
const CANVAS_PADDING_PX = 32;

const NEW_ELEMENTS = {
  shopName: { type: 'text', field: 'shopName', fontSizePt: 7, bold: true },
  itemName: { type: 'text', field: 'itemName', fontSizePt: 7, bold: false },
  itemCode: { type: 'text', field: 'itemCode', fontSizePt: 7, bold: false },
  price: { type: 'text', field: 'price', prefix: 'Rs. ', fontSizePt: 8, bold: true },
  custom: { type: 'text', field: 'custom', text: 'Text', fontSizePt: 7, bold: false },
  barcode: { type: 'barcode', field: 'itemCode' },
};

const clamp = (value, min, max) => Math.min(max, Math.max(min, value));

const LabelDesigner = ({ notify }) => {
  const [baseUrl, setBaseUrl] = useState(null);
  const [shopName, setShopName] = useState('');
  const [design, setDesign] = useState(DEFAULT_DESIGN);
  const [savedDesign, setSavedDesign] = useState(DEFAULT_DESIGN);
  const [selectedId, setSelectedId] = useState(null);
  const [zoomChoice, setZoomChoice] = useState('fit');
  const [canvasWidthPx, setCanvasWidthPx] = useState(0);
  const [customSize, setCustomSize] = useState(false);
  const [sample, setSample] = useState({ itemCode: '8901234500011', itemName: 'Sample Item', price: 999 });
  const dragRef = useRef(null);
  const canvasRef = useRef(null);

  useEffect(() => {
    const observer = new ResizeObserver(([entry]) => setCanvasWidthPx(entry.contentRect.width));
    observer.observe(canvasRef.current);
    return () => observer.disconnect();
  }, []);

  const fitZoom = Math.max(1, (canvasWidthPx - CANVAS_PADDING_PX) / (design.widthMm * PX_PER_MM));
  const zoom = zoomChoice === 'fit' ? fitZoom : zoomChoice;

  const isDirty = JSON.stringify(design) !== JSON.stringify(savedDesign);
  const selected = design.elements.find(el => el.id === selectedId) ?? null;

  useEffect(() => {
    Promise.all([getBackendUrl(), getConfig()])
      .then(([url, config]) => {
        setBaseUrl(url);
        setShopName(config.shopName ?? '');
        return loadLabelDesign(url);
      })
      .then(loaded => {
        setDesign(loaded);
        setSavedDesign(loaded);
      })
      .catch(error => notify('error', error.message));
    // Load once; notify is recreated on every App render and must not trigger a reload over unsaved edits.
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, []);

  const updateElement = useCallback((id, changes) => {
    setDesign(prev => ({
      ...prev,
      elements: prev.elements.map(el => (el.id === id ? { ...el, ...changes } : el)),
    }));
  }, []);

  const deleteElement = useCallback((id) => {
    setDesign(prev => ({ ...prev, elements: prev.elements.filter(el => el.id !== id) }));
    setSelectedId(null);
  }, []);

  const addElement = (kind) => {
    const id = `el${Date.now()}`;
    const isBarcode = kind === 'barcode';
    const wMm = Math.min(design.widthMm, isBarcode ? 40 : 30);
    const hMm = Math.min(design.heightMm, isBarcode ? 9 : 4);
    const element = {
      id, prefix: '', text: '', align: 'center', fontSizePt: 7, bold: false,
      ...NEW_ELEMENTS[kind],
      xMm: snapMm((design.widthMm - wMm) / 2),
      yMm: snapMm((design.heightMm - hMm) / 2),
      wMm, hMm,
    };
    setDesign(prev => ({ ...prev, elements: [...prev.elements, element] }));
    setSelectedId(id);
  };

  // --- Drag to move / resize -------------------------------------------------------------

  const onPointerMove = useCallback((e) => {
    const drag = dragRef.current;
    if (!drag) return;
    const dx = (e.clientX - drag.startX) / drag.scale;
    const dy = (e.clientY - drag.startY) / drag.scale;
    const { orig, labelW, labelH } = drag;
    if (drag.mode === 'move') {
      updateElement(orig.id, {
        xMm: clamp(snapMm(orig.xMm + dx), 0, Math.max(0, labelW - orig.wMm)),
        yMm: clamp(snapMm(orig.yMm + dy), 0, Math.max(0, labelH - orig.hMm)),
      });
    } else {
      updateElement(orig.id, {
        wMm: clamp(snapMm(orig.wMm + dx), 1, labelW - orig.xMm),
        hMm: clamp(snapMm(orig.hMm + dy), 1, labelH - orig.yMm),
      });
    }
  }, [updateElement]);

  const onPointerUp = useCallback(() => {
    dragRef.current = null;
    window.removeEventListener('pointermove', onPointerMove);
  }, [onPointerMove]);

  const onElementPointerDown = (e, element, mode) => {
    e.preventDefault();
    e.stopPropagation();
    setSelectedId(element.id);
    dragRef.current = {
      mode,
      orig: { ...element },
      startX: e.clientX,
      startY: e.clientY,
      scale: PX_PER_MM * zoom,
      labelW: design.widthMm,
      labelH: design.heightMm,
    };
    window.addEventListener('pointermove', onPointerMove);
    window.addEventListener('pointerup', onPointerUp, { once: true });
  };

  useEffect(() => () => window.removeEventListener('pointermove', onPointerMove), [onPointerMove]);

  // Arrow keys nudge the selected element (Shift for bigger steps); Delete removes it.
  useEffect(() => {
    const onKeyDown = (e) => {
      if (!selected || ['INPUT', 'SELECT', 'TEXTAREA'].includes(document.activeElement?.tagName)) return;
      const step = e.shiftKey ? 2 : SNAP_MM;
      const moves = { ArrowLeft: [-step, 0], ArrowRight: [step, 0], ArrowUp: [0, -step], ArrowDown: [0, step] };
      if (moves[e.key]) {
        e.preventDefault();
        const [dx, dy] = moves[e.key];
        updateElement(selected.id, {
          xMm: clamp(selected.xMm + dx, 0, Math.max(0, design.widthMm - selected.wMm)),
          yMm: clamp(selected.yMm + dy, 0, Math.max(0, design.heightMm - selected.hMm)),
        });
      } else if (e.key === 'Delete') {
        deleteElement(selected.id);
      }
    };
    window.addEventListener('keydown', onKeyDown);
    return () => window.removeEventListener('keydown', onKeyDown);
  }, [selected, design.widthMm, design.heightMm, updateElement, deleteElement]);

  // --- Label size -----------------------------------------------------------------------

  const onPresetChange = (value) => {
    const preset = SIZE_PRESETS.find(p => p.value === value);
    if (!preset) return; // "Custom" keeps the current size and unlocks the size fields.
    setDesign(prev => ({
      ...resizeDesign(prev, preset.widthMm, preset.heightMm),
      columns: preset.columns,
      columnGapMm: preset.columnGapMm,
    }));
  };

  const onSizeChange = (changes) => {
    setDesign(prev => {
      const widthMm = clamp(Number(changes.widthMm ?? prev.widthMm) || prev.widthMm, 10, 200);
      const heightMm = clamp(Number(changes.heightMm ?? prev.heightMm) || prev.heightMm, 10, 200);
      return {
        ...resizeDesign(prev, widthMm, heightMm),
        columns: clamp(Math.round(Number(changes.columns ?? prev.columns)) || 1, 1, 4),
        columnGapMm: clamp(Number(changes.columnGapMm ?? prev.columnGapMm) || 0, 0, 20),
      };
    });
  };

  // --- Save / reset ---------------------------------------------------------------------

  const onSave = async () => {
    try {
      const toSave = normalizeDesign(design);
      await saveLabelDesign(baseUrl, toSave);
      setDesign(toSave);
      setSavedDesign(toSave);
      notify('success', 'Label design saved');
    } catch (error) {
      notify('error', error.message);
    }
  };

  const preset = findPreset(design);
  const showSizeFields = customSize || preset === 'custom';

  return (
    <div className="ld-page">
      <div className="ld-toolbar">
        <label>
          Label size
          <select
            value={showSizeFields ? 'custom' : preset}
            onChange={(e) => { setCustomSize(e.target.value === 'custom'); onPresetChange(e.target.value); }}
          >
            {SIZE_PRESETS.map(p => <option key={p.value} value={p.value}>{p.label}</option>)}
            <option value="custom">Custom size…</option>
          </select>
        </label>
        {showSizeFields && (
          <>
            <label>Width (mm)<input type="number" min="10" max="200" step="0.5" value={design.widthMm} onChange={(e) => onSizeChange({ widthMm: e.target.value })} /></label>
            <label>Height (mm)<input type="number" min="10" max="200" step="0.5" value={design.heightMm} onChange={(e) => onSizeChange({ heightMm: e.target.value })} /></label>
            <label>Labels per row<input type="number" min="1" max="4" step="1" value={design.columns} onChange={(e) => onSizeChange({ columns: e.target.value })} /></label>
            <label>Gap between (mm)<input type="number" min="0" max="20" step="0.5" value={design.columnGapMm} onChange={(e) => onSizeChange({ columnGapMm: e.target.value })} /></label>
          </>
        )}
        <label>
          Zoom
          <select value={zoomChoice} onChange={(e) => setZoomChoice(e.target.value === 'fit' ? 'fit' : Number(e.target.value))}>
            <option value="fit">Fit</option>
            {ZOOM_LEVELS.map(z => <option key={z} value={z}>{z}×</option>)}
          </select>
        </label>
        <div className="ld-toolbar-actions">
          {isDirty && <span className="ld-unsaved">Unsaved changes</span>}
          <button onClick={onSave} disabled={!baseUrl || !isDirty}>Save design</button>
          <button className="ld-secondary" onClick={() => { setDesign(savedDesign); setSelectedId(null); }} disabled={!isDirty}>Undo changes</button>
          <button className="ld-secondary" onClick={() => { setDesign(resizeDesign(structuredClone(DEFAULT_DESIGN), design.widthMm, design.heightMm)); setSelectedId(null); }}>Reset layout</button>
        </div>
      </div>

      <div className="ld-body">
        <div className="ld-panel ld-toolbox">
          <h3>Add to label</h3>
          <button onClick={() => addElement('barcode')}>Barcode</button>
          <button onClick={() => addElement('itemCode')}>Item code</button>
          <button onClick={() => addElement('itemName')}>Item name</button>
          <button onClick={() => addElement('price')}>Price</button>
          <button onClick={() => addElement('shopName')}>Shop name</button>
          <button onClick={() => addElement('custom')}>Custom text</button>

          <h3>Preview item</h3>
          <label>Item code<input value={sample.itemCode} onChange={(e) => setSample({ ...sample, itemCode: e.target.value })} /></label>
          <label>Item name<input value={sample.itemName} onChange={(e) => setSample({ ...sample, itemName: e.target.value })} /></label>
          <label>Price<input value={sample.price} onChange={(e) => setSample({ ...sample, price: e.target.value })} /></label>
        </div>

        <div className="ld-canvas" ref={canvasRef} onPointerDown={() => setSelectedId(null)}>
          <div className="ld-canvas-info">
            {design.widthMm} × {design.heightMm} mm
            {design.columns > 1 && ` · ${design.columns} per row, ${design.columnGapMm} mm gap`}
            {' · drag to move, drag the blue corner to resize, arrow keys to nudge'}
          </div>
          <div
            className="ld-label-frame"
            style={{ width: `${design.widthMm * zoom}mm`, height: `${design.heightMm * zoom}mm` }}
          >
            <div style={{ width: `${design.widthMm}mm`, transform: `scale(${zoom})`, transformOrigin: 'top left' }}>
              <LabelContent
                design={design}
                item={sample}
                shopName={shopName || 'Shop name'}
                selectedId={selectedId}
                onElementPointerDown={onElementPointerDown}
              />
            </div>
          </div>
        </div>

        <div className="ld-panel ld-properties">
          <h3>Properties</h3>
          {!selected && <p className="ld-hint">Select an element on the label to edit it.</p>}
          {selected && (
            <>
              {selected.type === 'barcode' ? (
                <p className="ld-hint">Barcode of the item code (CODE128).</p>
              ) : (
                <>
                  <label>
                    Shows
                    <select value={selected.field} onChange={(e) => updateElement(selected.id, { field: e.target.value })}>
                      {TEXT_FIELDS.map(f => <option key={f.value} value={f.value}>{f.label}</option>)}
                    </select>
                  </label>
                  {selected.field === 'custom' ? (
                    <label>Text<input value={selected.text} onChange={(e) => updateElement(selected.id, { text: e.target.value })} /></label>
                  ) : (
                    <label>Text before<input value={selected.prefix} placeholder="e.g. MRP: " onChange={(e) => updateElement(selected.id, { prefix: e.target.value })} /></label>
                  )}
                  <div className="ld-row">
                    <label>Font size (pt)<input type="number" min="4" max="48" step="0.5" value={selected.fontSizePt} onChange={(e) => updateElement(selected.id, { fontSizePt: clamp(Number(e.target.value) || 4, 4, 48) })} /></label>
                    <label className="ld-check"><input type="checkbox" checked={selected.bold} onChange={(e) => updateElement(selected.id, { bold: e.target.checked })} />Bold</label>
                  </div>
                  <label>
                    Align
                    <select value={selected.align} onChange={(e) => updateElement(selected.id, { align: e.target.value })}>
                      <option value="left">Left</option>
                      <option value="center">Center</option>
                      <option value="right">Right</option>
                    </select>
                  </label>
                </>
              )}
              <div className="ld-row">
                <label>X (mm)<input type="number" step="0.5" value={selected.xMm} onChange={(e) => updateElement(selected.id, { xMm: clamp(Number(e.target.value) || 0, 0, design.widthMm - selected.wMm) })} /></label>
                <label>Y (mm)<input type="number" step="0.5" value={selected.yMm} onChange={(e) => updateElement(selected.id, { yMm: clamp(Number(e.target.value) || 0, 0, design.heightMm - selected.hMm) })} /></label>
              </div>
              <div className="ld-row">
                <label>Width (mm)<input type="number" step="0.5" value={selected.wMm} onChange={(e) => updateElement(selected.id, { wMm: clamp(Number(e.target.value) || 1, 1, design.widthMm - selected.xMm) })} /></label>
                <label>Height (mm)<input type="number" step="0.5" value={selected.hMm} onChange={(e) => updateElement(selected.id, { hMm: clamp(Number(e.target.value) || 1, 1, design.heightMm - selected.yMm) })} /></label>
              </div>
              <button className="ld-danger" onClick={() => deleteElement(selected.id)}>Remove from label</button>
            </>
          )}
        </div>
      </div>
    </div>
  );
};

export default LabelDesigner;
