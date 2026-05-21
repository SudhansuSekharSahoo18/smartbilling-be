let configPromise = null;
let backendUrlPromise = null;

export const getConfig = () => {
  if (!configPromise) {
    configPromise = fetch('/config.json').then(response => {
      if (!response.ok) throw new Error('Failed to load config');
      return response.json();
    });
  }
  return configPromise;
};

export const getBackendUrl = () => {
  if (!backendUrlPromise) {
    backendUrlPromise = getConfig().then(data => {
      const base = process.env.REACT_APP_ENVIRONMENT === 'PRODUCTION'
        ? data.backendUrl_PROD
        : data.backendUrl_DEV;
      if (!base) {
        throw new Error(`Backend URL is not configured for environment: ${process.env.REACT_APP_ENVIRONMENT}`);
      }
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
