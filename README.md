# SmartBilling

SmartBilling is a billing / point-of-sale application for a retail shop. It covers stock (items), billing (POS), saved bills, barcode generation, sales reports (including an HSN-wise tax summary) and a dashboard with sales graphs.

The repository contains:

| Folder | What it is | Tech |
|---|---|---|
| [SmartBillingServer/](SmartBillingServer/) | REST API (and host for the built React UI) | ASP.NET Core Web API, .NET 8 |
| [DataAccess/](DataAccess/) | EF Core `DbContext`, migrations, repositories | EF Core 8 + SQLite |
| [Models/](Models/) | Entity models and DTOs | .NET 8 class library |
| [Utility/](Utility/) | Shared helpers / constants | .NET 8 class library |
| [SmartBilling.Tests/](SmartBilling.Tests/) | Unit tests | xUnit |
| [SmartBilling.Desktop/](SmartBilling.Desktop/) | Desktop app: the web UI in a window, plus silent barcode label printing | WPF + WebView2, .NET 8 (Windows only) |
| [Client/](Client/) | Web front end | React 18 (Create React App), ag-grid, recharts |

All .NET projects are in [SmartBillingServer.sln](SmartBillingServer.sln).

---

## Single exe for one PC (quick start)

For a shop with one billing PC, give the user one file, `SmartBilling.exe`. It contains the .NET runtime, the API, the SQLite engine and the web app, so nothing else has to be installed or configured. The only exception is the Microsoft Edge WebView2 Runtime, which is preinstalled on Windows 10/11.

Build it (needs the .NET 8 SDK and Node.js on the build PC):

```bash
dotnet publish SmartBilling.Desktop -p:PublishProfile=SingleExe
# -> SmartBilling.Desktop/bin/publish/SmartBilling.exe  (about 90 MB)
```

On the user's PC:

1. Copy `SmartBilling.exe` anywhere, for example the Desktop, and run it.
2. On first run, the app creates its database and asks for the shop details: name, address, GSTIN and the barcode export file. You can change them later on the **Setting** page.
3. The first time you print labels, the app asks which printer is the label printer. You can change it later on **Setting** or the Barcode page.

| What | Where |
|---|---|
| Database | `%LOCALAPPDATA%\SmartBilling\SmartBilling.db`. Back up this file. To move existing data in, copy a `SmartBilling.db` here while the app is closed. |
| Desktop settings (label printer, optional server URL) | `%LOCALAPPDATA%\SmartBilling\desktopsettings.json` |
| Error logs | `%LOCALAPPDATA%\SmartBilling\Logs` |
| Barcode export (default) | `Documents\SmartBilling\Barcode.csv` |

Only one copy of the app runs at a time. Starting it again brings the open window to the front. The built-in server listens on `127.0.0.1` only, on a free port, so other PCs can't reach it. For several PCs sharing one database, use the server deployment instead (section 6) and enter its address in the desktop app.

---

## 1. Prerequisites

- **Windows** — the server launch code uses `cmd` to open the browser, the SPA proxy uses `npm.cmd`, and the desktop project is WPF.
- **.NET 8 SDK** (8.0.300 or later) — check with `dotnet --list-sdks`
- **Node.js 18+ and npm** (developed with Node 20) — check with `node --version`
- *(Optional)* **Visual Studio 2022** with the "ASP.NET and web development" workload
- *(Optional)* **EF Core CLI**, for database migrations: `dotnet tool install --global dotnet-ef`
- *(Optional, for HTTPS locally)* trust the dev certificate once: `dotnet dev-certs https --trust`

---

## 2. One-time setup

### 2.1 Clone and restore

```bash
git clone <repo-url>
cd smartbilling-be
dotnet restore SmartBillingServer.sln
cd Client
npm install
```

> In Debug builds, building `SmartBillingServer` also runs `npm install` in `Client/` automatically, and fails if Node.js is missing.

### 2.2 Client config file (development only)

The React app reads the API address from `Client/public/config.json`, which is git-ignored. You need it for `npm start` (Option C), because the dev server on port 3000 isn't the API. When the API serves the UI (the desktop exe, or a production build in `wwwroot`), the file is optional: without it the UI calls the API on its own origin.

Shop name, address and GSTIN are entered in the app (first-run setup / **Setting** page) and stored in the database. The `shopName`, `shopAddress`, `shopGSTNumber` and `BarcodeGenerateFilePath` keys below are only fallbacks for older setups, used until settings are saved in the app.

`Client/public/config.json`:

```json
{
  "backendUrl_DEV": "https://localhost:7271",
  "backendUrl_PROD": "http://localhost:5000",
  "BarcodeGenerateFilePath": "C:\\SmartBilling\\Barcode.csv",
  "shopName": "My Shop",
  "shopAddress": "Shop address line",
  "shopGSTNumber": "GSTIN0000000000"
}
```

| Key | Purpose |
|---|---|
| `backendUrl_DEV` | API base URL used when `REACT_APP_ENVIRONMENT` is not `PRODUCTION`. `/api/` is appended automatically. |
| `backendUrl_PROD` | API base URL used when `REACT_APP_ENVIRONMENT=PRODUCTION`. |
| `BarcodeGenerateFilePath` | Fallback for the barcode export file (Setting page). The folder is created if it doesn't exist. |
| `shopName`, `shopAddress`, `shopGSTNumber` | Fallbacks for the shop details (Setting page). |

Which URL is used depends on `REACT_APP_ENVIRONMENT`, which [Client/.env](Client/.env) sets to `DEVELOPMENT`.

> Use the HTTPS URL (`https://localhost:7271`) for `backendUrl_DEV` when the API runs with the `https` profile. The API calls `UseHttpsRedirection`, so requests to the HTTP port get redirected, which breaks CORS preflight requests.

### 2.3 Server configuration

[SmartBillingServer/appsettings.Development.json](SmartBillingServer/appsettings.Development.json) is committed and is used when `ASPNETCORE_ENVIRONMENT=Development`:

```json
"ConnectionStrings": { "DefaultConnection": "Data Source=SmartBilling.db" },
"ConfigSettings":   { "BarcodeGenerationFilePath": "" }
```

`SmartBillingServer/appsettings.json` is **git-ignored** and optional. Without it, the database is `SmartBilling.db` in the content root. Create it (you can copy `appsettings.Development.json`) to use a different connection string in a production or IIS deployment. The desktop exe doesn't use these files.

### 2.4 Database (SQLite)

- The app uses SQLite. The connection string `Data Source=SmartBilling.db` is relative to the process's **working directory**. With `dotnet run` / Visual Studio that is `SmartBillingServer/`, where a database file [SmartBillingServer/SmartBilling.db](SmartBillingServer/SmartBilling.db) is already committed.
- The server **applies pending migrations on startup**. If the database file doesn't exist yet, it is created. Back up `SmartBilling.db` before deploying a version that contains new migrations.
- `dotnet-ef` is installed as a repo-local tool. Run `dotnet tool restore` once after cloning. To apply migrations without starting the server, run:

  ```bash
  # from the repo root
  dotnet ef database update --project DataAccess --startup-project SmartBillingServer
  ```

  In the Visual Studio Package Manager Console (default project = `DataAccess`): `Update-Database`.

- Other migration commands:

  ```bash
  dotnet ef migrations add <Name> --project DataAccess --startup-project SmartBillingServer
  dotnet ef migrations remove    --project DataAccess --startup-project SmartBillingServer
  # Generate a full SQL script (PMC: Script-Migration -Output DataAccess\Script\full-database.sql)
  dotnet ef migrations script -o DataAccess/Script/full-database.sql --project DataAccess --startup-project SmartBillingServer
  ```

You can check database connectivity at any time with `GET /api/health/db` (see [Health checks](#5-health-checks)).

---

## 3. Running in development

### Option A — Visual Studio (API and UI together)

1. Open `SmartBillingServer.sln`.
2. Set **SmartBillingServer** as the startup project.
3. Choose the **`https`** (or `http`) launch profile and press **F5**.

The `https` / `http` profiles enable the ASP.NET Core **SPA proxy**, which runs `npm start` in `Client/` for you.

| What | URL |
|---|---|
| React UI | http://localhost:3000 |
| API (https profile) | https://localhost:7271 (also http://localhost:5242) |
| API (http profile) | http://localhost:5242 |
| Swagger UI | https://localhost:7271/swagger |

### Option B — Command line (API and UI together)

```bash
# from the repo root
dotnet run --project SmartBillingServer --launch-profile https
```

This starts the API on https://localhost:7271 and http://localhost:5242, and the SPA proxy starts the React dev server on http://localhost:3000. On startup the server also opens `https://localhost:7271` in your browser.

### Option C — Run API and UI separately

Terminal 1, the API:

```bash
dotnet run --project SmartBillingServer --launch-profile https
```

Terminal 2, the UI:

```bash
cd Client
npm start
```

Then open http://localhost:3000. CORS on the API allows any origin, so the React dev server can call it directly.

### Desktop app

`SmartBilling.Desktop` shows the web app in a Windows window, using WebView2. By default it runs `SmartBillingServer` inside its own process (the built-in server), with the React build embedded in the exe, so a single PC needs nothing else. See [Single exe for one PC](#single-exe-for-one-pc-quick-start). It can also connect to a shared `SmartBillingServer` so several PCs use one database. Inside the desktop app, the Barcode page can print labels straight to a label printer, without a print dialog.

**Requirements:** Windows 10/11 with the Microsoft Edge WebView2 Runtime, which is preinstalled on most PCs. If it's missing, the app shows a download link.

**Server address.** `ServerUrl` in `%LOCALAPPDATA%\SmartBilling\desktopsettings.json` picks the server.
- Empty (the default): the built-in server. It is started on `127.0.0.1` with a free port, and the database is `%LOCALAPPDATA%\SmartBilling\SmartBilling.db`.
- A URL: that server. If it can't be reached, the app shows a screen where you can edit the URL (or clear it to use the built-in server) and click **Save & Retry**.

| Scenario | `ServerUrl` |
|---|---|
| One PC (single exe) | empty |
| Development (API + `npm start` running, see Option C) | `http://localhost:3000` |
| Server on another PC on the LAN | `http://<server-ip>:<port>`, for example `http://192.168.1.10:5000` |

For the LAN case, the server must serve the React build. See section 6.

```bash
# run from source. The built-in server needs Client/build: run "npm run build" in Client/ first,
# or build in Release (-c Release), which builds the client for you.
dotnet run --project SmartBilling.Desktop

# the single exe (see "Single exe for one PC")
dotnet publish SmartBilling.Desktop -p:PublishProfile=SingleExe
```

**Barcode label printing** (Barcode page):
- **Print Labels** prints one label per unit of quantity, using the saved label design. For example, quantity 3 gives 3 labels.
- The first time, the app asks you to choose the label printer. It remembers the choice. To change it later, use **Label printer…**.
- In a normal browser, **Print Labels** opens the browser print dialog instead.
- The desktop app draws the labels itself and prints them with Windows printing. It sends the exact page size from the design to the driver: one row of labels per page, portrait. It doesn't print the HTML page, because Chromium rotates pages that are wider than they are tall by 90°. Bars are placed on whole printer dots so they scan reliably.
- Label printer setup: install the printer's Windows driver, choose gap/notch labels, and calibrate the printer once. On the Label Design page, set the label size, labels per row and gap to match the roll exactly.

### Label Design page

The **Label Design** page sets what goes on a barcode label. The design is saved on the server, so every PC and browser prints the same labels.

- **Label size**:
  - **50 × 25 mm**: one label per row.
  - **2 per row, 38 × 25 mm**: two labels side by side, with a 2 mm gap.
  - **Custom size…**: any width and height, 1–4 labels per row, and the gap between them.
  - When you change the size, the existing elements scale to match.
- **Add to label**: Barcode, Item code, Item name, Price, Shop name, or Custom text.
- **Moving and resizing**:
  - Drag an element to move it. Drag its blue corner to resize it. Positions snap to 0.5 mm.
  - Arrow keys nudge the selected element (hold Shift for 2 mm steps). Delete removes it.
- **Properties** (for the selected element):
  - which value it shows;
  - text before the value, such as `MRP: `;
  - font size, bold, alignment;
  - exact position and size in mm.
- **Preview item**: change the sample code, name and price to check that long values fit.
- **Buttons**:
  - **Save design** saves the design for everyone.
  - **Undo changes** returns to the last saved design.
  - **Reset layout** restores the default layout at the current size.

The barcode is CODE128 and stretches to fill its box. The item code text under it is a separate element that you can move or remove.

The design is stored as JSON in the `LabelDesigns` table (`GET /api/labelDesign`, `POST /api/labelDesign/save`). Until a design is saved, the default layout is used.

---

## 4. Running tests

```bash
# .NET unit tests (xUnit)
dotnet test SmartBillingServer.sln

# React tests (Jest, interactive watch mode)
cd Client
npm test
```

---

## 5. Health checks

| Endpoint | Checks |
|---|---|
| `GET /api/health` | The API is running |
| `GET /api/health/db` | The SQLite database can be opened |
| `GET /api/health/config` | The shop details have been entered; also shows the database file in use |

Shop settings: `GET /api/settings`, `POST /api/settings/save`.

All other endpoints are listed in Swagger at `/swagger`. The client-side routes are in [Client/src/APIEndpoints.js](Client/src/APIEndpoints.js).

---

## 6. Production build and deployment (IIS)

The API serves the built React app from `wwwroot/` (`UseStaticFiles` + `MapFallbackToFile("index.html")`), so you deploy a single site.

1. **Build the React app for production**

   ```bash
   cd Client
   set REACT_APP_ENVIRONMENT=PRODUCTION && npm run build
   ```

   You can instead create `Client/.env.production.local` containing `REACT_APP_ENVIRONMENT=PRODUCTION`. That file overrides `.env` during `npm run build`.

2. **Copy the build into the API.** Copy the contents of `Client/build/` into `SmartBillingServer/wwwroot/`, without `config.json`. The UI then calls the API on the address it was loaded from.

3. *(Optional)* **Create `SmartBillingServer/appsettings.json`** to change the connection string (section 2.3). Use an absolute path in the SQLite `Data Source` if the site's working directory differs from the publish folder.

4. **Publish the API**

   ```bash
   dotnet publish SmartBillingServer -c Release -o ./publish
   ```

   Or in Visual Studio: right-click **SmartBillingServer → Publish**.

5. **Host in IIS**
   - Install the **ASP.NET Core 8 Hosting Bundle**.
   - Enable IIS under *Turn Windows features on or off*.
   - Install the IIS **URL Rewrite** module. The React client uses it for deep links; see [Client/public/web.config](Client/public/web.config) and [Client/Notes.md](Client/Notes.md).
   - Create a site whose physical path is the publish folder, for example `C:\inetpub\wwwroot\smartbilling`, and bind it to a port.
   - Give the app-pool identity (`IIS APPPOOL\<pool-name>`) **write access** to the folder that contains `SmartBilling.db`, and to the barcode CSV folder.
   - To reach the site from other machines on the LAN, open the site's port in Windows Firewall.

   More notes are in [SmartBillingServer/Readme.md](SmartBillingServer/Readme.md).

Outside Development, the server opens `http://localhost:5000` on startup. Swagger is also enabled in Production.

---

## 7. Troubleshooting

| Problem | Fix |
|---|---|
| `Node.js is required to build and run this project.` | Install Node.js and make sure `node` is on your `PATH`. |
| UI shows "Failed to load shop settings" with `npm start` | `Client/public/config.json` is missing, so the UI calls port 3000 instead of the API. See section 2.2. |
| `localhost:3000/<route>` returns 404 under IIS | Install the IIS URL Rewrite module and keep `web.config` next to `index.html`. |
| Barcode export fails with a path error | Check the barcode export file on the Setting page. It must be a full file path that the app can write to. |
| Desktop app says "This build of Smart Billing doesn't contain the web app" | The exe was built in Debug without `Client/build`. Publish with the SingleExe profile, or run `npm run build` in `Client/` and rebuild. |
| Desktop app says "Smart Billing could not start" | The message names a log file in `%LOCALAPPDATA%\SmartBilling\Logs`. A common cause is a database file from a newer version. |
| `no such table` or other SQLite errors | Run `dotnet ef database update ...` (section 2.4). Check that the working directory contains the right `SmartBilling.db`. |
| HTTPS certificate warnings locally, or `Unable to configure HTTPS endpoint` on startup | Run `dotnet dev-certs https --trust`. Or use `--launch-profile http` and set `backendUrl_DEV` to `http://localhost:5242`. |
| `Couldn't start the SPA development server with command 'npm.cmd start'` | Start the UI yourself with `cd Client && npm start` (Option C). |
| `'react-scripts' is not recognized` | Run `npm install` in `Client/`. |
| Desktop app shows "Cannot reach the SmartBilling server" | Start the server and check `ServerUrl` (see "Desktop app"). Edit the URL on that screen and click **Save & Retry**. |
| Labels print in the wrong place, across label gaps, or with blank labels in between | Check the label size, labels per row and gap on the Label Design page against the real roll (measure it). Turn on gap sensing in the printer driver and calibrate the printer. If the success message says the printer is using a different paper size, the driver doesn't accept the custom size: set the label size in the driver's preferences instead. |
| Startup crashes | The exception message is written to `Logs/log<timestamp>.txt` under the working directory. |
