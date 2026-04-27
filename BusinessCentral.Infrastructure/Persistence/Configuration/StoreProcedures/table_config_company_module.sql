/*
  Tabla: CompanyModule
  Permite habilitar/deshabilitar módulos por compañía (además del plan).
*/

IF OBJECT_ID('[config].[CompanyModule]', 'U') IS NULL
BEGIN
    CREATE TABLE [config].[CompanyModule] (
        [CompanyId] INT NOT NULL,
        [ModuleId] INT NOT NULL,
        [IsEnabled] BIT NOT NULL CONSTRAINT DF_CompanyModule_IsEnabled DEFAULT (1),
        [CreatedAt] DATETIME2(7) NOT NULL CONSTRAINT DF_CompanyModule_CreatedAt DEFAULT (SYSUTCDATETIME()),
        [UpdatedAt] DATETIME2(7) NOT NULL CONSTRAINT DF_CompanyModule_UpdatedAt DEFAULT (SYSUTCDATETIME()),
        CONSTRAINT PK_CompanyModule PRIMARY KEY ([CompanyId], [ModuleId]),
        CONSTRAINT FK_CompanyModule_Companies FOREIGN KEY ([CompanyId]) REFERENCES [business].[Companies]([Id]),
        CONSTRAINT FK_CompanyModule_Module FOREIGN KEY ([ModuleId]) REFERENCES [config].[Module]([Id])
    );
END

