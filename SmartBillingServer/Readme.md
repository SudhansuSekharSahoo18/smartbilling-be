# Hosting dotnet project in IIS
Hosting your ASP.NET website in IIS | Host website on local network
https://www.youtube.com/watch?v=tWtx-G_JfJ0&ab_channel=Codingiva

## Steps

### Install dotnet hosting bundle

### Turn on windows feature on or off > enable all properties of IIS
  
### Create a website in IIS 
Site name: mywebsite.com
Physical path: C:\inetpub\wwwroot\<website_name>
Port: 80 (any)
Host name: mywebsite.com

### Run notepad in admin mode
open> C:\Windows\System32\drivers\etc\hosts and add
127.0.0.1 mywebsite.com

### Open Visual Studio in admin mode
Run the project if it is running or not (optional)
Select project and publish
Create a profile for publish
server: localhost
site name: mywebsite.com
desination URL: http://mywebsite.com

click on validate connection
Then click on Publish

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


# How to access site from different system.
### How To Open a port on IIS - Access from inside and outside network
https://www.youtube.com/watch?v=AaRc8048HB0&ab_channel=IntCoder


# EntityFramework

update-database

add-migration
remove-migration