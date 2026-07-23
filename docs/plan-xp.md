# Plan XP — Sistema de Gestión de Licitaciones

Este documento define el plan de liberación y el plan de cada iteración, siguiendo
Extreme Programming (XP) como única metodología rectora. No se utilizan artefactos
de Scrum ni Kanban (no hay Sprint Backlog, Sprint Planning ni tablero Kanban rector).

## Plan de liberación (Release Plan)

Se plantean **cinco iteraciones** de duración uniforme (1 semana cada una), cerrando
con una versión ejecutable y demostrable al final de cada una, tal como exige la
práctica de "pequeñas liberaciones".

| Iteración | Objetivo | Historias incluidas | Entregable demostrable |
|---|---|---|---|
| **1** | Dominio y persistencia base | HU-01, HU-02 (parcial) | Modelo de dominio con pruebas unitarias en verde; migraciones iniciales en PostgreSQL |
| **2** | Proveedores y Licitaciones | HU-01, HU-02, HU-03, HU-04, HU-05 | CRUD de Proveedores y Licitaciones funcionando en MVC + API |
| **3** | Ofertas y reglas de negocio | HU-06, HU-07, HU-08 | Registro de ofertas con validaciones completas; mejor oferta visible |
| **4** | Aprobación, moneda e interfaz | HU-09, HU-10, HU-11, HU-12, HU-13 | Niveles de aprobación, conversión CRC/USD, landing page, modo oscuro |
| **5** | API completa, calidad y despliegue | HU-14 + pruebas E2E, Docker, Kubernetes, CI | Sistema desplegado en Kubernetes con pipeline de CI/CD en verde |

## Reglas de trabajo XP acordadas con el cliente

- **Planning Game:** al inicio de cada iteración se revisan las historias
  pendientes, se confirma o reestima su costo y se seleccionan las que entran
  en la iteración según la velocidad observada.
- **Iteraciones cortas:** una semana calendario cada una, mínimo tres
  (se plantean cinco para cubrir todo el alcance funcional).
- **Pequeñas liberaciones:** cada iteración cierra con una versión ejecutable,
  incluso si el alcance funcional es parcial.
- **TDD:** ninguna regla de negocio se implementa sin una prueba que falle primero.
- **Integración continua:** cada rama se integra frecuentemente contra `main`;
  el pipeline de GitHub Actions debe estar en verde antes de cerrar una historia.
- **Diseño simple:** se implementa la solución más simple que cumpla las
  historias vigentes; no se anticipan requisitos no solicitados.
- **Refactorización:** se ejecuta de forma continua, nunca pospuesta a un
  "sprint de limpieza" (terminología que, de hecho, no aplica aquí).
- **Propiedad colectiva del código:** ambos integrantes (si aplica modalidad
  en pareja) pueden modificar cualquier módulo.
- **Ritmo sostenible:** el trabajo se distribuye a lo largo de la semana; el
  historial de commits debe reflejar avance incremental, no concentración al final.
- **Programación en parejas:** con rotación de roles (conductor/observador)
  documentada en `bitacora-xp.md`, cuando la modalidad sea de dos integrantes.

## Velocidad planificada vs. observada

La velocidad (puntos de historia completados por iteración) se registrará al
cierre de cada iteración en `bitacora-xp.md`, comparando lo planificado contra
lo realmente completado, sin usar terminología de "sprint".

| Iteración | Puntos planificados | Puntos completados (se completa al cierre) |
|---|---|---|
| 1 | 8 | — |
| 2 | 18 | — |
| 3 | 16 | — |
| 4 | 16 | — |
| 5 | 8 (+ calidad/infra) | — |

## Artefactos que sí se usan (recordatorio)

Historias de usuario · Planning Game · plan de liberación · plan de iteración ·
cliente/programadores/pruebas de aceptación · velocidad XP · integración continua
· refactorización.

## Artefactos que NO se usan

Product Backlog · Sprint Backlog · Scrum Board · Sprint Planning · Daily Scrum ·
Sprint Review/Retrospective · Product Owner/Scrum Master · límites WIP · flujo Kanban.
