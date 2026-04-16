CREATE OR ALTER PROCEDURE [auth].[sp_create_user]
(
    @CompanyId INT,
    @DocumentTypeId INT,
    @DocumentNumber VARCHAR(50),
    @FirstName VARCHAR(150),
    @LastName VARCHAR(150),
    @Email VARCHAR(150),
    @Phone VARCHAR(20),
    @PasswordHash NVARCHAR(255),
    @AuthProvider VARCHAR(50),
    @ExternalId VARCHAR(150),
    @RoleId INT
)
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO [auth].[UsersInfo]
    (CompanyId, DocumentTypeId, DocumentNumber, FirstName, LastName, Email, Phone, Password, AuthProvider, ExternalId, RoleId, ConfirmedAccount, Active, Created, Updated)
    VALUES
    (@CompanyId, @DocumentTypeId, @DocumentNumber, @FirstName, @LastName, @Email, @Phone, @PasswordHash, @AuthProvider, @ExternalId, @RoleId, 0, 1, SYSUTCDATETIME(), SYSUTCDATETIME());

    SELECT CAST(SCOPE_IDENTITY() AS INT) AS InsertedId;
END
GO