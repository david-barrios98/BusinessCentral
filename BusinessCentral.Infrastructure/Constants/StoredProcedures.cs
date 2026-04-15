namespace BusinessCentral.Infrastructure.Constants
{
    /// <summary>
    /// Constantes centralizadas para todos los Stored Procedures
    /// </summary>
    public static class StoredProcedures
    {
        public static class Auth
        {
            //Login
            public const string sp_login_user = "[auth].[sp_login_user]";
            public const string sp_get_user_by_email_company = "[auth].[sp_get_user_by_email_company]";
            public const string sp_insert_password_reset_token = "[auth].[sp_insert_password_reset_token]";
            public const string sp_get_active_password_reset_token = "[auth].[sp_get_active_password_reset_token]";
            public const string sp_mark_password_reset_used = "[auth].[sp_mark_password_reset_used]";
            public const string sp_update_user_password = "[auth].[sp_update_user_password]";
        }
        public static class Config
        {
            public const string sp_check_tenant_access = "[config].[sp_check_tenant_access]";
        }
    }
}