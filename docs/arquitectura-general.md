# Arquitectura General

## Estilo

Monolito modular en capas, organizado por proyectos independientes dentro de
`/src`, con separación estricta de responsabilidades. Puede evolucionar a
microservicios si la separación se justifica técnicamente (ver sección 6.3 del
enunciado), sin que esto cambie el cumplimiento de los requisitos funcionales.

## Diagrama de capas

```mermaid
graph TD
    Web["Licitaciones.Web<br/>(ASP.NET Core MVC)"]
    Api["Licitaciones.Api<br/>(ASP.NET Core Web API)"]
    App["Licitaciones.Application<br/>(Casos de uso, DTO, validadores)"]
    Dom["Licitaciones.Domain<br/>(Entidades, reglas centrales)"]
    Infra["Licitaciones.Infrastructure<br/>(EF Core, PostgreSQL, repositorios)"]

    Web --> App
    Api --> App
    App --> Dom
    Infra --> Dom
    App --> Infra
```

- **Domain** no depende de ninguna otra capa (sin referencias a EF Core ni a
  ASP.NET Core).
- **Application** orquesta casos de uso y depende solo de contratos
  (interfaces) implementados por Infrastructure.
- **Infrastructure** implementa los puertos definidos en Application/Domain.
- **Web** y **Api** son las dos formas de exponer los mismos casos de uso;
  ninguna contiene lógica de negocio propia (controladores delgados).

## Flujo de una operación típica (registrar oferta)

```mermaid
sequenceDiagram
    participant U as Usuario/Cliente API
    participant W as Web/API (controlador delgado)
    participant A as Application (caso de uso)
    participant D as Domain (reglas de negocio)
    participant I as Infrastructure (EF Core + PostgreSQL)

    U->>W: POST /licitaciones/{id}/ofertas
    W->>A: RegistrarOfertaCommand
    A->>D: Validar reglas (vencimiento, presupuesto, duplicado)
    D-->>A: Oferta válida / excepción de dominio
    A->>I: Persistir oferta
    I-->>A: Confirmación
    A-->>W: Resultado (DTO)
    W-->>U: 201 Created / 400-409-422 con ProblemDetails
```
