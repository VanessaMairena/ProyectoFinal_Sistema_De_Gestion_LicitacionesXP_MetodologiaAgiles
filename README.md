# Sistema de Gestión de Licitaciones

Proyecto final — Metodologías Ágiles de Desarrollo de Software (ITI-822).
Desarrollado exclusivamente con **Extreme Programming (XP)**.

> Toda la documentación del proyecto vive en [`/docs`](docs/README.md),
> conforme al requisito del enunciado (sección 15). Este README raíz es solo
> un punto de entrada rápido.

## Estado

**Módulos de Proveedores, Niveles de Aprobación y Tipo de Cambio completados**
(dueño: Persona 1). Licitaciones y Ofertas (Persona 2) también están
implementados como base compartida en `main`.

## Cómo ejecutar las pruebas unitarias

```bash
dotnet test tests/Licitaciones.UnitTests/Licitaciones.UnitTests.csproj
```

## Cómo levantar la base de datos y correr el sistema (PostgreSQL local)

1. **Creá la base de datos** en tu PostgreSQL local (usando psql, pgAdmin o DBeaver):
   ```sql
   CREATE DATABASE licitaciones;
   ```
2. **Actualizá la contraseña** en la cadena de conexión de:
   - `src/Licitaciones.Web/appsettings.json`
   - `src/Licitaciones.Api/appsettings.json`

3. **Instalá la herramienta de migraciones de EF Core** (una sola vez):
   ```bash
   dotnet tool install --global dotnet-ef
   ```
4. **Generá y aplicá las migraciones**, desde la carpeta `licitaciones`:

   - Si es la **primera vez** que corrés el proyecto:
     ```bash
     dotnet ef migrations add InicialSchema --project src/Licitaciones.Infrastructure --startup-project src/Licitaciones.Web
     dotnet ef database update --project src/Licitaciones.Infrastructure --startup-project src/Licitaciones.Web
     ```
   - Si **ya tenías la Iteración 2** (proveedores/licitaciones):
     ```bash
     dotnet ef migrations add AgregarOfertas --project src/Licitaciones.Infrastructure --startup-project src/Licitaciones.Web
     dotnet ef database update --project src/Licitaciones.Infrastructure --startup-project src/Licitaciones.Web
     ```
   - Ahora, con **Niveles de Aprobación y Tipo de Cambio** (esta entrega),
     agregá una migración más:
     ```bash
     dotnet ef migrations add AgregarAprobacionYTipoCambio --project src/Licitaciones.Infrastructure --startup-project src/Licitaciones.Web
     dotnet ef database update --project src/Licitaciones.Infrastructure --startup-project src/Licitaciones.Web
     ```

   > Si te da error de "ya existe una migración con cambios pendientes" o
   > algo similar, revisá que no hayas corrido `migrations add` dos veces
   > sin aplicar `database update` en el medio.

5. **Corré el sitio web (MVC):**
   ```bash
   dotnet run --project src/Licitaciones.Web
   ```
   Ya vas a ver en el menú "Niveles de Aprobación" y "Tipo de Cambio".

6. **Corré la API REST** (en otra terminal):
   ```bash
   dotnet run --project src/Licitaciones.Api
   ```
   Documentación interactiva en `https://localhost:7xxx/swagger`, con los
   endpoints nuevos: `/api/v1/niveles-aprobacion` y `/api/v1/tipo-cambio`.

## Estructura

```
/src
  /Licitaciones.Domain          <- entidades y reglas de negocio (sin dependencias externas)
  /Licitaciones.Application     <- próxima iteración
  /Licitaciones.Infrastructure  <- próxima iteración
  /Licitaciones.Web             <- próxima iteración
  /Licitaciones.Api             <- próxima iteración
/tests
  /Licitaciones.UnitTests
  /Licitaciones.IntegrationTests
  /Licitaciones.FunctionalTests
/docs                           <- documentación completa del proyecto (ver docs/README.md)
/k8s                            <- manifiestos de Kubernetes (próxima iteración)
```

Ver [`docs/plan-xp.md`](docs/plan-xp.md) para el plan de liberación completo.
