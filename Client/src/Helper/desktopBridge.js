// Bridge to the SmartBilling desktop app (WPF + WebView2).
// In a normal browser window.chrome.webview is undefined and isDesktop() returns false.

const REQUEST_TIMEOUT_MS = 60000;

let nextRequestId = 1;
const pendingRequests = new Map();
let listening = false;

export const isDesktop = () => !!window.chrome?.webview;

const onDesktopMessage = (event) => {
  const reply = event.data;
  if (!reply || !pendingRequests.has(reply.requestId)) return;
  const { resolve, timer } = pendingRequests.get(reply.requestId);
  clearTimeout(timer);
  pendingRequests.delete(reply.requestId);
  resolve(reply);
};

export const sendToDesktop = (message) => {
  if (!isDesktop()) {
    return Promise.reject(new Error('Not running inside the SmartBilling desktop app'));
  }
  if (!listening) {
    window.chrome.webview.addEventListener('message', onDesktopMessage);
    listening = true;
  }

  const requestId = String(nextRequestId++);
  return new Promise((resolve, reject) => {
    const timer = setTimeout(() => {
      pendingRequests.delete(requestId);
      reject(new Error('The desktop app did not respond in time'));
    }, REQUEST_TIMEOUT_MS);
    pendingRequests.set(requestId, { resolve, timer });
    window.chrome.webview.postMessage({ ...message, requestId });
  });
};
