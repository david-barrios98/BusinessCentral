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

            // --- 5. SEGURIDAD DETALLADA (Dependen de Roles/Permissions) ---
            await SeedEntity<RolePermission>(context, "role_permissions.json");

            // --- 6. USUARIOS Y SESIONES (Nivel Final - Dependen de Companies/Roles) ---
            await SeedEntity<UsersInfo>(context, "users_info.json");
            await SeedEntity<UserAddress>(context, "user_addresses.json");

            // --- 7. DATOS DE PRUEBA (módulos) ---
            await SeedBusinessNatureModulesMerge(context, "business_nature_modules.json");
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
            await SeedEntity<CashSession>(context, "com_cash_sessions.json");
            await SeedEntity<PosTicket>(context, "com_pos_tickets.json");
            await SeedEntity<PosTicketLine>(context, "com_pos_ticket_lines.json");
            await SeedEntity<PosPayment>(context, "com_pos_payments.json");
            await SeedEntity<InventoryMovement>(context, "com_inventory_movements.json");

            // FIN (PUC)
            await SeedPucAccountsMerge(context, "fin_puc_accounts.json");
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
        // 🧩 SEED MERGE: Permission (por ModuleId + Code/Name)
        // =============================
        private static async Task SeedPermissionsMerge(BusinessCentralDbContext context, string fileName)
        {
            var dbSet = context.Set<Permission>();
            var data = await LoadJsonAsync<Permission>(fileName);

            if (data == null || !data.Any())
            {
                Console.WriteLine($"⚠️ {fileName} vacío");
                return;
            }

            var existing = await dbSet
                .Select(p => new { p.ModuleId, p.Code, p.Name })
                .ToListAsync();

            static string Norm(string? s) => (s ?? string.Empty).Trim().ToLowerInvariant();

            var existingSet = new HashSet<string>(
                existing.Select(x => $"{x.ModuleId}|{Norm(x.Code)}|{Norm(x.Name)}")
            );

            var toInsert = data
                .Where(p => p.ModuleId > 0)
                .Where(p => !string.IsNullOrWhiteSpace(p.Name))
                .Where(p => !existingSet.Contains($"{p.ModuleId}|{Norm(p.Code)}|{Norm(p.Name)}"))
                .ToList();

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