import { AuthChangePassword, AuthLogin, AuthLogout, AuthSetup, AuthStatus } from '../APIEndpoints.js';
import { getBackendUrl, postRequest, readError } from './apiHelper.js';

// Fired when the server answers 401: the session has ended and the app shows the sign-in screen.
export const AUTH_REQUIRED_EVENT = 'smartbilling:auth-required';

let isFetchInstalled = false;

// Every request sends the sign-in cookie (in development the UI runs on another port than the API),
// and any 401 outside the sign-in calls themselves sends the user back to the sign-in screen.
export const installAuthFetch = () => {
  if (isFetchInstalled) return;
  isFetchInstalled = true;
  const nativeFetch = window.fetch.bind(window);
  window.fetch = async (input, init = {}) => {
    const response = await nativeFetch(input, { credentials: 'include', ...init });
    const url = typeof input === 'string' ? input : input?.url ?? '';
    if (response.status === 401 && !url.toLowerCase().includes('/auth/')) {
      window.dispatchEvent(new Event(AUTH_REQUIRED_EVENT));
    }
    return response;
  };
};

// { signedIn, userName, needsAdminPassword }
export const getAuthStatus = async () => {
  const base = await getBackendUrl();
  const response = await fetch(base + AuthStatus);
  if (!response.ok) throw new Error(await readError(response));
  return response.json();
};

const post = async (endpoint, body) => {
  const base = await getBackendUrl();
  const response = await postRequest(base + endpoint, body);
  if (!response.ok) throw new Error(await readError(response));
  return response.status === 204 ? null : response.json();
};

export const signIn = (userName, password) => post(AuthLogin, { userName, password });

// First start only: replaces the built-in System account's password.
export const setUpAdmin = (userName, password) => post(AuthSetup, { userName, password });

export const signOut = () => post(AuthLogout, {});

export const changePassword = (currentPassword, newPassword) => post(AuthChangePassword, { currentPassword, newPassword });
