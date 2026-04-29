# Julisys — Guía de integración Backoffice + capa compartida (MAUI futuro)

Esta guía concentra **todo lo necesario** para el sitio web de configuración (Centro de Control) contra la API **BusinessCentral**: login, recuperación de clave, refresh token, onboarding de compañías, módulos, usuarios, y cierre de sesión. Incluye una **arquitectura limpia sugerida** en la solución cliente **Julisys** para reutilizar código con los proyectos **móvil y escritorio** (fase posterior).

**Referencia exhaustiva por endpoint** (headers, JSON de entrada/salida, códigos HTTP): ver el README del repo, sección **«Respuesta HTTP y cuerpo»** y subsecciones por ruta (`README.md` → Autenticación).

---

## 1) Principios (backend como “juez”)

- **Canales**: el API distingue Backoffice (`X-Client: backoffice`) y App tenant (`X-Client: tenant-app`).
- **Rutas**:
  - `GET/POST /api/v1/system/**` → **solo staff**; header **`X-Client: backoffice`**.
  - `GET/POST /api/v1/secure/**` → **usuarios de compañía**; header **`X-Client: tenant-app`**.
  - `GET/POST /api/v1/private/**` → backoffice / staff con `SystemRole` (rutas “private”); **no** entran en el filtro estricto `system/secure` del header; aun así envía `Authorization: Bearer` y, por consistencia, puedes enviar `X-Client: backoffice` (recomendado).
- **Respuesta**: cuerpo estándar `ApiResponse<T>` (`isSuccess`, `data`, `message`, `code`, …).
- **JWT** (claims útiles): `userId`, `sub`, `companyId`, `roleId`, `role`, `isSystemRole`, `isSuperUser`, `perm` (repetible), `module` (repetible).

---

## 2) Arquitectura limpia en el cliente (Julisys) — reutilizable

Objetivo: el **mismo núcleo** sirve para `Julisys.Web.Backoffice` hoy y `Julisys.App` (MAUI) mañana.

| Capa | Proyecto sugerido | Contenido |
|------|-------------------|-----------|
| **Domain** (opcional) | `Julisys.Shared.Domain` o carpeta en Shared | Entidades ligeras del cliente (Session, UserContext) sin dependencia de UI. |
| **Application** | `Julisys.Shared.Application` o `Julisys.Shared` | Contratos: `IAuthService`, `ICompanyOnboardingApi`, `IUsersAdminApi`, `ITokenStore`, DTOs de request/response alineados al API. |
| **Infrastructure** | `Julisys.Shared.Infrastructure` o subcarpeta en Shared | `HttpClient` + `DelegatingHandler` (Bearer, `X-Client`, base URL), serialización JSON, manejo de `ApiResponse<T>`. |
| **Presentation** | `Julisys.Web.Backoffice` / `Julisys.App` | Blazor: páginas, componentes, estado (cascading auth), **sin** lógica HTTP duplicada. |

**Regla de oro**: un solo `JulisysApiClient` (o servicios por dominio que usen el mismo `HttpClient` configurado) para Web y MAUI.

### Handlers HTTP recomendados

1. **`AuthorizationHandler`**: adjunta `Bearer` si hay sesión.
2. **`ClientChannelHandler`**: según la base URL de la petición o un contexto `ICurrentChannel`:
   - llamadas a `.../api/v1/system/` → `X-Client: backoffice`
   - llamadas a `.../api/v1/secure/` → `X-Client: tenant-app`
   - para `.../api/v1/private/` → `X-Client: backoffice` (recomendado; coherente con staff).

### Almacenamiento de sesión (Web)

- **Blazor Server**: preferir almacenamiento del lado servidor acotado a circuito o cookie auth según el modelo; documentar la opción elegida.
- Guardar mínimo: `accessToken`, `refreshToken`, `expiresAt`, `userSessionId` (si el login lo devuelve).

---

## 3) Autenticación — Backoffice (sitio web de configuración)

### 3.1 Login (recomendado)

`POST /api/v1/public/auth/login`

**Alternativa explícita (recomendada para Julisys):**

`POST /api/v1/public/auth/backoffice/login`

- Body (sin `companyId`):

```json
{
  "UserName": "superuser@businesscentral.local",
  "Password": "***"
}
```

- Respuesta (`LoginResponseDTO`): incluye `accessToken`, `refreshToken`, `userSessionId`, `userId`, `companyId` (puede ser `0` para staff), `modules[]`, `permissions[]`, `roleId`, `roleName`, `isSystemRole`, `isSuperUser`, etc.

### 3.2 Recuperación de contraseña

**Solicitar reset**

`POST /api/v1/public/auth/password/forgot`

Body (según DTO del API):

```json
{
  "email": "usuario@empresa.com",
  "companyId": 1
}
```

**Importante**: el flujo actual del API está orientado a **usuario tenant** (requiere `companyId` + email). Los **superusuarios sin compañía** pueden necesitar un flujo distinto en producto (correo de staff, soporte, o endpoint dedicado). Si solo gestionas reset para usuarios de una compañía desde el backoffice, envías el `companyId` de esa empresa.

**Confirmar reset**

`POST /api/v1/public/auth/password/reset`

```json
{
  "token": "<token del correo>",
  "newPassword": "********"
}
```

### 3.3 Refresh token

El endpoint expuesto hoy es:

`POST /api/v1/private/users/refresh`

- **Authorization**: `Bearer <accessToken actual (aunque esté por vencer)>`
- Body:

```json
{
  "refreshToken": "<refreshToken>"
}
```

- Respuesta: mismo estilo que login (nuevo access + refresh según implementación del handler).

**Policy**: `SystemRole` (igual que el resto de `private/users`). El backoffice Julisys debe llamar este endpoint **tras login staff**, no la ruta pública.

### 3.4 Logout

Opciones alineadas con el API:

| Ruta | Uso típico Backoffice |
|------|------------------------|
| `POST /api/v1/private/users/logout` | Staff: body opcional `refreshToken`, `sessionId`; lleva Bearer del access token en header (ver implementación `UsersController`). |
| `POST /api/v1/secure/auth/logout` | Pensado para canal **tenant** (`X-Client: tenant-app`); no es el flujo principal del backoffice staff. |

Recomendación Julisys backoffice: **`POST /api/v1/private/users/logout`** con `Authorization: Bearer` + `X-Client: backoffice` (opcional pero útil para trazas).

---

## 4) Gestión de compañías (onboarding y catálogos)

### 4.1 Catálogos previos (GET)

Prefijo: `api/v1/system/config/onboarding` — **`Authorize(Policy = "SystemRole")`** — header **`X-Client: backoffice`**.

| Método | Ruta | Descripción |
|--------|------|-------------|
| GET | `/business-natures` | Naturalezas de negocio |
| GET | `/business-natures/{code}/modules` | Módulos por defecto de una naturaleza |
| GET | `/companies/{companyId}/business-natures` | Naturalezas asignadas a una compañía |
| PUT | `/companies/{companyId}/business-natures/{natureCode}?enabled=&primary=` | Activar/asignar naturaleza |
| GET | `/facility-types` | Tipos de sede |

Catálogos **públicos** (sin token) útiles en formularios:

- `GET /api/v1/public/common/membership-plans`
- `GET /api/v1/public/common/membership-plans/{id}/modules`

### 4.2 Crear compañía (alta completa)

`POST /api/v1/system/config/onboarding/companies`

- Body: `OnboardCompanyRequestDTO` (empresa, plan, sedes, usuario propietario).

Campos clave (resumen):

- Empresa: `companyName`, `tradeName`, `subdomain`, `documentTypeId`, `documentNumber`, `email`, `phone`, `businessNatureCode`
- Suscripción: `membershipPlanId`, `startDateUtc`, `autoRenew`
- Sedes: **`facilities`** (lista) **o** modo legacy `facilityTypeId` + `facilityName` (una sede)
- Propietario: `ownerDocumentTypeId`, `ownerDocumentNumber`, `ownerFirstName`, `ownerLastName`, `ownerEmail`, `ownerPhone`, `ownerPassword`, **`ownerRoleId`**

El hash de `ownerPassword` lo aplica el controller en servidor; el cliente envía **texto plano** según el diseño actual del API.

### 4.3 Módulos por compañía (selector “checklist”)

Prefijo: `api/v1/system/config/modules` — **`SystemRole`** — **`X-Client: backoffice`**.

| Método | Ruta |
|--------|------|
| GET | `/` | Lista módulos del sistema |
| GET | `/companies/{companyId}` | Módulos de la compañía |
| PUT | `/companies/{companyId}/{moduleCode}?enabled=true|false` | Activar/desactivar módulo |

### 4.4 Métodos de pago / cumplimiento (global vs sistema)

| Ámbito | Prefijo | Notas |
|--------|---------|--------|
| System (staff) | `GET /api/v1/system/config/payment-methods` | Catálogo/config sistema |
| System | `GET /api/v1/system/config/fulfillment-methods` | Igual |
| Tenant (empresa) | `GET /api/v1/secure/config/payment-methods` | Requiere JWT de empresa + `X-Client: tenant-app` |

Para el **backoffice** que configura una empresa, lo habitual es usar rutas **`system`** con `companyId` en ruta cuando exista endpoint, o administrar vía onboarding/módulos según el caso.

---

## 5) Usuarios (desde el backoffice staff)

CRUD de usuarios por compañía para soporte / administración global:

- Prefijo: `api/v1/private/users`
- **Policy**: `SystemRole`
- **Authorization**: Bearer del staff

| Método | Ruta |
|--------|------|
| POST | `/` | Crear (body `CreateUserDTO` incluye `companyId`) |
| GET | `/company/{companyId}` | Listado paginado |
| GET | `/company/{companyId}/users/{userId}` | Detalle |
| PUT | `/company/{companyId}/users/{userId}` | Actualizar |
| DELETE | `/company/{companyId}/users/{userId}` | Eliminar |

**Refresh / logout** también viven bajo este controlador (ver sección 3).

---

## 6) Otros endpoints útiles en backoffice

- **Tokens HR / cuentas públicas** (casos especiales): `UserAccessController` bajo `api/v1/system/auth/users/...` (revisar rutas exactas en el proyecto).

---

## 7) Checklist de implementación Julisys.Web.Backoffice

1. Configurar `ApiBaseUrl` en `appsettings`.
2. Implementar `ITokenStore` + login `backoffice/login`.
3. `HttpClient` + handlers: Bearer + `X-Client` según ruta.
4. Pantallas: Login → Dashboard → Lista/selección de compañía (o búsqueda) → Onboarding → Edición de módulos.
5. Integrar refresh antes de expiración del access token (`private/users/refresh`).
6. Logout con `private/users/logout`.
7. Errores: mapear `ApiResponse.message` y HTTP 401/403/402 según negocio.

---

## 8) Checklist fase MAUI / escritorio (tenant)

Cuando implementes la app de compañías:

- Login: `POST /api/v1/public/auth/tenant/login` + `companyId`.
- Todas las llamadas `secure`: `Authorization` + `X-Client: tenant-app`.
- Bootstrap UI: `GET /api/v1/secure/auth/bootstrap`.
- Logout: `POST /api/v1/secure/auth/logout` + `refreshToken` recomendado.

**Reutilización**: mismos contratos en `Julisys.Shared`, solo cambia el canal y las pantallas.

---

## 9) Referencia rápida de headers

| Escenario | Authorization | X-Client |
|-----------|---------------|----------|
| Backoffice → `system` | Bearer staff | backoffice |
| Backoffice → `private` | Bearer staff | backoffice (recomendado) |
| App tenant → `secure` | Bearer usuario empresa | tenant-app |
| Público login / forgot | — | opcional / inferido |

---

*Documento generado para alinear el front Julisys con el estado actual del API BusinessCentral. Si cambian rutas, actualizar este archivo y el README principal.*
