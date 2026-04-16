CREATE OR ALTER PROCEDURE [auth].[sp_update_user]
(
    @UserId INT,
    @DocumentTypeId INT,
    @DocumentNumber VARCHAR(50),
    @FirstName VARCHAR(150),
    @LastName VARCHAR(150),
    @Email VARCHAR(150),
    @Phone VARCHAR(20),
    @PasswordHash NVARCHAR(255), -- NULL si no se actualiza
    @AuthProvider VARCHAR(50),
    @ExternalId VARCHAR(150),
    @RoleId INT,
    @Active BIT
)
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE [auth].[UsersInfo]
    SET DocumentTypeId = @DocumentTypeId,
        DocumentNumber = @DocumentNumber,
        FirstName = @FirstName,
        LastName = @LastName,
        Email = @Email,
        Phone = @Phone,
        AuthProvider = @AuthProvider,
        ExternalId = @ExternalId,
        RoleId = @RoleId,
        Active = @Active,
        Updated = SYSUTCDATETIME(),
        Password = CASE WHEN @PasswordHash IS NOT NULL THEN @PasswordHash ELSE Password END
    WHERE Id = @UserId;
END
GO