import React from 'react';
import './Setting.css';
import ShopSettingsForm from './ShopSettingsForm.jsx';

// Shown instead of the app until the shop details have been saved once.
const SetupView = ({ notify, onComplete }) => (
  <div className="setup-page">
    <div className="card setup-card">
      <div className="setup-brand">
        <span className="brand-mark">SB</span>
        <h1>Welcome to Smart Billing</h1>
      </div>
      <p className="card-subtitle">Enter your shop details to finish setting up. They are printed on bills and barcode labels, and you can change them later on the Setting page.</p>
      <ShopSettingsForm notify={notify} onSaved={onComplete} submitLabel="Save and start" />
    </div>
  </div>
);

export default SetupView;
