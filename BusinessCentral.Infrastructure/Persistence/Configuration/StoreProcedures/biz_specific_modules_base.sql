/*
  Módulos específicos por tipo de negocio (base mínima):
  - FARM (Producción Café)   -> schema: farm
  - SERVICES (Lavadero)      -> schema: svc
  - COMMERCE + POS           -> schema: com (inventario/ventas rápidas)

  Nota:
  - Todo multi-tenant por CompanyId
  - SP-first: la API se conecta únicamente por stored procedures.
*/

/* =========================
   Schemas
   ========================= */
IF NOT EXISTS (SELECT 1 FROM sys.schemas WHERE name = 'farm')
    EXEC('CREATE SCHEMA [farm]');
GO
IF NOT EXISTS (SELECT 1 FROM sys.schemas WHERE name = 'svc')
    EXEC('CREATE SCHEMA [svc]');
GO
IF NOT EXISTS (SELECT 1 FROM sys.schemas WHERE name = 'com')
    EXEC('CREATE SCHEMA [com]');
GO

/* =========================
   FARM: Producción café
   ========================= */

IF OBJECT_ID('[farm].[FarmZone]', 'U') IS NULL
BEGIN
    CREATE TABLE [farm].[FarmZone] (
        [Id]        INT IDENTITY(1,1) NOT NULL,
        [CompanyId] INT NOT NULL,
        [Code]      NVARCHAR(50) NOT NULL,
        [Name]      NVARCHAR(150) NOT NULL,
        [Active]    BIT NOT NULL CONSTRAINT DF_FarmZone_Active DEFAULT (1),
        [CreatedAt] DATETIME2(7) NOT NULL CONSTRAINT DF_FarmZone_CreatedAt DEFAULT (SYSUTCDATETIME()),
        [UpdatedAt] DATETIME2(7) NOT NULL CONSTRAINT DF_FarmZone_UpdatedAt DEFAULT (SYSUTCDATETIME()),
        CONSTRAINT PK_FarmZone PRIMARY KEY ([Id]),
        CONSTRAINT FK_FarmZone_Companies FOREIGN KEY ([CompanyId]) REFERENCES [business].[Companies]([Id]),
        CONSTRAINT UQ_FarmZone_Company_Code UNIQUE ([CompanyId], [Code])
    );
END
GO

IF OBJECT_ID('[farm].[HarvestLot]', 'U') IS NULL
BEGIN
    CREATE TABLE [farm].[HarvestLot] (
        [Id]          BIGINT IDENTITY(1,1) NOT NULL,
        [CompanyId]   INT NOT NULL,
        [ZoneId]      INT NULL,
        [HarvestDate] DATE NOT NULL,
        [ProductForm] NVARCHAR(50) NOT NULL,  -- CHERRY / PULP / DRY_PARCHMENT / GREEN / ROASTED
        [QuantityKg]  DECIMAL(18,3) NOT NULL,
        [Notes]       NVARCHAR(500) NULL,
        [CreatedAt]   DATETIME2(7) NOT NULL CONSTRAINT DF_HarvestLot_CreatedAt DEFAULT (SYSUTCDATETIME()),
        CONSTRAINT PK_HarvestLot PRIMARY KEY ([Id]),
        CONSTRAINT FK_HarvestLot_Companies FOREIGN KEY ([CompanyId]) REFERENCES [business].[Companies]([Id]),
        CONSTRAINT FK_HarvestLot_Zone FOREIGN KEY ([ZoneId]) REFERENCES [farm].[FarmZone]([Id])
    );
END
GO

IF OBJECT_ID('[farm].[CoffeeProcessStep]', 'U') IS NULL
BEGIN
    CREATE TABLE [farm].[CoffeeProcessStep] (
        [Id]          BIGINT IDENTITY(1,1) NOT NULL,
        [CompanyId]   INT NOT NULL,
        [HarvestLotId] BIGINT NOT NULL,
        [StepDate]    DATE NOT NULL,
        [StepType]    NVARCHAR(50) NOT NULL,  -- PULPING / FERMENT / WASH / DRY / HULL / SORT / ROAST
        [InputKg]     DECIMAL(18,3) NULL,
        [OutputKg]    DECIMAL(18,3) NULL,
        [Notes]       NVARCHAR(500) NULL,
        [CreatedAt]   DATETIME2(7) NOT NULL CONSTRAINT DF_CoffeeProcessStep_CreatedAt DEFAULT (SYSUTCDATETIME()),
        CONSTRAINT PK_CoffeeProcessStep PRIMARY KEY ([Id]),
        CONSTRAINT FK_CoffeeProcessStep_Companies FOREIGN KEY ([CompanyId]) REFERENCES [business].[Companies]([Id]),
        CONSTRAINT FK_CoffeeProcessStep_Lot FOREIGN KEY ([HarvestLotId]) REFERENCES [farm].[HarvestLot]([Id])
    );
END
GO

CREATE OR ALTER PROCEDURE [farm].[sp_upsert_zone]
    @company_id INT,
    @code NVARCHAR(50),
    @name NVARCHAR(150),
    @active BIT = 1
AS
BEGIN
    SET NOCOUNT ON;

    IF EXISTS (SELECT 1 FROM [farm].[FarmZone] WHERE CompanyId=@company_id AND Code=@code)
        UPDATE [farm].[FarmZone]
        SET Name=@name, Active=@active, UpdatedAt=SYSUTCDATETIME()
        WHERE CompanyId=@company_id AND Code=@code;
    ELSE
        INSERT INTO [farm].[FarmZone] (CompanyId, Code, Name, Active) VALUES (@company_id, @code, @name, @active);

    SELECT CAST(1 AS BIT) AS Success;
END
GO

CREATE OR ALTER PROCEDURE [farm].[sp_list_zones]
    @company_id INT,
    @only_active BIT = 1
AS
BEGIN
    SET NOCOUNT ON;
    SELECT Id, CompanyId, Code, Name, Active
    FROM [farm].[FarmZone]
    WHERE CompanyId=@company_id AND (@only_active=0 OR Active=1)
    ORDER BY Name;
END
GO

CREATE OR ALTER PROCEDURE [farm].[sp_create_harvest_lot]
    @company_id INT,
    @zone_id INT = NULL,
    @harvest_date DATE,
    @product_form NVARCHAR(50),
    @quantity_kg DECIMAL(18,3),
    @notes NVARCHAR(500) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    INSERT INTO [farm].[HarvestLot] (CompanyId, ZoneId, HarvestDate, ProductForm, QuantityKg, Notes)
    VALUES (@company_id, @zone_id, @harvest_date, @product_form, @quantity_kg, @notes);
    SELECT CAST(SCOPE_IDENTITY() AS BIGINT) AS InsertedId;
END
GO

CREATE OR ALTER PROCEDURE [farm].[sp_list_harvest_lots]
    @company_id INT,
    @from_date DATE = NULL,
    @to_date DATE = NULL,
    @zone_id INT = NULL
AS
BEGIN
    SET NOCOUNT ON;
    SELECT hl.Id, hl.CompanyId, hl.ZoneId, z.Name AS ZoneName, hl.HarvestDate, hl.ProductForm, hl.QuantityKg, hl.Notes
    FROM [farm].[HarvestLot] hl
    LEFT JOIN [farm].[FarmZone] z ON z.Id = hl.ZoneId
    WHERE hl.CompanyId=@company_id
      AND (@zone_id IS NULL OR hl.ZoneId=@zone_id)
      AND (@from_date IS NULL OR hl.HarvestDate>=@from_date)
      AND (@to_date IS NULL OR hl.HarvestDate<=@to_date)
    ORDER BY hl.HarvestDate DESC, hl.Id DESC;
END
GO

CREATE OR ALTER PROCEDURE [farm].[sp_create_process_step]
    @company_id INT,
    @harvest_lot_id BIGINT,
    @step_date DATE,
    @step_type NVARCHAR(50),
    @input_kg DECIMAL(18,3) = NULL,
    @output_kg DECIMAL(18,3) = NULL,
    @notes NVARCHAR(500) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    INSERT INTO [farm].[CoffeeProcessStep] (CompanyId, HarvestLotId, StepDate, StepType, InputKg, OutputKg, Notes)
    VALUES (@company_id, @harvest_lot_id, @step_date, @step_type, @input_kg, @output_kg, @notes);
    SELECT CAST(SCOPE_IDENTITY() AS BIGINT) AS InsertedId;
END
GO

CREATE OR ALTER PROCEDURE [farm].[sp_list_process_steps]
    @company_id INT,
    @harvest_lot_id BIGINT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT Id, CompanyId, HarvestLotId, StepDate, StepType, InputKg, OutputKg, Notes
    FROM [farm].[CoffeeProcessStep]
    WHERE CompanyId=@company_id AND HarvestLotId=@harvest_lot_id
    ORDER BY StepDate ASC, Id ASC;
END
GO

/* =========================
   SERVICES: Lavadero
   ========================= */

IF OBJECT_ID('[svc].[ServiceCatalog]', 'U') IS NULL
BEGIN
    CREATE TABLE [svc].[ServiceCatalog] (
        [Id]        INT IDENTITY(1,1) NOT NULL,
        [CompanyId] INT NOT NULL,
        [Code]      NVARCHAR(50) NOT NULL,
        [Name]      NVARCHAR(150) NOT NULL,
        [BasePrice] DECIMAL(18,2) NOT NULL CONSTRAINT DF_ServiceCatalog_BasePrice DEFAULT (0),
        [Active]    BIT NOT NULL CONSTRAINT DF_ServiceCatalog_Active DEFAULT (1),
        [CreatedAt] DATETIME2(7) NOT NULL CONSTRAINT DF_ServiceCatalog_CreatedAt DEFAULT (SYSUTCDATETIME()),
        [UpdatedAt] DATETIME2(7) NOT NULL CONSTRAINT DF_ServiceCatalog_UpdatedAt DEFAULT (SYSUTCDATETIME()),
        CONSTRAINT PK_ServiceCatalog PRIMARY KEY ([Id]),
        CONSTRAINT FK_ServiceCatalog_Companies FOREIGN KEY ([CompanyId]) REFERENCES [business].[Companies]([Id]),
        CONSTRAINT UQ_ServiceCatalog_Company_Code UNIQUE ([CompanyId], [Code])
    );
END
GO

IF OBJECT_ID('[svc].[ServiceOrder]', 'U') IS NULL
BEGIN
    CREATE TABLE [svc].[ServiceOrder] (
        [Id]          BIGINT IDENTITY(1,1) NOT NULL,
        [CompanyId]   INT NOT NULL,
        [OrderDate]   DATETIME2(7) NOT NULL CONSTRAINT DF_ServiceOrder_OrderDate DEFAULT (SYSUTCDATETIME()),
        [VehicleType] NVARCHAR(50) NULL,
        [Plate]       NVARCHAR(20) NULL,
        [CustomerName] NVARCHAR(150) NULL,
        [Status]      NVARCHAR(20) NOT NULL CONSTRAINT DF_ServiceOrder_Status DEFAULT ('open'), -- open/closed/cancelled
        [Total]       DECIMAL(18,2) NOT NULL CONSTRAINT DF_ServiceOrder_Total DEFAULT (0),
        [CreatedAt]   DATETIME2(7) NOT NULL CONSTRAINT DF_ServiceOrder_CreatedAt DEFAULT (SYSUTCDATETIME()),
        CONSTRAINT PK_ServiceOrder PRIMARY KEY ([Id]),
        CONSTRAINT FK_ServiceOrder_Companies FOREIGN KEY ([CompanyId]) REFERENCES [business].[Companies]([Id])
    );
END
GO

IF OBJECT_ID('[svc].[ServiceOrderLine]', 'U') IS NULL
BEGIN
    CREATE TABLE [svc].[ServiceOrderLine] (
        [Id]          BIGINT IDENTITY(1,1) NOT NULL,
        [CompanyId]   INT NOT NULL,
        [OrderId]     BIGINT NOT NULL,
        [ServiceId]   INT NOT NULL,
        [Quantity]    DECIMAL(18,3) NOT NULL CONSTRAINT DF_ServiceOrderLine_Qty DEFAULT (1),
        [UnitPrice]   DECIMAL(18,2) NOT NULL,
        [EmployeeUserId] INT NULL, -- asignación de empleado (UsersInfo)
        [LineTotal]   AS (CONVERT(DECIMAL(18,2), [Quantity] * [UnitPrice])) PERSISTED,
        CONSTRAINT PK_ServiceOrderLine PRIMARY KEY ([Id]),
        CONSTRAINT FK_ServiceOrderLine_Companies FOREIGN KEY ([CompanyId]) REFERENCES [business].[Companies]([Id]),
        CONSTRAINT FK_ServiceOrderLine_Order FOREIGN KEY ([OrderId]) REFERENCES [svc].[ServiceOrder]([Id]),
        CONSTRAINT FK_ServiceOrderLine_Service FOREIGN KEY ([ServiceId]) REFERENCES [svc].[ServiceCatalog]([Id]),
        CONSTRAINT FK_ServiceOrderLine_Employee FOREIGN KEY ([EmployeeUserId]) REFERENCES [auth].[UsersInfo]([Id])
    );
END
GO

CREATE OR ALTER PROCEDURE [svc].[sp_upsert_service]
    @company_id INT,
    @code NVARCHAR(50),
    @name NVARCHAR(150),
    @base_price DECIMAL(18,2),
    @active BIT = 1
AS
BEGIN
    SET NOCOUNT ON;
    IF EXISTS (SELECT 1 FROM [svc].[ServiceCatalog] WHERE CompanyId=@company_id AND Code=@code)
        UPDATE [svc].[ServiceCatalog]
        SET Name=@name, BasePrice=@base_price, Active=@active, UpdatedAt=SYSUTCDATETIME()
        WHERE CompanyId=@company_id AND Code=@code;
    ELSE
        INSERT INTO [svc].[ServiceCatalog] (CompanyId, Code, Name, BasePrice, Active)
        VALUES (@company_id, @code, @name, @base_price, @active);
    SELECT CAST(1 AS BIT) AS Success;
END
GO

CREATE OR ALTER PROCEDURE [svc].[sp_list_services]
    @company_id INT,
    @only_active BIT = 1
AS
BEGIN
    SET NOCOUNT ON;
    SELECT Id, CompanyId, Code, Name, BasePrice, Active
    FROM [svc].[ServiceCatalog]
    WHERE CompanyId=@company_id AND (@only_active=0 OR Active=1)
    ORDER BY Name;
END
GO

CREATE OR ALTER PROCEDURE [svc].[sp_create_service_order]
    @company_id INT,
    @vehicle_type NVARCHAR(50) = NULL,
    @plate NVARCHAR(20) = NULL,
    @customer_name NVARCHAR(150) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    INSERT INTO [svc].[ServiceOrder] (CompanyId, VehicleType, Plate, CustomerName)
    VALUES (@company_id, @vehicle_type, @plate, @customer_name);
    SELECT CAST(SCOPE_IDENTITY() AS BIGINT) AS InsertedId;
END
GO

CREATE OR ALTER PROCEDURE [svc].[sp_add_service_order_line]
    @company_id INT,
    @order_id BIGINT,
    @service_id INT,
    @quantity DECIMAL(18,3),
    @unit_price DECIMAL(18,2),
    @employee_user_id INT = NULL
AS
BEGIN
    SET NOCOUNT ON;
    INSERT INTO [svc].[ServiceOrderLine] (CompanyId, OrderId, ServiceId, Quantity, UnitPrice, EmployeeUserId)
    VALUES (@company_id, @order_id, @service_id, @quantity, @unit_price, @employee_user_id);

    UPDATE [svc].[ServiceOrder]
    SET Total = (
        SELECT COALESCE(SUM(LineTotal),0) FROM [svc].[ServiceOrderLine] WHERE CompanyId=@company_id AND OrderId=@order_id
    )
    WHERE CompanyId=@company_id AND Id=@order_id;

    SELECT CAST(SCOPE_IDENTITY() AS BIGINT) AS InsertedId;
END
GO

CREATE OR ALTER PROCEDURE [svc].[sp_get_service_order]
    @company_id INT,
    @order_id BIGINT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT TOP 1 Id, CompanyId, OrderDate, VehicleType, Plate, CustomerName, Status, Total
    FROM [svc].[ServiceOrder]
    WHERE CompanyId=@company_id AND Id=@order_id;

    SELECT l.Id, l.OrderId, l.ServiceId, s.Name AS ServiceName, l.Quantity, l.UnitPrice, l.LineTotal, l.EmployeeUserId
    FROM [svc].[ServiceOrderLine] l
    INNER JOIN [svc].[ServiceCatalog] s ON s.Id=l.ServiceId
    WHERE l.CompanyId=@company_id AND l.OrderId=@order_id
    ORDER BY l.Id;
END
GO

/* =========================
   COMMERCE + POS: inventario y ventas rápidas
   ========================= */

IF OBJECT_ID('[com].[Product]', 'U') IS NULL
BEGIN
    CREATE TABLE [com].[Product] (
        [Id]        INT IDENTITY(1,1) NOT NULL,
        [CompanyId] INT NOT NULL,
        [Sku]       NVARCHAR(50) NOT NULL,
        [Name]      NVARCHAR(200) NOT NULL,
        [Unit]      NVARCHAR(20) NULL, -- UN / KG / LT
        [Price]     DECIMAL(18,2) NOT NULL CONSTRAINT DF_Product_Price DEFAULT (0),
        [Active]    BIT NOT NULL CONSTRAINT DF_Product_Active DEFAULT (1),
        [CreatedAt] DATETIME2(7) NOT NULL CONSTRAINT DF_Product_CreatedAt DEFAULT (SYSUTCDATETIME()),
        [UpdatedAt] DATETIME2(7) NOT NULL CONSTRAINT DF_Product_UpdatedAt DEFAULT (SYSUTCDATETIME()),
        CONSTRAINT PK_Product PRIMARY KEY ([Id]),
        CONSTRAINT FK_Product_Companies FOREIGN KEY ([CompanyId]) REFERENCES [business].[Companies]([Id]),
        CONSTRAINT UQ_Product_Company_Sku UNIQUE ([CompanyId], [Sku])
    );
END
GO

IF OBJECT_ID('[com].[InventoryMovement]', 'U') IS NULL
BEGIN
    CREATE TABLE [com].[InventoryMovement] (
        [Id]        BIGINT IDENTITY(1,1) NOT NULL,
        [CompanyId] INT NOT NULL,
        [ProductId] INT NOT NULL,
        [MoveDate]  DATETIME2(7) NOT NULL CONSTRAINT DF_Inv_MoveDate DEFAULT (SYSUTCDATETIME()),
        [Type]      NVARCHAR(20) NOT NULL, -- IN/OUT/ADJUST
        [Quantity]  DECIMAL(18,3) NOT NULL,
        [ReferenceType] NVARCHAR(50) NULL, -- POS_TICKET / PURCHASE / SALE
        [ReferenceId] NVARCHAR(50) NULL,
        [Notes]     NVARCHAR(500) NULL,
        CONSTRAINT PK_InventoryMovement PRIMARY KEY ([Id]),
        CONSTRAINT FK_InventoryMovement_Companies FOREIGN KEY ([CompanyId]) REFERENCES [business].[Companies]([Id]),
        CONSTRAINT FK_InventoryMovement_Product FOREIGN KEY ([ProductId]) REFERENCES [com].[Product]([Id])
    );
END
GO

IF OBJECT_ID('[com].[CashSession]', 'U') IS NULL
BEGIN
    CREATE TABLE [com].[CashSession] (
        [Id]        BIGINT IDENTITY(1,1) NOT NULL,
        [CompanyId] INT NOT NULL,
        [OpenedAt]  DATETIME2(7) NOT NULL CONSTRAINT DF_CashSession_OpenedAt DEFAULT (SYSUTCDATETIME()),
        [ClosedAt]  DATETIME2(7) NULL,
        [OpenedByUserId] INT NULL,
        [Status]    NVARCHAR(20) NOT NULL CONSTRAINT DF_CashSession_Status DEFAULT ('open'), -- open/closed
        [OpeningAmount] DECIMAL(18,2) NOT NULL CONSTRAINT DF_CashSession_OpeningAmount DEFAULT (0),
        [ClosingAmount] DECIMAL(18,2) NULL,
        CONSTRAINT PK_CashSession PRIMARY KEY ([Id]),
        CONSTRAINT FK_CashSession_Companies FOREIGN KEY ([CompanyId]) REFERENCES [business].[Companies]([Id]),
        CONSTRAINT FK_CashSession_User FOREIGN KEY ([OpenedByUserId]) REFERENCES [auth].[UsersInfo]([Id])
    );
END
GO

IF OBJECT_ID('[com].[PosTicket]', 'U') IS NULL
BEGIN
    CREATE TABLE [com].[PosTicket] (
        [Id]        BIGINT IDENTITY(1,1) NOT NULL,
        [CompanyId] INT NOT NULL,
        [CashSessionId] BIGINT NULL,
        [TicketDate] DATETIME2(7) NOT NULL CONSTRAINT DF_PosTicket_TicketDate DEFAULT (SYSUTCDATETIME()),
        [Status]    NVARCHAR(20) NOT NULL CONSTRAINT DF_PosTicket_Status DEFAULT ('open'), -- open/paid/void
        [Total]     DECIMAL(18,2) NOT NULL CONSTRAINT DF_PosTicket_Total DEFAULT (0),
        CONSTRAINT PK_PosTicket PRIMARY KEY ([Id]),
        CONSTRAINT FK_PosTicket_Companies FOREIGN KEY ([CompanyId]) REFERENCES [business].[Companies]([Id]),
        CONSTRAINT FK_PosTicket_CashSession FOREIGN KEY ([CashSessionId]) REFERENCES [com].[CashSession]([Id])
    );
END
GO

IF OBJECT_ID('[com].[PosTicketLine]', 'U') IS NULL
BEGIN
    CREATE TABLE [com].[PosTicketLine] (
        [Id]        BIGINT IDENTITY(1,1) NOT NULL,
        [CompanyId] INT NOT NULL,
        [TicketId]  BIGINT NOT NULL,
        [ProductId] INT NOT NULL,
        [Quantity]  DECIMAL(18,3) NOT NULL,
        [UnitPrice] DECIMAL(18,2) NOT NULL,
        [LineTotal] AS (CONVERT(DECIMAL(18,2), [Quantity] * [UnitPrice])) PERSISTED,
        CONSTRAINT PK_PosTicketLine PRIMARY KEY ([Id]),
        CONSTRAINT FK_PosTicketLine_Companies FOREIGN KEY ([CompanyId]) REFERENCES [business].[Companies]([Id]),
        CONSTRAINT FK_PosTicketLine_Ticket FOREIGN KEY ([TicketId]) REFERENCES [com].[PosTicket]([Id]),
        CONSTRAINT FK_PosTicketLine_Product FOREIGN KEY ([ProductId]) REFERENCES [com].[Product]([Id])
    );
END
GO

IF OBJECT_ID('[com].[PosPayment]', 'U') IS NULL
BEGIN
    CREATE TABLE [com].[PosPayment] (
        [Id]        BIGINT IDENTITY(1,1) NOT NULL,
        [CompanyId] INT NOT NULL,
        [TicketId]  BIGINT NOT NULL,
        [Method]    NVARCHAR(20) NOT NULL, -- CASH/CARD/TRANSFER
        [Amount]    DECIMAL(18,2) NOT NULL,
        [PaidAt]    DATETIME2(7) NOT NULL CONSTRAINT DF_PosPayment_PaidAt DEFAULT (SYSUTCDATETIME()),
        CONSTRAINT PK_PosPayment PRIMARY KEY ([Id]),
        CONSTRAINT FK_PosPayment_Companies FOREIGN KEY ([CompanyId]) REFERENCES [business].[Companies]([Id]),
        CONSTRAINT FK_PosPayment_Ticket FOREIGN KEY ([TicketId]) REFERENCES [com].[PosTicket]([Id])
    );
END
GO

CREATE OR ALTER PROCEDURE [com].[sp_upsert_product]
    @company_id INT,
    @sku NVARCHAR(50),
    @name NVARCHAR(200),
    @unit NVARCHAR(20) = NULL,
    @price DECIMAL(18,2),
    @active BIT = 1
AS
BEGIN
    SET NOCOUNT ON;
    IF EXISTS (SELECT 1 FROM [com].[Product] WHERE CompanyId=@company_id AND Sku=@sku)
        UPDATE [com].[Product]
        SET Name=@name, Unit=@unit, Price=@price, Active=@active, UpdatedAt=SYSUTCDATETIME()
        WHERE CompanyId=@company_id AND Sku=@sku;
    ELSE
        INSERT INTO [com].[Product] (CompanyId, Sku, Name, Unit, Price, Active)
        VALUES (@company_id, @sku, @name, @unit, @price, @active);
    SELECT CAST(1 AS BIT) AS Success;
END
GO

CREATE OR ALTER PROCEDURE [com].[sp_list_products]
    @company_id INT,
    @only_active BIT = 1
AS
BEGIN
    SET NOCOUNT ON;
    SELECT Id, CompanyId, Sku, Name, Unit, Price, Active
    FROM [com].[Product]
    WHERE CompanyId=@company_id AND (@only_active=0 OR Active=1)
    ORDER BY Name;
END
GO

CREATE OR ALTER PROCEDURE [com].[sp_create_cash_session]
    @company_id INT,
    @opened_by_user_id INT = NULL,
    @opening_amount DECIMAL(18,2) = 0
AS
BEGIN
    SET NOCOUNT ON;
    INSERT INTO [com].[CashSession] (CompanyId, OpenedByUserId, OpeningAmount)
    VALUES (@company_id, @opened_by_user_id, @opening_amount);
    SELECT CAST(SCOPE_IDENTITY() AS BIGINT) AS InsertedId;
END
GO

CREATE OR ALTER PROCEDURE [com].[sp_create_pos_ticket]
    @company_id INT,
    @cash_session_id BIGINT = NULL
AS
BEGIN
    SET NOCOUNT ON;
    INSERT INTO [com].[PosTicket] (CompanyId, CashSessionId)
    VALUES (@company_id, @cash_session_id);
    SELECT CAST(SCOPE_IDENTITY() AS BIGINT) AS InsertedId;
END
GO

CREATE OR ALTER PROCEDURE [com].[sp_add_pos_ticket_line]
    @company_id INT,
    @ticket_id BIGINT,
    @product_id INT,
    @quantity DECIMAL(18,3),
    @unit_price DECIMAL(18,2)
AS
BEGIN
    SET NOCOUNT ON;
    INSERT INTO [com].[PosTicketLine] (CompanyId, TicketId, ProductId, Quantity, UnitPrice)
    VALUES (@company_id, @ticket_id, @product_id, @quantity, @unit_price);

    UPDATE [com].[PosTicket]
    SET Total = (SELECT COALESCE(SUM(LineTotal),0) FROM [com].[PosTicketLine] WHERE CompanyId=@company_id AND TicketId=@ticket_id)
    WHERE CompanyId=@company_id AND Id=@ticket_id;

    -- inventario OUT
    INSERT INTO [com].[InventoryMovement] (CompanyId, ProductId, Type, Quantity, ReferenceType, ReferenceId)
    VALUES (@company_id, @product_id, 'OUT', @quantity, 'POS_TICKET', CONVERT(NVARCHAR(50), @ticket_id));

    SELECT CAST(SCOPE_IDENTITY() AS BIGINT) AS InsertedId;
END
GO

CREATE OR ALTER PROCEDURE [com].[sp_pay_pos_ticket]
    @company_id INT,
    @ticket_id BIGINT,
    @method NVARCHAR(20),
    @amount DECIMAL(18,2)
AS
BEGIN
    SET NOCOUNT ON;
    INSERT INTO [com].[PosPayment] (CompanyId, TicketId, Method, Amount)
    VALUES (@company_id, @ticket_id, @method, @amount);

    UPDATE [com].[PosTicket]
    SET Status='paid'
    WHERE CompanyId=@company_id AND Id=@ticket_id;

    SELECT CAST(1 AS BIT) AS Success;
END
GO

