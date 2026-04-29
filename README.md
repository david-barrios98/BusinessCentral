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
- **RunSeedDemoData**: si `true` ejecuta **data demo** (HR transaccional, FARM, SERVICES, COMMERCE/POS, etc.) además del seed base. En `appsettings.Development.json` del repo está en `true` para desarrollo; en producción conviene `false` si no quieres datos de prueba.
- Las **plantillas por naturaleza** (`business_nature_modules`, `business_nature_fulfillment_methods`, `business_nature_payment_methods`) se ejecutan **siempre** con el seed base (no dependen de `RunSeedDemoData`).

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
- **Data demo (HR/FARM/SERVICES/COMMERCE-POS)**: se controla con `RunSeedDemoData` (en desarrollo del repo suele estar `true`). Si faltan filas en esas tablas, revisa que `RunSeed=true` y, si quieres demo, `RunSeedDemoData=true`.
- **`SeedEntity<T>`**: si la tabla **ya tiene al menos un registro**, ese seed **no vuelve a insertar** (se salta). Si necesitas re-seed, vacía la tabla o usa migración + BD limpia.

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

Guía detallada para el front **Julisys** (backoffice, refresh, onboarding, usuarios, arquitectura compartida): [docs/Julisys-Backoffice-Integration.md](docs/Julisys-Backoffice-Integration.md)

- Login (legacy / compatibilidad): `POST /api/v1/public/auth/login`
  - Request DTO: `LoginRequestDTO`
  - Response DTO: `ApiResponse<LoginResponseDTO>`
- Login recomendado (por canal):
  - Backoffice: `POST /api/v1/public/auth/backoffice/login`
    - Request DTO: `LoginRequestDTO`
    - Response DTO: `ApiResponse<LoginResponseDTO>`
  - Tenant App: `POST /api/v1/public/auth/tenant/login`
    - Request DTO: `LoginRequestDTO`
    - Response DTO: `ApiResponse<LoginResponseDTO>`
- El JWT incluye claims como:
  - `companyId`
  - `userId` (también `sub`)
  - `roleId`
  - `role`

Controllers seguros leen:
- `CompanyId` desde el claim `companyId`
- `UserId` desde el claim `userId` (o `sub`)

### Claims de autorización (rol, permisos, módulos)

En `POST /api/v1/public/auth/login` (Request DTO: `LoginRequestDTO` / Response DTO: `ApiResponse<LoginResponseDTO>`) el servidor devuelve también la información de autorización, y la replica en el JWT como claims:

- `role`: nombre del rol.
- `isSystemRole`: `true|false`.
- `isSuperUser`: `true|false`.
- `perm`: claim repetible (una por permiso), formato `MODULE.PERMISSION_CODE` (ej: `AUTH.USERS_WRITE`).
- `module`: claim repetible (una por módulo habilitado en la compañía), formato `MODULE_CODE` (ej: `FIN`).

### Separación estricta por canal (Backoffice vs App)

Este backend soporta 2 canales con reglas distintas:

- **Backoffice (Web “Centro de Control”)**
  - Solo permite usuarios **staff**: `isSystemRole = true` o `isSuperUser = true`.
  - Login **sin** `companyId`.
- **Tenant App (MAUI/Blazor Hybrid “Terminal del Negocio”)**
  - Solo permite usuarios **tenant** asociados a una empresa (login **con** `companyId`).
  - Bloquea usuarios staff en la app.

Header recomendado (en login):

- `X-Client: backoffice` o `X-Client: tenant-app`

Compatibilidad:
- Si no envías `X-Client`, el servidor infiere el canal por presencia de `companyId` (si hay `companyId` ⇒ tenant-app; si no ⇒ backoffice).

Enforcement por ruta:
- `api/v1/system/**` requiere `X-Client: backoffice`
- `api/v1/secure/**` requiere `X-Client: tenant-app`

Errores:
- Backoffice + `companyId` ⇒ **400**
- Tenant-app sin `companyId` ⇒ **400**
- Usuario no staff intentando backoffice ⇒ **403**
- Usuario staff intentando tenant-app ⇒ **403**

### System web (superusuario) vs apps tenant (móvil/escritorio)

Se separa el uso por canal:

- **Web**: solo para **superusuario / usuario de sistemas** (configuración global de clientes).
- **Móvil + escritorio (MAUI)**: solo para operación de una **compañía** (tenant).

Login:
- **Tenant**: `POST /api/v1/public/auth/login` enviando `companyId`.
-   Request DTO: `LoginRequestDTO`
-   Response DTO: `ApiResponse<LoginResponseDTO>`
- **Superusuario web**: `POST /api/v1/public/auth/login` sin `companyId` (usa `auth.sp_login_system_user`). En este modo el JWT emite `companyId = 0` y el usuario en BD puede tener `UsersInfo.CompanyId = NULL`.
-   Request DTO: `LoginRequestDTO`
-   Response DTO: `ApiResponse<LoginResponseDTO>`

Logout:
- `POST /api/v1/secure/auth/logout` (requiere `Authorization: Bearer <accessToken>`)
  - Request DTO: `LogoutRequestDTO`
  - Response DTO: `ApiResponse<bool>`
- Body recomendado:

```json
{
  "refreshToken": "<refreshToken>"
}
```

Efecto:
- Cierra sesiones abiertas del usuario en `audit.UserSession` asignando `LogoutAt` y poniendo `IsSuccess = 0`.
- Revoca refresh tokens activos del usuario. Si envías `refreshToken`, el backend puede resolver la sesión exacta; si no, revoca por `UserId` (todas las sesiones del usuario).

Bootstrap (recomendado para inicializar UI sin re-login):
- `GET /api/v1/secure/auth/bootstrap` (requiere `Authorization: Bearer <accessToken>` y `X-Client: tenant-app`)
  - Request DTO: N/A
  - Response DTO: `ApiResponse<AuthBootstrapResponseDTO>`
- Devuelve: usuario/rol/empresa + `modules[]` + `permissions[]`

### Respuesta HTTP y cuerpo (`ApiResponse<T>`)

Salvo indique `200` “crudo” en un controlador, las respuestas van envueltas en `ApiResponse<T>`:

| Campo | Tipo | Descripción |
|--------|------|-------------|
| `isSuccess` | bool | `true` si la operación fue correcta |
| `data` | T \| null | Payload (login, listas, etc.) |
| `message` | string | Mensaje descriptivo |
| `code` | int | Suele coincidir con el código HTTP devuelto |
| `isException` | bool | `true` si fue error no controlado |
| `traceId` | string \| null | Trazabilidad si aplica |

**MediatR (`ProcessResult`)** traduce `Result<T>.ErrorType` a HTTP: `NotFound`→404, `Validation`→422, `Unauthorized`→401, `Conflict`→409; **cualquier otro** tipo de error de negocio→**400** (incluye algunos mensajes “prohibido” si el handler usa otro `ErrorType`). Cabecera típica: `Content-Type: application/json`.

**Cabeceras globales útiles**

| Header | Cuándo |
|--------|--------|
| `Content-Type: application/json` | Requests con body JSON |
| `Authorization: Bearer <accessToken>` | Rutas `[Authorize]` |
| `X-Client: backoffice` | **`/api/v1/system/**`** (obligatorio) |
| `X-Client: tenant-app` | **`/api/v1/secure/**`** (obligatorio) |
| `X-Client` opcional | **`/api/v1/public/**`** y **`/api/v1/private/**`** (recomendado `backoffice` en private para consistencia) |

---

### `POST /api/v1/public/auth/login`

| | |
|--|--|
| **Auth** | No |
| **Headers** | `Content-Type: application/json`. Opcional: `X-Client: backoffice` \| `tenant-app`; si falta, el canal se infiere (`companyId` en body ⇒ tenant-app; si no ⇒ backoffice). Opcional: `User-Agent`, `sec-ch-ua-platform` (se usa como metadata de plataforma). |
| **Body** | `{ "UserName": string, "Password": string, "companyId"?: string, "applicationCode"?: string }` — `companyId` en JSON **camelCase** según `LoginRequestDTO`; omitir para login sistema/backoffice. `applicationCode` (opcional): identifica el cliente embebido en APK/instalador; debe coincidir con `config.ApplicationCompanies.ApplicationCode` para esa compañía. Si se omite o va vacío, el SP acepta cualquier fila activa de la compañía (comportamiento previo). |
| **Request DTO** | `LoginRequestDTO` |
| **Response DTO** | `ApiResponse<LoginResponseDTO>` |

**200** — `ApiResponse<LoginResponseDTO>` con `data` similar a:

```json
{
  "userSessionId": 0,
  "userId": 0,
  "userName": "...",
  "loginField": "email|phone|document",
  "companyId": 0,
  "companyName": "...",
  "roleId": 0,
  "roleName": "...",
  "isSystemRole": false,
  "isSuperUser": false,
  "permissions": [],
  "modules": [],
  "accessToken": "jwt...",
  "refreshToken": "...",
  "tokenType": "Bearer",
  "expiresIn": 7200,
  "issuedAt": "2026-01-01T00:00:00Z"
}
```

**Errores típicos** — `ApiResponse` con `isSuccess: false`: **401** credenciales; **404** usuario no encontrado; **400** validación / canal inválido; **403** usuario no permitido para el canal (staff vs tenant).

---

### `POST /api/v1/public/auth/backoffice/login`

Igual contrato que login, pero el servidor fuerza canal **backoffice** y no debe enviarse `companyId`.

| **Headers** | `Content-Type: application/json` |
| **Body** | `{ "UserName": string, "Password": string }` |
| **Request DTO** | `LoginRequestDTO` |
| **Response DTO** | `ApiResponse<LoginResponseDTO>` |

**200** — mismo shape que `LoginResponseDTO`. **403/400** — usuario no staff o body inválido según reglas del handler.

---

### `POST /api/v1/public/auth/tenant/login`

| **Headers** | `Content-Type: application/json` |
| **Body** | `{ "UserName": string, "Password": string, "companyId": string, "applicationCode"?: string }` — `companyId` obligatorio (string numérica). `applicationCode` **recomendado** para móvil/escritorio: el mismo valor que definas en backoffice en `config.ApplicationCompanies` (p. ej. `001` web, `002` APK). Si no se envía, se mantiene compatibilidad con despliegues que aún no envían el código. |
| **Request DTO** | `LoginRequestDTO` |
| **Response DTO** | `ApiResponse<LoginResponseDTO>` |

**200** — `LoginResponseDTO`. **403** — usuario staff en app tenant. **400** — falta `companyId`. Si envías `applicationCode` y no hay fila habilitada para ese código en la compañía, el SP puede responder error de negocio (equivalente a usuario no encontrado / configuración inválida).

---

### Clientes de aplicación por compañía (`config.ApplicationCompanies`)

Desde el **backoffice** puedes definir, por compañía, qué **código de aplicación** (APK, instalador de escritorio, build web, etc.) usa qué **campo de login** (`email`, `phone`, `document`; `documentNumber` en datos legacy se normaliza a `document`).

| **Auth** | `Authorization: Bearer` con usuario sistema (`SystemRole`) |
| **Headers** | Recomendado: `X-Client: backoffice` en rutas `api/v1/system/...` |

| Acción | Método y ruta |
|--------|----------------|
| Listar configuraciones de la compañía | `GET /api/v1/system/config/companies/{companyId}/application-companies` |
| Alta | `POST /api/v1/system/config/companies/{companyId}/application-companies` |
| Actualizar | `PUT /api/v1/system/config/companies/{companyId}/application-companies/{id}` |
| Eliminar | `DELETE /api/v1/system/config/companies/{companyId}/application-companies/{id}` |

| | |
|--|--|
| **Request DTO** | GET/DELETE: N/A. POST/PUT: `UpsertApplicationCompanyRequestDTO` |
| **Response DTO** | GET: `ApiResponse<List<ApplicationCompanyDTO>>`. POST/PUT: `ApiResponse<object>` (`{ id }`). DELETE: `ApiResponse<object>` (`{ companyId, id, deleted: true }`) |

**POST/PUT body** (`UpsertApplicationCompanyRequestDTO`):

```json
{
  "id": 0,
  "applicationCode": "002",
  "loginField": "phone",
  "priority": 1,
  "isEnabled": true,
  "active": true
}
```

En **POST** el servidor ignora `id` en el body. En **PUT**, `id` en ruta debe coincidir con el registro (el body puede incluir el mismo `id`).

**Respuesta** típica de alta/actualización: `ApiResponse` con `data: { "id": <int> }`.

**Nota de despliegue:** ejecutar en SQL Server el script que define `[config].[sp_list_application_companies]`, `[config].[sp_upsert_application_company]`, `[config].[sp_delete_application_company]` y la versión actualizada de `[auth].[sp_login_user]` (parámetro `@application_code`), incluidos en `BusinessCentral.Infrastructure/Persistence/Configuration/StoreProcedures/stored_procedures_all.sql`.

---

### `POST /api/v1/public/auth/password/forgot`

| **Auth** | No |
| **Headers** | `Content-Type: application/json` |
| **Body** | `{ "email": string, "companyId": number }` |
| **Request DTO** | `PasswordResetRequestDTO` |
| **Response DTO** | `ApiResponse<bool>` |

**200** — `ApiResponse<bool>` con `data: true` (por seguridad puede devolver OK aunque el correo no exista). **401** — si el usuario ya tiene token de reset activo (mensaje en `message`). Errores de validación → **422** si el modelo no pasa `[Required]` / `[EmailAddress]`.

---

### `POST /api/v1/public/auth/password/reset`

| **Headers** | `Content-Type: application/json` |
| **Body** | `{ "token": string, "newPassword": string }` (`newPassword` mínimo 6 caracteres según DTO). |
| **Request DTO** | `PasswordResetConfirmDTO` |
| **Response DTO** | `ApiResponse<bool>` |

**200** — `ApiResponse<bool>`. Fallos de negocio según handler → **401**/**400**/`ApiResponse` con `isSuccess: false`.

---

### `POST /api/v1/private/users/refresh`

| **Auth** | `Authorization: Bearer <accessToken actual>` |
| **Policy** | `SystemRole` |
| **Headers** | `Content-Type: application/json`. Recomendado: `X-Client: backoffice`. |
| **Body** | `{ "refreshToken": string }` |
| **Request DTO** | `RefreshTokenRequestDTO` |
| **Response DTO** | `ApiResponse<LoginResponseDTO>` |

**200** — `ApiResponse<LoginResponseDTO>` (nuevo access y refresh tras rotación). **401** — refresh inválido o expirado.

---

### `POST /api/v1/private/users/logout`

| **Auth** | `Authorization: Bearer <accessToken>` |
| **Policy** | `SystemRole` |
| **Headers** | `Content-Type: application/json`. Recomendado: `X-Client: backoffice`. |
| **Body** (opcional) | `{ "userId"?: number, "companyId"?: number, "refreshToken"?: string, "sessionId"?: number }` |
| **Request DTO** | `LogoutRequestDTO` |
| **Response DTO** | `ApiResponse<bool>` |

**200** — `ApiResponse<bool>` con `data: true`. El controlador pasa el Bearer al comando de logout.

---

### `POST /api/v1/private/users/logout/user/{userId}` y `.../logout/company/{companyId}`

| **Auth** | Bearer + `SystemRole` |
| **Body** | Vacío |
| **Request DTO** | N/A |
| **Response DTO** | `ApiResponse<bool>` |

---

### `POST /api/v1/private/users` (crear usuario)

| **Auth** | Bearer + `SystemRole` |
| **Headers** | `Content-Type: application/json` |
| **Body** | `CreateUserDTO`: `companyId`, `documentTypeId`, `documentNumber`, `firstName`, `lastName`, `email`, `phone`, `password`, `roleId`, opcional `authProvider`, `externalId` |
| **Request DTO** | `CreateUserDTO` |
| **Response DTO** | `ApiResponse<int?>` (data = `userId`) |

**200** — `ApiResponse` con el resultado del comando (según handler). Validación → **422**.

---

### `GET /api/v1/private/users/company/{companyId}`

| **Query** | `page` (default 1), `pageSize` (default 20) |
| **Request DTO** | N/A |
| **Response DTO** | `ApiResponse<PagedResult<UserResponseDTO>>` |

---

### `GET|PUT|DELETE /api/v1/private/users/company/{companyId}/users/{userId}`

| **PUT body** | `UpdateUserDTO` (incluye `userId` coherente con ruta) |
| **Request DTO** | GET/DELETE: N/A. PUT: `UpdateUserDTO` |
| **Response DTO** | GET: `ApiResponse<UserResponseDTO>`. PUT: `ApiResponse<bool>`. DELETE: `ApiResponse<bool>` |

---

### `GET /api/v1/secure/auth/bootstrap`

| **Auth** | `Authorization: Bearer` |
| **Headers** | **`X-Client: tenant-app`** (requerido por middleware en `/secure`). |
| **Policy** | Módulo **AUTH** (`RequiresModule("AUTH")`). |
| **Request DTO** | N/A |
| **Response DTO** | `ApiResponse<AuthBootstrapResponseDTO>` |

**200** — `ApiResponse<object>` con `data`:

```json
{
  "userId": 0,
  "userName": "...",
  "companyId": 0,
  "companyName": "...",
  "roleId": 0,
  "roleName": "...",
  "isSystemRole": false,
  "isSuperUser": false,
  "modules": [],
  "permissions": []
}
```

Si `companyId` en token es `0`, `modules` puede ir vacío.

---

### `POST /api/v1/secure/auth/logout`

| **Auth** | Bearer |
| **Headers** | **`X-Client: tenant-app`**, `Content-Type: application/json` |
| **Body** | Opcional: `{ "refreshToken", "sessionId", "userId", "companyId" }` |
| **Request DTO** | `LogoutRequestDTO` |
| **Response DTO** | `ApiResponse<bool>` |

**200** — `ApiResponse<bool>`. Cierra sesiones y revoca refresh tokens según `LogoutHandler`.

---

### Onboarding (`/api/v1/system/config/onboarding/...`)

Todos requieren **`Authorization: Bearer`** (staff) + **`X-Client: backoffice`** + policy **`SystemRole`**.

| Método | Ruta | Request DTO | Response DTO |
|--------|------|-------------|--------------|
| GET | `/business-natures?onlyActive=` | N/A | `ApiResponse<List<BusinessNatureDTO>>` |
| GET | `/business-natures/{code}/modules` | N/A | `ApiResponse<List<BusinessNatureModuleDTO>>` |
| GET | `/companies/{companyId}/business-natures` | N/A | `ApiResponse<List<CompanyBusinessNatureDTO>>` |
| PUT | `/companies/{companyId}/business-natures/{natureCode}?enabled=&primary=` | N/A | `ApiResponse<object>` (`{ companyId, natureCode, enabled, primary }`) |
| GET | `/facility-types?onlyActive=` | N/A | `ApiResponse<List<FacilityTypeDTO>>` |
| POST | `/companies` | `OnboardCompanyRequestDTO` | `ApiResponse<OnboardCompanyResultDTO>` |

---

### Catálogos maestros (CRUD backoffice)

Todos requieren **`Authorization: Bearer`** (staff) + **`X-Client: backoffice`** + policy **`SystemRole`**.

> Estos endpoints son para mantener **tablas maestras** (catálogos globales). Distinto a “habilitar por compañía” (CompanyModule, CompanyPaymentMethod, etc.).

| Catálogo | Ruta | Request DTO | Response DTO |
|---------|------|-------------|--------------|
| Módulos (`config.Module`) | `GET /api/v1/system/config/modules?onlyActive=` | N/A | `ApiResponse<List<ModuleDTO>>` |
| | `GET /api/v1/system/config/modules/{id}` | N/A | `ApiResponse<ModuleDTO>` |
| | `POST /api/v1/system/config/modules` | `UpsertModuleRequestDTO` | `ApiResponse<object>` (`{ id }`) |
| | `PUT /api/v1/system/config/modules/{id}` | `UpsertModuleRequestDTO` | `ApiResponse<object>` (`{ id }`) |
| | `DELETE /api/v1/system/config/modules/{id}` | N/A | `ApiResponse<object>` (`{ id, deleted: true }`) |
| Naturalezas (`config.BusinessNature`) | `GET /api/v1/system/config/catalog/business-natures?onlyActive=` | N/A | `ApiResponse<List<BusinessNatureDTO>>` |
| | `GET /api/v1/system/config/catalog/business-natures/{id}` | N/A | `ApiResponse<BusinessNatureDTO>` |
| | `POST /api/v1/system/config/catalog/business-natures` | `UpsertBusinessNatureRequestDTO` | `ApiResponse<object>` (`{ id }`) |
| | `PUT /api/v1/system/config/catalog/business-natures/{id}` | `UpsertBusinessNatureRequestDTO` | `ApiResponse<object>` (`{ id }`) |
| | `DELETE /api/v1/system/config/catalog/business-natures/{id}` | N/A | `ApiResponse<object>` (`{ id, deleted: true }`) |
| Tipos de sede (`business.FacilityType`) | `GET /api/v1/system/config/catalog/facility-types?onlyActive=` | N/A | `ApiResponse<List<FacilityTypeDTO>>` |
| | `GET /api/v1/system/config/catalog/facility-types/{id}` | N/A | `ApiResponse<FacilityTypeDTO>` |
| | `POST /api/v1/system/config/catalog/facility-types` | `UpsertFacilityTypeRequestDTO` | `ApiResponse<object>` (`{ id }`) |
| | `PUT /api/v1/system/config/catalog/facility-types/{id}` | `UpsertFacilityTypeRequestDTO` | `ApiResponse<object>` (`{ id }`) |
| | `DELETE /api/v1/system/config/catalog/facility-types/{id}` | N/A | `ApiResponse<object>` (`{ id, deleted: true }`) |
| Métodos de pago (`config.PaymentMethod`) | `GET /api/v1/system/config/payment-methods?onlyActive=` | N/A | `ApiResponse<List<PaymentMethodDTO>>` |
| | `GET /api/v1/system/config/payment-methods/{id}` | N/A | `ApiResponse<PaymentMethodDTO>` |
| | `POST /api/v1/system/config/payment-methods` | `UpsertPaymentMethodRequestDTO` | `ApiResponse<object>` (`{ id }`) |
| | `PUT /api/v1/system/config/payment-methods/{id}` | `UpsertPaymentMethodRequestDTO` | `ApiResponse<object>` (`{ id }`) |
| | `DELETE /api/v1/system/config/payment-methods/{id}` | N/A | `ApiResponse<object>` (`{ id, deleted: true }`) |
| Métodos de fulfillment (`config.FulfillmentMethod`) | `GET /api/v1/system/config/fulfillment-methods?onlyActive=` | N/A | `ApiResponse<List<FulfillmentMethodDTO>>` |
| | `GET /api/v1/system/config/fulfillment-methods/{id}` | N/A | `ApiResponse<FulfillmentMethodDTO>` |
| | `POST /api/v1/system/config/fulfillment-methods` | `UpsertFulfillmentMethodRequestDTO` | `ApiResponse<object>` (`{ id }`) |
| | `PUT /api/v1/system/config/fulfillment-methods/{id}` | `UpsertFulfillmentMethodRequestDTO` | `ApiResponse<object>` (`{ id }`) |
| | `DELETE /api/v1/system/config/fulfillment-methods/{id}` | N/A | `ApiResponse<object>` (`{ id, deleted: true }`) |
| Tipos de documento (`common.DocumentType`) | `GET /api/v1/system/config/catalog/document-types` | N/A | `ApiResponse<List<DocumentTypeResponse>>` |
| | `GET /api/v1/system/config/catalog/document-types/{id}` | N/A | `ApiResponse<DocumentTypeResponse>` |
| | `POST /api/v1/system/config/catalog/document-types` | `DocumentTypeRequest` | `ApiResponse<object>` (`{ id }`) |
| | `PUT /api/v1/system/config/catalog/document-types/{id}` | `DocumentTypeRequest` | `ApiResponse<object>` (`{ id }`) |
| | `DELETE /api/v1/system/config/catalog/document-types/{id}` | N/A | `ApiResponse<object>` (`{ id, deleted: true }`) |
| Planes (`config.MembershipPlan`) | `GET /api/v1/system/config/catalog/membership-plans` | N/A | `ApiResponse<List<MembershipPlanResponse>>` |
| | `GET /api/v1/system/config/catalog/membership-plans/{id}` | N/A | `ApiResponse<MembershipPlanResponse>` |
| | `POST /api/v1/system/config/catalog/membership-plans` | `MembershipPlanRequest` | `ApiResponse<object>` (`{ id }`) |
| | `PUT /api/v1/system/config/catalog/membership-plans/{id}` | `MembershipPlanRequest` | `ApiResponse<object>` (`{ id }`) |
| | `DELETE /api/v1/system/config/catalog/membership-plans/{id}` | N/A | `ApiResponse<object>` (`{ id, deleted: true }`) |

**Bodies**:

- `POST/PUT /modules`: `UpsertModuleRequestDTO`
- `POST/PUT /catalog/business-natures`: `UpsertBusinessNatureRequestDTO`
- `POST/PUT /catalog/facility-types`: `UpsertFacilityTypeRequestDTO`
- `POST/PUT /payment-methods`: `UpsertPaymentMethodRequestDTO`
- `POST/PUT /fulfillment-methods`: `UpsertFulfillmentMethodRequestDTO`
- `POST/PUT /catalog/document-types`: `DocumentTypeRequest`
- `POST/PUT /catalog/membership-plans`: `MembershipPlanRequest`

**Respuestas**:
- List/Get: `ApiResponse<object>` con el DTO correspondiente.
- Create/Update: `ApiResponse<object>` con `data: { "id": <int> }`.
- Delete: `ApiResponse<object>` con `data: { "id": <int>, "deleted": true }`.

### Módulos sistema (`/api/v1/system/config/modules`)

Requisitos: Bearer staff, **`X-Client: backoffice`**, `SystemRole`.

| Método | Ruta | Request DTO | Response DTO |
|--------|------|-------------|--------------|
| GET | `/` | N/A | `ApiResponse<List<ModuleDTO>>` |
| GET | `/{id}` | N/A | `ApiResponse<ModuleDTO>` |
| POST | `/` | `UpsertModuleRequestDTO` | `ApiResponse<object>` (`{ id }`) |
| PUT | `/{id}` | `UpsertModuleRequestDTO` | `ApiResponse<object>` (`{ id }`) |
| DELETE | `/{id}` | N/A | `ApiResponse<object>` (`{ id, deleted: true }`) |
| GET | `/companies/{companyId}` | N/A | `ApiResponse<List<CompanyModuleDTO>>` |
| PUT | `/companies/{companyId}/{moduleCode}?enabled=true|false` | N/A | `ApiResponse<object>` (`{ companyId, moduleCode, enabled }`) |

---

### Despliegue de scripts SQL

Si cambias procedimientos en `stored_procedures_all.sql`, ejecuta el script en la base de datos para que coincidan con la API (p. ej. `config.sp_onboard_company`).

### Superusuario sin compañía (modelo de datos)

Se permite que:
- `auth.UsersInfo.CompanyId` sea **NULL** para usuarios de sistema/superusuarios.
- `config.Role.CompanyId` sea **NULL** para roles globales del sistema (por ejemplo `SuperUser`).

Para usuarios tenant, `CompanyId` debe existir y el login requiere `companyId`.

---

## Enforcements por módulo (feature flags reales)

Los endpoints `api/v1/secure/**` pueden requerir un módulo. Para eso se usa:

- Atributo: `[RequiresModule("MODULE_CODE")]`
- Middleware: `TenantSubscriptionMiddleware`
  - Verifica que el módulo esté habilitado en `CompanyModule` para el tenant.
  - (Además) valida estado de suscripción/empresa.

Regla:
- Si la compañía **no** tiene el módulo habilitado, el API responde **403** (plan no incluye el módulo).

Mapa actual (alto nivel):
- **AUTH**: `api/v1/secure/auth/**`, `api/v1/secure/company/users`, `api/v1/secure/auth/permissions`, `api/v1/secure/auth/roles/**/permissions`
- **BUSS**: `api/v1/secure/config/payment-methods`, `api/v1/secure/config/fulfillment-methods`
- **COMMERCE**: `api/v1/secure/commerce/**`, `api/v1/secure/business/storage-locations` (inventario)
- **POS**: `api/v1/secure/pos/**`
- **FIN**: `api/v1/secure/finance/**`
- **HR**: `api/v1/secure/hr/**`
- **SERVICES**: `api/v1/secure/services/**`
- **FARM**: `api/v1/secure/farm/**`
- **AGRO**: `api/v1/secure/agro/**`
- **MFG**: `api/v1/secure/mfg/**`

### Usuarios por compañía (JWT + módulo AUTH)

CRUD con **ámbito de tenant** (el `CompanyId` del token; no se fía del body). Requiere módulo **AUTH** y token con permisos habituales de API segura.

| Acción | Método y ruta |
|--------|-----------------|
| Listar (paginado) | `GET /api/v1/secure/company/users?page=&pageSize=` |
| Detalle | `GET /api/v1/secure/company/users/{userId}` |
| Crear | `POST /api/v1/secure/company/users` |
| Actualizar | `PUT /api/v1/secure/company/users/{userId}` |
| Eliminar | `DELETE /api/v1/secure/company/users/{userId}` |

| | |
|--|--|
| **Request DTO** | GET: N/A. POST: `CreateUserDTO`. PUT: `UpdateUserDTO`. DELETE: N/A |
| **Response DTO** | GET list: `ApiResponse<PagedResult<UserResponseDTO>>`. GET detalle: `ApiResponse<UserResponseDTO>`. POST: `ApiResponse<int?>` (data = `userId`). PUT: `ApiResponse<bool>`. DELETE: `ApiResponse<bool>` |

### Usuarios (rol sistema, `SystemRole`)

Mismos casos de uso con **companyId explícito** en la ruta (backoffice / soporte). Prefijo: `api/v1/private/users`.

| Acción | Método y ruta |
|--------|-----------------|
| Listar | `GET /api/v1/private/users/company/{companyId}?page=&pageSize=` |
| Detalle | `GET /api/v1/private/users/company/{companyId}/users/{userId}` |
| Crear | `POST /api/v1/private/users` (body incluye `companyId`) |
| Actualizar | `PUT /api/v1/private/users/company/{companyId}/users/{userId}` |
| Eliminar | `DELETE /api/v1/private/users/company/{companyId}/users/{userId}` |

| | |
|--|--|
| **Request DTO** | GET list/detalle: N/A. POST: `CreateUserDTO`. PUT: `UpdateUserDTO`. DELETE: N/A |
| **Response DTO** | GET list: `ApiResponse<PagedResult<UserResponseDTO>>`. GET detalle: `ApiResponse<UserResponseDTO>`. POST: `ApiResponse<int?>` (data = `userId`). PUT: `ApiResponse<bool>`. DELETE: `ApiResponse<bool>` |

---

## Multi-naturaleza y módulos

Una empresa puede tener varias naturalezas y módulos habilitados.

Sistema:
- `GET /api/v1/system/config/modules`
-   Request DTO: N/A
-   Response DTO: `ApiResponse<List<ModuleDTO>>`
- `GET /api/v1/system/config/modules/companies/{companyId}`
-   Request DTO: N/A
-   Response DTO: `ApiResponse<List<CompanyModuleDTO>>`
- `PUT /api/v1/system/config/modules/companies/{companyId}/{moduleCode}?enabled=true`
-   Request DTO: N/A
-   Response DTO: `ApiResponse<object>` (`{ companyId, moduleCode, enabled }`)

### Onboarding de compañías (`SystemRole`)

Creación inicial de empresa, primera suscripción (`CompanySubscription`), usuario propietario, primera(s) sede(s) (`business.Facility`) y plantillas derivadas de la naturaleza (fulfillment/payment).

| Recurso | Método y ruta |
|---------|----------------|
| Naturalezas | `GET /api/v1/system/config/onboarding/business-natures?onlyActive=` |
| Módulos sugeridos por naturaleza | `GET /api/v1/system/config/onboarding/business-natures/{code}/modules` |
| Tipos de sede (Matriz, Local…) | `GET /api/v1/system/config/onboarding/facility-types?onlyActive=` |
| Alta compañía | `POST /api/v1/system/config/onboarding/companies` |

| | |
|--|--|
| **Request DTO** | GETs: N/A. POST alta compañía: `OnboardCompanyRequestDTO` |
| **Response DTO** | Naturalezas: `ApiResponse<List<BusinessNatureDTO>>`. Módulos sugeridos: `ApiResponse<List<BusinessNatureModuleDTO>>`. Tipos sede: `ApiResponse<List<FacilityTypeDTO>>`. Alta: `ApiResponse<OnboardCompanyResultDTO>` |

**Membresía en el alta**

- Body incluye `membershipPlanId`; el SP registra la fila en `config.CompanySubscription` (fechas según `DurationDays` del plan).
- Primero se aplican los módulos por plantilla de naturaleza (`BusinessNatureModule`).
- **Después**, si el plan tiene filas en `config.PlanModule`, los módulos efectivos se **alinean al plan**: los incluidos en `PlanModule` quedan habilitados en `CompanyModule`; el resto de filas `CompanyModule` de esa compañía pasan a **deshabilitados**. Si el plan no tiene filas en `PlanModule`, solo rigen los defaults de naturaleza.

Para ver qué trae cada plan antes de vender/contratar:

- `GET /api/v1/public/common/membership-plans`
-   Request DTO: N/A
-   Response DTO: `ApiResponse<List<MembershipPlanResponse>>`
- `GET /api/v1/public/common/membership-plans/{id}`
-   Request DTO: N/A
-   Response DTO: `ApiResponse<MembershipPlanResponse>`
- `GET /api/v1/public/common/membership-plans/{id}/modules`
-   Request DTO: N/A
-   Response DTO: `ApiResponse<List<MembershipPlanModuleResponse>>`

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
    - Request DTO: N/A
    - Response DTO: `ApiResponse<List<PermissionDTO>>`
- **Permisos default por naturaleza** (estándar calculado):
  - `GET /api/v1/secure/auth/permissions/business-natures/{natureCode}/defaults`
    - Request DTO: N/A
    - Response DTO: `ApiResponse<List<RolePermissionDTO>>`
- **Permisos asignados a un rol**:
  - `GET /api/v1/secure/auth/roles/{roleId}/permissions`
    - Request DTO: N/A
    - Response DTO: `ApiResponse<List<RolePermissionDTO>>`
- **Asignar / revocar permiso a un rol**:
  - `PUT /api/v1/secure/auth/roles/{roleId}/permissions` body `{ "permissionId": 123, "enabled": true }`
    - Request DTO: `RolePermissionsController.SetPermissionRequest`
    - Response DTO: `ApiResponse<object>` (`{ roleId, permissionId, enabled }`)

### Aplicación automática al habilitar naturaleza

Al habilitar una naturaleza en una compañía (`PUT /api/v1/system/config/onboarding/companies/{companyId}/business-natures/{natureCode}?enabled=true`), el SP:
- asegura filas de `CompanyModule` según la plantilla de la naturaleza (no rompe personalizaciones ya existentes),
- deja listo el estándar de defaults por naturaleza para que el admin decida cómo aplicarlo a roles.

**DTOs del endpoint** `PUT /api/v1/system/config/onboarding/companies/{companyId}/business-natures/{natureCode}?enabled=&primary=`:
- Request DTO: N/A (usa query params)
- Response DTO: `ApiResponse<object>` (`{ companyId, natureCode, enabled, primary }`)


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
-   Request DTO: N/A
-   Response DTO: `ApiResponse<CompanyFinancialBootstrapDTO>`
- `PUT /api/v1/secure/finance/bootstrap/profile`
-   Request DTO: `FinancialBootstrapController.SetProfileRequest`
-   Response DTO: `ApiResponse<bool>`
- `POST /api/v1/secure/finance/bootstrap/opening/constitution`
-   Request DTO: `FinancialBootstrapController.ConstitutionRequest`
-   Response DTO: `ApiResponse<long>` (data = `journalEntryId`)
- `POST /api/v1/secure/finance/bootstrap/opening/balances`
-   Request DTO: `FinancialBootstrapController.OpeningBalancesRequest`
-   Response DTO: `ApiResponse<long>` (data = `journalEntryId`)
- `GET /api/v1/secure/finance/bootstrap/validate/balance-equation?asOfUtc=...`
-   Request DTO: N/A
-   Response DTO: `ApiResponse<BalanceEquationCheckDTO>`

### Transacciones financieras (no contables sueltas)

Módulo **FIN**. `CompanyId` por JWT.

| Acción | Método y ruta |
|--------|-----------------|
| Listar (paginado, opcional `from` / `to` en UTC) | `GET /api/v1/secure/finance/transactions?from=&to=&page=&pageSize=` |
| Crear | `POST /api/v1/secure/finance/transactions` |

| | |
|--|--|
| **Request DTO** | GET: N/A. POST: `CreateFinancialTransactionDTO` |
| **Response DTO** | GET: `ApiResponse<PagedResult<FinancialTransactionListItemDTO>>`. POST: `ApiResponse<long>` (data = `txId`) |

---

## POS (motor de venta reutilizable)

El POS está pensado para operar rápido en cualquier naturaleza (todos “venden”).

### Métodos de entrega/recepción (Fulfillment)

Catálogo parametrizable por compañía:
- `GET /api/v1/secure/config/fulfillment-methods?onlyEnabled=true`
  - Request DTO: N/A
  - Response DTO: `ApiResponse<List<FulfillmentMethodDTO>>`

Sistema:
- `GET /api/v1/system/config/fulfillment-methods`
-   Request DTO: N/A
-   Response DTO: `ApiResponse<List<FulfillmentMethodDTO>>`
- `GET /api/v1/system/config/fulfillment-methods/companies/{companyId}`
-   Request DTO: N/A
-   Response DTO: `ApiResponse<List<FulfillmentMethodDTO>>`
- `PUT /api/v1/system/config/fulfillment-methods/companies/{companyId}/{methodCode}?enabled=true`
-   Request DTO: N/A
-   Response DTO: `ApiResponse<object>` (`{ companyId, methodCode, enabled }`)

### Métodos de pago

Catálogo parametrizable por compañía:
- `GET /api/v1/secure/config/payment-methods?onlyEnabled=true`
  - Request DTO: N/A
  - Response DTO: `ApiResponse<List<PaymentMethodDTO>>`

Sistema:
- `GET /api/v1/system/config/payment-methods`
-   Request DTO: N/A
-   Response DTO: `ApiResponse<List<PaymentMethodDTO>>`
- `GET /api/v1/system/config/payment-methods/companies/{companyId}`
-   Request DTO: N/A
-   Response DTO: `ApiResponse<List<PaymentMethodDTO>>`
- `PUT /api/v1/system/config/payment-methods/companies/{companyId}/{methodCode}?enabled=true`
-   Request DTO: N/A
-   Response DTO: `ApiResponse<object>` (`{ companyId, methodCode, enabled }`)

Códigos de cliente / campo de login por app (`ApplicationCompanies`):

- `GET|POST|PUT|DELETE /api/v1/system/config/companies/{companyId}/application-companies` (detalle en la sección **Clientes de aplicación por compañía** más arriba).
  - Request DTO: GET/DELETE: N/A. POST/PUT: `UpsertApplicationCompanyRequestDTO`
  - Response DTO: GET: `ApiResponse<List<ApplicationCompanyDTO>>`. POST/PUT: `ApiResponse<object>` (`{ id }`). DELETE: `ApiResponse<object>` (`{ companyId, id, deleted: true }`)

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
  - Request DTO: `PosController.CreateTicketRequest`
  - Response DTO: `ApiResponse<long>` (data = `ticketId`)

- Agregar línea:
  - `POST /api/v1/secure/pos/tickets/{ticketId}/lines`
  - Request DTO: `PosController.AddTicketLineRequest`
  - Response DTO: `ApiResponse<long>` (data = `lineId`)

- Pagar:
  - `POST /api/v1/secure/pos/tickets/{ticketId}/pay`
  - Nota: el SP valida que el método de pago esté habilitado para la compañía.
  - Request DTO: `PosController.PayTicketRequest`
  - Response DTO: `ApiResponse<bool>`

- Consultar ticket/recibo:
  - `GET /api/v1/secure/pos/tickets/{ticketId}`
  - Devuelve header + líneas + pagos.
  - Request DTO: N/A
  - Response DTO: `ApiResponse<PosTicketReceiptDTO>`

---

## Caja (apertura → movimientos → arqueo → cierre)

Flujo operativo:

- Abrir caja:
  - `POST /api/v1/secure/pos/cash-sessions`
  - Request DTO: `PosController.OpenCashRequest`
  - Response DTO: `ApiResponse<long>` (data = `cashSessionId`)
- Registrar movimiento (entrada/salida):
  - `POST /api/v1/secure/pos/cash-sessions/{cashSessionId}/movements`
  - Request DTO: `PosController.AddCashMovementRequest`
  - Response DTO: `ApiResponse<long>` (data = `movementId`)
- Consultar caja (con movimientos):
  - `GET /api/v1/secure/pos/cash-sessions/{cashSessionId}`
  - Request DTO: N/A
  - Response DTO: `ApiResponse<CashSessionDetailsDTO>`
- Cerrar / arqueo:
  - `POST /api/v1/secure/pos/cash-sessions/{cashSessionId}/close`
  - Request DTO: `PosController.CloseCashSessionRequest`
  - Response DTO: `ApiResponse<CashSessionCloseResultDTO>`

En cierre:
- El sistema calcula **ExpectedClosingAmount** usando apertura + movimientos + pagos en efectivo (configurable).
- Si hay diferencia, genera un asiento PUC opcional (sobrante/faltante) con cuentas por defecto:
  - Caja `110505`
  - Faltantes (gasto) `519595`
  - Sobrantes (ingreso) `429595`

---

## SERVICES: agendas, cobertura, turnos y frecuencias (opcional)

En la naturaleza **SERVICES**, ahora puedes habilitar (por compañía) de forma **opcional**:

- **Agendas** (disponibilidad / calendario operativo)
- **Cobertura** (zonas donde el servicio está disponible)
- **Turnos** y **frecuencias** (plantillas de turno reutilizables)

> Todo esto vive bajo el módulo **SERVICES** y requiere `Authorization: Bearer` + `X-Client: tenant-app` (rutas `/secure`).

### `GET|PUT /api/v1/secure/services/ops/settings`

**GET** devuelve `ServiceCompanySettingsDTO`:

```json
{
  "companyId": 1,
  "enableAgendas": false,
  "enableCoverage": false,
  "enableShifts": false,
  "shiftFrequencyType": "WEEKLY",
  "shiftSlotMinutes": 30
}
```

**PUT body** (`UpdateServiceCompanySettingsRequest`):

```json
{
  "enableAgendas": true,
  "enableCoverage": true,
  "enableShifts": true,
  "shiftFrequencyType": "WEEKLY",
  "shiftSlotMinutes": 30
}
```

| | |
|--|--|
| **Request DTO** | GET: N/A. PUT: `UpdateServiceCompanySettingsRequest` |
| **Response DTO** | GET: `ApiResponse<ServiceCompanySettingsDTO>`. PUT: `ApiResponse<object>` (`{ updated: true }`) |

### Cobertura: `GET|POST|PUT|DELETE /api/v1/secure/services/ops/coverage-areas`

- **GET**: `.../coverage-areas?onlyActive=true`
- **POST/PUT body** (`UpsertServiceCoverageAreaRequest`):

```json
{
  "coverageType": "CITY",
  "countryId": 1,
  "departmentId": 10,
  "cityId": 100,
  "notes": "Zona urbana",
  "active": true
}
```

| | |
|--|--|
| **Request DTO** | GET/DELETE: N/A. POST/PUT: `UpsertServiceCoverageAreaRequest` |
| **Response DTO** | GET: `ApiResponse<List<ServiceCoverageAreaDTO>>`. POST/PUT: `ApiResponse<object>` (`{ id }`). DELETE: `ApiResponse<object>` (`{ id, deleted: true }`) |

### Turnos: `GET|POST|PUT|DELETE /api/v1/secure/services/ops/shifts`

- **GET**: `.../shifts?onlyActive=true`
- **POST/PUT body** (`UpsertServiceShiftTemplateRequest`):

```json
{
  "code": "AM",
  "name": "Turno mañana",
  "startTime": "08:00:00",
  "endTime": "12:00:00",
  "frequencyType": "WEEKLY",
  "interval": 1,
  "daysOfWeekMask": 62,
  "active": true
}
```

| | |
|--|--|
| **Request DTO** | GET/DELETE: N/A. POST/PUT: `UpsertServiceShiftTemplateRequest` |
| **Response DTO** | GET: `ApiResponse<List<ServiceShiftTemplateDTO>>`. POST/PUT: `ApiResponse<object>` (`{ id }`). DELETE: `ApiResponse<object>` (`{ id, deleted: true }`) |

`daysOfWeekMask` usa bitmask: Dom=1, Lun=2, Mar=4, Mié=8, Jue=16, Vie=32, Sáb=64. Ejemplo **62** = Lun–Vie.

### Nota de despliegue (DB)

Estos endpoints dependen de nuevas tablas/SPs incluidos en:
`BusinessCentral.Infrastructure/Persistence/Configuration/StoreProcedures/stored_procedures_all.sql`

- Tablas: `svc.ServiceCompanySettings`, `svc.ServiceCoverageArea`, `svc.ServiceShiftTemplate`
- SPs: `svc.sp_get_service_company_settings`, `svc.sp_upsert_service_company_settings`,
  `svc.sp_list_service_coverage_areas`, `svc.sp_upsert_service_coverage_area`, `svc.sp_delete_service_coverage_area`,
  `svc.sp_list_service_shift_templates`, `svc.sp_upsert_service_shift_template`, `svc.sp_delete_service_shift_template`

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
  - Request DTO: N/A
  - Response DTO: `ApiResponse<EmployeeAvailabilityDTO>`
- `PUT /api/v1/secure/hr/employees/{userId}/availability`
  - Request DTO: `EmployeeAvailabilityDTO`
  - Response DTO: `ApiResponse<bool>`

---

## Servicios (órdenes de servicio)

Módulo **SERVICES**. `CompanyId` por JWT.

| Acción | Método y ruta |
|--------|-----------------|
| Listar (paginado, `status` opcional) | `GET /api/v1/secure/services/orders?status=&page=&pageSize=` |
| Crear | `POST /api/v1/secure/services/orders` |
| Detalle | `GET /api/v1/secure/services/orders/{orderId}` |
| Agregar línea | `POST /api/v1/secure/services/orders/{orderId}/lines` |

| | |
|--|--|
| **Request DTO** | GET list/detalle: N/A. POST create: `ServiceOrdersController.CreateOrderRequest`. POST add line: `ServiceOrdersController.AddLineRequest` |
| **Response DTO** | GET list: `ApiResponse<PagedResult<ServiceOrderDTO>>`. GET detalle: `ApiResponse<ServiceOrderDetailsDTO>`. POST create: `ApiResponse<long>` (data = `orderId`). POST add line: `ApiResponse<long>` (data = `lineId`) |

---

## Base de datos: stored procedures

Los repositorios invocan SPs definidos en `BusinessCentral.Infrastructure/Persistence/Configuration/StoreProcedures/stored_procedures_all.sql`. Tras pull o cambios en ese archivo, **vuelve a ejecutar el script** (o el despliegue equivalente) en SQL Server para que listados, paginación y firmas de `auth.sp_update_user` / `auth.sp_delete_user` coincidan con el código.

---



## Licencia / contribución

Pendiente: define licencia y flujo de contribución según tu modelo de negocio.
