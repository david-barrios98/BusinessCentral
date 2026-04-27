using System.Reflection;
using BusinessCentral.Domain.Entities.Audit;
using BusinessCentral.Domain.Entities.Auth;
using BusinessCentral.Domain.Entities.Business;
using BusinessCentral.Domain.Entities.Commerce;
using BusinessCentral.Domain.Entities.Farm;
using BusinessCentral.Domain.Entities.Hr;
using BusinessCentral.Domain.Entities.Common;
using BusinessCentral.Domain.Entities.Config;
using BusinessCentral.Domain.Entities.Services;
using BusinessCentral.Domain.Entities.Finance;
using BusinessCentral.Infrastructure.Persistence.Adapters;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using Module = BusinessCentral.Domain.Entities.Config.Module;

namespace BusinessCentral.Infrastructure.Seed
{
    public static class DbInitializer
    {
        public static async Task SeedAsync(BusinessCentralDbContext context)
        {
            await context.Database.MigrateAsync();

            // --- 1. CATÁLOGOS BASE (Nivel 0 - No dependen de nadie) ---
            await SeedEntity<Countries>(context, "countries.json");
            await SeedEntity<DocumentType>(context, "document_types.json");
            await SeedEntity<MembershipPlan>(context, "membership_plans.json");
            // Modules: seed por "merge" (no se detiene si ya hay data)
            await SeedModulesMerge(context, "modules.json");
            await SeedBusinessNaturesMerge(context, "business_natures.json");
            await SeedFulfillmentMethodsMerge(context, "fulfillment_methods.json");
            await SeedPaymentMethodsMerge(context, "payment_methods.json");
            await SeedEntity<FacilityType>(context, "facility_type.json");

            // --- 2. GEOGRAFÍA (Nivel 1 - Dependen de Countries) ---
            await SeedEntity<Department>(context, "department.json");
            await SeedEntity<City>(context, "city.json");

            // --- 3. ESTRUCTURA EMPRESARIAL (Nivel 2 - Dependen de DocumentType/City) ---
            await SeedEntity<Companies>(context, "company.json");
            await SeedPermissionsMerge(context, "permissions.json");

            // --- 4. CONFIGURACIÓN Y SEDES (Dependen de Companies/Plans/Modules) ---
            await SeedEntity<Facility>(context, "facility.json");
            //await SeedEntity<FacilityAddress>(context, "facility_address.json");
            await SeedEntity<ApplicationCompanies>(context, "application_companies.json");
            await SeedEntity<CompanySubscription>(context, "company_subscription.json");
            await SeedPlanModulesMerge(context, "plan_module.json");
            await SeedRolesMerge(context, "roles.json");
            await SeedCompanyModulesMerge(context, "company_modules.json");
            await SeedCompanyBusinessNaturesMerge(context, "company_business_natures.json");

            // --- 5. SEGURIDAD DETALLADA (Dependen de Roles/Permissions) ---
            await SeedRolePermissionsMerge(context, "role_permissions.json");

            // --- 6. USUARIOS Y SESIONES (Nivel Final - Dependen de Companies/Roles) ---
            await SeedEntity<UsersInfo>(context, "users_info.json");
            await SeedEntity<UserAddress>(context, "user_addresses.json");

            // --- 7. DATOS DE PRUEBA (módulos) ---
            await SeedBusinessNatureModulesMerge(context, "business_nature_modules.json");
            await SeedBusinessNatureFulfillmentMethodsMerge(context, "business_nature_fulfillment_methods.json");
            await SeedBusinessNaturePaymentMethodsMerge(context, "business_nature_payment_methods.json");
            // HR
            await SeedEntity<EmployeeProfile>(context, "hr_employee_profiles.json");
            await SeedEntity<PayScheme>(context, "hr_pay_schemes.json");
            await SeedEntity<WorkLog>(context, "hr_work_logs.json");
            await SeedEntity<LoanAdvance>(context, "hr_loan_advances.json");
            await SeedEntity<Deduction>(context, "hr_deductions.json");

            // FARM
            await SeedEntity<CoffeeProductFormType>(context, "farm_coffee_product_form_types.json");
            await SeedEntity<CoffeeProcessType>(context, "farm_coffee_process_types.json");
            await SeedEntity<FarmZone>(context, "farm_zones.json");
            await SeedEntity<HarvestLot>(context, "farm_harvest_lots.json");
            await SeedEntity<CoffeeProcessStep>(context, "farm_process_steps.json");

            // SERVICES
            await SeedEntity<Customer>(context, "svc_customers.json");
            await SeedEntity<Vehicle>(context, "svc_vehicles.json");
            await SeedEntity<ServiceCatalog>(context, "svc_catalog.json");
            await SeedEntity<ServiceOrder>(context, "svc_orders.json");
            await SeedEntity<ServiceOrderLine>(context, "svc_order_lines.json");

            // COMMERCE / POS
            await SeedEntity<Category>(context, "com_categories.json");
            await SeedEntity<Product>(context, "com_products.json");
            await SeedEntity<Supplier>(context, "com_suppliers.json");
            await SeedEntity<ProductVariant>(context, "com_product_variants.json");
            await SeedEntity<CashSession>(context, "com_cash_sessions.json");
            await SeedEntity<PosTicket>(context, "com_pos_tickets.json");
            await SeedEntity<PosTicketLine>(context, "com_pos_ticket_lines.json");
            await SeedEntity<PosPayment>(context, "com_pos_payments.json");
            await SeedEntity<InventoryMovement>(context, "com_inventory_movements.json");

            // FIN (PUC)
            await SeedPucAccountsMerge(context, "fin_puc_accounts.json");

            // BUSINESS (Ubicaciones opcionales)
            await SeedStorageLocationsMerge(context, "business_storage_locations.json");
        }

        // =============================
        // 🧠 GENERIC SEED
        // =============================
        private static async Task SeedEntity<T>(
            BusinessCentralDbContext context,
            string fileName
        ) where T : class
        {
            var dbSet = context.Set<T>();

            if (await dbSet.AnyAsync()) // Usa AnyAsync para no bloquear el hilo
            {
                Console.WriteLine($"⚠️ {typeof(T).Name} ya tiene datos");
                return;
            }

            var data = await LoadJsonAsync<T>(fileName);

            if (data == null || !data.Any())
            {
                Console.WriteLine($"⚠️ {fileName} vacío");
                return;
            }

            try
            {
                await dbSet.AddRangeAsync(data);
                await context.SaveChangesAsync();

                // 💡 ESTO ES LO QUE FALTA:
                // Limpia el rastreador de EF para que la siguiente entidad (ej. Departments) 
                // no crea que los objetos de la anterior (ej. Countries) están en conflicto.
                context.ChangeTracker.Clear();

                Console.WriteLine($"✅ Seed {typeof(T).Name} insertado");
            }
            catch (Exception ex)
            {
                // Limpia también en caso de error para no contaminar el siguiente intento
                context.ChangeTracker.Clear();
                throw new Exception($"❌ Error en Seed de {typeof(T).Name}", ex);
            }
        }

        // =============================
        // 🧩 SEED MERGE: Modules (por Code)
        // =============================
        private static async Task SeedModulesMerge(BusinessCentralDbContext context, string fileName)
        {
            var dbSet = context.Set<Module>();
            var data = await LoadJsonAsync<Module>(fileName);

            if (data == null || !data.Any())
            {
                Console.WriteLine($"⚠️ {fileName} vacío");
                return;
            }

            // Solo insertamos los que no existen por Code (case-insensitive)
            var existingCodes = await dbSet
                .Select(m => m.Code)
                .ToListAsync();

            var existing = new HashSet<string>(
                existingCodes.Where(c => !string.IsNullOrWhiteSpace(c)).Select(c => c.ToLowerInvariant())
            );

            var toInsert = data
                .Where(m => !string.IsNullOrWhiteSpace(m.Code))
                .Where(m => !existing.Contains(m.Code.ToLowerInvariant()))
                .ToList();

            if (!toInsert.Any())
            {
                Console.WriteLine("⚠️ Module ya contiene todos los registros del seed");
                return;
            }

            try
            {
                await dbSet.AddRangeAsync(toInsert);
                await context.SaveChangesAsync();
                context.ChangeTracker.Clear();
                Console.WriteLine($"✅ Seed merge Module insertado: {toInsert.Count} nuevos");
            }
            catch (Exception ex)
            {
                context.ChangeTracker.Clear();
                throw new Exception("❌ Error en Seed merge de Module", ex);
            }
        }

        // =============================
        // 🧩 SEED MERGE: BusinessNature (por Code)
        // =============================
        private static async Task SeedBusinessNaturesMerge(BusinessCentralDbContext context, string fileName)
        {
            var dbSet = context.Set<BusinessNature>();
            var data = await LoadJsonAsync<BusinessNature>(fileName);

            if (data == null || !data.Any())
            {
                Console.WriteLine($"⚠️ {fileName} vacío");
                return;
            }

            var existingCodes = await dbSet.Select(x => x.Code).ToListAsync();
            var existing = new HashSet<string>(
                existingCodes.Where(c => !string.IsNullOrWhiteSpace(c)).Select(c => c.ToLowerInvariant())
            );

            var toInsert = data
                .Where(x => !string.IsNullOrWhiteSpace(x.Code))
                .Where(x => !existing.Contains(x.Code.Trim().ToLowerInvariant()))
                .ToList();

            if (!toInsert.Any())
            {
                Console.WriteLine("⚠️ BusinessNature ya contiene todos los registros del seed");
                return;
            }

            var now = DateTime.UtcNow;
            foreach (var x in toInsert)
            {
                if (x.CreatedAt == default) x.CreatedAt = now;
                if (x.UpdatedAt == default) x.UpdatedAt = now;
            }

            try
            {
                await dbSet.AddRangeAsync(toInsert);
                await context.SaveChangesAsync();
                context.ChangeTracker.Clear();
                Console.WriteLine($"✅ Seed merge BusinessNature insertado: {toInsert.Count} nuevos");
            }
            catch (Exception ex)
            {
                context.ChangeTracker.Clear();
                throw new Exception("❌ Error en Seed merge de BusinessNature", ex);
            }
        }

        private sealed class BusinessNatureModuleSeedRow
        {
            public string NatureCode { get; set; } = string.Empty;
            public string ModuleCode { get; set; } = string.Empty;
            public bool IsDefaultEnabled { get; set; } = true;
            public int SortOrder { get; set; } = 0;
        }

        private sealed class BusinessNatureFulfillmentMethodSeedRow
        {
            public string NatureCode { get; set; } = string.Empty;
            public string MethodCode { get; set; } = string.Empty;
            public bool IsDefaultEnabled { get; set; } = true;
            public int SortOrder { get; set; } = 0;
        }

        private sealed class BusinessNaturePaymentMethodSeedRow
        {
            public string NatureCode { get; set; } = string.Empty;
            public string MethodCode { get; set; } = string.Empty;
            public bool IsDefaultEnabled { get; set; } = true;
            public int SortOrder { get; set; } = 0;
        }

        private sealed class PucAccountSeedRow
        {
            public int CompanyId { get; set; }
            public string Code { get; set; } = string.Empty;
            public string Name { get; set; } = string.Empty;
            public string Nature { get; set; } = "D";
            public int Level { get; set; } = 1;
            public string? ParentCode { get; set; }
            public bool IsAuxiliary { get; set; } = false;
            public bool Active { get; set; } = true;
        }

        // =============================
        // 🧩 SEED MERGE: PUC Accounts (por CompanyId + Code)
        // =============================
        private static async Task SeedPucAccountsMerge(BusinessCentralDbContext context, string fileName)
        {
            List<PucAccountSeedRow> raw;
            try
            {
                raw = await LoadJsonAsync<PucAccountSeedRow>(fileName);
            }
            catch (FileNotFoundException)
            {
                Console.WriteLine($"⚠️ {fileName} no existe (seed opcional)");
                return;
            }

            if (raw == null || !raw.Any())
            {
                Console.WriteLine($"⚠️ {fileName} vacío");
                return;
            }

            // Cache existing per company
            var existing = await context.Set<Account>()
                .Select(a => new { a.CompanyId, a.Code, a.Id })
                .ToListAsync();

            static string Norm(string? s) => (s ?? string.Empty).Trim();

            var existingMap = existing
                .Where(x => !string.IsNullOrWhiteSpace(x.Code))
                .ToDictionary(x => $"{x.CompanyId}|{Norm(x.Code)}", x => x.Id);

            // We'll insert parents before children by sorting by Level then Code length.
            var ordered = raw
                .Where(r => r.CompanyId > 0 && !string.IsNullOrWhiteSpace(r.Code))
                .OrderBy(r => r.Level)
                .ThenBy(r => r.Code.Length)
                .ToList();

            var now = DateTime.UtcNow;
            var inserted = 0;

            foreach (var r in ordered)
            {
                var key = $"{r.CompanyId}|{Norm(r.Code)}";
                if (existingMap.ContainsKey(key))
                    continue;

                long? parentId = null;
                if (!string.IsNullOrWhiteSpace(r.ParentCode))
                {
                    var pKey = $"{r.CompanyId}|{Norm(r.ParentCode)}";
                    if (existingMap.TryGetValue(pKey, out var pid))
                        parentId = pid;
                }

                var entity = new Account
                {
                    CompanyId = r.CompanyId,
                    Code = Norm(r.Code),
                    Name = r.Name?.Trim() ?? string.Empty,
                    Nature = string.IsNullOrWhiteSpace(r.Nature) ? "D" : r.Nature.Trim().ToUpperInvariant(),
                    Level = r.Level <= 0 ? 1 : r.Level,
                    ParentAccountId = parentId,
                    IsAuxiliary = r.IsAuxiliary,
                    Active = r.Active,
                    CreatedAt = now,
                    UpdatedAt = now
                };

                context.Set<Account>().Add(entity);
                await context.SaveChangesAsync();
                context.ChangeTracker.Clear();

                // refresh map with new inserted row: reload id by querying just inserted (safe by unique key)
                var newId = await context.Set<Account>()
                    .Where(a => a.CompanyId == r.CompanyId && a.Code == entity.Code)
                    .Select(a => a.Id)
                    .FirstAsync();

                existingMap[key] = newId;
                inserted++;
            }

            Console.WriteLine($"✅ Seed merge PUC Account insertado: {inserted} nuevos");
        }

        private sealed class StorageLocationSeedRow
        {
            public int CompanyId { get; set; }
            public int? FacilityId { get; set; }
            public string Code { get; set; } = string.Empty;
            public string Name { get; set; } = string.Empty;
            public string Type { get; set; } = "WAREHOUSE";
            public string? ParentCode { get; set; }
            public string? Notes { get; set; }
            public bool Active { get; set; } = true;
        }

        // =============================
        // 🧩 SEED MERGE: StorageLocation (por CompanyId + Code)
        // =============================
        private static async Task SeedStorageLocationsMerge(BusinessCentralDbContext context, string fileName)
        {
            List<StorageLocationSeedRow> raw;
            try
            {
                raw = await LoadJsonAsync<StorageLocationSeedRow>(fileName);
            }
            catch (FileNotFoundException)
            {
                Console.WriteLine($"⚠️ {fileName} no existe (seed opcional)");
                return;
            }

            if (raw == null || !raw.Any())
            {
                Console.WriteLine($"⚠️ {fileName} vacío");
                return;
            }

            static string Norm(string? s) => (s ?? string.Empty).Trim();

            var existing = await context.Set<StorageLocation>()
                .Select(x => new { x.CompanyId, x.Code, x.Id })
                .ToListAsync();

            var map = existing
                .Where(x => !string.IsNullOrWhiteSpace(x.Code))
                .ToDictionary(x => $"{x.CompanyId}|{Norm(x.Code)}", x => x.Id);

            var ordered = raw
                .Where(r => r.CompanyId > 0 && !string.IsNullOrWhiteSpace(r.Code))
                .OrderBy(r => string.IsNullOrWhiteSpace(r.ParentCode) ? 0 : 1)
                .ThenBy(r => r.Code.Length)
                .ToList();

            var now = DateTime.UtcNow;
            var inserted = 0;

            foreach (var r in ordered)
            {
                var key = $"{r.CompanyId}|{Norm(r.Code)}";
                if (map.ContainsKey(key))
                    continue;

                long? parentId = null;
                if (!string.IsNullOrWhiteSpace(r.ParentCode))
                {
                    var pKey = $"{r.CompanyId}|{Norm(r.ParentCode)}";
                    if (map.TryGetValue(pKey, out var pid))
                        parentId = pid;
                }

                var entity = new StorageLocation
                {
                    CompanyId = r.CompanyId,
                    FacilityId = r.FacilityId,
                    Code = Norm(r.Code),
                    Name = r.Name?.Trim() ?? string.Empty,
                    Type = string.IsNullOrWhiteSpace(r.Type) ? "WAREHOUSE" : r.Type.Trim().ToUpperInvariant(),
                    ParentLocationId = parentId,
                    Notes = r.Notes,
                    Active = r.Active,
                    CreatedAt = now,
                    UpdatedAt = now
                };

                context.Set<StorageLocation>().Add(entity);
                await context.SaveChangesAsync();
                context.ChangeTracker.Clear();

                var newId = await context.Set<StorageLocation>()
                    .Where(x => x.CompanyId == r.CompanyId && x.Code == entity.Code)
                    .Select(x => x.Id)
                    .FirstAsync();

                map[key] = newId;
                inserted++;
            }

            Console.WriteLine($"✅ Seed merge StorageLocation insertado: {inserted} nuevos");
        }

        // =============================
        // 🧩 SEED MERGE: BusinessNatureModule (por NatureId + ModuleId)
        // =============================
        private static async Task SeedBusinessNatureModulesMerge(BusinessCentralDbContext context, string fileName)
        {
            List<BusinessNatureModuleSeedRow> raw;
            try
            {
                raw = await LoadJsonAsync<BusinessNatureModuleSeedRow>(fileName);
            }
            catch (FileNotFoundException)
            {
                Console.WriteLine($"⚠️ {fileName} no existe (seed opcional)");
                return;
            }

            if (raw == null || !raw.Any())
            {
                Console.WriteLine($"⚠️ {fileName} vacío");
                return;
            }

            var natureCodeToId = await context.Set<BusinessNature>()
                .Where(x => x.Code != null)
                .ToDictionaryAsync(x => x.Code.ToLowerInvariant(), x => x.Id);

            var moduleCodeToId = await context.Set<Module>()
                .Where(x => x.Code != null)
                .ToDictionaryAsync(x => x.Code!.ToLowerInvariant(), x => x.Id);

            var rows = raw
                .Select(r =>
                {
                    var nk = r.NatureCode?.Trim().ToLowerInvariant();
                    var mk = r.ModuleCode?.Trim().ToLowerInvariant();
                    var natureId = (!string.IsNullOrWhiteSpace(nk) && natureCodeToId.TryGetValue(nk, out var nId)) ? nId : 0;
                    var moduleId = (!string.IsNullOrWhiteSpace(mk) && moduleCodeToId.TryGetValue(mk, out var mId)) ? mId : 0;
                    return new BusinessNatureModule
                    {
                        BusinessNatureId = natureId,
                        ModuleId = moduleId,
                        IsDefaultEnabled = r.IsDefaultEnabled,
                        SortOrder = r.SortOrder,
                        CreatedAt = DateTime.UtcNow
                    };
                })
                .Where(x => x.BusinessNatureId > 0 && x.ModuleId > 0)
                .ToList();

            var dbSet = context.Set<BusinessNatureModule>();
            var existing = await dbSet.Select(x => new { x.BusinessNatureId, x.ModuleId }).ToListAsync();
            var existingSet = new HashSet<string>(existing.Select(x => $"{x.BusinessNatureId}|{x.ModuleId}"));

            var toInsert = rows
                .Where(x => !existingSet.Contains($"{x.BusinessNatureId}|{x.ModuleId}"))
                .ToList();

            if (!toInsert.Any())
            {
                Console.WriteLine("⚠️ BusinessNatureModule ya contiene todos los registros del seed");
                return;
            }

            try
            {
                await dbSet.AddRangeAsync(toInsert);
                await context.SaveChangesAsync();
                context.ChangeTracker.Clear();
                Console.WriteLine($"✅ Seed merge BusinessNatureModule insertado: {toInsert.Count} nuevos");
            }
            catch (Exception ex)
            {
                context.ChangeTracker.Clear();
                throw new Exception("❌ Error en Seed merge de BusinessNatureModule", ex);
            }
        }

        // =============================
        // 🧩 SEED MERGE: FulfillmentMethod (por Code)
        // =============================
        private static async Task SeedFulfillmentMethodsMerge(BusinessCentralDbContext context, string fileName)
        {
            var dbSet = context.Set<FulfillmentMethod>();
            List<FulfillmentMethod> data;
            try
            {
                data = await LoadJsonAsync<FulfillmentMethod>(fileName);
            }
            catch (FileNotFoundException)
            {
                Console.WriteLine($"⚠️ {fileName} no existe (seed opcional)");
                return;
            }

            if (data == null || !data.Any())
            {
                Console.WriteLine($"⚠️ {fileName} vacío");
                return;
            }

            var existingCodes = await dbSet.Select(x => x.Code).ToListAsync();
            var existing = new HashSet<string>(
                existingCodes.Where(c => !string.IsNullOrWhiteSpace(c)).Select(c => c.Trim().ToLowerInvariant())
            );

            var now = DateTime.UtcNow;
            foreach (var x in data)
            {
                if (x.CreatedAt == default) x.CreatedAt = now;
                if (x.UpdatedAt == default) x.UpdatedAt = now;
            }

            var toInsert = data
                .Where(x => !string.IsNullOrWhiteSpace(x.Code))
                .Where(x => !existing.Contains(x.Code.Trim().ToLowerInvariant()))
                .ToList();

            if (!toInsert.Any())
            {
                Console.WriteLine("⚠️ FulfillmentMethod ya contiene todos los registros del seed");
                return;
            }

            await dbSet.AddRangeAsync(toInsert);
            await context.SaveChangesAsync();
            context.ChangeTracker.Clear();
            Console.WriteLine($"✅ Seed merge FulfillmentMethod insertado: {toInsert.Count} nuevos");
        }

        // =============================
        // 🧩 SEED MERGE: BusinessNatureFulfillmentMethod (por NatureId + MethodId)
        // =============================
        private static async Task SeedBusinessNatureFulfillmentMethodsMerge(BusinessCentralDbContext context, string fileName)
        {
            List<BusinessNatureFulfillmentMethodSeedRow> raw;
            try
            {
                raw = await LoadJsonAsync<BusinessNatureFulfillmentMethodSeedRow>(fileName);
            }
            catch (FileNotFoundException)
            {
                Console.WriteLine($"⚠️ {fileName} no existe (seed opcional)");
                return;
            }

            if (raw == null || !raw.Any())
            {
                Console.WriteLine($"⚠️ {fileName} vacío");
                return;
            }

            var natureCodeToId = await context.Set<BusinessNature>()
                .Where(x => x.Code != null)
                .ToDictionaryAsync(x => x.Code.ToLowerInvariant(), x => x.Id);

            var methodCodeToId = await context.Set<FulfillmentMethod>()
                .Where(x => x.Code != null)
                .ToDictionaryAsync(x => x.Code.ToLowerInvariant(), x => x.Id);

            var rows = raw
                .Select(r =>
                {
                    var nk = r.NatureCode?.Trim().ToLowerInvariant();
                    var mk = r.MethodCode?.Trim().ToLowerInvariant();
                    var natureId = (!string.IsNullOrWhiteSpace(nk) && natureCodeToId.TryGetValue(nk, out var nId)) ? nId : 0;
                    var methodId = (!string.IsNullOrWhiteSpace(mk) && methodCodeToId.TryGetValue(mk, out var mId)) ? mId : 0;
                    return new BusinessNatureFulfillmentMethod
                    {
                        BusinessNatureId = natureId,
                        FulfillmentMethodId = methodId,
                        IsDefaultEnabled = r.IsDefaultEnabled,
                        SortOrder = r.SortOrder
                    };
                })
                .Where(x => x.BusinessNatureId > 0 && x.FulfillmentMethodId > 0)
                .ToList();

            var dbSet = context.Set<BusinessNatureFulfillmentMethod>();
            var existing = await dbSet.Select(x => new { x.BusinessNatureId, x.FulfillmentMethodId }).ToListAsync();
            var existingSet = new HashSet<string>(existing.Select(x => $"{x.BusinessNatureId}|{x.FulfillmentMethodId}"));

            var toInsert = rows
                .Where(x => !existingSet.Contains($"{x.BusinessNatureId}|{x.FulfillmentMethodId}"))
                .ToList();

            if (!toInsert.Any())
            {
                Console.WriteLine("⚠️ BusinessNatureFulfillmentMethod ya contiene todos los registros del seed");
                return;
            }

            await dbSet.AddRangeAsync(toInsert);
            await context.SaveChangesAsync();
            context.ChangeTracker.Clear();
            Console.WriteLine($"✅ Seed merge BusinessNatureFulfillmentMethod insertado: {toInsert.Count} nuevos");
        }

        // =============================
        // 🧩 SEED MERGE: PaymentMethod (por Code)
        // =============================
        private static async Task SeedPaymentMethodsMerge(BusinessCentralDbContext context, string fileName)
        {
            var dbSet = context.Set<PaymentMethod>();
            List<PaymentMethod> data;
            try
            {
                data = await LoadJsonAsync<PaymentMethod>(fileName);
            }
            catch (FileNotFoundException)
            {
                Console.WriteLine($"⚠️ {fileName} no existe (seed opcional)");
                return;
            }

            if (data == null || !data.Any())
            {
                Console.WriteLine($"⚠️ {fileName} vacío");
                return;
            }

            var existingCodes = await dbSet.Select(x => x.Code).ToListAsync();
            var existing = new HashSet<string>(
                existingCodes.Where(c => !string.IsNullOrWhiteSpace(c)).Select(c => c.Trim().ToLowerInvariant())
            );

            var now = DateTime.UtcNow;
            foreach (var x in data)
            {
                if (x.CreatedAt == default) x.CreatedAt = now;
                if (x.UpdatedAt == default) x.UpdatedAt = now;
            }

            var toInsert = data
                .Where(x => !string.IsNullOrWhiteSpace(x.Code))
                .Where(x => !existing.Contains(x.Code.Trim().ToLowerInvariant()))
                .ToList();

            if (!toInsert.Any())
            {
                Console.WriteLine("⚠️ PaymentMethod ya contiene todos los registros del seed");
                return;
            }

            await dbSet.AddRangeAsync(toInsert);
            await context.SaveChangesAsync();
            context.ChangeTracker.Clear();
            Console.WriteLine($"✅ Seed merge PaymentMethod insertado: {toInsert.Count} nuevos");
        }

        // =============================
        // 🧩 SEED MERGE: BusinessNaturePaymentMethod (por NatureId + MethodId)
        // =============================
        private static async Task SeedBusinessNaturePaymentMethodsMerge(BusinessCentralDbContext context, string fileName)
        {
            List<BusinessNaturePaymentMethodSeedRow> raw;
            try
            {
                raw = await LoadJsonAsync<BusinessNaturePaymentMethodSeedRow>(fileName);
            }
            catch (FileNotFoundException)
            {
                Console.WriteLine($"⚠️ {fileName} no existe (seed opcional)");
                return;
            }

            if (raw == null || !raw.Any())
            {
                Console.WriteLine($"⚠️ {fileName} vacío");
                return;
            }

            var natureCodeToId = await context.Set<BusinessNature>()
                .Where(x => x.Code != null)
                .ToDictionaryAsync(x => x.Code.ToLowerInvariant(), x => x.Id);

            var methodCodeToId = await context.Set<PaymentMethod>()
                .Where(x => x.Code != null)
                .ToDictionaryAsync(x => x.Code.ToLowerInvariant(), x => x.Id);

            var rows = raw
                .Select(r =>
                {
                    var nk = r.NatureCode?.Trim().ToLowerInvariant();
                    var mk = r.MethodCode?.Trim().ToLowerInvariant();
                    var natureId = (!string.IsNullOrWhiteSpace(nk) && natureCodeToId.TryGetValue(nk, out var nId)) ? nId : 0;
                    var methodId = (!string.IsNullOrWhiteSpace(mk) && methodCodeToId.TryGetValue(mk, out var mId)) ? mId : 0;
                    return new BusinessNaturePaymentMethod
                    {
                        BusinessNatureId = natureId,
                        PaymentMethodId = methodId,
                        IsDefaultEnabled = r.IsDefaultEnabled,
                        SortOrder = r.SortOrder
                    };
                })
                .Where(x => x.BusinessNatureId > 0 && x.PaymentMethodId > 0)
                .ToList();

            var dbSet = context.Set<BusinessNaturePaymentMethod>();
            var existing = await dbSet.Select(x => new { x.BusinessNatureId, x.PaymentMethodId }).ToListAsync();
            var existingSet = new HashSet<string>(existing.Select(x => $"{x.BusinessNatureId}|{x.PaymentMethodId}"));

            var toInsert = rows
                .Where(x => !existingSet.Contains($"{x.BusinessNatureId}|{x.PaymentMethodId}"))
                .ToList();

            if (!toInsert.Any())
            {
                Console.WriteLine("⚠️ BusinessNaturePaymentMethod ya contiene todos los registros del seed");
                return;
            }

            await dbSet.AddRangeAsync(toInsert);
            await context.SaveChangesAsync();
            context.ChangeTracker.Clear();
            Console.WriteLine($"✅ Seed merge BusinessNaturePaymentMethod insertado: {toInsert.Count} nuevos");
        }

        // =============================
        // 🧩 SEED MERGE: Permission (ModuleCode + Code/Name → ModuleId real)
        // =============================
        private static async Task SeedPermissionsMerge(BusinessCentralDbContext context, string fileName)
        {
            var dbSet = context.Set<Permission>();
            var raw = await LoadJsonAsync<PermissionSeedRow>(fileName);

            if (raw == null || !raw.Any())
            {
                Console.WriteLine($"⚠️ {fileName} vacío");
                return;
            }

            var moduleCodeToId = await context.Set<Module>()
                .Where(m => m.Code != null && m.Code != "")
                .ToDictionaryAsync(m => m.Code!.Trim().ToLowerInvariant(), m => m.Id);

            static string Norm(string? s) => (s ?? string.Empty).Trim().ToLowerInvariant();

            var existing = await dbSet
                .Select(p => new { p.ModuleId, p.Code, p.Name })
                .ToListAsync();

            var existingSet = new HashSet<string>(
                existing.Select(x => $"{x.ModuleId}|{Norm(x.Code)}|{Norm(x.Name)}")
            );

            var toInsert = new List<Permission>();
            foreach (var row in raw)
            {
                if (string.IsNullOrWhiteSpace(row.Name))
                    continue;

                int moduleId = 0;
                if (row.ModuleId > 0)
                    moduleId = row.ModuleId;
                else if (!string.IsNullOrWhiteSpace(row.ModuleCode) &&
                         moduleCodeToId.TryGetValue(row.ModuleCode.Trim().ToLowerInvariant(), out var resolved))
                    moduleId = resolved;

                if (moduleId <= 0)
                {
                    Console.WriteLine($"⚠️ Permission seed omitido (ModuleId/Code inválido): {row.Name} ModuleCode={row.ModuleCode}");
                    continue;
                }

                var entity = new Permission
                {
                    ModuleId = moduleId,
                    Name = row.Name.Trim(),
                    Code = string.IsNullOrWhiteSpace(row.Code) ? null : row.Code.Trim()
                };

                if (!existingSet.Contains($"{moduleId}|{Norm(entity.Code)}|{Norm(entity.Name)}"))
                {
                    toInsert.Add(entity);
                    existingSet.Add($"{moduleId}|{Norm(entity.Code)}|{Norm(entity.Name)}");
                }
            }

            if (!toInsert.Any())
            {
                Console.WriteLine("⚠️ Permission ya contiene todos los registros del seed");
                return;
            }

            try
            {
                await dbSet.AddRangeAsync(toInsert);
                await context.SaveChangesAsync();
                context.ChangeTracker.Clear();
                Console.WriteLine($"✅ Seed merge Permission insertado: {toInsert.Count} nuevos");
            }
            catch (Exception ex)
            {
                context.ChangeTracker.Clear();
                throw new Exception("❌ Error en Seed merge de Permission", ex);
            }
        }

        // =============================
        // 🧩 SEED MERGE: RolePermission (RoleName+CompanyId + ModuleCode+PermissionCode)
        // =============================
        private static async Task SeedRolePermissionsMerge(BusinessCentralDbContext context, string fileName)
        {
            var raw = await LoadJsonAsync<RolePermissionSeedRow>(fileName);

            if (raw == null || !raw.Any())
            {
                Console.WriteLine($"⚠️ {fileName} vacío");
                return;
            }

            var dbSet = context.Set<RolePermission>();

            var existingRp = await dbSet
                .Select(rp => new { rp.RoleId, rp.PermissionId })
                .ToListAsync();

            var existingRpSet = new HashSet<string>(
                existingRp.Select(x => $"{x.RoleId}|{x.PermissionId}")
            );

            static string Norm(string? s) => (s ?? string.Empty).Trim().ToLowerInvariant();

            var permRows = await (
                from p in context.Set<Permission>()
                join m in context.Set<Module>() on p.ModuleId equals m.Id
                select new { p.Id, PCode = p.Code ?? "", MCode = m.Code ?? "" }
            ).ToListAsync();

            var permKeyToId = permRows
                .GroupBy(x => $"{Norm(x.MCode)}|{Norm(x.PCode)}")
                .ToDictionary(g => g.Key, g => g.First().Id);

            var roles = await context.Set<Role>().ToListAsync();

            var toInsert = new List<RolePermission>();

            foreach (var r in raw)
            {
                int? roleId = null;
                if (r.RoleId > 0)
                    roleId = r.RoleId;
                else if (r.CompanyId > 0 && !string.IsNullOrWhiteSpace(r.RoleName))
                {
                    var rn = Norm(r.RoleName);
                    roleId = roles.FirstOrDefault(x => x.CompanyId == r.CompanyId && Norm(x.Name) == rn)?.Id;
                }

                if (roleId is null or <= 0)
                {
                    Console.WriteLine($"⚠️ RolePermission seed omitido (rol no encontrado): RoleName={r.RoleName} CompanyId={r.CompanyId}");
                    continue;
                }

                int? permissionId = null;
                if (r.PermissionId > 0)
                    permissionId = r.PermissionId;
                else if (!string.IsNullOrWhiteSpace(r.ModuleCode) && !string.IsNullOrWhiteSpace(r.PermissionCode))
                {
                    var k = $"{Norm(r.ModuleCode)}|{Norm(r.PermissionCode)}";
                    if (permKeyToId.TryGetValue(k, out var pid))
                        permissionId = pid;
                }

                if (permissionId is null or <= 0)
                {
                    Console.WriteLine($"⚠️ RolePermission seed omitido (permiso no encontrado): ModuleCode={r.ModuleCode} PermissionCode={r.PermissionCode}");
                    continue;
                }

                var key = $"{roleId}|{permissionId}";
                if (existingRpSet.Contains(key))
                    continue;

                toInsert.Add(new RolePermission
                {
                    RoleId = roleId.Value,
                    PermissionId = permissionId.Value,
                    IsGranted = r.IsGranted
                });
                existingRpSet.Add(key);
            }

            if (!toInsert.Any())
            {
                Console.WriteLine("⚠️ RolePermission ya contiene todos los registros del seed");
                return;
            }

            try
            {
                await dbSet.AddRangeAsync(toInsert);
                await context.SaveChangesAsync();
                context.ChangeTracker.Clear();
                Console.WriteLine($"✅ Seed merge RolePermission insertado: {toInsert.Count} nuevos");
            }
            catch (Exception ex)
            {
                context.ChangeTracker.Clear();
                throw new Exception("❌ Error en Seed merge de RolePermission", ex);
            }
        }

        // =============================
        // 🧩 SEED MERGE: Role (por CompanyId + Name)
        // =============================
        private static async Task SeedRolesMerge(BusinessCentralDbContext context, string fileName)
        {
            var dbSet = context.Set<Role>();
            var data = await LoadJsonAsync<Role>(fileName);

            if (data == null || !data.Any())
            {
                Console.WriteLine($"⚠️ {fileName} vacío");
                return;
            }

            static string Norm(string? s) => (s ?? string.Empty).Trim().ToLowerInvariant();

            var existing = await dbSet
                .Select(r => new { r.CompanyId, r.Name })
                .ToListAsync();

            var existingSet = new HashSet<string>(
                existing.Select(x => $"{x.CompanyId}|{Norm(x.Name)}")
            );

            var toInsert = data
                .Where(r => r.CompanyId > 0)
                .Where(r => !string.IsNullOrWhiteSpace(r.Name))
                .Where(r => !existingSet.Contains($"{r.CompanyId}|{Norm(r.Name)}"))
                .ToList();

            if (!toInsert.Any())
            {
                Console.WriteLine("⚠️ Role ya contiene todos los registros del seed");
                return;
            }

            try
            {
                await dbSet.AddRangeAsync(toInsert);
                await context.SaveChangesAsync();
                context.ChangeTracker.Clear();
                Console.WriteLine($"✅ Seed merge Role insertado: {toInsert.Count} nuevos");
            }
            catch (Exception ex)
            {
                context.ChangeTracker.Clear();
                throw new Exception("❌ Error en Seed merge de Role", ex);
            }
        }

        // =============================
        // 🧩 SEED MERGE: PlanModule (por MembershipPlanId + ModuleId)
        // =============================
        private static async Task SeedPlanModulesMerge(BusinessCentralDbContext context, string fileName)
        {
            var dbSet = context.Set<PlanModule>();
            var raw = await LoadJsonAsync<PlanModuleSeedRow>(fileName);

            if (raw == null || !raw.Any())
            {
                Console.WriteLine($"⚠️ {fileName} vacío");
                return;
            }

            // Resolver ModuleId por ModuleCode si viene.
            var moduleCodeToId = await context.Set<Module>()
                .Where(m => m.Code != null)
                .ToDictionaryAsync(m => m.Code!.ToLowerInvariant(), m => m.Id);

            var data = raw.Select(r => new PlanModule
            {
                MembershipPlanId = r.MembershipPlanId,
                ModuleId = r.ModuleId > 0
                    ? r.ModuleId
                    : (!string.IsNullOrWhiteSpace(r.ModuleCode) && moduleCodeToId.TryGetValue(r.ModuleCode.Trim().ToLowerInvariant(), out var id) ? id : 0)
            }).ToList();

            var existing = await dbSet
                .Select(pm => new { pm.MembershipPlanId, pm.ModuleId })
                .ToListAsync();

            var existingSet = new HashSet<string>(
                existing.Select(x => $"{x.MembershipPlanId}|{x.ModuleId}")
            );

            var toInsert = data
                .Where(pm => pm.MembershipPlanId > 0 && pm.ModuleId > 0)
                .Where(pm => !existingSet.Contains($"{pm.MembershipPlanId}|{pm.ModuleId}"))
                .ToList();

            if (!toInsert.Any())
            {
                Console.WriteLine("⚠️ PlanModule ya contiene todos los registros del seed");
                return;
            }

            try
            {
                await dbSet.AddRangeAsync(toInsert);
                await context.SaveChangesAsync();
                context.ChangeTracker.Clear();
                Console.WriteLine($"✅ Seed merge PlanModule insertado: {toInsert.Count} nuevos");
            }
            catch (Exception ex)
            {
                context.ChangeTracker.Clear();
                throw new Exception("❌ Error en Seed merge de PlanModule", ex);
            }
        }

        // =============================
        // 🧩 SEED MERGE: CompanyModule (por CompanyId + ModuleId)
        // =============================
        private static async Task SeedCompanyModulesMerge(BusinessCentralDbContext context, string fileName)
        {
            // Si no existe el JSON, lo ignoramos (es opcional).
            List<CompanyModuleSeedRow> raw;
            try
            {
                raw = await LoadJsonAsync<CompanyModuleSeedRow>(fileName);
            }
            catch (FileNotFoundException)
            {
                Console.WriteLine($"⚠️ {fileName} no existe (seed opcional)");
                return;
            }

            if (raw == null || !raw.Any())
            {
                Console.WriteLine($"⚠️ {fileName} vacío");
                return;
            }

            var dbSet = context.Set<CompanyModule>();

            var moduleCodeToId = await context.Set<Module>()
                .Where(m => m.Code != null)
                .ToDictionaryAsync(m => m.Code!.ToLowerInvariant(), m => m.Id);

            var data = raw.Select(r => new CompanyModule
            {
                CompanyId = r.CompanyId,
                ModuleId = r.ModuleId > 0
                    ? r.ModuleId
                    : (!string.IsNullOrWhiteSpace(r.ModuleCode) && moduleCodeToId.TryGetValue(r.ModuleCode.Trim().ToLowerInvariant(), out var id) ? id : 0),
                IsEnabled = r.IsEnabled,
                CreatedAt = r.CreatedAt,
                UpdatedAt = r.UpdatedAt
            }).ToList();

            var existing = await dbSet
                .Select(cm => new { cm.CompanyId, cm.ModuleId })
                .ToListAsync();

            var existingSet = new HashSet<string>(
                existing.Select(x => $"{x.CompanyId}|{x.ModuleId}")
            );

            var toInsert = data
                .Where(cm => cm.CompanyId > 0 && cm.ModuleId > 0)
                .Where(cm => !existingSet.Contains($"{cm.CompanyId}|{cm.ModuleId}"))
                .ToList();

            if (!toInsert.Any())
            {
                Console.WriteLine("⚠️ CompanyModule ya contiene todos los registros del seed");
                return;
            }

            // defaults fechas si vienen vacías
            var now = DateTime.UtcNow;
            foreach (var cm in toInsert)
            {
                if (cm.CreatedAt == default) cm.CreatedAt = now;
                if (cm.UpdatedAt == default) cm.UpdatedAt = now;
            }

            try
            {
                await dbSet.AddRangeAsync(toInsert);
                await context.SaveChangesAsync();
                context.ChangeTracker.Clear();
                Console.WriteLine($"✅ Seed merge CompanyModule insertado: {toInsert.Count} nuevos");
            }
            catch (Exception ex)
            {
                context.ChangeTracker.Clear();
                throw new Exception("❌ Error en Seed merge de CompanyModule", ex);
            }
        }

        private sealed class PermissionSeedRow
        {
            /// <summary>Opcional si se usa ModuleCode.</summary>
            public int ModuleId { get; set; }

            public string? ModuleCode { get; set; }
            public string Name { get; set; } = string.Empty;
            public string? Code { get; set; }
        }

        private sealed class RolePermissionSeedRow
        {
            /// <summary>Modo legacy si RoleName no se usa.</summary>
            public int RoleId { get; set; }

            public int CompanyId { get; set; }
            public string? RoleName { get; set; }

            /// <summary>Modo legacy si ModuleCode+PermissionCode no se usan.</summary>
            public int PermissionId { get; set; }

            public string? ModuleCode { get; set; }
            public string? PermissionCode { get; set; }
            public bool IsGranted { get; set; } = true;
        }

        private sealed class PlanModuleSeedRow
        {
            public int MembershipPlanId { get; set; }
            public int ModuleId { get; set; }
            public string? ModuleCode { get; set; }
        }

        private sealed class CompanyModuleSeedRow
        {
            public int CompanyId { get; set; }
            public int ModuleId { get; set; }
            public string? ModuleCode { get; set; }
            public bool IsEnabled { get; set; } = true;
            public DateTime CreatedAt { get; set; }
            public DateTime UpdatedAt { get; set; }
        }

        private sealed class CompanyBusinessNatureSeedRow
        {
            public int CompanyId { get; set; }
            public string NatureCode { get; set; } = string.Empty;
            public bool IsPrimary { get; set; } = false;
            public DateTime CreatedAt { get; set; }
        }

        private static async Task SeedCompanyBusinessNaturesMerge(BusinessCentralDbContext context, string fileName)
        {
            List<CompanyBusinessNatureSeedRow> raw;
            try
            {
                raw = await LoadJsonAsync<CompanyBusinessNatureSeedRow>(fileName);
            }
            catch (FileNotFoundException)
            {
                Console.WriteLine($"⚠️ {fileName} no existe (seed opcional)");
                return;
            }

            if (raw == null || !raw.Any())
            {
                Console.WriteLine($"⚠️ {fileName} vacío");
                return;
            }

            var natureCodeToId = await context.Set<BusinessNature>()
                .Where(x => x.Code != null)
                .ToDictionaryAsync(x => x.Code.ToLowerInvariant(), x => x.Id);

            var dbSet = context.Set<CompanyBusinessNature>();

            var existing = await dbSet
                .Select(x => new { x.CompanyId, x.BusinessNatureId })
                .ToListAsync();

            var existingSet = new HashSet<string>(existing.Select(x => $"{x.CompanyId}|{x.BusinessNatureId}"));

            var now = DateTime.UtcNow;
            var toInsert = new List<CompanyBusinessNature>();

            foreach (var r in raw)
            {
                var nk = r.NatureCode?.Trim().ToLowerInvariant();
                if (r.CompanyId <= 0 || string.IsNullOrWhiteSpace(nk) || !natureCodeToId.TryGetValue(nk, out var bnId))
                    continue;

                if (existingSet.Contains($"{r.CompanyId}|{bnId}"))
                    continue;

                toInsert.Add(new CompanyBusinessNature
                {
                    CompanyId = r.CompanyId,
                    BusinessNatureId = bnId,
                    IsPrimary = r.IsPrimary,
                    CreatedAt = r.CreatedAt == default ? now : r.CreatedAt
                });
            }

            if (!toInsert.Any())
            {
                Console.WriteLine("⚠️ CompanyBusinessNature ya contiene todos los registros del seed");
                return;
            }

            try
            {
                await dbSet.AddRangeAsync(toInsert);
                await context.SaveChangesAsync();
                context.ChangeTracker.Clear();
                Console.WriteLine($"✅ Seed merge CompanyBusinessNature insertado: {toInsert.Count} nuevos");
            }
            catch (Exception ex)
            {
                context.ChangeTracker.Clear();
                throw new Exception("❌ Error en Seed merge de CompanyBusinessNature", ex);
            }
        }

        // =============================
        // 📦 JSON LOADER
        // =============================
        private static async Task<List<T>> LoadJsonAsync<T>(string fileName)
        {
            var assembly = Assembly.GetExecutingAssembly();

            // Busca el recurso ignorando mayúsculas/minúsculas en el nombre del archivo
            var resourceName = assembly.GetManifestResourceNames()
                .FirstOrDefault(r => r.EndsWith(fileName, StringComparison.OrdinalIgnoreCase));

            if (string.IsNullOrEmpty(resourceName))
            {
                throw new FileNotFoundException($"No se encontró el recurso embebido JSON: {fileName}");
            }

            using var stream = assembly.GetManifestResourceStream(resourceName);
            if (stream == null) return new List<T>();

            using var reader = new StreamReader(stream);
            var json = await reader.ReadToEndAsync();

            // Configuración crítica para el match entre JSON y Entidades C#
            var options = new JsonSerializerOptions
            {

                PropertyNameCaseInsensitive = true, 
                PropertyNamingPolicy = null,
                AllowTrailingCommas = true 
            };

            try
            {
                return JsonSerializer.Deserialize<List<T>>(json, options) ?? new List<T>();
            }
            catch (JsonException ex)
            {
                throw new Exception($"Error de formato en el JSON '{fileName}': {ex.Message}", ex);
            }
        }
    }
}