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
        public static Task SeedAsync(BusinessCentralDbContext context) => SeedAsync(context, seedDemoData: false);

        public static async Task SeedAsync(BusinessCentralDbContext context, bool seedDemoData)
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
            await SeedUsersInfoMerge(context, "users_info.json");
            await SeedUserAddressesMerge(context, "user_addresses.json");

            // --- 7. DATOS DE PRUEBA (DEMO) ---
            // Estos seeds suelen depender de IDs generados (ordenes, tickets, sesiones, etc.).
            // Actívalos solo si necesitas data de demo en un ambiente limpio.
            if (seedDemoData)
            {
                await SeedBusinessNatureModulesMerge(context, "business_nature_modules.json");
                await SeedBusinessNatureFulfillmentMethodsMerge(context, "business_nature_fulfillment_methods.json");
                await SeedBusinessNaturePaymentMethodsMerge(context, "business_nature_payment_methods.json");
                // HR
                await SeedEmployeeProfilesMerge(context, "hr_employee_profiles.json");
                await SeedEntity<PayScheme>(context, "hr_pay_schemes.json");
                await SeedWorkLogsMerge(context, "hr_work_logs.json");
                await SeedLoanAdvancesMerge(context, "hr_loan_advances.json");
                await SeedDeductionsMerge(context, "hr_deductions.json");

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
                await SeedServiceOrderLinesMerge(context, "svc_order_lines.json");

                // COMMERCE / POS
                await SeedEntity<Category>(context, "com_categories.json");
                await SeedEntity<Product>(context, "com_products.json");
                await SeedEntity<Supplier>(context, "com_suppliers.json");
                await SeedEntity<ProductVariant>(context, "com_product_variants.json");
                await SeedCashSessionsMerge(context, "com_cash_sessions.json");
                await SeedEntity<PosTicket>(context, "com_pos_tickets.json");
                await SeedEntity<PosTicketLine>(context, "com_pos_ticket_lines.json");
                await SeedEntity<PosPayment>(context, "com_pos_payments.json");
                await SeedEntity<InventoryMovement>(context, "com_inventory_movements.json");
            }

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
        // 🧩 SEED MERGE: Role (por CompanyId + Name) + UPDATE flags
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
                .Select(r => new { r.Id, r.CompanyId, r.Name, r.IsSystemRole, r.IsSuperUser, r.Active })
                .ToListAsync();

            var existingSet = new HashSet<string>(
                existing.Select(x => $"{x.CompanyId}|{Norm(x.Name)}")
            );

            // Update existing flags (IsSystemRole/IsSuperUser/Active) to match seed
            var seedByKey = data
                // Permite roles globales (CompanyId NULL) y tenant (CompanyId > 0). Excluye CompanyId=0.
                .Where(r => (r.CompanyId == null || r.CompanyId > 0) && !string.IsNullOrWhiteSpace(r.Name))
                .ToDictionary(r => $"{r.CompanyId}|{Norm(r.Name)}", r => r);

            var toUpdate = existing
                .Where(e => seedByKey.ContainsKey($"{e.CompanyId}|{Norm(e.Name)}"))
                .ToList();

            if (toUpdate.Any())
            {
                foreach (var e in toUpdate)
                {
                    var seed = seedByKey[$"{e.CompanyId}|{Norm(e.Name)}"];
                    var entity = await dbSet.FindAsync(e.Id);
                    if (entity == null) continue;
                    entity.IsSystemRole = seed.IsSystemRole;
                    entity.IsSuperUser = seed.IsSuperUser;
                    entity.Active = seed.Active;
                }

                await context.SaveChangesAsync();
                context.ChangeTracker.Clear();
            }

            var toInsert = data
                // Permite roles globales (CompanyId NULL) y tenant (CompanyId > 0). Excluye CompanyId=0.
                .Where(r => r.CompanyId == null || r.CompanyId > 0)
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
        // 🧩 SEED MERGE: UsersInfo (resuelve RoleId por RoleName + CompanyId nullable)
        // =============================
        private sealed class UsersInfoSeedRow
        {
            public int? CompanyId { get; set; }
            public string RoleName { get; set; } = string.Empty;
            public int DocumentTypeId { get; set; }
            public string? DocumentNumber { get; set; }
            public string? FirstName { get; set; }
            public string? LastName { get; set; }
            public string? Email { get; set; }
            public string Phone { get; set; } = string.Empty;
            public string? Password { get; set; }
            public string AuthProvider { get; set; } = "Local";
            public string? ExternalId { get; set; }
            public bool CanLogin { get; set; } = true;
            public bool Active { get; set; } = true;
            public DateTime Create { get; set; }
        }

        private static async Task SeedUsersInfoMerge(BusinessCentralDbContext context, string fileName)
        {
            List<UsersInfoSeedRow> raw;
            try
            {
                raw = await LoadJsonAsync<UsersInfoSeedRow>(fileName);
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

            static string Norm(string? s) => (s ?? string.Empty).Trim().ToLowerInvariant();

            // Cargar roles para resolver RoleId (incluye CompanyId null para SuperUser)
            var roles = await context.Set<Role>()
                .Select(r => new { r.Id, r.CompanyId, r.Name })
                .ToListAsync();

            int? ResolveRoleId(int? companyId, string roleName)
            {
                var rn = Norm(roleName);
                if (string.IsNullOrWhiteSpace(rn)) return null;

                // match exacto por CompanyId y name
                var match = roles.FirstOrDefault(r => r.CompanyId == companyId && Norm(r.Name) == rn);
                if (match != null) return match.Id;

                // fallback: rol global (CompanyId null) con mismo name
                match = roles.FirstOrDefault(r => r.CompanyId == null && Norm(r.Name) == rn);
                return match?.Id;
            }

            var dbSet = context.Set<UsersInfo>();

            // Existentes: usamos Email/Doc/Phone como keys. Para rows de sistema (CompanyId NULL)
            // permitimos match por Email/Doc/Phone ignorando CompanyId para poder "corregir" un superuser
            // previamente creado con CompanyId incorrecto.
            var existing = await dbSet
                .Select(u => new
                {
                    u.Id,
                    u.CompanyId,
                    u.Email,
                    u.DocumentNumber,
                    u.Phone,
                    u.RoleId,
                    u.CanLogin,
                    u.Active,
                    u.AuthProvider,
                    u.ExternalId,
                    u.Password
                })
                .ToListAsync();

            bool Exists(int? companyId, UsersInfoSeedRow row)
            {
                var email = Norm(row.Email);
                var doc = Norm(row.DocumentNumber);
                var phone = Norm(row.Phone);

                return existing.Any(e =>
                    e.CompanyId == companyId &&
                    (
                        (!string.IsNullOrWhiteSpace(email) && Norm(e.Email) == email) ||
                        (!string.IsNullOrWhiteSpace(doc) && Norm(e.DocumentNumber) == doc) ||
                        (!string.IsNullOrWhiteSpace(phone) && Norm(e.Phone) == phone)
                    )
                );
            }

            int? FindExistingIdForUpsert(int? companyId, UsersInfoSeedRow row)
            {
                var email = Norm(row.Email);
                var doc = Norm(row.DocumentNumber);
                var phone = Norm(row.Phone);

                // match normal: mismo CompanyId
                var match = existing.FirstOrDefault(e =>
                    e.CompanyId == companyId &&
                    (
                        (!string.IsNullOrWhiteSpace(email) && Norm(e.Email) == email) ||
                        (!string.IsNullOrWhiteSpace(doc) && Norm(e.DocumentNumber) == doc) ||
                        (!string.IsNullOrWhiteSpace(phone) && Norm(e.Phone) == phone)
                    ));
                if (match != null) return match.Id;

                // match sistema: CompanyId NULL -> permitimos ignorar CompanyId (pero exige Email o Phone)
                if (companyId == null)
                {
                    match = existing.FirstOrDefault(e =>
                        (
                            (!string.IsNullOrWhiteSpace(email) && Norm(e.Email) == email) ||
                            (!string.IsNullOrWhiteSpace(phone) && Norm(e.Phone) == phone) ||
                            (!string.IsNullOrWhiteSpace(doc) && Norm(e.DocumentNumber) == doc)
                        ));
                    return match?.Id;
                }

                return null;
            }

            var now = DateTime.UtcNow;
            var toInsert = new List<UsersInfo>();
            var toUpdate = new List<UsersInfo>();

            foreach (var row in raw)
            {
                if (string.IsNullOrWhiteSpace(row.Phone))
                {
                    Console.WriteLine("⚠️ UsersInfo seed omitido (Phone requerido).");
                    continue;
                }

                var roleId = ResolveRoleId(row.CompanyId, row.RoleName);
                if (roleId == null || roleId <= 0)
                {
                    Console.WriteLine($"⚠️ UsersInfo seed omitido (rol no encontrado): CompanyId={row.CompanyId?.ToString() ?? "NULL"} RoleName={row.RoleName}");
                    continue;
                }

                var existingId = FindExistingIdForUpsert(row.CompanyId, row);
                if (existingId != null && existingId > 0)
                {
                    // Upsert: corrige registros existentes (ej. superuser con CompanyId incorrecto)
                    var entity = await dbSet.FirstAsync(u => u.Id == existingId.Value);

                    entity.CompanyId = row.CompanyId;
                    entity.RoleId = roleId.Value;
                    entity.DocumentTypeId = row.DocumentTypeId;
                    entity.DocumentNumber = row.DocumentNumber;
                    entity.FirstName = row.FirstName;
                    entity.LastName = row.LastName;
                    entity.Email = row.Email;
                    entity.Phone = row.Phone;
                    entity.Password = row.Password;
                    entity.AuthProvider = string.IsNullOrWhiteSpace(row.AuthProvider) ? "Local" : row.AuthProvider;
                    entity.ExternalId = row.ExternalId;
                    entity.CanLogin = row.CanLogin;
                    entity.Active = row.Active;
                    entity.Updated = now;

                    toUpdate.Add(entity);
                    continue;
                }

                toInsert.Add(new UsersInfo
                {
                    CompanyId = row.CompanyId,
                    RoleId = roleId.Value,
                    DocumentTypeId = row.DocumentTypeId,
                    DocumentNumber = row.DocumentNumber,
                    FirstName = row.FirstName,
                    LastName = row.LastName,
                    Email = row.Email,
                    Phone = row.Phone,
                    Password = row.Password,
                    AuthProvider = string.IsNullOrWhiteSpace(row.AuthProvider) ? "Local" : row.AuthProvider,
                    ExternalId = row.ExternalId,
                    CanLogin = row.CanLogin,
                    Active = row.Active,
                    Created = row.Create == default ? now : row.Create,
                    Updated = row.Create == default ? now : row.Create
                });
            }

            if (!toInsert.Any() && !toUpdate.Any())
            {
                Console.WriteLine("⚠️ UsersInfo ya contiene todos los registros del seed");
                return;
            }

            try
            {
                if (toInsert.Any())
                    await dbSet.AddRangeAsync(toInsert);

                await context.SaveChangesAsync();
                context.ChangeTracker.Clear();
                Console.WriteLine($"✅ Seed merge UsersInfo: {toInsert.Count} nuevos, {toUpdate.Count} actualizados");
            }
            catch (Exception ex)
            {
                context.ChangeTracker.Clear();
                throw new Exception("❌ Error en Seed merge de UsersInfo", ex);
            }
        }

        // =============================
        // 🧩 SEED MERGE: UserAddress (resuelve UserInfoId por CompanyId + Email/Doc/Phone)
        // =============================
        private sealed class UserAddressSeedRow
        {
            public int? CompanyId { get; set; } = null;
            public string? UserEmail { get; set; } = null;
            public string? UserDocumentNumber { get; set; } = null;
            public string? UserPhone { get; set; } = null;

            // legacy
            public int UserInfoId { get; set; }

            public string Address { get; set; } = string.Empty;
            public string? Alias { get; set; }
            public double? Latitude { get; set; }
            public double? Longitude { get; set; }
            public string? Place_id { get; set; }
            public string? PlaceId { get; set; }
            public int CityId { get; set; }
            public bool IsMain { get; set; } = false;
        }

        private static async Task SeedUserAddressesMerge(BusinessCentralDbContext context, string fileName)
        {
            List<UserAddressSeedRow> raw;
            try
            {
                raw = await LoadJsonAsync<UserAddressSeedRow>(fileName);
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

            static string Norm(string? s) => (s ?? string.Empty).Trim().ToLowerInvariant();

            var users = await context.Set<UsersInfo>()
                .Select(u => new { u.Id, u.CompanyId, u.Email, u.DocumentNumber, u.Phone })
                .ToListAsync();

            int? ResolveUserId(UserAddressSeedRow row)
            {
                // Preferente: por CompanyId + Email/Doc/Phone
                var email = Norm(row.UserEmail);
                var doc = Norm(row.UserDocumentNumber);
                var phone = Norm(row.UserPhone);

                if (!string.IsNullOrWhiteSpace(email) || !string.IsNullOrWhiteSpace(doc) || !string.IsNullOrWhiteSpace(phone))
                {
                    var match = users.FirstOrDefault(u =>
                        u.CompanyId == row.CompanyId &&
                        (
                            (!string.IsNullOrWhiteSpace(email) && Norm(u.Email) == email) ||
                            (!string.IsNullOrWhiteSpace(doc) && Norm(u.DocumentNumber) == doc) ||
                            (!string.IsNullOrWhiteSpace(phone) && Norm(u.Phone) == phone)
                        ));
                    return match?.Id;
                }

                // Legacy: usa UserInfoId
                return row.UserInfoId > 0 ? row.UserInfoId : null;
            }

            var dbSet = context.Set<UserAddress>();

            // key para evitar duplicados: UserInfoId + Address + CityId
            var existing = await dbSet
                .Select(a => new { a.UserInfoId, a.Address, a.CityId })
                .ToListAsync();

            var existingSet = new HashSet<string>(
                existing.Select(x => $"{x.UserInfoId}|{Norm(x.Address)}|{x.CityId}")
            );

            var toInsert = new List<UserAddress>();

            foreach (var row in raw)
            {
                var userId = ResolveUserId(row);
                if (userId == null || userId <= 0)
                {
                    Console.WriteLine($"⚠️ UserAddress seed omitido (usuario no encontrado). CompanyId={row.CompanyId?.ToString() ?? "NULL"} Email={row.UserEmail}");
                    continue;
                }

                if (string.IsNullOrWhiteSpace(row.Address) || row.CityId <= 0)
                {
                    Console.WriteLine("⚠️ UserAddress seed omitido (Address/CityId requeridos).");
                    continue;
                }

                var k = $"{userId}|{Norm(row.Address)}|{row.CityId}";
                if (existingSet.Contains(k))
                    continue;

                toInsert.Add(new UserAddress
                {
                    UserInfoId = userId.Value,
                    Address = row.Address,
                    Alias = row.Alias,
                    Latitude = row.Latitude,
                    Longitude = row.Longitude,
                    PlaceId = row.PlaceId ?? row.Place_id,
                    CityId = row.CityId,
                    IsMain = row.IsMain
                });
            }

            if (!toInsert.Any())
            {
                Console.WriteLine("⚠️ UserAddress ya contiene todos los registros del seed");
                return;
            }

            try
            {
                await dbSet.AddRangeAsync(toInsert);
                await context.SaveChangesAsync();
                context.ChangeTracker.Clear();
                Console.WriteLine($"✅ Seed merge UserAddress insertado: {toInsert.Count} nuevos");
            }
            catch (Exception ex)
            {
                context.ChangeTracker.Clear();
                throw new Exception("❌ Error en Seed merge de UserAddress", ex);
            }
        }

        // =============================
        // 🧩 SEED MERGE: EmployeeProfile (resuelve UserId por CompanyId + Email/Doc/Phone)
        // =============================
        private sealed class EmployeeProfileSeedRow
        {
            public int CompanyId { get; set; }
            public string? UserEmail { get; set; }
            public string? UserDocumentNumber { get; set; }
            public string? UserPhone { get; set; }
            // legacy
            public int UserId { get; set; }

            public bool IsEmployee { get; set; } = true;
            public bool ActiveEmployee { get; set; } = true;
            public DateTime? HireDate { get; set; }
            public bool LodgingIncluded { get; set; }
            public string? LodgingLocation { get; set; }
            public bool MattressIncluded { get; set; }
            public string? MealPlanCode { get; set; }
            public decimal? MealUnitCost { get; set; }
            public DateTime CreatedAt { get; set; }
            public DateTime UpdatedAt { get; set; }
        }

        private static async Task SeedEmployeeProfilesMerge(BusinessCentralDbContext context, string fileName)
        {
            List<EmployeeProfileSeedRow> raw;
            try
            {
                raw = await LoadJsonAsync<EmployeeProfileSeedRow>(fileName);
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

            static string Norm(string? s) => (s ?? string.Empty).Trim().ToLowerInvariant();

            var users = await context.Set<UsersInfo>()
                .Select(u => new { u.Id, u.CompanyId, u.Email, u.DocumentNumber, u.Phone })
                .ToListAsync();

            int? ResolveUserId(EmployeeProfileSeedRow row)
            {
                var email = Norm(row.UserEmail);
                var doc = Norm(row.UserDocumentNumber);
                var phone = Norm(row.UserPhone);

                if (!string.IsNullOrWhiteSpace(email) || !string.IsNullOrWhiteSpace(doc) || !string.IsNullOrWhiteSpace(phone))
                {
                    var match = users.FirstOrDefault(u =>
                        u.CompanyId == row.CompanyId &&
                        (
                            (!string.IsNullOrWhiteSpace(email) && Norm(u.Email) == email) ||
                            (!string.IsNullOrWhiteSpace(doc) && Norm(u.DocumentNumber) == doc) ||
                            (!string.IsNullOrWhiteSpace(phone) && Norm(u.Phone) == phone)
                        ));
                    return match?.Id;
                }

                return row.UserId > 0 ? row.UserId : null;
            }

            var dbSet = context.Set<EmployeeProfile>();

            var existing = await dbSet
                .Select(e => new { e.UserId })
                .ToListAsync();

            var existingSet = new HashSet<int>(existing.Select(x => x.UserId));
            var toInsert = new List<EmployeeProfile>();
            var now = DateTime.UtcNow;

            foreach (var row in raw)
            {
                if (row.CompanyId <= 0)
                {
                    Console.WriteLine("⚠️ EmployeeProfile seed omitido (CompanyId requerido).");
                    continue;
                }

                var userId = ResolveUserId(row);
                if (userId == null || userId <= 0)
                {
                    Console.WriteLine($"⚠️ EmployeeProfile seed omitido (usuario no encontrado). CompanyId={row.CompanyId} Email={row.UserEmail}");
                    continue;
                }

                if (existingSet.Contains(userId.Value))
                    continue;

                toInsert.Add(new EmployeeProfile
                {
                    UserId = userId.Value,
                    CompanyId = row.CompanyId,
                    IsEmployee = row.IsEmployee,
                    ActiveEmployee = row.ActiveEmployee,
                    HireDate = row.HireDate,
                    LodgingIncluded = row.LodgingIncluded,
                    LodgingLocation = row.LodgingLocation,
                    MattressIncluded = row.MattressIncluded,
                    MealPlanCode = row.MealPlanCode,
                    MealUnitCost = row.MealUnitCost,
                    CreatedAt = row.CreatedAt == default ? now : row.CreatedAt,
                    UpdatedAt = row.UpdatedAt == default ? now : row.UpdatedAt
                });
            }

            if (!toInsert.Any())
            {
                Console.WriteLine("⚠️ EmployeeProfile ya contiene todos los registros del seed");
                return;
            }

            try
            {
                await dbSet.AddRangeAsync(toInsert);
                await context.SaveChangesAsync();
                context.ChangeTracker.Clear();
                Console.WriteLine($"✅ Seed merge EmployeeProfile insertado: {toInsert.Count} nuevos");
            }
            catch (Exception ex)
            {
                context.ChangeTracker.Clear();
                throw new Exception("❌ Error en Seed merge de EmployeeProfile", ex);
            }
        }

        private class HrUserSeedRowBase
        {
            public int CompanyId { get; set; }
            public string? UserEmail { get; set; }
            public string? UserDocumentNumber { get; set; }
            public string? UserPhone { get; set; }
            public int UserId { get; set; } // legacy
        }

        private static async Task<int?> ResolveUserIdForHrAsync(BusinessCentralDbContext context, int companyId, string? email, string? doc, string? phone, int legacyUserId)
        {
            static string Norm(string? s) => (s ?? string.Empty).Trim().ToLowerInvariant();
            var e = Norm(email);
            var d = Norm(doc);
            var p = Norm(phone);

            if (!string.IsNullOrWhiteSpace(e) || !string.IsNullOrWhiteSpace(d) || !string.IsNullOrWhiteSpace(p))
            {
                var match = await context.Set<UsersInfo>()
                    .Where(u => u.CompanyId == companyId)
                    .Where(u =>
                        (!string.IsNullOrWhiteSpace(e) && (u.Email ?? "").ToLower() == e) ||
                        (!string.IsNullOrWhiteSpace(d) && (u.DocumentNumber ?? "").ToLower() == d) ||
                        (!string.IsNullOrWhiteSpace(p) && (u.Phone ?? "").ToLower() == p)
                    )
                    .Select(u => (int?)u.Id)
                    .FirstOrDefaultAsync();
                return match;
            }

            return legacyUserId > 0 ? legacyUserId : null;
        }

        private sealed class WorkLogSeedRow : HrUserSeedRowBase
        {
            public DateTime WorkDate { get; set; }
            public int PaySchemeId { get; set; }
            public decimal Quantity { get; set; } = 1;
            public string? Unit { get; set; }
            public string? ReferenceType { get; set; }
            public string? ReferenceId { get; set; }
            public string? Notes { get; set; }
            public DateTime CreatedAt { get; set; }
        }

        private static async Task SeedWorkLogsMerge(BusinessCentralDbContext context, string fileName)
        {
            List<WorkLogSeedRow> raw;
            try { raw = await LoadJsonAsync<WorkLogSeedRow>(fileName); }
            catch (FileNotFoundException) { Console.WriteLine($"⚠️ {fileName} no existe (seed opcional)"); return; }
            if (raw == null || !raw.Any()) { Console.WriteLine($"⚠️ {fileName} vacío"); return; }

            var dbSet = context.Set<WorkLog>();
            var existing = await dbSet.Select(w => new { w.UserId, w.WorkDate, w.PaySchemeId }).ToListAsync();
            var existingSet = new HashSet<string>(existing.Select(x => $"{x.UserId}|{x.WorkDate:O}|{x.PaySchemeId}"));
            var toInsert = new List<WorkLog>();

            foreach (var row in raw)
            {
                var userId = await ResolveUserIdForHrAsync(context, row.CompanyId, row.UserEmail, row.UserDocumentNumber, row.UserPhone, row.UserId);
                if (userId == null || userId <= 0) continue;
                if (row.PaySchemeId <= 0) continue;
                var k = $"{userId.Value}|{row.WorkDate:O}|{row.PaySchemeId}";
                if (existingSet.Contains(k)) continue;
                toInsert.Add(new WorkLog
                {
                    CompanyId = row.CompanyId,
                    UserId = userId.Value,
                    WorkDate = row.WorkDate,
                    PaySchemeId = row.PaySchemeId,
                    Quantity = row.Quantity,
                    Unit = row.Unit,
                    ReferenceType = row.ReferenceType,
                    ReferenceId = row.ReferenceId,
                    Notes = row.Notes,
                    CreatedAt = row.CreatedAt == default ? DateTime.UtcNow : row.CreatedAt
                });
            }

            if (!toInsert.Any()) { Console.WriteLine("⚠️ WorkLog ya contiene todos los registros del seed"); return; }
            await dbSet.AddRangeAsync(toInsert);
            await context.SaveChangesAsync();
            context.ChangeTracker.Clear();
        }

        private sealed class LoanAdvanceSeedRow : HrUserSeedRowBase
        {
            public DateTime Date { get; set; }
            public decimal Amount { get; set; }
            public string? Notes { get; set; }
            public string? Status { get; set; }
            public DateTime CreatedAt { get; set; }
        }

        private static async Task SeedLoanAdvancesMerge(BusinessCentralDbContext context, string fileName)
        {
            List<LoanAdvanceSeedRow> raw;
            try { raw = await LoadJsonAsync<LoanAdvanceSeedRow>(fileName); }
            catch (FileNotFoundException) { Console.WriteLine($"⚠️ {fileName} no existe (seed opcional)"); return; }
            if (raw == null || !raw.Any()) { Console.WriteLine($"⚠️ {fileName} vacío"); return; }

            var dbSet = context.Set<LoanAdvance>();
            var existing = await dbSet.Select(w => new { w.UserId, w.Date, w.Amount }).ToListAsync();
            var existingSet = new HashSet<string>(existing.Select(x => $"{x.UserId}|{x.Date:O}|{x.Amount}"));
            var toInsert = new List<LoanAdvance>();

            foreach (var row in raw)
            {
                var userId = await ResolveUserIdForHrAsync(context, row.CompanyId, row.UserEmail, row.UserDocumentNumber, row.UserPhone, row.UserId);
                if (userId == null || userId <= 0) continue;
                var k = $"{userId.Value}|{row.Date:O}|{row.Amount}";
                if (existingSet.Contains(k)) continue;
                toInsert.Add(new LoanAdvance
                {
                    CompanyId = row.CompanyId,
                    UserId = userId.Value,
                    Date = row.Date,
                    Amount = row.Amount,
                    Notes = row.Notes,
                    Status = string.IsNullOrWhiteSpace(row.Status) ? "open" : row.Status!,
                    CreatedAt = row.CreatedAt == default ? DateTime.UtcNow : row.CreatedAt
                });
            }

            if (!toInsert.Any()) { Console.WriteLine("⚠️ LoanAdvance ya contiene todos los registros del seed"); return; }
            await dbSet.AddRangeAsync(toInsert);
            await context.SaveChangesAsync();
            context.ChangeTracker.Clear();
        }

        private sealed class DeductionSeedRow : HrUserSeedRowBase
        {
            public DateTime Date { get; set; }
            public decimal Amount { get; set; }
            public string? Type { get; set; }
            public string? Notes { get; set; }
            public DateTime CreatedAt { get; set; }
        }

        private static async Task SeedDeductionsMerge(BusinessCentralDbContext context, string fileName)
        {
            List<DeductionSeedRow> raw;
            try { raw = await LoadJsonAsync<DeductionSeedRow>(fileName); }
            catch (FileNotFoundException) { Console.WriteLine($"⚠️ {fileName} no existe (seed opcional)"); return; }
            if (raw == null || !raw.Any()) { Console.WriteLine($"⚠️ {fileName} vacío"); return; }

            var dbSet = context.Set<Deduction>();
            var existing = await dbSet.Select(w => new { w.UserId, w.Date, w.Amount }).ToListAsync();
            var existingSet = new HashSet<string>(existing.Select(x => $"{x.UserId}|{x.Date:O}|{x.Amount}"));
            var toInsert = new List<Deduction>();

            foreach (var row in raw)
            {
                var userId = await ResolveUserIdForHrAsync(context, row.CompanyId, row.UserEmail, row.UserDocumentNumber, row.UserPhone, row.UserId);
                if (userId == null || userId <= 0) continue;
                var k = $"{userId.Value}|{row.Date:O}|{row.Amount}";
                if (existingSet.Contains(k)) continue;
                toInsert.Add(new Deduction
                {
                    CompanyId = row.CompanyId,
                    UserId = userId.Value,
                    Date = row.Date,
                    Amount = row.Amount,
                    Type = row.Type,
                    Notes = row.Notes,
                    CreatedAt = row.CreatedAt == default ? DateTime.UtcNow : row.CreatedAt
                });
            }

            if (!toInsert.Any()) { Console.WriteLine("⚠️ Deduction ya contiene todos los registros del seed"); return; }
            await dbSet.AddRangeAsync(toInsert);
            await context.SaveChangesAsync();
            context.ChangeTracker.Clear();
        }

        // =============================
        // 🧩 SEED MERGE: ServiceOrderLine (resuelve EmployeeUserId por CompanyId + Email/Doc/Phone)
        // =============================
        private sealed class ServiceOrderLineSeedRow
        {
            public int CompanyId { get; set; }
            public long OrderId { get; set; }
            public int ServiceId { get; set; }
            public decimal Quantity { get; set; } = 1;
            public decimal UnitPrice { get; set; }

            public int? EmployeeUserId { get; set; } // legacy
            public string? EmployeeEmail { get; set; }
            public string? EmployeeDocumentNumber { get; set; }
            public string? EmployeePhone { get; set; }
        }

        private static async Task SeedServiceOrderLinesMerge(BusinessCentralDbContext context, string fileName)
        {
            List<ServiceOrderLineSeedRow> raw;
            try { raw = await LoadJsonAsync<ServiceOrderLineSeedRow>(fileName); }
            catch (FileNotFoundException) { Console.WriteLine($"⚠️ {fileName} no existe (seed opcional)"); return; }
            if (raw == null || !raw.Any()) { Console.WriteLine($"⚠️ {fileName} vacío"); return; }

            static string Norm(string? s) => (s ?? string.Empty).Trim().ToLowerInvariant();

            var users = await context.Set<UsersInfo>()
                .Select(u => new { u.Id, u.CompanyId, u.Email, u.DocumentNumber, u.Phone })
                .ToListAsync();

            int? ResolveEmployeeId(ServiceOrderLineSeedRow row)
            {
                var email = Norm(row.EmployeeEmail);
                var doc = Norm(row.EmployeeDocumentNumber);
                var phone = Norm(row.EmployeePhone);

                if (!string.IsNullOrWhiteSpace(email) || !string.IsNullOrWhiteSpace(doc) || !string.IsNullOrWhiteSpace(phone))
                {
                    var match = users.FirstOrDefault(u =>
                        u.CompanyId == row.CompanyId &&
                        (
                            (!string.IsNullOrWhiteSpace(email) && Norm(u.Email) == email) ||
                            (!string.IsNullOrWhiteSpace(doc) && Norm(u.DocumentNumber) == doc) ||
                            (!string.IsNullOrWhiteSpace(phone) && Norm(u.Phone) == phone)
                        ));
                    return match?.Id;
                }

                return row.EmployeeUserId;
            }

            var dbSet = context.Set<ServiceOrderLine>();
            var existing = await dbSet.Select(l => new { l.CompanyId, l.OrderId, l.ServiceId }).ToListAsync();
            var existingSet = new HashSet<string>(existing.Select(x => $"{x.CompanyId}|{x.OrderId}|{x.ServiceId}"));

            var toInsert = new List<ServiceOrderLine>();
            foreach (var row in raw)
            {
                if (row.CompanyId <= 0 || row.OrderId <= 0 || row.ServiceId <= 0) continue;
                var k = $"{row.CompanyId}|{row.OrderId}|{row.ServiceId}";
                if (existingSet.Contains(k)) continue;

                var empId = ResolveEmployeeId(row);
                toInsert.Add(new ServiceOrderLine
                {
                    CompanyId = row.CompanyId,
                    OrderId = row.OrderId,
                    ServiceId = row.ServiceId,
                    Quantity = row.Quantity,
                    UnitPrice = row.UnitPrice,
                    EmployeeUserId = empId
                });
            }

            if (!toInsert.Any()) { Console.WriteLine("⚠️ ServiceOrderLine ya contiene todos los registros del seed"); return; }
            await dbSet.AddRangeAsync(toInsert);
            await context.SaveChangesAsync();
            context.ChangeTracker.Clear();
        }

        // =============================
        // 🧩 SEED MERGE: CashSession (resuelve OpenedByUserId por CompanyId + Email/Doc/Phone)
        // =============================
        private sealed class CashSessionSeedRow
        {
            public int CompanyId { get; set; }
            public DateTime OpenedAt { get; set; }
            public DateTime? ClosedAt { get; set; }

            public int? OpenedByUserId { get; set; } // legacy
            public string? OpenedByUserEmail { get; set; }
            public string? OpenedByUserDocumentNumber { get; set; }
            public string? OpenedByUserPhone { get; set; }

            public int? ClosedByUserId { get; set; } // legacy
            public string? ClosedByUserEmail { get; set; }
            public string? ClosedByUserDocumentNumber { get; set; }
            public string? ClosedByUserPhone { get; set; }

            public string Status { get; set; } = "open";
            public decimal OpeningAmount { get; set; }
            public decimal? CountedClosingAmount { get; set; }
            public decimal? ExpectedClosingAmount { get; set; }
            public decimal? DifferenceAmount { get; set; }
        }

        private static async Task SeedCashSessionsMerge(BusinessCentralDbContext context, string fileName)
        {
            List<CashSessionSeedRow> raw;
            try { raw = await LoadJsonAsync<CashSessionSeedRow>(fileName); }
            catch (FileNotFoundException) { Console.WriteLine($"⚠️ {fileName} no existe (seed opcional)"); return; }
            if (raw == null || !raw.Any()) { Console.WriteLine($"⚠️ {fileName} vacío"); return; }

            static string Norm(string? s) => (s ?? string.Empty).Trim().ToLowerInvariant();

            var users = await context.Set<UsersInfo>()
                .Select(u => new { u.Id, u.CompanyId, u.Email, u.DocumentNumber, u.Phone })
                .ToListAsync();

            int? ResolveUserId(int companyId, string? email, string? doc, string? phone, int? legacyId)
            {
                var e = Norm(email);
                var d = Norm(doc);
                var p = Norm(phone);
                if (!string.IsNullOrWhiteSpace(e) || !string.IsNullOrWhiteSpace(d) || !string.IsNullOrWhiteSpace(p))
                {
                    var match = users.FirstOrDefault(u =>
                        u.CompanyId == companyId &&
                        (
                            (!string.IsNullOrWhiteSpace(e) && Norm(u.Email) == e) ||
                            (!string.IsNullOrWhiteSpace(d) && Norm(u.DocumentNumber) == d) ||
                            (!string.IsNullOrWhiteSpace(p) && Norm(u.Phone) == p)
                        ));
                    return match?.Id;
                }
                return legacyId;
            }

            var dbSet = context.Set<CashSession>();
            var existing = await dbSet.Select(cs => new { cs.CompanyId, cs.OpenedAt }).ToListAsync();
            var existingSet = new HashSet<string>(existing.Select(x => $"{x.CompanyId}|{x.OpenedAt:O}"));

            var toInsert = new List<CashSession>();
            foreach (var row in raw)
            {
                if (row.CompanyId <= 0) continue;
                var k = $"{row.CompanyId}|{row.OpenedAt:O}";
                if (existingSet.Contains(k)) continue;

                var openedBy = ResolveUserId(row.CompanyId, row.OpenedByUserEmail, row.OpenedByUserDocumentNumber, row.OpenedByUserPhone, row.OpenedByUserId);
                var closedBy = ResolveUserId(row.CompanyId, row.ClosedByUserEmail, row.ClosedByUserDocumentNumber, row.ClosedByUserPhone, row.ClosedByUserId);

                toInsert.Add(new CashSession
                {
                    CompanyId = row.CompanyId,
                    OpenedAt = row.OpenedAt,
                    ClosedAt = row.ClosedAt,
                    OpenedByUserId = openedBy,
                    ClosedByUserId = closedBy,
                    Status = string.IsNullOrWhiteSpace(row.Status) ? "open" : row.Status,
                    OpeningAmount = row.OpeningAmount,
                    CountedClosingAmount = row.CountedClosingAmount,
                    ExpectedClosingAmount = row.ExpectedClosingAmount,
                    DifferenceAmount = row.DifferenceAmount
                });
            }

            if (!toInsert.Any()) { Console.WriteLine("⚠️ CashSession ya contiene todos los registros del seed"); return; }
            await dbSet.AddRangeAsync(toInsert);
            await context.SaveChangesAsync();
            context.ChangeTracker.Clear();
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