import React, { useState } from 'react';
import '../Setting/Setting.css';
import { setUpAdmin, signIn } from '../../../Helper/auth.js';

const MIN_PASSWORD_LENGTH = 6;

// The sign-in screen. On the very first start (isFirstStart) it asks for the admin's user name and password instead.
const LoginView = ({ isFirstStart, onSignedIn }) => {
  const [userName, setUserName] = useState('');
  const [password, setPassword] = useState('');
  const [confirmPassword, setConfirmPassword] = useState('');
  const [error, setError] = useState('');
  const [busy, setBusy] = useState(false);

  const onSubmit = async (event) => {
    event.preventDefault();
    setError('');
    if (!userName.trim() || !password) {
      setError('Enter the user name and password.');
      return;
    }
    if (isFirstStart) {
      if (password.length < MIN_PASSWORD_LENGTH) {
        setError(`The password must have at least ${MIN_PASSWORD_LENGTH} characters.`);
        return;
      }
      if (password !== confirmPassword) {
        setError('The two passwords are different.');
        return;
      }
    }

    setBusy(true);
    try {
      const result = isFirstStart ? await setUpAdmin(userName.trim(), password) : await signIn(userName.trim(), password);
      setPassword('');
      setConfirmPassword('');
      onSignedIn(result);
    } catch (err) {
      setError(err.message);
    } finally {
      setBusy(false);
    }
  };

  return (
    <div className="setup-page">
      <div className="card setup-card login-card">
        <div className="setup-brand">
          <span className="brand-mark">SB</span>
          <h1>{isFirstStart ? 'Create the admin account' : 'Sign in to Smart Billing'}</h1>
        </div>
        {isFirstStart && (
          <p className="card-subtitle">Choose the user name and password used to sign in to Smart Billing on this PC and from other PCs. Keep the password safe: it is needed every time the app opens.</p>
        )}
        <form className="ss-form login-form" onSubmit={onSubmit}>
          <div className="ss-field ss-field--wide">
            <label htmlFor="login-userName">User name</label>
            <input id="login-userName" type="text" value={userName} onChange={e => setUserName(e.target.value)}
              autoComplete="username" maxLength={100} autoFocus />
          </div>
          <div className="ss-field ss-field--wide">
            <label htmlFor="login-password">{isFirstStart ? 'New password' : 'Password'}</label>
            <input id="login-password" type="password" value={password} onChange={e => setPassword(e.target.value)}
              autoComplete={isFirstStart ? 'new-password' : 'current-password'} />
          </div>
          {isFirstStart && (
            <div className="ss-field ss-field--wide">
              <label htmlFor="login-confirmPassword">Confirm password</label>
              <input id="login-confirmPassword" type="password" value={confirmPassword} onChange={e => setConfirmPassword(e.target.value)}
                autoComplete="new-password" />
            </div>
          )}
          {error && <p className="login-error" role="alert">{error}</p>}
          <div className="ss-actions">
            <button type="submit" disabled={busy}>
              {busy ? 'Please wait…' : isFirstStart ? 'Create account and sign in' : 'Sign in'}
            </button>
          </div>
        </form>
      </div>
    </div>
  );
};

export default LoginView;
