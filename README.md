# BusinessCentral ERP API (.NET 10)

API multi-tenant para un ERP modular (COMMERCE, POS, SERVICES, FARM, AGRO, MFG, HR, FIN) con enfoque en:

- **Multi-tenancy por `CompanyId`**
- **Stored Procedures para operaciones de datos**
- **Arquitectura limpia** (Api → Application/MediatR → Infrastructure/Repositories → SQL/SPs)
- **PUC Colombia**: diario contable + reportes (balance, estado de resultados, balance de prueba)
- **Seed JSON merge-idempotente** para catálogos y datos base
- **Módulo POS reutilizable** para cualquier naturaleza de negocio (rápido para vender)

---

## Requisitos

- .NET **10**
- SQL Server (o LocalDB para desarrollo)
- `dotnet-ef` (se usa como herramienta local del repo)

---

## Configuración (appsettings)

Archivo clave: `BusinessCentral.Api/appsettings.Development.json`

- **ConnectionStrings**: `DefaultConnection`
- **JwtSettings**: `SecretKey`, `Issuer`, `Audience`, `ExpiryInMinutes`
- **RunSeed**: si `true` ejecuta el seed al iniciar (ver `BusinessCentral.Infrastructure/Seed/DbInitializer.cs`)

---

## Cómo ejecutar (desarrollo)

En la raíz del repo:

```bash
dotnet build "BusinessCentral.Api.sln" -c Debug
dotnet run --project "BusinessCentral.Api/BusinessCentral.Api.csproj"
```

---

## Migraciones (EF Core)

Restaurar herramienta local:

```bash
dotnet tool restore
```

Aplicar migraciones a la base configurada:

```bash
dotnet tool run dotnet-ef database update --project "BusinessCentral.Infrastructure/BusinessCentral.Infrastructure.csproj" --startup-project "BusinessCentral.Api/BusinessCentral.Api.csproj"
```

Crear migración nueva:

```bash
dotnet tool run dotnet-ef migrations add "MiMigracion" --project "BusinessCentral.Infrastructure/BusinessCentral.Infrastructure.csproj" --startup-project "BusinessCentral.Api/BusinessCentral.Api.csproj"
```

---

## Seed (JSON merge-idempotente)

El seed está en `BusinessCentral.Infrastructure/Seed/DbInitializer.cs` y lee JSON desde `BusinessCentral.Infrastructure/Seed/Data/`.

Catálogos y plantillas importantes:

- **Módulos y naturalezas**: `modules.json`, `business_natures.json`, `business_nature_modules.json`
- **Métodos de entrega/recepción**: `fulfillment_methods.json`, `business_nature_fulfillment_methods.json`
- **Métodos de pago**: `payment_methods.json`, `business_nature_payment_methods.json`
- **PUC**: `fin_puc_accounts.json`
- **Ubicaciones**: `business_storage_locations.json`

Notas:

- Algunas entidades usan `SeedEntity<T>` (inserta solo si tabla está vacía).
- Otras usan “merge” por Code / keys compuestas (no duplica si ya existe).

---

## Convención de respuestas (uniforme)

Todos los endpoints deben responder con `ApiResponse<T>`:

```json
{
  "isSuccess": true,
  "data": {},
  "message": "OK",
  "code": 200,
  "isException": false,
  "traceId": null
}
```

Errores:

```json
{
  "isSuccess": false,
  "data": null,
  "message": "Mensaje de error",
  "code": 422,
  "isException": false,
  "traceId": null
}
```

En controllers con MediatR se usa `ProcessResult()` desde `BusinessCentral.Api/Common/ApiControllerBase.cs`.

---

## Paginación (listados grandes)

Los listados grandes usan query params:

- `page` (default 1)
- `pageSize` (default 50, max 500)

Y devuelven:

```json
{
  "items": [],
  "page": 1,
  "pageSize": 50,
  "total": 123
}
```

Ya aplicado en SPs/listados como productos/proveedores/variantes/ubicaciones/lotes.

---

## Autenticación (JWT)

- Login: `POST /api/v1/public/auth/login`
- El JWT incluye claims como:
  - `companyId`
  - `userId` (también `sub`)

Controllers seguros leen:
- `CompanyId` desde el claim `companyId`
- `UserId` desde el claim `userId` (o `sub`)

### Claims de autorización (rol, permisos, módulos)

En `POST /api/v1/public/auth/login` el servidor devuelve también la información de autorización, y la replica en el JWT como claims:

- `role`: nombre del rol.
- `isSystemRole`: `true|false`.
- `isSuperUser`: `true|false`.
- `perm`: claim repetible (una por permiso), formato `MODULE.PERMISSION_CODE` (ej: `AUTH.USERS_WRITE`).
- `module`: claim repetible (una por módulo habilitado en la compañía), formato `MODULE_CODE` (ej: `FIN`).

### System web (superusuario) vs apps tenant (móvil/escritorio)

Se separa el uso por canal:

- **Web**: solo para **superusuario / usuario de sistemas** (configuración global de clientes).
- **Móvil + escritorio (MAUI)**: solo para operación de una **compañía** (tenant).

Login:
- **Tenant**: `POST /api/v1/public/auth/login` enviando `companyId`.
- **Superusuario web**: `POST /api/v1/public/auth/login` sin `companyId` (usa `auth.sp_login_system_user`). En este modo el JWT emite `companyId = 0`.

### Usuarios por compañía (JWT + módulo AUTH)

CRUD con **ámbito de tenant** (el `CompanyId` del token; no se fía del body). Requiere módulo **AUTH** y token con permisos habituales de API segura.

| Acción | Método y ruta |
|--------|-----------------|
| Listar (paginado) | `GET /api/v1/secure/company/users?page=&pageSize=` |
| Detalle | `GET /api/v1/secure/company/users/{userId}` |
| Crear | `POST /api/v1/secure/company/users` |
| Actualizar | `PUT /api/v1/secure/company/users/{userId}` |
| Eliminar | `DELETE /api/v1/secure/company/users/{userId}` |

### Usuarios (rol sistema, `SystemRole`)

Mismos casos de uso con **companyId explícito** en la ruta (backoffice / soporte). Prefijo: `api/v1/private/users`.

| Acción | Método y ruta |
|--------|-----------------|
| Listar | `GET /api/v1/private/users/company/{companyId}?page=&pageSize=` |
| Detalle | `GET /api/v1/private/users/company/{companyId}/users/{userId}` |
| Crear | `POST /api/v1/private/users` (body incluye `companyId`) |
| Actualizar | `PUT /api/v1/private/users/company/{companyId}/users/{userId}` |
| Eliminar | `DELETE /api/v1/private/users/company/{companyId}/users/{userId}` |

---

## Multi-naturaleza y módulos

Una empresa puede tener varias naturalezas y módulos habilitados.

Sistema:
- `GET /api/v1/system/config/modules`
- `GET /api/v1/system/config/modules/companies/{companyId}`
- `PUT /api/v1/system/config/modules/companies/{companyId}/{moduleCode}?enabled=true`

### Onboarding de compañías (`SystemRole`)

Creación inicial de empresa, primera suscripción (`CompanySubscription`), usuario propietario, primera(s) sede(s) (`business.Facility`) y plantillas derivadas de la naturaleza (fulfillment/payment).

| Recurso | Método y ruta |
|---------|----------------|
| Naturalezas | `GET /api/v1/system/config/onboarding/business-natures?onlyActive=` |
| Módulos sugeridos por naturaleza | `GET /api/v1/system/config/onboarding/business-natures/{code}/modules` |
| Tipos de sede (Matriz, Local…) | `GET /api/v1/system/config/onboarding/facility-types?onlyActive=` |
| Alta compañía | `POST /api/v1/system/config/onboarding/companies` |

**Membresía en el alta**

- Body incluye `membershipPlanId`; el SP registra la fila en `config.CompanySubscription` (fechas según `DurationDays` del plan).
- Primero se aplican los módulos por plantilla de naturaleza (`BusinessNatureModule`).
- **Después**, si el plan tiene filas en `config.PlanModule`, los módulos efectivos se **alinean al plan**: los incluidos en `PlanModule` quedan habilitados en `CompanyModule`; el resto de filas `CompanyModule` de esa compañía pasan a **deshabilitados**. Si el plan no tiene filas en `PlanModule`, solo rigen los defaults de naturaleza.

Para ver qué trae cada plan antes de vender/contratar:

- `GET /api/v1/public/common/membership-plans`
- `GET /api/v1/public/common/membership-plans/{id}`
- `GET /api/v1/public/common/membership-plans/{id}/modules`

**Varias sedes en el mismo alta**

Envía en el JSON `facilities`: arreglo de objetos `{ "facilityTypeId", "name", "code", "email", "phone", "priority" }` (camelCase). Si hay al menos un elemento válido, el SP crea todas las sedes; si envías la lista, **no** uses los campos legacy `facilityTypeId` / `facilityName` para una sola sede.

**Límites / huecos revisados en código** (útiles para roadmap):

- No hay endpoints REST dedicados para **añadir/editar sedes (`Facility`)** después del onboarding (solo ubicaciones `storage-locations` referenciando `facilityId`).
- No hay API unificada para **cambiar de plan / renovación** de suscripción después del alta (solo EF/seed sobre `CompanySubscription`; habría que exponer SP + controller si lo necesitas).
- Direcciones de sede (`FacilityAddress`) y otros catálogos siguen modelo de datos pero sin flujo HTTP completo en esta API.

Todos los cambios recientes en `stored_procedures_all.sql` deben **desplegarse en SQL Server** para que coincida el comportamiento anterior.

---

## Estándares: módulos y permisos por naturaleza (default)

### Regla base (simple y consistente)

- **Módulos default por naturaleza**: se definen en `config.BusinessNatureModule` (`IsDefaultEnabled`).
- **Permisos default por naturaleza**: en este proyecto se calculan como **todos los permisos (`config.Permission`)** de los **módulos** que están `IsDefaultEnabled = 1` para esa naturaleza.

Esto permite:
- una compañía con **múltiples naturalezas** → unión de módulos/permissions sugeridos.
- que siempre puedas **personalizar** después (módulos por `CompanyModule`, permisos por `RolePermission`).

### SuperUser

El claim `isSuperUser=true` indica que el usuario debe tener acceso total **a nivel UI/cliente**. La API expone módulos/permissions para construir el menú y la autorización del cliente.

> Nota: enforcement fino por permiso en endpoints (más allá de `RequiresModule`) se puede activar con un middleware/policies si quieres (roadmap).

### Endpoints (tenant seguro) para administrar permisos

Módulo **AUTH**, `CompanyId` por JWT.

- **Catálogo de permisos**:
  - `GET /api/v1/secure/auth/permissions?onlyActive=true&moduleCode=AUTH`
- **Permisos default por naturaleza** (estándar calculado):
  - `GET /api/v1/secure/auth/permissions/business-natures/{natureCode}/defaults`
- **Permisos asignados a un rol**:
  - `GET /api/v1/secure/auth/roles/{roleId}/permissions`
- **Asignar / revocar permiso a un rol**:
  - `PUT /api/v1/secure/auth/roles/{roleId}/permissions` body `{ "permissionId": 123, "enabled": true }`

### Aplicación automática al habilitar naturaleza

Al habilitar una naturaleza en una compañía (`PUT /api/v1/system/config/onboarding/companies/{companyId}/business-natures/{natureCode}?enabled=true`), el SP:
- asegura filas de `CompanyModule` según la plantilla de la naturaleza (no rompe personalizaciones ya existentes),
- deja listo el estándar de defaults por naturaleza para que el admin decida cómo aplicarlo a roles.


## FIN / PUC (Contabilidad)

Entidades clave:
- `fin.Account` (PUC)
- `fin.JournalEntry` / `fin.JournalEntryLine`

Reportes:
- Balance de prueba / Estado resultados / Balance general (clases 1–3)

Arranque financiero (3 escenarios reales):

- **CONSTITUTION** (desde cero): capital inicial y activos vs patrimonio
- **SANITATION** (negocio sin ERP): saldos iniciales “limpios”
- **MIGRATION** (viene de otro software): saldos de cierre y cuadre

Endpoints:
- `GET /api/v1/secure/finance/bootstrap/profile`
- `PUT /api/v1/secure/finance/bootstrap/profile`
- `POST /api/v1/secure/finance/bootstrap/opening/constitution`
- `POST /api/v1/secure/finance/bootstrap/opening/balances`
- `GET /api/v1/secure/finance/bootstrap/validate/balance-equation?asOfUtc=...`

### Transacciones financieras (no contables sueltas)

Módulo **FIN**. `CompanyId` por JWT.

| Acción | Método y ruta |
|--------|-----------------|
| Listar (paginado, opcional `from` / `to` en UTC) | `GET /api/v1/secure/finance/transactions?from=&to=&page=&pageSize=` |
| Crear | `POST /api/v1/secure/finance/transactions` |

---

## POS (motor de venta reutilizable)

El POS está pensado para operar rápido en cualquier naturaleza (todos “venden”).

### Métodos de entrega/recepción (Fulfillment)

Catálogo parametrizable por compañía:
- `GET /api/v1/secure/config/fulfillment-methods?onlyEnabled=true`

Sistema:
- `GET /api/v1/system/config/fulfillment-methods`
- `GET /api/v1/system/config/fulfillment-methods/companies/{companyId}`
- `PUT /api/v1/system/config/fulfillment-methods/companies/{companyId}/{methodCode}?enabled=true`

### Métodos de pago

Catálogo parametrizable por compañía:
- `GET /api/v1/secure/config/payment-methods?onlyEnabled=true`

Sistema:
- `GET /api/v1/system/config/payment-methods`
- `GET /api/v1/system/config/payment-methods/companies/{companyId}`
- `PUT /api/v1/system/config/payment-methods/companies/{companyId}/{methodCode}?enabled=true`

### Operación POS (tickets)

- Crear ticket:
  - `POST /api/v1/secure/pos/tickets`
  - Body:

```json
{
  "cashSessionId": 1,
  "fulfillmentMethodCode": "DELIVERY",
  "fulfillmentDetails": "Calle 123 #45-67"
}
```

- Agregar línea:
  - `POST /api/v1/secure/pos/tickets/{ticketId}/lines`

- Pagar:
  - `POST /api/v1/secure/pos/tickets/{ticketId}/pay`
  - Nota: el SP valida que el método de pago esté habilitado para la compañía.

- Consultar ticket/recibo:
  - `GET /api/v1/secure/pos/tickets/{ticketId}`
  - Devuelve header + líneas + pagos.

---

## Caja (apertura → movimientos → arqueo → cierre)

Flujo operativo:

- Abrir caja:
  - `POST /api/v1/secure/pos/cash-sessions`
- Registrar movimiento (entrada/salida):
  - `POST /api/v1/secure/pos/cash-sessions/{cashSessionId}/movements`
- Consultar caja (con movimientos):
  - `GET /api/v1/secure/pos/cash-sessions/{cashSessionId}`
- Cerrar / arqueo:
  - `POST /api/v1/secure/pos/cash-sessions/{cashSessionId}/close`

En cierre:
- El sistema calcula **ExpectedClosingAmount** usando apertura + movimientos + pagos en efectivo (configurable).
- Si hay diferencia, genera un asiento PUC opcional (sobrante/faltante) con cuentas por defecto:
  - Caja `110505`
  - Faltantes (gasto) `519595`
  - Sobrantes (ingreso) `429595`

---

## HR (empleados)

Un solo `UsersInfo` para todo: usuarios con login y empleados sin login.

### CanLogin + acceso público (opcional)

- `UsersInfo.CanLogin` controla si el usuario puede autenticarse.
- Tokens públicos (hash SHA-256, revocables) para lectura limitada (ej: resumen HR).

### Disponibilidad (opcional)

Permite registrar:
- Horarios por día (slots)
- Excepciones (vacaciones/no disponible o disponibilidad extra)
- Capacidad (`MaxServicesPerDay`, etc.)

Endpoints:
- `GET /api/v1/secure/hr/employees/{userId}/availability`
- `PUT /api/v1/secure/hr/employees/{userId}/availability`

---

## Servicios (órdenes de servicio)

Módulo **SERVICES**. `CompanyId` por JWT.

| Acción | Método y ruta |
|--------|-----------------|
| Listar (paginado, `status` opcional) | `GET /api/v1/secure/services/orders?status=&page=&pageSize=` |
| Crear | `POST /api/v1/secure/services/orders` |
| Detalle | `GET /api/v1/secure/services/orders/{orderId}` |
| Agregar línea | `POST /api/v1/secure/services/orders/{orderId}/lines` |

---

## Base de datos: stored procedures

Los repositorios invocan SPs definidos en `BusinessCentral.Infrastructure/Persistence/Configuration/StoreProcedures/stored_procedures_all.sql`. Tras pull o cambios en ese archivo, **vuelve a ejecutar el script** (o el despliegue equivalente) en SQL Server para que listados, paginación y firmas de `auth.sp_update_user` / `auth.sp_delete_user` coincidan con el código.

---



## Licencia / contribución

Pendiente: define licencia y flujo de contribución según tu modelo de negocio.
