import React, { useEffect, useState } from 'react';
import './Setting.css';
import { SaveSettings } from '../../../APIEndpoints.js';
import { getBackendUrl, getConfig, postRequest, refreshConfig } from '../../../Helper/apiHelper.js';

const EMPTY = { shopName: '', shopAddress: '', shopGSTNumber: '', barcodeFilePath: '' };

// Shop details printed on bills and labels. Used by the first-run setup screen and the Setting page.
const ShopSettingsForm = ({ notify, onSaved, submitLabel = 'Save settings' }) => {
  const [form, setForm] = useState(EMPTY);
  const [defaultBarcodeFilePath, setDefaultBarcodeFilePath] = useState('');
  const [saving, setSaving] = useState(false);

  useEffect(() => {
    getConfig()
      .then(config => {
        setForm({
          shopName: config.shopName ?? '',
          shopAddress: config.shopAddress ?? '',
          shopGSTNumber: config.shopGSTNumber ?? '',
          barcodeFilePath: config.barcodeFilePath ?? '',
        });
        setDefaultBarcodeFilePath(config.defaultBarcodeFilePath ?? '');
      })
      .catch(error => notify('error', error.message));
    // Load once; notify is recreated on every App render and must not reset the form while typing.
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, []);

  const onChange = (field) => (event) => setForm({ ...form, [field]: event.target.value });

  const onSubmit = async (event) => {
    event.preventDefault();
    if (!form.shopName.trim()) {
      notify('warning', 'Enter the shop name');
      return;
    }
    setSaving(true);
    try {
      const base = await getBackendUrl();
      const response = await postRequest(base + SaveSettings, form);
      if (!response.ok) {
        throw new Error((await response.text()) || 'Could not save the settings');
      }
      const config = await refreshConfig();
      notify('success', 'Settings saved');
      onSaved?.(config);
    } catch (error) {
      notify('error', error.message);
    } finally {
      setSaving(false);
    }
  };

  return (
    <form className="ss-form" onSubmit={onSubmit}>
      <div className="ss-field">
        <label htmlFor="ss-shopName">Shop name *</label>
        <input id="ss-shopName" type="text" value={form.shopName} onChange={onChange('shopName')} maxLength={200} autoFocus />
      </div>

      <div className="ss-field">
        <label htmlFor="ss-shopGSTNumber">GSTIN</label>
        <input id="ss-shopGSTNumber" type="text" value={form.shopGSTNumber} onChange={onChange('shopGSTNumber')} maxLength={20}
          placeholder="e.g. 21ABCDE1234F1Z5" />
      </div>

      <div className="ss-field ss-field--wide">
        <label htmlFor="ss-shopAddress">Shop address</label>
        <input id="ss-shopAddress" type="text" value={form.shopAddress} onChange={onChange('shopAddress')} maxLength={500} />
      </div>

      <div className="ss-field ss-field--wide">
        <label htmlFor="ss-barcodeFilePath">Barcode export file</label>
        <input id="ss-barcodeFilePath" type="text" value={form.barcodeFilePath} onChange={onChange('barcodeFilePath')}
          maxLength={500} placeholder={defaultBarcodeFilePath} />
        <p className="ss-hint">A full path to a .csv file. Leave blank to use {defaultBarcodeFilePath || 'the default'}. The folder is created if it doesn't exist.</p>
      </div>

      <div className="ss-actions">
        <button type="submit" disabled={saving}>{saving ? 'Saving…' : submitLabel}</button>
      </div>
    </form>
  );
};

export default ShopSettingsForm;
