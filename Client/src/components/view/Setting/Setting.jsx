import React, { useState, useCallback } from 'react';
import './Setting.css';
import { HealthApi, HealthDb, HealthConfig } from '../../../APIEndpoints.js';
import { getBackendUrl } from '../../../Helper/apiHelper.js';

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

const Setting = () => {
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

  return (
    <div className="hc-page">
      <div className="hc-header">
        <h2 className="hc-title">Health Check</h2>
        <button className="hc-btn" onClick={runAll}>Run Checks</button>
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
          description="Checks that all required settings (connection string, file paths) are present in appsettings."
          check={config}
        />
      </div>
    </div>
  );
};

export default Setting;




