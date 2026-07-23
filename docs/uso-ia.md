# Uso de Inteligencia Artificial

Registro de transparencia sobre el uso de asistentes de IA durante el
desarrollo del proyecto. El objetivo es dejar evidencia clara de qué se generó
con asistencia, y cómo el/los integrante(s) comprendieron, validaron y
adoptaron cada aporte como propio, dado que la defensa individual exige poder
explicar y modificar en vivo cualquier parte del sistema.

> **Importante:** usar un asistente de IA no exime de comprender cada línea de
> código, cada decisión de diseño y cada regla de negocio implementada. La
> defensa oral y la modificación práctica en vivo verifican precisamente eso.

## Formato de registro (completar por cada uso relevante)

| Fecha | Herramienta | Tarea asistida | Qué se usó tal cual / qué se modificó | Cómo se validó |
|---|---|---|---|---|
| _(ej. 2026-07-20)_ | _(ej. Claude)_ | _(ej. Andamiaje inicial de carpetas y documentación XP)_ | _(ej. estructura y documentación generadas y luego revisadas/editadas por el equipo)_ | _(ej. lectura completa, ajuste de estimaciones propias, ejecución del build)_ |

## Principios seguidos por el equipo

- Toda regla de negocio implementada con ayuda de IA fue leída, entendida y,
  cuando fue necesario, corregida antes de integrarse.
- Ninguna prueba se aceptó como "pasando" sin ejecutarla localmente.
- Las decisiones de arquitectura (separación Domain/Application/Infrastructure/
  Web/API) fueron discutidas y comprendidas por el equipo, no solo copiadas.
- Este archivo se actualiza en cada iteración, junto con `bitacora-xp.md`.
