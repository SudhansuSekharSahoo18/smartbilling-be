## For connection issue

An unhandled exception occurred while processing the request.
SqlException: Cannot open database "SB_NandiniFashion" requested by the login. The login failed.
Login failed for user 'IIS APPPOOL\demo'.

-- Step 1: Create the SQL Server Login
IF NOT EXISTS (SELECT name FROM sys.server_principals WHERE name = N'IIS APPPOOL\demo')
BEGIN
    CREATE LOGIN [IIS APPPOOL\demo] FROM WINDOWS;
END
GO

-- Step 2: Create the Database User
USE [SB_NandiniFashion];
GO
IF NOT EXISTS (SELECT name FROM sys.database_principals WHERE name = N'IIS APPPOOL\demo')
BEGIN
    CREATE USER [IIS APPPOOL\demo] FOR LOGIN [IIS APPPOOL\demo];
END
GO

-- Step 3: Assign Roles to the User
EXEC sp_addrolemember N'db_owner', N'IIS APPPOOL\demo';
-- Alternatively, if you prefer more restricted access:
-- EXEC sp_addrolemember N'db_datareader', N'IIS APPPOOL\demo';
-- EXEC sp_addrolemember N'db_datawriter', N'IIS APPPOOL\demo';
GO

