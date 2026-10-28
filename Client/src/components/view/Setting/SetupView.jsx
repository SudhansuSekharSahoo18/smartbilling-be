import React from 'react';
import './Setting.css';
import ShopSettingsForm from './ShopSettingsForm.jsx';

// Shown instead of the app until the shop details have been saved once.
const SetupView = ({ notify, onComplete }) => (
  <div className="setup-page">
    <div className="setup-card">
      <h2 className="hc-title">Welcome to Smart Billing</h2>
      <p>Enter your shop details to finish setting up. They are printed on bills and barcode labels, and you can change them later on the Setting page.</p>
      <ShopSettingsForm notify={notify} onSaved={onComplete} submitLabel="Save and start" />
    </div>
  </div>
);

export default SetupView;
