IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;
GO

BEGIN TRANSACTION;
GO

CREATE TABLE [ApplicationConfiguration] (
    [Id] int NOT NULL IDENTITY,
    [Barcode] int NOT NULL,
    [Version] nvarchar(max) NULL,
    CONSTRAINT [PK_ApplicationConfiguration] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [Bills] (
    [Id] int NOT NULL IDENTITY,
    [SubTotal] float NOT NULL,
    [DiscountAmount] float NOT NULL,
    [TotalAmount] float NOT NULL,
    [ModeOfPayment] nvarchar(max) NOT NULL,
    [CustomerName] nvarchar(max) NOT NULL,
    [CustomerAddress] nvarchar(max) NOT NULL,
    [CreatedById] int NOT NULL,
    [UpdatedById] int NULL,
    [CreatedDateTime] datetime2 NOT NULL,
    [UpdatedDateTime] datetime2 NULL,
    CONSTRAINT [PK_Bills] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [Categories] (
    [Id] int NOT NULL IDENTITY,
    [Name] nvarchar(30) NOT NULL,
    [DisplayOrder] int NOT NULL,
    CONSTRAINT [PK_Categories] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [Customers] (
    [Id] int NOT NULL IDENTITY,
    [Name] nvarchar(max) NULL,
    [Address] nvarchar(max) NULL,
    [PhoneNumber] nvarchar(max) NOT NULL,
    [Due] float NOT NULL,
    [CreatedById] int NOT NULL,
    [UpdatedById] int NULL,
    [CreatedDateTime] datetime2 NOT NULL,
    [UpdatedDateTime] datetime2 NULL,
    CONSTRAINT [PK_Customers] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [Employees] (
    [Id] int NOT NULL IDENTITY,
    [Name] nvarchar(max) NOT NULL,
    [UserName] nvarchar(max) NOT NULL,
    [PhoneNumber] nvarchar(max) NOT NULL,
    [Password] nvarchar(max) NOT NULL,
    [IsAdmin] bit NOT NULL,
    [CreatedById] int NOT NULL,
    [UpdatedById] int NULL,
    [CreatedDateTime] datetime2 NOT NULL,
    [UpdatedDateTime] datetime2 NULL,
    CONSTRAINT [PK_Employees] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [BillItem] (
    [Id] int NOT NULL IDENTITY,
    [ItemId] int NOT NULL,
    [BillId] int NOT NULL,
    [Barcode] nvarchar(max) NULL,
    [ItemName] nvarchar(max) NULL,
    [Quantity] int NOT NULL,
    [Price] float NOT NULL,
    [DiscountAmount] float NOT NULL,
    [Amount] float NOT NULL,
    CONSTRAINT [PK_BillItem] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_BillItem_Bills_BillId] FOREIGN KEY ([BillId]) REFERENCES [Bills] ([Id]) ON DELETE CASCADE
);
GO

CREATE TABLE [Items] (
    [Id] int NOT NULL IDENTITY,
    [Barcode] nvarchar(max) NOT NULL,
    [ItemName] nvarchar(30) NOT NULL,
    [Description] nvarchar(max) NULL,
    [Unit] nvarchar(max) NOT NULL,
    [Quantity] int NOT NULL,
    [CostPrice] float NOT NULL,
    [SellPrice] float NOT NULL,
    [Tax] float NOT NULL,
    [CategoryId] int NULL,
    [CreatedById] int NOT NULL,
    [UpdatedById] int NULL,
    [CreatedDateTime] datetime2 NOT NULL,
    [UpdatedDateTime] datetime2 NULL,
    CONSTRAINT [PK_Items] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Items_Categories_CategoryId] FOREIGN KEY ([CategoryId]) REFERENCES [Categories] ([Id])
);
GO

IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Barcode', N'Version') AND [object_id] = OBJECT_ID(N'[ApplicationConfiguration]'))
    SET IDENTITY_INSERT [ApplicationConfiguration] ON;
INSERT INTO [ApplicationConfiguration] ([Id], [Barcode], [Version])
VALUES (1, 101, N'1.0.0');
IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Barcode', N'Version') AND [object_id] = OBJECT_ID(N'[ApplicationConfiguration]'))
    SET IDENTITY_INSERT [ApplicationConfiguration] OFF;
GO

IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'CreatedById', N'CreatedDateTime', N'CustomerAddress', N'CustomerName', N'DiscountAmount', N'ModeOfPayment', N'SubTotal', N'TotalAmount', N'UpdatedById', N'UpdatedDateTime') AND [object_id] = OBJECT_ID(N'[Bills]'))
    SET IDENTITY_INSERT [Bills] ON;
INSERT INTO [Bills] ([Id], [CreatedById], [CreatedDateTime], [CustomerAddress], [CustomerName], [DiscountAmount], [ModeOfPayment], [SubTotal], [TotalAmount], [UpdatedById], [UpdatedDateTime])
VALUES (1, 0, '0001-01-01T00:00:00.0000000', N'NA', N'default', 0.0E0, N'Cash', 2000.0E0, 2000.0E0, NULL, NULL);
IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'CreatedById', N'CreatedDateTime', N'CustomerAddress', N'CustomerName', N'DiscountAmount', N'ModeOfPayment', N'SubTotal', N'TotalAmount', N'UpdatedById', N'UpdatedDateTime') AND [object_id] = OBJECT_ID(N'[Bills]'))
    SET IDENTITY_INSERT [Bills] OFF;
GO

IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'DisplayOrder', N'Name') AND [object_id] = OBJECT_ID(N'[Categories]'))
    SET IDENTITY_INSERT [Categories] ON;
INSERT INTO [Categories] ([Id], [DisplayOrder], [Name])
VALUES (1, 1, N'Action'),
(2, 2, N'SciFi'),
(3, 3, N'History');
IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'DisplayOrder', N'Name') AND [object_id] = OBJECT_ID(N'[Categories]'))
    SET IDENTITY_INSERT [Categories] OFF;
GO

IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'CreatedById', N'CreatedDateTime', N'IsAdmin', N'Name', N'Password', N'PhoneNumber', N'UpdatedById', N'UpdatedDateTime', N'UserName') AND [object_id] = OBJECT_ID(N'[Employees]'))
    SET IDENTITY_INSERT [Employees] ON;
INSERT INTO [Employees] ([Id], [CreatedById], [CreatedDateTime], [IsAdmin], [Name], [Password], [PhoneNumber], [UpdatedById], [UpdatedDateTime], [UserName])
VALUES (1, 1, '2024-06-30T19:13:04.5774027+05:30', CAST(1 AS bit), N'System', N'12345', N'1234567890', NULL, NULL, N'System');
IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'CreatedById', N'CreatedDateTime', N'IsAdmin', N'Name', N'Password', N'PhoneNumber', N'UpdatedById', N'UpdatedDateTime', N'UserName') AND [object_id] = OBJECT_ID(N'[Employees]'))
    SET IDENTITY_INSERT [Employees] OFF;
GO

IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Amount', N'Barcode', N'BillId', N'DiscountAmount', N'ItemId', N'ItemName', N'Price', N'Quantity') AND [object_id] = OBJECT_ID(N'[BillItem]'))
    SET IDENTITY_INSERT [BillItem] ON;
INSERT INTO [BillItem] ([Id], [Amount], [Barcode], [BillId], [DiscountAmount], [ItemId], [ItemName], [Price], [Quantity])
VALUES (1, 500.0E0, N'101', 1, 0.0E0, 0, N'Shirt', 500.0E0, 1),
(2, 500.0E0, N'102', 1, 0.0E0, 0, N'T-Shirt', 500.0E0, 1),
(3, 500.0E0, N'103', 1, 0.0E0, 0, N'Jeans', 500.0E0, 1),
(4, 500.0E0, N'104', 1, 0.0E0, 0, N'T-Shirt', 500.0E0, 1);
IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Amount', N'Barcode', N'BillId', N'DiscountAmount', N'ItemId', N'ItemName', N'Price', N'Quantity') AND [object_id] = OBJECT_ID(N'[BillItem]'))
    SET IDENTITY_INSERT [BillItem] OFF;
GO

IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Barcode', N'CategoryId', N'CostPrice', N'CreatedById', N'CreatedDateTime', N'Description', N'ItemName', N'Quantity', N'SellPrice', N'Tax', N'Unit', N'UpdatedById', N'UpdatedDateTime') AND [object_id] = OBJECT_ID(N'[Items]'))
    SET IDENTITY_INSERT [Items] ON;
INSERT INTO [Items] ([Id], [Barcode], [CategoryId], [CostPrice], [CreatedById], [CreatedDateTime], [Description], [ItemName], [Quantity], [SellPrice], [Tax], [Unit], [UpdatedById], [UpdatedDateTime])
VALUES (1, N'101', 1, 300.0E0, 0, '0001-01-01T00:00:00.0000000', N'', N'Saree', 1, 500.0E0, 0.0E0, N'Pieces', NULL, NULL),
(2, N'102', 2, 300.0E0, 0, '0001-01-01T00:00:00.0000000', N'', N'Jeans', 1, 1500.0E0, 0.0E0, N'Pieces', NULL, NULL),
(3, N'103', 2, 300.0E0, 0, '0001-01-01T00:00:00.0000000', N'', N'Shirt', 1, 400.0E0, 0.0E0, N'Pieces', NULL, NULL),
(4, N'104', 1, 300.0E0, 0, '0001-01-01T00:00:00.0000000', N'', N'Socks', 1, 150.0E0, 0.0E0, N'Pieces', NULL, NULL),
(5, N'105', 3, 300.0E0, 0, '0001-01-01T00:00:00.0000000', N'', N'Lungi', 1, 80.0E0, 0.0E0, N'Pieces', NULL, NULL),
(6, N'106', 1, 300.0E0, 0, '0001-01-01T00:00:00.0000000', N'', N'Frock', 1, 345.0E0, 0.0E0, N'Pieces', NULL, NULL),
(7, N'107', 3, 300.0E0, 0, '0001-01-01T00:00:00.0000000', N'', N'Lengha', 1, 800.0E0, 0.0E0, N'Pieces', NULL, NULL),
(8, N'108', 1, 300.0E0, 0, '0001-01-01T00:00:00.0000000', N'', N'Gamcha', 1, 120.0E0, 0.0E0, N'Pieces', NULL, NULL),
(9, N'109', 2, 300.0E0, 0, '0001-01-01T00:00:00.0000000', N'', N'Shoes', 1, 700.0E0, 0.0E0, N'Pieces', NULL, NULL),
(10, N'110', 1, 300.0E0, 0, '0001-01-01T00:00:00.0000000', N'', N'Cap', 1, 50.0E0, 0.0E0, N'Pieces', NULL, NULL);
IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Barcode', N'CategoryId', N'CostPrice', N'CreatedById', N'CreatedDateTime', N'Description', N'ItemName', N'Quantity', N'SellPrice', N'Tax', N'Unit', N'UpdatedById', N'UpdatedDateTime') AND [object_id] = OBJECT_ID(N'[Items]'))
    SET IDENTITY_INSERT [Items] OFF;
GO

CREATE INDEX [IX_BillItem_BillId] ON [BillItem] ([BillId]);
GO

CREATE INDEX [IX_Items_CategoryId] ON [Items] ([CategoryId]);
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20240630134305_InitAndSeededDB', N'8.0.5');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

ALTER TABLE [Items] ADD [HSNCode] nvarchar(max) NULL;
GO

ALTER TABLE [Items] ADD [TaxInclusive] bit NOT NULL DEFAULT CAST(0 AS bit);
GO

UPDATE [Employees] SET [CreatedDateTime] = '2024-07-01T21:43:14.0895660+05:30'
WHERE [Id] = 1;
SELECT @@ROWCOUNT;

GO

UPDATE [Items] SET [HSNCode] = NULL, [TaxInclusive] = CAST(0 AS bit)
WHERE [Id] = 1;
SELECT @@ROWCOUNT;

GO

UPDATE [Items] SET [HSNCode] = NULL, [TaxInclusive] = CAST(0 AS bit)
WHERE [Id] = 2;
SELECT @@ROWCOUNT;

GO

UPDATE [Items] SET [HSNCode] = NULL, [TaxInclusive] = CAST(0 AS bit)
WHERE [Id] = 3;
SELECT @@ROWCOUNT;

GO

UPDATE [Items] SET [HSNCode] = NULL, [TaxInclusive] = CAST(0 AS bit)
WHERE [Id] = 4;
SELECT @@ROWCOUNT;

GO

UPDATE [Items] SET [HSNCode] = NULL, [TaxInclusive] = CAST(0 AS bit)
WHERE [Id] = 5;
SELECT @@ROWCOUNT;

GO

UPDATE [Items] SET [HSNCode] = NULL, [TaxInclusive] = CAST(0 AS bit)
WHERE [Id] = 6;
SELECT @@ROWCOUNT;

GO

UPDATE [Items] SET [HSNCode] = NULL, [TaxInclusive] = CAST(0 AS bit)
WHERE [Id] = 7;
SELECT @@ROWCOUNT;

GO

UPDATE [Items] SET [HSNCode] = NULL, [TaxInclusive] = CAST(0 AS bit)
WHERE [Id] = 8;
SELECT @@ROWCOUNT;

GO

UPDATE [Items] SET [HSNCode] = NULL, [TaxInclusive] = CAST(0 AS bit)
WHERE [Id] = 9;
SELECT @@ROWCOUNT;

GO

UPDATE [Items] SET [HSNCode] = NULL, [TaxInclusive] = CAST(0 AS bit)
WHERE [Id] = 10;
SELECT @@ROWCOUNT;

GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20240701161315_addedHSNCode', N'8.0.5');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

EXEC sp_rename N'[Items].[TaxInclusive]', N'IsTaxInclusive', N'COLUMN';
GO

UPDATE [Employees] SET [CreatedDateTime] = '2024-07-01T22:18:11.6938095+05:30'
WHERE [Id] = 1;
SELECT @@ROWCOUNT;

GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20240701164813_UpdateTaxInclusiveToIsTaxInclusive', N'8.0.5');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

CREATE TABLE [Barcodes] (
    [Id] int NOT NULL IDENTITY,
    [ItemCode] nvarchar(30) NOT NULL,
    [ItemName] nvarchar(30) NOT NULL,
    [Price] int NOT NULL,
    [Quantity] int NOT NULL,
    CONSTRAINT [PK_Barcodes] PRIMARY KEY ([Id])
);
GO

IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'ItemCode', N'ItemName', N'Price', N'Quantity') AND [object_id] = OBJECT_ID(N'[Barcodes]'))
    SET IDENTITY_INSERT [Barcodes] ON;
INSERT INTO [Barcodes] ([Id], [ItemCode], [ItemName], [Price], [Quantity])
VALUES (1, N'101', N'Barcode 1', 100, 1),
(2, N'102', N'Barcode 2', 200, 1);
IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'ItemCode', N'ItemName', N'Price', N'Quantity') AND [object_id] = OBJECT_ID(N'[Barcodes]'))
    SET IDENTITY_INSERT [Barcodes] OFF;
GO

UPDATE [Employees] SET [CreatedDateTime] = '2024-07-24T00:43:05.0842064+05:30'
WHERE [Id] = 1;
SELECT @@ROWCOUNT;

GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20240723191306_addedBarcodeModel', N'8.0.5');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

ALTER TABLE [Items] ADD [DiscountAmount] float NOT NULL DEFAULT 0.0E0;
GO

ALTER TABLE [Items] ADD [DiscountPercentage] float NOT NULL DEFAULT 0.0E0;
GO

ALTER TABLE [Items] ADD [MRP] float NOT NULL DEFAULT 0.0E0;
GO

UPDATE [Employees] SET [CreatedDateTime] = '2024-07-29T14:39:21.7643502+05:30'
WHERE [Id] = 1;
SELECT @@ROWCOUNT;

GO

UPDATE [Items] SET [DiscountAmount] = 0.0E0, [DiscountPercentage] = 0.0E0, [IsTaxInclusive] = CAST(1 AS bit), [MRP] = 0.0E0
WHERE [Id] = 1;
SELECT @@ROWCOUNT;

GO

UPDATE [Items] SET [DiscountAmount] = 0.0E0, [DiscountPercentage] = 0.0E0, [IsTaxInclusive] = CAST(1 AS bit), [MRP] = 0.0E0
WHERE [Id] = 2;
SELECT @@ROWCOUNT;

GO

UPDATE [Items] SET [DiscountAmount] = 0.0E0, [DiscountPercentage] = 0.0E0, [IsTaxInclusive] = CAST(1 AS bit), [MRP] = 0.0E0
WHERE [Id] = 3;
SELECT @@ROWCOUNT;

GO

UPDATE [Items] SET [DiscountAmount] = 0.0E0, [DiscountPercentage] = 0.0E0, [IsTaxInclusive] = CAST(1 AS bit), [MRP] = 0.0E0
WHERE [Id] = 4;
SELECT @@ROWCOUNT;

GO

UPDATE [Items] SET [DiscountAmount] = 0.0E0, [DiscountPercentage] = 0.0E0, [IsTaxInclusive] = CAST(1 AS bit), [MRP] = 0.0E0
WHERE [Id] = 5;
SELECT @@ROWCOUNT;

GO

UPDATE [Items] SET [DiscountAmount] = 0.0E0, [DiscountPercentage] = 0.0E0, [IsTaxInclusive] = CAST(1 AS bit), [MRP] = 0.0E0
WHERE [Id] = 6;
SELECT @@ROWCOUNT;

GO

UPDATE [Items] SET [DiscountAmount] = 0.0E0, [DiscountPercentage] = 0.0E0, [IsTaxInclusive] = CAST(1 AS bit), [MRP] = 0.0E0
WHERE [Id] = 7;
SELECT @@ROWCOUNT;

GO

UPDATE [Items] SET [DiscountAmount] = 0.0E0, [DiscountPercentage] = 0.0E0, [IsTaxInclusive] = CAST(1 AS bit), [MRP] = 0.0E0
WHERE [Id] = 8;
SELECT @@ROWCOUNT;

GO

UPDATE [Items] SET [DiscountAmount] = 0.0E0, [DiscountPercentage] = 0.0E0, [IsTaxInclusive] = CAST(1 AS bit), [MRP] = 0.0E0
WHERE [Id] = 9;
SELECT @@ROWCOUNT;

GO

UPDATE [Items] SET [DiscountAmount] = 0.0E0, [DiscountPercentage] = 0.0E0, [IsTaxInclusive] = CAST(1 AS bit), [MRP] = 0.0E0
WHERE [Id] = 10;
SELECT @@ROWCOUNT;

GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20240729090923_addedMRPandDiscountToItem', N'8.0.5');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

DELETE FROM [Barcodes]
WHERE [Id] = 1;
SELECT @@ROWCOUNT;

GO

DELETE FROM [Barcodes]
WHERE [Id] = 2;
SELECT @@ROWCOUNT;

GO

DELETE FROM [BillItem]
WHERE [Id] = 1;
SELECT @@ROWCOUNT;

GO

DELETE FROM [BillItem]
WHERE [Id] = 2;
SELECT @@ROWCOUNT;

GO

DELETE FROM [BillItem]
WHERE [Id] = 3;
SELECT @@ROWCOUNT;

GO

DELETE FROM [BillItem]
WHERE [Id] = 4;
SELECT @@ROWCOUNT;

GO

DELETE FROM [Items]
WHERE [Id] = 1;
SELECT @@ROWCOUNT;

GO

DELETE FROM [Items]
WHERE [Id] = 2;
SELECT @@ROWCOUNT;

GO

DELETE FROM [Items]
WHERE [Id] = 3;
SELECT @@ROWCOUNT;

GO

DELETE FROM [Items]
WHERE [Id] = 4;
SELECT @@ROWCOUNT;

GO

DELETE FROM [Items]
WHERE [Id] = 5;
SELECT @@ROWCOUNT;

GO

DELETE FROM [Items]
WHERE [Id] = 6;
SELECT @@ROWCOUNT;

GO

DELETE FROM [Items]
WHERE [Id] = 7;
SELECT @@ROWCOUNT;

GO

DELETE FROM [Items]
WHERE [Id] = 8;
SELECT @@ROWCOUNT;

GO

DELETE FROM [Items]
WHERE [Id] = 9;
SELECT @@ROWCOUNT;

GO

DELETE FROM [Items]
WHERE [Id] = 10;
SELECT @@ROWCOUNT;

GO

DELETE FROM [Bills]
WHERE [Id] = 1;
SELECT @@ROWCOUNT;

GO

DELETE FROM [Categories]
WHERE [Id] = 1;
SELECT @@ROWCOUNT;

GO

DELETE FROM [Categories]
WHERE [Id] = 2;
SELECT @@ROWCOUNT;

GO

DELETE FROM [Categories]
WHERE [Id] = 3;
SELECT @@ROWCOUNT;

GO

DECLARE @var0 sysname;
SELECT @var0 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Items]') AND [c].[name] = N'DiscountAmount');
IF @var0 IS NOT NULL EXEC(N'ALTER TABLE [Items] DROP CONSTRAINT [' + @var0 + '];');
ALTER TABLE [Items] DROP COLUMN [DiscountAmount];
GO

DECLARE @var1 sysname;
SELECT @var1 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Items]') AND [c].[name] = N'SellPrice');
IF @var1 IS NOT NULL EXEC(N'ALTER TABLE [Items] DROP CONSTRAINT [' + @var1 + '];');
ALTER TABLE [Items] DROP COLUMN [SellPrice];
GO

DECLARE @var2 sysname;
SELECT @var2 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[BillItem]') AND [c].[name] = N'Barcode');
IF @var2 IS NOT NULL EXEC(N'ALTER TABLE [BillItem] DROP CONSTRAINT [' + @var2 + '];');
ALTER TABLE [BillItem] DROP COLUMN [Barcode];
GO

EXEC sp_rename N'[BillItem].[Price]', N'MRP', N'COLUMN';
GO

EXEC sp_rename N'[BillItem].[DiscountAmount]', N'DiscountPercentage', N'COLUMN';
GO

ALTER TABLE [BillItem] ADD [Unit] nvarchar(max) NOT NULL DEFAULT N'';
GO

UPDATE [Employees] SET [CreatedDateTime] = '2024-08-03T17:43:44.6109680+05:30'
WHERE [Id] = 1;
SELECT @@ROWCOUNT;

GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20240803121346_Updated_ItemBillItemModels', N'8.0.5');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

DECLARE @var3 sysname;
SELECT @var3 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[BillItem]') AND [c].[name] = N'Unit');
IF @var3 IS NOT NULL EXEC(N'ALTER TABLE [BillItem] DROP CONSTRAINT [' + @var3 + '];');
ALTER TABLE [BillItem] ALTER COLUMN [Unit] nvarchar(max) NULL;
GO

UPDATE [Employees] SET [CreatedDateTime] = '2024-08-06T18:50:33.0028586+05:30'
WHERE [Id] = 1;
SELECT @@ROWCOUNT;

GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20240806132034_updatedBill_unitNullable', N'8.0.5');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

DECLARE @var4 sysname;
SELECT @var4 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Categories]') AND [c].[name] = N'Name');
IF @var4 IS NOT NULL EXEC(N'ALTER TABLE [Categories] DROP CONSTRAINT [' + @var4 + '];');
ALTER TABLE [Categories] ALTER COLUMN [Name] nvarchar(50) NOT NULL;
GO

UPDATE [Employees] SET [CreatedDateTime] = '2025-02-21T00:16:28.2118295+05:30'
WHERE [Id] = 1;
SELECT @@ROWCOUNT;

GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20250220184629_UpdateItemNameSize', N'8.0.5');
GO

COMMIT;
GO

