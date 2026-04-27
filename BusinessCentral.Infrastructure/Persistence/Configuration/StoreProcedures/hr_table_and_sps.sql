/*
  HR (Workforce) - módulo común para finca/lavadero/comercio.
  Todo multi-tenant por CompanyId y referencia directa a auth.UsersInfo (UserId).
*/

-- Schema
IF NOT EXISTS (SELECT 1 FROM sys.schemas WHERE name = 'hr')
    EXEC('CREATE SCHEMA [hr]');
GO

-- EmployeeProfile (1:1 con UsersInfo)
IF OBJECT_ID('[hr].[EmployeeProfile]', 'U') IS NULL
BEGIN
    CREATE TABLE [hr].[EmployeeProfile] (
        [UserId]            INT           NOT NULL,
        [CompanyId]         INT           NOT NULL,
        [IsEmployee]        BIT           NOT NULL CONSTRAINT DF_EmployeeProfile_IsEmployee DEFAULT (1),
        [ActiveEmployee]    BIT           NOT NULL CONSTRAINT DF_EmployeeProfile_ActiveEmployee DEFAULT (1),
        [HireDate]          DATE          NULL,

        -- Hospedaje
        [LodgingIncluded]   BIT           NOT NULL CONSTRAINT DF_EmployeeProfile_LodgingIncluded DEFAULT (0),
        [LodgingLocation]   NVARCHAR(200) NULL, -- cambuche/ubicación
        [MattressIncluded]  BIT           NOT NULL CONSTRAINT DF_EmployeeProfile_MattressIncluded DEFAULT (0),

        -- Alimentación parametrizable
        [MealPlanCode]      NVARCHAR(50)  NULL, -- FULL_DAY / ONE_MEAL / NONE
        [MealUnitCost]      DECIMAL(18,2) NULL, -- costo por unidad (si aplica)

        [CreatedAt]         DATETIME2(7)  NOT NULL CONSTRAINT DF_EmployeeProfile_CreatedAt DEFAULT (SYSUTCDATETIME()),
        [UpdatedAt]         DATETIME2(7)  NOT NULL CONSTRAINT DF_EmployeeProfile_UpdatedAt DEFAULT (SYSUTCDATETIME()),

        CONSTRAINT PK_EmployeeProfile PRIMARY KEY ([UserId]),
        CONSTRAINT FK_EmployeeProfile_UsersInfo FOREIGN KEY ([UserId]) REFERENCES [auth].[UsersInfo]([Id]),
        CONSTRAINT FK_EmployeeProfile_Companies FOREIGN KEY ([CompanyId]) REFERENCES [business].[Companies]([Id])
    );
END
GO

-- PayScheme (cómo se paga)
IF OBJECT_ID('[hr].[PayScheme]', 'U') IS NULL
BEGIN
    CREATE TABLE [hr].[PayScheme] (
        [Id]        INT IDENTITY(1,1) NOT NULL,
        [CompanyId] INT NOT NULL,
        [Code]      NVARCHAR(50) NOT NULL,  -- DAILY / BY_WEIGHT / BY_SERVICE / FIXED
        [Name]      NVARCHAR(150) NOT NULL,
        [Unit]      NVARCHAR(50) NULL,      -- DAY / KG / SERVICE
        [Active]    BIT NOT NULL CONSTRAINT DF_PayScheme_Active DEFAULT (1),
        [CreatedAt] DATETIME2(7) NOT NULL CONSTRAINT DF_PayScheme_CreatedAt DEFAULT (SYSUTCDATETIME()),
        [UpdatedAt] DATETIME2(7) NOT NULL CONSTRAINT DF_PayScheme_UpdatedAt DEFAULT (SYSUTCDATETIME()),
        CONSTRAINT PK_PayScheme PRIMARY KEY ([Id]),
        CONSTRAINT FK_PayScheme_Companies FOREIGN KEY ([CompanyId]) REFERENCES [business].[Companies]([Id]),
        CONSTRAINT UQ_PayScheme_Company_Code UNIQUE ([CompanyId], [Code])
    );
END
GO

-- PayRate (tarifa por esquema)
IF OBJECT_ID('[hr].[PayRate]', 'U') IS NULL
BEGIN
    CREATE TABLE [hr].[PayRate] (
        [Id]          INT IDENTITY(1,1) NOT NULL,
        [CompanyId]   INT NOT NULL,
        [PaySchemeId] INT NOT NULL,
        [Rate]        DECIMAL(18,2) NOT NULL, -- COP por unidad
        [EffectiveFrom] DATE NULL,
        [EffectiveTo]   DATE NULL,
        [Active]      BIT NOT NULL CONSTRAINT DF_PayRate_Active DEFAULT (1),
        [CreatedAt]   DATETIME2(7) NOT NULL CONSTRAINT DF_PayRate_CreatedAt DEFAULT (SYSUTCDATETIME()),
        [UpdatedAt]   DATETIME2(7) NOT NULL CONSTRAINT DF_PayRate_UpdatedAt DEFAULT (SYSUTCDATETIME()),
        CONSTRAINT PK_PayRate PRIMARY KEY ([Id]),
        CONSTRAINT FK_PayRate_Companies FOREIGN KEY ([CompanyId]) REFERENCES [business].[Companies]([Id]),
        CONSTRAINT FK_PayRate_PayScheme FOREIGN KEY ([PaySchemeId]) REFERENCES [hr].[PayScheme]([Id])
    );
END
GO

-- WorkLog (registro de trabajo que genera pago)
IF OBJECT_ID('[hr].[WorkLog]', 'U') IS NULL
BEGIN
    CREATE TABLE [hr].[WorkLog] (
        [Id]          BIGINT IDENTITY(1,1) NOT NULL,
        [CompanyId]   INT NOT NULL,
        [UserId]      INT NOT NULL,
        [WorkDate]    DATE NOT NULL,
        [PaySchemeId] INT NOT NULL,
        [Quantity]    DECIMAL(18,3) NOT NULL CONSTRAINT DF_WorkLog_Quantity DEFAULT (1),
        [Unit]        NVARCHAR(50) NULL,
        [ReferenceType] NVARCHAR(50) NULL,  -- ProductionBatch / ServiceOrder / POS / etc
        [ReferenceId]  NVARCHAR(50) NULL,
        [Notes]       NVARCHAR(500) NULL,
        [CreatedAt]   DATETIME2(7) NOT NULL CONSTRAINT DF_WorkLog_CreatedAt DEFAULT (SYSUTCDATETIME()),
        CONSTRAINT PK_WorkLog PRIMARY KEY ([Id]),
        CONSTRAINT FK_WorkLog_Companies FOREIGN KEY ([CompanyId]) REFERENCES [business].[Companies]([Id]),
        CONSTRAINT FK_WorkLog_UsersInfo FOREIGN KEY ([UserId]) REFERENCES [auth].[UsersInfo]([Id]),
        CONSTRAINT FK_WorkLog_PayScheme FOREIGN KEY ([PaySchemeId]) REFERENCES [hr].[PayScheme]([Id])
    );
END
GO

-- LoanAdvance (anticipos/préstamos)
IF OBJECT_ID('[hr].[LoanAdvance]', 'U') IS NULL
BEGIN
    CREATE TABLE [hr].[LoanAdvance] (
        [Id]        BIGINT IDENTITY(1,1) NOT NULL,
        [CompanyId] INT NOT NULL,
        [UserId]    INT NOT NULL,
        [Date]      DATE NOT NULL,
        [Amount]    DECIMAL(18,2) NOT NULL,
        [Notes]     NVARCHAR(500) NULL,
        [Status]    NVARCHAR(20) NOT NULL CONSTRAINT DF_LoanAdvance_Status DEFAULT ('open'), -- open/paid/cancelled
        [CreatedAt] DATETIME2(7) NOT NULL CONSTRAINT DF_LoanAdvance_CreatedAt DEFAULT (SYSUTCDATETIME()),
        CONSTRAINT PK_LoanAdvance PRIMARY KEY ([Id]),
        CONSTRAINT FK_LoanAdvance_Companies FOREIGN KEY ([CompanyId]) REFERENCES [business].[Companies]([Id]),
        CONSTRAINT FK_LoanAdvance_UsersInfo FOREIGN KEY ([UserId]) REFERENCES [auth].[UsersInfo]([Id])
    );
END
GO

-- Deduction (deducciones)
IF OBJECT_ID('[hr].[Deduction]', 'U') IS NULL
BEGIN
    CREATE TABLE [hr].[Deduction] (
        [Id]        BIGINT IDENTITY(1,1) NOT NULL,
        [CompanyId] INT NOT NULL,
        [UserId]    INT NOT NULL,
        [Date]      DATE NOT NULL,
        [Amount]    DECIMAL(18,2) NOT NULL,
        [Type]      NVARCHAR(50) NULL,  -- MEAL / LOAN / OTHER
        [Notes]     NVARCHAR(500) NULL,
        [CreatedAt] DATETIME2(7) NOT NULL CONSTRAINT DF_Deduction_CreatedAt DEFAULT (SYSUTCDATETIME()),
        CONSTRAINT PK_Deduction PRIMARY KEY ([Id]),
        CONSTRAINT FK_Deduction_Companies FOREIGN KEY ([CompanyId]) REFERENCES [business].[Companies]([Id]),
        CONSTRAINT FK_Deduction_UsersInfo FOREIGN KEY ([UserId]) REFERENCES [auth].[UsersInfo]([Id])
    );
END
GO

/* =========================
   Stored Procedures (CRUD)
   ========================= */

-- EmployeeProfile upsert
CREATE OR ALTER PROCEDURE [hr].[sp_upsert_employee_profile]
    @company_id INT,
    @user_id INT,
    @is_employee BIT,
    @active_employee BIT,
    @hire_date DATE = NULL,
    @lodging_included BIT,
    @lodging_location NVARCHAR(200) = NULL,
    @mattress_included BIT,
    @meal_plan_code NVARCHAR(50) = NULL,
    @meal_unit_cost DECIMAL(18,2) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    IF EXISTS (SELECT 1 FROM [hr].[EmployeeProfile] WHERE UserId=@user_id AND CompanyId=@company_id)
    BEGIN
        UPDATE [hr].[EmployeeProfile]
        SET
            IsEmployee=@is_employee,
            ActiveEmployee=@active_employee,
            HireDate=@hire_date,
            LodgingIncluded=@lodging_included,
            LodgingLocation=@lodging_location,
            MattressIncluded=@mattress_included,
            MealPlanCode=@meal_plan_code,
            MealUnitCost=@meal_unit_cost,
            UpdatedAt=SYSUTCDATETIME()
        WHERE UserId=@user_id AND CompanyId=@company_id;
    END
    ELSE
    BEGIN
        INSERT INTO [hr].[EmployeeProfile]
        (UserId, CompanyId, IsEmployee, ActiveEmployee, HireDate, LodgingIncluded, LodgingLocation, MattressIncluded, MealPlanCode, MealUnitCost)
        VALUES
        (@user_id, @company_id, @is_employee, @active_employee, @hire_date, @lodging_included, @lodging_location, @mattress_included, @meal_plan_code, @meal_unit_cost);
    END

    SELECT CAST(1 AS BIT) AS Success;
END
GO

CREATE OR ALTER PROCEDURE [hr].[sp_get_employee_profile]
    @company_id INT,
    @user_id INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT TOP 1
        ep.UserId,
        ep.CompanyId,
        ep.IsEmployee,
        ep.ActiveEmployee,
        ep.HireDate,
        ep.LodgingIncluded,
        ep.LodgingLocation,
        ep.MattressIncluded,
        ep.MealPlanCode,
        ep.MealUnitCost
    FROM [hr].[EmployeeProfile] ep
    WHERE ep.CompanyId=@company_id AND ep.UserId=@user_id;
END
GO

CREATE OR ALTER PROCEDURE [hr].[sp_list_employees]
    @company_id INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        u.Id AS UserId,
        u.CompanyId,
        u.FirstName,
        u.LastName,
        u.DocumentNumber,
        u.Phone,
        u.Email,
        u.RoleId,
        u.Active AS UserActive,
        ep.ActiveEmployee,
        ep.HireDate
    FROM [auth].[UsersInfo] u
    LEFT JOIN [hr].[EmployeeProfile] ep ON ep.UserId = u.Id AND ep.CompanyId = u.CompanyId
    WHERE u.CompanyId=@company_id
      AND (ep.IsEmployee = 1 OR ep.UserId IS NULL) -- lista todos, pero marca empleado si existe
    ORDER BY u.FirstName, u.LastName;
END
GO

-- PayScheme
CREATE OR ALTER PROCEDURE [hr].[sp_upsert_pay_scheme]
    @company_id INT,
    @code NVARCHAR(50),
    @name NVARCHAR(150),
    @unit NVARCHAR(50) = NULL,
    @active BIT = 1
AS
BEGIN
    SET NOCOUNT ON;

    IF EXISTS (SELECT 1 FROM [hr].[PayScheme] WHERE CompanyId=@company_id AND Code=@code)
    BEGIN
        UPDATE [hr].[PayScheme]
        SET Name=@name, Unit=@unit, Active=@active, UpdatedAt=SYSUTCDATETIME()
        WHERE CompanyId=@company_id AND Code=@code;
    END
    ELSE
    BEGIN
        INSERT INTO [hr].[PayScheme] (CompanyId, Code, Name, Unit, Active)
        VALUES (@company_id, @code, @name, @unit, @active);
    END

    SELECT CAST(1 AS BIT) AS Success;
END
GO

CREATE OR ALTER PROCEDURE [hr].[sp_list_pay_schemes]
    @company_id INT,
    @only_active BIT = 1
AS
BEGIN
    SET NOCOUNT ON;

    SELECT Id, CompanyId, Code, Name, Unit, Active
    FROM [hr].[PayScheme]
    WHERE CompanyId=@company_id
      AND (@only_active=0 OR Active=1)
    ORDER BY Name;
END
GO

-- WorkLog
CREATE OR ALTER PROCEDURE [hr].[sp_create_work_log]
    @company_id INT,
    @user_id INT,
    @work_date DATE,
    @pay_scheme_id INT,
    @quantity DECIMAL(18,3),
    @unit NVARCHAR(50) = NULL,
    @reference_type NVARCHAR(50) = NULL,
    @reference_id NVARCHAR(50) = NULL,
    @notes NVARCHAR(500) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO [hr].[WorkLog]
    (CompanyId, UserId, WorkDate, PaySchemeId, Quantity, Unit, ReferenceType, ReferenceId, Notes)
    VALUES
    (@company_id, @user_id, @work_date, @pay_scheme_id, @quantity, @unit, @reference_type, @reference_id, @notes);

    SELECT CAST(SCOPE_IDENTITY() AS BIGINT) AS InsertedId;
END
GO

CREATE OR ALTER PROCEDURE [hr].[sp_list_work_logs]
    @company_id INT,
    @user_id INT = NULL,
    @from_date DATE = NULL,
    @to_date DATE = NULL
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        wl.Id,
        wl.CompanyId,
        wl.UserId,
        wl.WorkDate,
        wl.PaySchemeId,
        ps.Code AS PaySchemeCode,
        wl.Quantity,
        wl.Unit,
        wl.ReferenceType,
        wl.ReferenceId,
        wl.Notes
    FROM [hr].[WorkLog] wl
    INNER JOIN [hr].[PayScheme] ps ON ps.Id = wl.PaySchemeId
    WHERE wl.CompanyId=@company_id
      AND (@user_id IS NULL OR wl.UserId=@user_id)
      AND (@from_date IS NULL OR wl.WorkDate >= @from_date)
      AND (@to_date IS NULL OR wl.WorkDate <= @to_date)
    ORDER BY wl.WorkDate DESC, wl.Id DESC;
END
GO

-- LoanAdvance
CREATE OR ALTER PROCEDURE [hr].[sp_create_loan_advance]
    @company_id INT,
    @user_id INT,
    @date DATE,
    @amount DECIMAL(18,2),
    @notes NVARCHAR(500) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    INSERT INTO [hr].[LoanAdvance] (CompanyId, UserId, Date, Amount, Notes)
    VALUES (@company_id, @user_id, @date, @amount, @notes);

    SELECT CAST(SCOPE_IDENTITY() AS BIGINT) AS InsertedId;
END
GO

CREATE OR ALTER PROCEDURE [hr].[sp_list_loan_advances]
    @company_id INT,
    @user_id INT = NULL,
    @status NVARCHAR(20) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    SELECT Id, CompanyId, UserId, Date, Amount, Notes, Status
    FROM [hr].[LoanAdvance]
    WHERE CompanyId=@company_id
      AND (@user_id IS NULL OR UserId=@user_id)
      AND (@status IS NULL OR Status=@status)
    ORDER BY Date DESC, Id DESC;
END
GO

-- Deduction
CREATE OR ALTER PROCEDURE [hr].[sp_create_deduction]
    @company_id INT,
    @user_id INT,
    @date DATE,
    @amount DECIMAL(18,2),
    @type NVARCHAR(50) = NULL,
    @notes NVARCHAR(500) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    INSERT INTO [hr].[Deduction] (CompanyId, UserId, Date, Amount, Type, Notes)
    VALUES (@company_id, @user_id, @date, @amount, @type, @notes);

    SELECT CAST(SCOPE_IDENTITY() AS BIGINT) AS InsertedId;
END
GO

CREATE OR ALTER PROCEDURE [hr].[sp_list_deductions]
    @company_id INT,
    @user_id INT = NULL,
    @type NVARCHAR(50) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    SELECT Id, CompanyId, UserId, Date, Amount, Type, Notes
    FROM [hr].[Deduction]
    WHERE CompanyId=@company_id
      AND (@user_id IS NULL OR UserId=@user_id)
      AND (@type IS NULL OR Type=@type)
    ORDER BY Date DESC, Id DESC;
END
GO

