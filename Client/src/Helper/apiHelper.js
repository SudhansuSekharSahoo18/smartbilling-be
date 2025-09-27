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
