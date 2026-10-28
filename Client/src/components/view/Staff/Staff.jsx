import React, { useEffect, useState } from 'react';
import './Staff.css';
import '../Setting/Setting.css';
import { CreateEmployee, DeleteEmployee, GetEmployees, ResetEmployeePassword, UpdateEmployee } from '../../../APIEndpoints.js';
import { getBackendUrl, postRequest, readError } from '../../../Helper/apiHelper.js';

const MIN_PASSWORD_LENGTH = 6;
const EMPTY = { id: 0, name: '', userName: '', phoneNumber: '', isAdmin: false, password: '', confirmPassword: '' };

// Staff accounts: who can sign in, and who is an admin. Only admins see this page.
const Staff = ({ notify, currentUserName }) => {
  const [employees, setEmployees] = useState([]);
  // 'add' | 'edit' | 'reset'
  const [mode, setMode] = useState('add');
  const [form, setForm] = useState(EMPTY);
  const [saving, setSaving] = useState(false);

  const loadEmployees = async () => {
    try {
      const base = await getBackendUrl();
      const response = await fetch(base + GetEmployees);
      if (!response.ok) throw new Error(await readError(response));
      setEmployees(await response.json());
    } catch (error) {
      notify('error', `Could not load the staff accounts: ${error.message}`);
    }
  };

  useEffect(() => {
    loadEmployees();
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, []);

  const onChange = (field) => (event) =>
    setForm({ ...form, [field]: event.target.type === 'checkbox' ? event.target.checked : event.target.value });

  const startAdd = () => {
    setMode('add');
    setForm(EMPTY);
  };

  const startEdit = (employee) => {
    setMode('edit');
    setForm({ ...EMPTY, ...employee, phoneNumber: employee.phoneNumber ?? '' });
  };

  const startReset = (employee) => {
    setMode('reset');
    setForm({ ...EMPTY, ...employee, phoneNumber: employee.phoneNumber ?? '' });
  };

  const isSelf = (employee) => employee.userName.toLowerCase() === (currentUserName ?? '').toLowerCase();

  const onDelete = async (employee) => {
    if (!window.confirm(`Delete the account of ${employee.name} (${employee.userName})? They will no longer be able to sign in.`)) return;
    try {
      const base = await getBackendUrl();
      const response = await fetch(base + DeleteEmployee + employee.id, { method: 'DELETE' });
      if (!response.ok) throw new Error(await readError(response));
      notify('success', `${employee.name}'s account deleted`);
      if (form.id === employee.id) startAdd();
      loadEmployees();
    } catch (error) {
      notify('error', error.message);
    }
  };

  const onSubmit = async (event) => {
    event.preventDefault();
    if (mode !== 'reset' && (!form.name.trim() || !form.userName.trim())) {
      notify('warning', 'Enter the name and user name');
      return;
    }
    if (mode !== 'edit') {
      if (form.password.length < MIN_PASSWORD_LENGTH) {
        notify('warning', `The password must have at least ${MIN_PASSWORD_LENGTH} characters`);
        return;
      }
      if (form.password !== form.confirmPassword) {
        notify('warning', 'The two passwords are different');
        return;
      }
    }

    setSaving(true);
    try {
      const base = await getBackendUrl();
      const { confirmPassword, ...details } = form;
      const response = mode === 'reset'
        ? await postRequest(base + ResetEmployeePassword, { id: form.id, password: form.password })
        : await postRequest(base + (mode === 'add' ? CreateEmployee : UpdateEmployee), mode === 'add' ? details : { ...details, password: null });
      if (!response.ok) throw new Error(await readError(response));
      notify('success', mode === 'add' ? `Account for ${form.name.trim()} created`
        : mode === 'edit' ? `${form.name.trim()}'s account saved`
        : `${form.name}'s password changed. They are signed out everywhere.`);
      startAdd();
      loadEmployees();
    } catch (error) {
      notify('error', error.message);
    } finally {
      setSaving(false);
    }
  };

  const title = mode === 'add' ? 'Add account' : mode === 'edit' ? `Edit ${form.name}` : `New password for ${form.name}`;

  return (
    <div>
      <div className="card">
        <div className="card-header">
          <div>
            <h2 className="card-title">Accounts</h2>
            <p className="card-subtitle ss-flush">Everyone who can sign in. Admins can also manage these accounts.</p>
          </div>
        </div>
        <div className="staff-tableWrap">
          <table className="staff-table">
            <thead>
              <tr><th>Name</th><th>User name</th><th>Phone</th><th>Role</th><th></th></tr>
            </thead>
            <tbody>
              {employees.map(employee => (
                <tr key={employee.id} className={form.id === employee.id ? 'is-selected' : ''}>
                  <td>{employee.name}{isSelf(employee) && <span className="muted"> (you)</span>}</td>
                  <td>{employee.userName}</td>
                  <td>{employee.phoneNumber}</td>
                  <td><span className={`staff-role ${employee.isAdmin ? 'staff-role--admin' : ''}`}>{employee.isAdmin ? 'Admin' : 'Staff'}</span></td>
                  <td className="staff-actions">
                    <button className="btn-secondary" onClick={() => startEdit(employee)}>Edit</button>
                    {!isSelf(employee) && <button className="btn-secondary" onClick={() => startReset(employee)}>Reset password</button>}
                    {!isSelf(employee) && <button className="btn-danger" onClick={() => onDelete(employee)}>Delete</button>}
                  </td>
                </tr>
              ))}
              {employees.length === 0 && <tr><td colSpan={5} className="empty-state">No accounts loaded</td></tr>}
            </tbody>
          </table>
        </div>
      </div>

      <div className="card">
        <div className="card-header">
          <h2 className="card-title">{title}</h2>
          {mode !== 'add' && <button className="btn-secondary" onClick={startAdd}>Cancel</button>}
        </div>
        <form className="ss-form" onSubmit={onSubmit}>
          {mode !== 'reset' && (
            <>
              <div className="ss-field">
                <label htmlFor="staff-name">Name *</label>
                <input id="staff-name" type="text" value={form.name} onChange={onChange('name')} maxLength={100} />
              </div>
              <div className="ss-field">
                <label htmlFor="staff-userName">User name *</label>
                <input id="staff-userName" type="text" value={form.userName} onChange={onChange('userName')} maxLength={100} autoComplete="off" />
              </div>
              <div className="ss-field">
                <label htmlFor="staff-phone">Phone</label>
                <input id="staff-phone" type="tel" value={form.phoneNumber} onChange={onChange('phoneNumber')} maxLength={20} />
              </div>
              <label className="staff-admin">
                <input type="checkbox" checked={form.isAdmin} onChange={onChange('isAdmin')} />
                Admin: can add, change and delete accounts
              </label>
            </>
          )}
          {mode !== 'edit' && (
            <>
              <div className="ss-field">
                <label htmlFor="staff-password">{mode === 'reset' ? 'New password' : 'Password'} *</label>
                <input id="staff-password" type="password" value={form.password} onChange={onChange('password')} autoComplete="new-password" />
              </div>
              <div className="ss-field">
                <label htmlFor="staff-confirmPassword">Confirm password *</label>
                <input id="staff-confirmPassword" type="password" value={form.confirmPassword} onChange={onChange('confirmPassword')} autoComplete="new-password" />
              </div>
            </>
          )}
          <div className="ss-actions">
            <button type="submit" disabled={saving}>
              {saving ? 'Saving…' : mode === 'add' ? 'Add account' : mode === 'edit' ? 'Save changes' : 'Set password'}
            </button>
          </div>
        </form>
      </div>
    </div>
  );
};

export default Staff;
