import React, { useState } from 'react';
import './Setting.css';
import { changePassword } from '../../../Helper/auth.js';

const MIN_PASSWORD_LENGTH = 6;
const EMPTY = { currentPassword: '', newPassword: '', confirmPassword: '' };

// Changes the signed-in user's password (Setting page).
const ChangePasswordForm = ({ notify }) => {
  const [form, setForm] = useState(EMPTY);
  const [saving, setSaving] = useState(false);

  const onChange = (field) => (event) => setForm({ ...form, [field]: event.target.value });

  const onSubmit = async (event) => {
    event.preventDefault();
    if (form.newPassword.length < MIN_PASSWORD_LENGTH) {
      notify('warning', `The new password must have at least ${MIN_PASSWORD_LENGTH} characters`);
      return;
    }
    if (form.newPassword !== form.confirmPassword) {
      notify('warning', 'The two new passwords are different');
      return;
    }
    setSaving(true);
    try {
      await changePassword(form.currentPassword, form.newPassword);
      setForm(EMPTY);
      notify('success', 'Password changed');
    } catch (error) {
      notify('error', error.message);
    } finally {
      setSaving(false);
    }
  };

  return (
    <form className="ss-form" onSubmit={onSubmit}>
      <div className="ss-field ss-field--wide">
        <label htmlFor="cp-currentPassword">Current password</label>
        <input id="cp-currentPassword" type="password" value={form.currentPassword} onChange={onChange('currentPassword')}
          autoComplete="current-password" />
      </div>
      <div className="ss-field">
        <label htmlFor="cp-newPassword">New password</label>
        <input id="cp-newPassword" type="password" value={form.newPassword} onChange={onChange('newPassword')}
          autoComplete="new-password" />
      </div>
      <div className="ss-field">
        <label htmlFor="cp-confirmPassword">Confirm new password</label>
        <input id="cp-confirmPassword" type="password" value={form.confirmPassword} onChange={onChange('confirmPassword')}
          autoComplete="new-password" />
      </div>
      <div className="ss-actions">
        <button type="submit" disabled={saving || !form.currentPassword}>{saving ? 'Saving…' : 'Change password'}</button>
      </div>
    </form>
  );
};

export default ChangePasswordForm;
