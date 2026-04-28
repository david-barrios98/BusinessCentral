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
            public const string sp_login_system_user = "[auth].[sp_login_system_user]";
            public const string sp_get_user_by_email_company = "[auth].[sp_get_user_by_email_company]";
            public const string sp_insert_password_reset_token = "[auth].[sp_insert_password_reset_token]";
            public const string sp_get_active_password_reset_token = "[auth].[sp_get_active_password_reset_token]";
            public const string sp_mark_password_reset_used = "[auth].[sp_mark_password_reset_used]";
            public const string sp_update_user_password = "[auth].[sp_update_user_password]";
            public const string sp_create_public_access_token = "[auth].[sp_create_public_access_token]";
            public const string sp_get_public_hr_account_summary = "[auth].[sp_get_public_hr_account_summary]";

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
            public const string sp_list_company_business_natures = "[config].[sp_list_company_business_natures]";
            public const string sp_set_company_business_nature = "[config].[sp_set_company_business_nature]";
            public const string sp_list_fulfillment_methods = "[config].[sp_list_fulfillment_methods]";
            public const string sp_list_company_fulfillment_methods = "[config].[sp_list_company_fulfillment_methods]";
            public const string sp_set_company_fulfillment_method = "[config].[sp_set_company_fulfillment_method]";
            public const string sp_list_payment_methods = "[config].[sp_list_payment_methods]";
            public const string sp_list_company_payment_methods = "[config].[sp_list_company_payment_methods]";
            public const string sp_set_company_payment_method = "[config].[sp_set_company_payment_method]";
            public const string sp_list_role_permissions = "[config].[sp_list_role_permissions]";
            public const string sp_set_role_permission = "[config].[sp_set_role_permission]";
            public const string sp_list_permissions = "[config].[sp_list_permissions]";
            public const string sp_list_business_nature_default_permissions = "[config].[sp_list_business_nature_default_permissions]";
            public const string sp_apply_business_nature_defaults_to_company = "[config].[sp_apply_business_nature_defaults_to_company]";
            // MembershipPlan
            public const string sp_get_membership_plan_by_id = "[config].[sp_get_membership_plan_by_id]";
            public const string sp_list_membership_plans = "[config].[sp_list_membership_plans]";
            public const string sp_list_plan_modules = "[config].[sp_list_plan_modules]";
        }

        public static class Business
        {
            public const string sp_list_facility_types = "[business].[sp_list_facility_types]";
            public const string sp_upsert_storage_location = "[business].[sp_upsert_storage_location]";
            public const string sp_list_storage_locations = "[business].[sp_list_storage_locations]";
            public const string sp_get_company_financial_bootstrap = "[business].[sp_get_company_financial_bootstrap]";
            public const string sp_update_company_financial_bootstrap = "[business].[sp_update_company_financial_bootstrap]";
        }

        public static class Hr
        {
            public const string sp_upsert_employee_profile = "[hr].[sp_upsert_employee_profile]";
            public const string sp_get_employee_profile = "[hr].[sp_get_employee_profile]";
            public const string sp_list_employees = "[hr].[sp_list_employees]";
            public const string sp_get_employee_availability = "[hr].[sp_get_employee_availability]";
            public const string sp_upsert_employee_availability = "[hr].[sp_upsert_employee_availability]";
            public const string sp_set_employee_availability_slots = "[hr].[sp_set_employee_availability_slots]";
            public const string sp_set_employee_availability_exceptions = "[hr].[sp_set_employee_availability_exceptions]";

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
            public const string sp_list_service_orders = "[svc].[sp_list_service_orders]";
        }

        public static class Commerce
        {
            public const string sp_upsert_product = "[com].[sp_upsert_product]";
            public const string sp_list_products = "[com].[sp_list_products]";
            public const string sp_create_cash_session = "[com].[sp_create_cash_session]";
            public const string sp_get_cash_session = "[com].[sp_get_cash_session]";
            public const string sp_add_cash_movement = "[com].[sp_add_cash_movement]";
            public const string sp_close_cash_session = "[com].[sp_close_cash_session]";
            public const string sp_create_pos_ticket = "[com].[sp_create_pos_ticket]";
            public const string sp_get_pos_ticket = "[com].[sp_get_pos_ticket]";
            public const string sp_add_pos_ticket_line = "[com].[sp_add_pos_ticket_line]";
            public const string sp_pay_pos_ticket = "[com].[sp_pay_pos_ticket]";
            public const string sp_report_inventory_by_location = "[com].[sp_report_inventory_by_location]";
            public const string sp_upsert_supplier = "[com].[sp_upsert_supplier]";
            public const string sp_list_suppliers = "[com].[sp_list_suppliers]";
            public const string sp_upsert_product_variant = "[com].[sp_upsert_product_variant]";
            public const string sp_list_product_variants = "[com].[sp_list_product_variants]";
            public const string sp_create_purchase_receipt = "[com].[sp_create_purchase_receipt]";
            public const string sp_add_purchase_receipt_line = "[com].[sp_add_purchase_receipt_line]";
            public const string sp_post_purchase_receipt = "[com].[sp_post_purchase_receipt]";
        }

        public static class Finance
        {
            public const string sp_create_financial_transaction = "[fin].[sp_create_financial_transaction]";
            public const string sp_list_financial_transactions = "[fin].[sp_list_financial_transactions]";
            public const string sp_report_financial_summary = "[fin].[sp_report_financial_summary]";
            public const string sp_report_pnl = "[fin].[sp_report_pnl]";
            public const string sp_report_tax_summary_co = "[fin].[sp_report_tax_summary_co]";
            public const string sp_report_renta_annual_co = "[fin].[sp_report_renta_annual_co]";
            public const string sp_list_accounts = "[fin].[sp_list_accounts]";
            public const string sp_get_account_id_by_code = "[fin].[sp_get_account_id_by_code]";
            public const string sp_create_journal_entry = "[fin].[sp_create_journal_entry]";
            public const string sp_add_journal_entry_line = "[fin].[sp_add_journal_entry_line]";
            public const string sp_get_journal_entry = "[fin].[sp_get_journal_entry]";
            public const string sp_post_journal_entry = "[fin].[sp_post_journal_entry]";
            public const string sp_report_trial_balance = "[fin].[sp_report_trial_balance]";
            public const string sp_report_income_statement_puc = "[fin].[sp_report_income_statement_puc]";
            public const string sp_report_balance_sheet_puc = "[fin].[sp_report_balance_sheet_puc]";
        }

        public static class Agro
        {
            public const string sp_create_lot = "[agro].[sp_create_lot]";
            public const string sp_list_lots = "[agro].[sp_list_lots]";
            public const string sp_create_feed_log = "[agro].[sp_create_feed_log]";
            public const string sp_create_mortality_log = "[agro].[sp_create_mortality_log]";
            public const string sp_create_harvest = "[agro].[sp_create_harvest]";
            public const string sp_report_lot_kpis = "[agro].[sp_report_lot_kpis]";
        }

        public static class Manufacturing
        {
            public const string sp_upsert_recipe = "[mfg].[sp_upsert_recipe]";
            public const string sp_set_recipe_items = "[mfg].[sp_set_recipe_items]";
            public const string sp_create_production_batch = "[mfg].[sp_create_production_batch]";
            public const string sp_post_production_batch = "[mfg].[sp_post_production_batch]";
            public const string sp_report_recipe_cost = "[mfg].[sp_report_recipe_cost]";
        }

        public static class Common
        {
            // Geograf?a
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