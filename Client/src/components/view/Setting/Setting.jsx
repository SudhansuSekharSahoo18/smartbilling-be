import React, { useState, useCallback } from 'react';
import './Setting.css';
import { HealthApi, HealthDb, HealthConfig } from '../../../APIEndpoints.js';
import { getBackendUrl } from '../../../Helper/apiHelper.js';
import { isDesktop, sendToDesktop } from '../../../Helper/desktopBridge.js';
import ShopSettingsForm from './ShopSettingsForm.jsx';

const STATUS = { IDLE: 'idle', LOADING: 'loading', OK: 'ok', ERROR: 'error' };

const initialCheck = () => ({ status: STATUS.IDLE, message: '' });

const StatusBadge = ({ status }) => {
  const map = {
    [STATUS.IDLE]:    { label: 'Not checked', className: 'hc-badge hc-badge--idle' },
    [STATUS.LOADING]: { label: 'Checking…',   className: 'hc-badge hc-badge--loading' },
    [STATUS.OK]:      { label: 'Healthy',      className: 'hc-badge hc-badge--ok' },
    [STATUS.ERROR]:   { label: 'Unhealthy',    className: 'hc-badge hc-badge--error' },
  };
  const { label, className } = map[status] ?? map[STATUS.IDLE];
  return <span className={className}>{label}</span>;
};

const HealthCard = ({ title, description, check }) => (
  <div className={`hc-card hc-card--${check.status}`}>
    <div className="hc-card__header">
      <span className="hc-card__title">{title}</span>
      <StatusBadge status={check.status} />
    </div>
    <p className="hc-card__desc">{description}</p>
    {check.message && <p className="hc-card__message">{check.message}</p>}
  </div>
);

const Setting = ({ notify, onSettingsSaved }) => {
  const [api,    setApi]    = useState(initialCheck());
  const [db,     setDb]     = useState(initialCheck());
  const [config, setConfig] = useState(initialCheck());

  const runCheck = useCallback(async (endpoint, setter) => {
    setter({ status: STATUS.LOADING, message: '' });
    try {
      const base = await getBackendUrl();
      const response = await fetch(`${base}${endpoint}`);
      const data = await response.json();
      setter({
        status: data.healthy ? STATUS.OK : STATUS.ERROR,
        message: data.message ?? '',
      });
    } catch (err) {
      setter({ status: STATUS.ERROR, message: err.message ?? 'Request failed.' });
    }
  }, []);

  const runAll = () => {
    runCheck(HealthApi,    setApi);
    runCheck(HealthDb,     setDb);
    runCheck(HealthConfig, setConfig);
  };

  const onChooseLabelPrinterClicked = async () => {
    try {
      const result = await sendToDesktop({ type: 'chooseLabelPrinter' });
      if (result.ok) notify('success', `Label printer: ${result.printer}`);
    } catch (error) {
      notify('error', error.message);
    }
  };

  return (
    <div className="hc-page">
      <div className="card">
        <h2 className="card-title">Shop details</h2>
        <p className="card-subtitle">Printed on bills and barcode labels.</p>
        <ShopSettingsForm notify={notify} onSaved={onSettingsSaved} />
      </div>

      {isDesktop() && (
        <div className="card">
          <div className="card-header">
            <div>
              <h2 className="card-title">Label printer</h2>
              <p className="card-subtitle ss-flush">The printer used by Print labels on the Barcode page.</p>
            </div>
            <button className="btn-secondary" onClick={onChooseLabelPrinterClicked}>Choose label printer…</button>
          </div>
        </div>
      )}

      <div className="card">
        <div className="card-header">
          <div>
            <h2 className="card-title">Health check</h2>
            <p className="card-subtitle ss-flush">Check that the app, database and settings are working.</p>
          </div>
          <button onClick={runAll}>Run checks</button>
        </div>

        <div className="hc-grid">
          <HealthCard
            title="API"
            description="Verifies the backend API is reachable and responding."
            check={api}
          />
          <HealthCard
            title="Database"
            description="Confirms the application can connect to the configured SQLite database."
            check={db}
          />
          <HealthCard
            title="Configuration"
            description="Checks that the shop details have been entered."
            check={config}
          />
        </div>
      </div>
    </div>
  );
};

export default Setting;




