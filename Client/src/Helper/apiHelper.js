let fileConfigPromise = null;
let configPromise = null;
let backendUrlPromise = null;

// config.json is optional. The desktop app and a server that hosts the UI serve the API from the
// same origin, so without it the API is assumed to be on this page's origin.
const getFileConfig = () => {
  if (!fileConfigPromise) {
    fileConfigPromise = fetch('/config.json')
      .then(response => {
        const isJson = (response.headers.get('content-type') ?? '').includes('json');
        return response.ok && isJson ? response.json() : {};
      })
      .catch(() => ({}));
  }
  return fileConfigPromise;
};

// Shop settings come from the server (entered on the Setting page). Values from an older config.json
// are kept as fallbacks so existing setups keep working until the settings are saved in the app.
export const getConfig = () => {
  if (!configPromise) {
    configPromise = Promise.all([getFileConfig(), getBackendUrl()])
      .then(([fileConfig, base]) => fetch(base + 'settings').then(response => {
        if (!response.ok) throw new Error('Failed to load shop settings');
        return response.json();
      }).then(settings => ({
        ...fileConfig,
        ...settings,
        shopName: settings.shopName || fileConfig.shopName || '',
        shopAddress: settings.shopAddress || fileConfig.shopAddress || '',
        shopGSTNumber: settings.shopGSTNumber || fileConfig.shopGSTNumber || '',
        BarcodeGenerateFilePath: settings.barcodeFilePath || fileConfig.BarcodeGenerateFilePath || '',
        isSetupComplete: settings.isSetupComplete || !!fileConfig.shopName,
      })));
    configPromise.catch(() => { configPromise = null; });
  }
  return configPromise;
};

// Call after saving settings so the next getConfig() reads them again.
export const refreshConfig = () => {
  configPromise = null;
  return getConfig();
};

export const getBackendUrl = () => {
  if (!backendUrlPromise) {
    backendUrlPromise = getFileConfig().then(data => {
      const configured = process.env.REACT_APP_ENVIRONMENT === 'PRODUCTION'
        ? data.backendUrl_PROD
        : data.backendUrl_DEV;
      const base = configured || window.location.origin;
      try {
        new URL(base);
      } catch {
        throw new Error(`Invalid backend URL: "${base}"`);
      }
      return base.replace(/\/$/, '') + '/api/';
    });
  }
  return backendUrlPromise;
};

export const fetchData = async (url) => {
  try {
    const response = await fetch(url);
    if (!response.ok) {
      throw new Error('Network response was not ok');
    }
    const data = await response.json();
    return data;
  } catch (error) {
    throw error;
  }
};

export const postRequest = async (url, bodyData) => {
  try {
    const response = await fetch(url, {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json'
      },
      body: JSON.stringify(bodyData)
    });

    return response;

    //   const data = await response.json();
    //   return data;
  } catch (error) {
    throw error;
  }
};

export const patchRequest = async (url, bodyData) => {
  try {
    const response = await fetch(url, {
      method: 'PATCH',
      headers: {
        'Content-Type': 'application/json'
      },
      body: JSON.stringify(bodyData)
    });

    return response;

    //   const data = await response.json();
    //   return data;
  } catch (error) {
    throw error;
  }
};
