namespace BusinessCentral.Infrastructure.Constants
{
    /// <summary>
    /// Constantes centralizadas para todos los Stored Procedures
    /// </summary>
    public static class StoredProcedures
    {
        public static class Audit
        {
            //Auditoria
            public const string sp_insert_user_session = "[audit].[sp_insert_user_session]";
            public const string sp_update_user_session = "[audit].[sp_update_user_session]";
            public const string sp_get_user_session_by_id = "[audit].[sp_get_user_session_by_id]";
            public const string sp_close_all_user_sessions = "[audit].[sp_close_all_user_sessions]";
            public const string sp_close_company_sessions = "[audit].[sp_close_company_sessions]";

        }
        public static class Auth
        {
            //Login
            public const string sp_login_user = "[auth].[sp_login_user]";
            public const string sp_get_user_by_email_company = "[auth].[sp_get_user_by_email_company]";
            public const string sp_insert_password_reset_token = "[auth].[sp_insert_password_reset_token]";
            public const string sp_get_active_password_reset_token = "[auth].[sp_get_active_password_reset_token]";
            public const string sp_mark_password_reset_used = "[auth].[sp_mark_password_reset_used]";
            public const string sp_update_user_password = "[auth].[sp_update_user_password]";

            public const string sp_insert_refresh_token = "[auth].[sp_insert_refresh_token]";
            public const string sp_get_active_refresh_token = "[auth].[sp_get_active_refresh_token]";
            public const string sp_revoke_refresh_token = "[auth].[sp_revoke_refresh_token]";
            public const string sp_revoke_all_tokens_by_user = "[auth].[sp_revoke_all_tokens_by_user]";
            public const string sp_revoke_all_tokens_by_company = "[auth].[sp_revoke_all_tokens_by_company]";
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
            public const string sp_get_company_id_by_subdomain = "[config].[sp_get_company_id_by_subdomain]";
            public const string sp_list_modules = "[config].[sp_list_modules]";
            public const string sp_list_company_modules = "[config].[sp_list_company_modules]";
            public const string sp_set_company_module = "[config].[sp_set_company_module]";
            public const string sp_is_company_module_enabled = "[config].[sp_is_company_module_enabled]";
            public const string sp_list_business_natures = "[config].[sp_list_business_natures]";
            public const string sp_list_business_nature_modules = "[config].[sp_list_business_nature_modules]";
            public const string sp_onboard_company = "[config].[sp_onboard_company]";
            // MembershipPlan
            public const string sp_get_membership_plan_by_id = "[config].[sp_get_membership_plan_by_id]";
            public const string sp_list_membership_plans = "[config].[sp_list_membership_plans]";
        }

        public static class Hr
        {
            public const string sp_upsert_employee_profile = "[hr].[sp_upsert_employee_profile]";
            public const string sp_get_employee_profile = "[hr].[sp_get_employee_profile]";
            public const string sp_list_employees = "[hr].[sp_list_employees]";

            public const string sp_upsert_pay_scheme = "[hr].[sp_upsert_pay_scheme]";
            public const string sp_list_pay_schemes = "[hr].[sp_list_pay_schemes]";

            public const string sp_create_work_log = "[hr].[sp_create_work_log]";
            public const string sp_list_work_logs = "[hr].[sp_list_work_logs]";

            public const string sp_create_loan_advance = "[hr].[sp_create_loan_advance]";
            public const string sp_list_loan_advances = "[hr].[sp_list_loan_advances]";

            public const string sp_create_deduction = "[hr].[sp_create_deduction]";
            public const string sp_list_deductions = "[hr].[sp_list_deductions]";
        }

        public static class Farm
        {
            public const string sp_upsert_zone = "[farm].[sp_upsert_zone]";
            public const string sp_list_zones = "[farm].[sp_list_zones]";
            public const string sp_create_harvest_lot = "[farm].[sp_create_harvest_lot]";
            public const string sp_list_harvest_lots = "[farm].[sp_list_harvest_lots]";
            public const string sp_create_process_step = "[farm].[sp_create_process_step]";
            public const string sp_list_process_steps = "[farm].[sp_list_process_steps]";
        }

        public static class Services
        {
            public const string sp_upsert_service = "[svc].[sp_upsert_service]";
            public const string sp_list_services = "[svc].[sp_list_services]";
            public const string sp_create_service_order = "[svc].[sp_create_service_order]";
            public const string sp_add_service_order_line = "[svc].[sp_add_service_order_line]";
            public const string sp_get_service_order = "[svc].[sp_get_service_order]";
        }

        public static class Commerce
        {
            public const string sp_upsert_product = "[com].[sp_upsert_product]";
            public const string sp_list_products = "[com].[sp_list_products]";
            public const string sp_create_cash_session = "[com].[sp_create_cash_session]";
            public const string sp_create_pos_ticket = "[com].[sp_create_pos_ticket]";
            public const string sp_add_pos_ticket_line = "[com].[sp_add_pos_ticket_line]";
            public const string sp_pay_pos_ticket = "[com].[sp_pay_pos_ticket]";
        }

        public static class Common
        {
            // Geografía
            public const string sp_list_countries = "[common].[sp_list_countries]";
            public const string sp_list_departments_by_country = "[common].[sp_list_departments_by_country]";
            public const string sp_list_cities_by_department = "[common].[sp_list_cities_by_department]";
            public const string sp_get_city_by_id = "[common].[sp_get_city_by_id]";


            // Documentos
            public const string sp_list_document_types = "[common].[sp_list_document_types]";
            public const string sp_get_document_type_by_id = "[common].[sp_get_document_type_by_id]";

        }
    }
}