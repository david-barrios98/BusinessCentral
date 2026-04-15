CREATE OR ALTER PROCEDURE [auth].[sp_get_user_by_email_company]
(
    @Email VARCHAR(255),
    @CompanyId INT
)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT TOP 1
        ui.Id AS UserId,
        ui.UserName,
        ui.Email,
        ui.FirstName,
        ui.LastName,
        ui.CompanyId
    FROM [auth].[UsersInfo] ui WITH (NOLOCK)
    WHERE ui.Email = @Email
      AND ui.CompanyId = @CompanyId
      AND ui.Active = 1;
END
GO