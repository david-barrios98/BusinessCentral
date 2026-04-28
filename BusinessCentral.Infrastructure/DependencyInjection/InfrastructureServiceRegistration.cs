using BusinessCentral.Application.Ports.Outbound;
using BusinessCentral.Infrastructure.ExternalServices;
using BusinessCentral.Infrastructure.Models;
using BusinessCentral.Infrastructure.Persistence.Repositories;
using BusinessCentral.Infrastructure.Security;
using BusinessCentral.Shared.Helper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BusinessCentral.Infrastructure.DependencyInjection
{
    public static class InfrastructureServiceRegistration
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration config)
        {
            services.Configure<FailedLoginOptions>(config.GetSection("FailedLoginOptions"));
            services.Configure<EmailOptions>(config.GetSection("EmailOptions"));
            services.AddDistributedMemoryCache();
            services.AddScoped<IRedisService, RedisService>();

            // Repositories
            services.AddScoped<ILoginRepository, LoginRepository>();
            services.AddScoped<ISubscriptionRepository, SubscriptionRepository>();
            services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
            services.AddScoped<IUserSessionRepository, UserSessionRepository>();
            services.AddScoped<IPasswordResetRepository, PasswordResetRepository>();
            services.AddScoped<IUserRepository, UsersRepository>();
            services.AddScoped<UsersRepository>();
            services.AddScoped<ICommonRepository, CommonRepository>();
            services.AddScoped<ITenantRepository, TenantRepository>();
            services.AddScoped<ICompanyModuleRepository, CompanyModuleRepository>();
            services.AddScoped<ICompanyOnboardingRepository, CompanyOnboardingRepository>();
            services.AddScoped<IRolePermissionRepository, RolePermissionRepository>();
            services.AddScoped<IPermissionRepository, PermissionRepository>();
            services.AddScoped<IHrRepository, HrRepository>();
            services.AddScoped<IFarmRepository, FarmRepository>();
            services.AddScoped<IServicesRepository, ServicesRepository>();
            services.AddScoped<ICommerceRepository, CommerceRepository>();
            services.AddScoped<IFinanceReportsRepository, FinanceReportsRepository>();
            services.AddScoped<IPucAccountingRepository, PucAccountingRepository>();
            services.AddScoped<ICompanyFinancialProfileRepository, CompanyFinancialProfileRepository>();
            services.AddScoped<IFulfillmentMethodRepository, FulfillmentMethodRepository>();
            services.AddScoped<IPaymentMethodRepository, PaymentMethodRepository>();
            services.AddScoped<IStorageLocationRepository, StorageLocationRepository>();
            services.AddScoped<IInventoryLocationReportRepository, InventoryLocationReportRepository>();
            services.AddScoped<ISupplierRepository, SupplierRepository>();
            services.AddScoped<IProductVariantRepository, ProductVariantRepository>();
            services.AddScoped<IPurchaseReceivingRepository, PurchaseReceivingRepository>();
            services.AddScoped<IManufacturingRepository, ManufacturingRepository>();
            services.AddScoped<IAgroRepository, AgroRepository>();
            services.AddScoped<IPublicAccessRepository, PublicAccessRepository>();


            // Services
            services.AddScoped<ITokenService, TokenService>();
            services.AddScoped<IHashPasswordService, HashPasswordService>();
            services.AddScoped<JwtService>();
            services.AddSingleton<IFailedLoginAttemptService, FailedLoginAttemptService>();
            services.AddScoped<IEmailService, SmtpEmailService>();
            services.AddScoped<MemoryCacheService>();


            return services;
        }
    }
}