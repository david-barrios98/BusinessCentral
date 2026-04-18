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
        public static class User
        {
            public const string sp_create_user = "[auth].[sp_create_user]";
            public const string sp_get_user_by_id = "[auth].[sp_get_user_by_id]";
            public const string sp_update_user = "[auth].[sp_update_user]";
            public const string sp_delete_user = "[auth].[sp_delete_user]";
            public const string sp_list_users = "[auth].[sp_list_users]"; 
            public const string sp_get_rol_user = "[auth].[sp_get_rol_user]";
        }
        public static class Config
        {
            public const string sp_check_tenant_access = "[config].[sp_check_tenant_access]";
            // MembershipPlan
            public const string sp_get_membership_plan_by_id = "[config].[sp_get_membership_plan_by_id]";
            public const string sp_list_membership_plans = "[config].[sp_list_membership_plans]";
        }

        public static class Common
        {
            // Geografía
            public const string sp_list_countries = "[common].[sp_list_countries]";
            public const string sp_list_departments_by_country = "[common].[sp_list_departments_by_country]";
            public const string sp_list_cities_by_department = "[common].[sp_list_cities_by_department]";

            // Documentos
            public const string sp_list_document_types = "[common].[sp_list_document_types]";
        }
    }
}