// Item
export const GetAllItems = 'item/Get';
export const CreateItem = 'item/create';
export const UpdateItem = 'item/Update';
export const DeleteItem = 'item/Delete';
export const AddToBarcode = 'item/AddToBarcode';
export const Export = 'item/Export';
export const Import = 'item/Import';

// Bill
export const GetAllBill = 'bill/Get';
export const CreateBill = 'bill/create';
export const ReturnBillItem = 'bill/ReturnItem';
export const DeleteBill = 'bill/delete/';
export const GetTotalSaleByDate = 'bill/GetTotalSaleByDate';
export const GetMonthlySales = 'bill/GetMonthlySales';
export const GetDailySales = 'bill/GetDailySales';
export const GenerateSaleReport = 'bill/GenerateSaleReport';
export const GetTaxSummaryReport = 'bill/GetTaxSummaryReport';

// Barcode
export const GetBarcodeList = 'barcode';
export const GenerateBarcode = 'Barcode/GenerateBarcode';
export const DeleteBarcode = 'Barcode/Delete/';

// Label design
export const GetLabelDesign = 'labelDesign';
export const SaveLabelDesign = 'labelDesign/save';

// Shop settings
export const GetSettings = 'settings';
export const SaveSettings = 'settings/save';

// Sign-in
export const AuthStatus = 'auth/status';
export const AuthLogin = 'auth/login';
export const AuthSetup = 'auth/setup';
export const AuthLogout = 'auth/logout';
export const AuthChangePassword = 'auth/changePassword';

// Staff accounts (admins only)
export const GetEmployees = 'employee';
export const CreateEmployee = 'employee/create';
export const UpdateEmployee = 'employee/update';
export const ResetEmployeePassword = 'employee/resetPassword';
export const DeleteEmployee = 'employee/delete/';

// Health
export const HealthApi = 'health';
export const HealthDb = 'health/db';
export const HealthConfig = 'health/config';
