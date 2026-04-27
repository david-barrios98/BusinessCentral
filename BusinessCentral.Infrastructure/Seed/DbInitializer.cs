using System.Reflection;
using BusinessCentral.Domain.Entities.Audit;
using BusinessCentral.Domain.Entities.Auth;
using BusinessCentral.Domain.Entities.Business;
using BusinessCentral.Domain.Entities.Common;
using BusinessCentral.Domain.Entities.Config;
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
            await SeedEntity<FacilityType>(context, "facility_type.json");

            // --- 2. GEOGRAFÍA (Nivel 1 - Dependen de Countries) ---
            await SeedEntity<Department>(context, "department.json");
            await SeedEntity<City>(context, "city.json");

            // --- 3. ESTRUCTURA EMPRESARIAL (Nivel 2 - Dependen de DocumentType/City) ---
            await SeedEntity<Companies>(context, "company.json");
            await SeedEntity<Permission>(context, "permissions.json");

            // --- 4. CONFIGURACIÓN Y SEDES (Dependen de Companies/Plans/Modules) ---
            await SeedEntity<Facility>(context, "facility.json");
            //await SeedEntity<FacilityAddress>(context, "facility_address.json");
            await SeedEntity<ApplicationCompanies>(context, "application_companies.json");
            await SeedEntity<CompanySubscription>(context, "company_subscription.json");
            await SeedEntity<PlanModule>(context, "plan_module.json");
            await SeedEntity<Role>(context, "roles.json");

            // --- 5. SEGURIDAD DETALLADA (Dependen de Roles/Permissions) ---
            await SeedEntity<RolePermission>(context, "role_permissions.json");

            // --- 6. USUARIOS Y SESIONES (Nivel Final - Dependen de Companies/Roles) ---
            await SeedEntity<UsersInfo>(context, "users_info.json");
            await SeedEntity<UserAddress>(context, "user_addresses.json");
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