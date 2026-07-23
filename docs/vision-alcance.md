# Visión y Alcance

## Propósito

Construir una aplicación web modular para administrar licitaciones, proveedores,
ofertas económicas, niveles de aprobación y conversión referencial de moneda,
aplicando exclusivamente **Extreme Programming (XP)** como metodología ágil.

## Problema que resuelve

Una organización necesita publicar licitaciones, recibir ofertas de proveedores,
determinar automáticamente la mejor oferta y el nivel de aprobación
correspondiente, manteniendo el colón costarricense (CRC) como fuente de verdad
y permitiendo una visualización referencial en dólares (USD).

## Alcance incluido

- CRUD completo de Licitaciones, Proveedores, Ofertas, Niveles de Aprobación y
  Tipos de Cambio.
- Ciclo de estados de licitación (Borrador → Publicada → Cerrada).
- Cálculo de mejor oferta y clasificación de ahorro.
- Determinación de aprobador mediante tabla parametrizable de rangos.
- Conversión CRC/USD basada en tipo de cambio administrable localmente.
- Exposición de las operaciones mediante ASP.NET Core MVC y una API REST versionada.
- Persistencia en PostgreSQL con EF Core, migraciones y control de concurrencia.
- Contenerización con Docker/Docker Compose y despliegue en Kubernetes.
- Integración continua con GitHub Actions.

## Alcance excluido

- Autenticación/autorización de usuarios finales (no forma parte de los
  requisitos funcionales mínimos del enunciado).
- Pasarelas de pago o ejecución real de transferencias monetarias.
- Conversión a monedas distintas de USD.
- Notificaciones por correo o mensajería externa.

## Restricción metodológica

El proyecto se desarrolla exclusivamente con XP. No se combina con Scrum,
Kanban ni marcos híbridos, y no se usan roles, ceremonias ni artefactos propios
de esos marcos (ver detalle en `plan-xp.md`).

## Partes interesadas (roles XP)

- **Cliente:** persona docente del curso ITI-822, quien valida historias de
  usuario y criterios de aceptación.
- **Programador(es):** estudiante(s) responsables del desarrollo (modalidad
  individual o en pareja).
- **Pruebas de aceptación:** derivadas directamente de los criterios de
  aceptación de cada historia de usuario.
