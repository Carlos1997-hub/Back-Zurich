# ZurichApp (.NET 8) – Backend API + xUnit Tests

Backend API en **.NET 8** para la administración de **Clientes, Pólizas y Cotizaciones**, con autenticación basada en **JWT**, autorización por **roles** (`Administrador`, `Cliente`), acceso a datos con **Dapper** y arquitectura por capas (**Controllers → Services → Repositories → SQL**).

---

## Contenido
- [Stack](#stack)
- [Arquitectura](#arquitectura)
- [Estructura del proyecto](#estructura-del-proyecto)
- [Flujo de autenticación JWT](#flujo-de-autenticación-jwt)
- [Roles y autorización](#roles-y-autorización)
- [Capas y responsabilidades](#capas-y-responsabilidades)
- [Configuración](#configuración)
- [Ejecución](#ejecución)
- [Endpoints principales](#endpoints-principales)
- [Archivo ZurichApp.http](#archivo-zurichapphttp)
- [Pruebas xUnit](#pruebas-xunit)
- [Buenas prácticas y seguridad](#buenas-prácticas-y-seguridad)

---

## Stack
- **.NET 8** (ASP.NET Core Web API)
- **Dapper** (acceso a datos)
- **JWT** (Bearer tokens)
- **xUnit + Moq + FluentAssertions** (unit tests)
- **Middleware global** para manejo de errores: `ExceptionMiddleware`

---

## Arquitectura
El proyecto está diseñado con una separación clara de responsabilidades:

- **Controllers**: exponen endpoints HTTP, validan parámetros simples y devuelven respuestas HTTP.
- **Services**: implementan reglas de negocio (ej. validaciones, “Mine” por JWT, cancelación de póliza, etc.).
- **Repositories**: acceso a datos; ejecutan queries con Dapper usando SQL centralizado.
- **Data/Sql**: queries SQL en archivos estáticos por módulo.
- **Models**: entidades base (User, Client, Policy, Quote).
- **Dtos**: contratos de entrada/salida de la API.
- **Middleware**: manejo centralizado de excepciones y respuestas de error.

---

## Flujo de autenticación JWT

### 1) Login
- El usuario envía credenciales a `AuthController`.
- `AuthService.LoginAsync`:
  - Consulta al repo: `_repo.GetByUsernameOrEmailAsync(request.UsernameOrEmail)`
  - Valida: usuario exista, tenga `RoleName`, esté activo (`IsActive`)
  - Verifica password con `IPasswordHasher.Verify(password, hash, salt)`
  - Genera JWT con `IJwtTokenService.CreateToken(user, roleName)`
  - Retorna `LoginResponse` con `AccessToken`, `ExpiresIn` y datos del usuario.

### 2) Consumo de API con token
- El frontend envía el token en header:
  - `Authorization: Bearer {accessToken}`
- El pipeline valida el token (issuer, audience, firma, expiración).
- Se aplican reglas de autorización por roles y endpoints.

---

## Roles y autorización

El proyecto usa `[Authorize]` y `[Authorize(Roles = "...")]` en controladores/acciones:

- **Clientes** (`ClientsController`):
  - `[Authorize(Roles = "Administrador")]` a nivel de controller
  - Solo Admin puede listar/crear/editar/eliminar clientes.

- **Pólizas** (`PoliciesController`):
  - Admin:
    - `GET /api/policies` (todas o por clientId)
    - `POST /api/policies` (crear)
    - `PATCH /api/policies/{id}/cancel` (cancelar)
  - Cliente:
    - `GET /api/policies/mine` (mis pólizas desde JWT)

- **Cotizaciones** (`QuotesController`):
  - Admin:
    - `GET /api/quotes` (todas o por clientId)
    - `POST /api/quotes` (crear)
  - Cliente:
    - `GET /api/quotes/mine` (mis cotizaciones desde JWT)

---

## Capas y responsabilidades

### Controllers
Responsables de exponer endpoints y retornar respuestas HTTP:

- `AuthController`: login
- `ClientsController`: CRUD clientes (Admin)
- `PoliciesController`: consulta y creación/cancelación pólizas (Admin/Cliente)
- `QuotesController`: consulta y creación de cotizaciones (Admin/Cliente)

### Services
Orquestan reglas de negocio y delegan al repositorio:

- `AuthService`: login y generación de token
- `ClientService`: CRUD de clientes
- `PolicyService`: consultar, crear, cancelar, y “mine”
- `QuoteService`: consultar, crear, y “mine”
- `CurrentUserService`: extrae `UserId/ClientId/Role` desde `HttpContext.User` (claims)
- `JwtTokenService`: construye JWT (claims, expiración, firma)
- `PasswordHasher`: hashing con salt, y verificación segura

### Repositories (Dapper)
- Ejecutan SQL mediante `DbConnectionFactory` y Dapper
- Mantienen las consultas SQL en `Data/Sql/*Sql.cs`

### SQL centralizado
- `AuthSql.cs`: usuario/roles, inserción y password update
- `ClientsSql.cs`: queries de clientes
- `PoliciesSql.cs`: queries de pólizas
- `QuotesSql.cs`: queries de cotizaciones

---

## Configuración

En `appsettings.json` típicamente se configuran:

- **ConnectionStrings** (SQL Server)
- **JWT**:
  - `Issuer`
  - `Audience`
  - `Secret` (clave para firmar)
  - `ExpiresMinutes` (o similar)

El proyecto lee estas opciones mediante:
- `AppSettings/ConnectionStringsOptions.cs`
- `AppSettings/JwtOptions.cs`

---

## Ejecución

### Requisitos
- .NET SDK 8 instalado
- SQL Server accesible (local o remoto)
- Connection string configurada en `appsettings.json` (o variables de entorno)

### Ejecutar API
Desde la carpeta del proyecto:

```bash
dotnet restore
dotnet build
dotnet run --project ZurichApp/ZurichApp.csproj


