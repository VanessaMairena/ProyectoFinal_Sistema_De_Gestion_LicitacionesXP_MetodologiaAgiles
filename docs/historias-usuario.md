# Historias de Usuario — Sistema de Gestión de Licitaciones

> Redactadas desde la perspectiva del cliente, como práctica de Planning Game de
> Extreme Programming (XP). Cada historia incluye prioridad, estimación en puntos
> de historia y criterios de aceptación verificables. El vínculo con commits e
> issues se agrega en `bitacora-xp.md` a medida que se implementa cada una.

Escala de estimación: puntos de historia (1, 2, 3, 5, 8), estilo XP clásico.
Prioridad: Alta / Media / Baja, asignada por el cliente en el Planning Game.

---

## Épica A — Proveedores

### HU-01 — Registrar proveedor
**Como** funcionario de compras
**Quiero** registrar un proveedor con un nombre válido y único
**Para** poder asociarle ofertas en las licitaciones

- **Prioridad:** Alta
- **Estimación:** 3
- **Criterios de aceptación:**
  - El nombre solo admite letras, números, espacios, punto, coma y paréntesis.
  - El sistema normaliza (trim, espacios repetidos, Unicode, mayúsculas/minúsculas)
    antes de comparar unicidad.
  - "Empresa Central", "empresa central" y "EMPRESA CENTRAL" se consideran el mismo
    proveedor y el segundo intento debe rechazarse con un mensaje controlado.
  - La unicidad se valida en interfaz, servidor y base de datos (índice único).

### HU-02 — Listar, consultar, editar y eliminar proveedores
**Como** funcionario de compras
**Quiero** administrar el ciclo de vida completo de un proveedor
**Para** mantener el catálogo actualizado

- **Prioridad:** Alta
- **Estimación:** 5
- **Criterios de aceptación:**
  - Listado con paginación, filtrado y ordenamiento.
  - Edición revalida unicidad y formato.
  - Eliminación solicita confirmación previa.
  - Un proveedor con ofertas relacionadas no se elimina físicamente (borrado lógico).
  - Consulta de ofertas relacionadas por proveedor.

---

## Épica B — Licitaciones

### HU-03 — Crear licitación
**Como** funcionario de compras
**Quiero** crear una licitación con código único y fecha/hora de cierre
**Para** iniciar el proceso de recepción de ofertas

- **Prioridad:** Alta
- **Estimación:** 5
- **Criterios de aceptación:**
  - El código es único ignorando espacios laterales y mayúsculas/minúsculas.
  - La fecha y hora de cierre se seleccionan mediante control de calendario, no texto libre.
  - El presupuesto estimado debe ser mayor que cero.
  - La licitación se crea en estado `Borrador`.

### HU-04 — Publicar y cerrar licitación
**Como** funcionario de compras
**Quiero** transicionar el estado de una licitación
**Para** controlar cuándo se reciben ofertas

- **Prioridad:** Alta
- **Estimación:** 5
- **Criterios de aceptación:**
  - Transiciones permitidas: `Borrador → Publicada`, `Borrador → Cerrada`,
    `Publicada → Cerrada`.
  - Transición `Publicada → Borrador` rechazada siempre.
  - `Cerrada → cualquier estado` rechazada, salvo reapertura explícita autorizada.
  - Una licitación con fecha de cierre alcanzada se trata como cerrada
    funcionalmente aunque el campo de estado no se haya actualizado.

### HU-05 — Listar, consultar, editar y eliminar licitaciones
**Como** funcionario de compras
**Quiero** administrar licitaciones existentes
**Para** mantener el proceso de compras al día

- **Prioridad:** Media
- **Estimación:** 5
- **Criterios de aceptación:**
  - Listado con paginación, filtrado por estado y ordenamiento.
  - Borrado lógico cuando existan ofertas relacionadas.
  - Consulta de ofertas, mejor oferta, clasificación y aprobador asociado.

---

## Épica C — Ofertas

### HU-06 — Registrar oferta
**Como** proveedor
**Quiero** registrar una oferta económica para una licitación publicada
**Para** participar en el proceso

- **Prioridad:** Alta
- **Estimación:** 8
- **Criterios de aceptación:**
  - Solo se aceptan ofertas para licitaciones en estado `Publicada` y no vencidas.
  - La oferta no puede superar el presupuesto (igual al presupuesto es válida).
  - Un proveedor no puede registrar más de una oferta por licitación
    (índice único compuesto LicitacionId + ProveedorId).
  - Fecha/hora actual igual o posterior al cierre ⇒ rechazo.
  - Montos mayores que cero, usando `numeric(18,2)`.

### HU-07 — Rechazo de ofertas inválidas
**Como** sistema
**Quiero** rechazar ofertas duplicadas, vencidas o superiores al presupuesto
**Para** proteger la integridad del proceso de licitación

- **Prioridad:** Alta
- **Estimación:** 3
- **Criterios de aceptación:**
  - Mensajes de error controlados y específicos por causa de rechazo.
  - Ninguna oferta inválida se persiste.

### HU-08 — Mejor oferta y clasificación
**Como** funcionario de compras
**Quiero** ver la mejor oferta y su clasificación de ahorro
**Para** decidir con cuál proveedor continuar

- **Prioridad:** Alta
- **Estimación:** 5
- **Criterios de aceptación:**
  - Mejor oferta = menor monto válido en CRC; empate resuelto por orden de registro.
  - Clasificaciones: "Sin ofertas válidas", "Oferta conveniente" (≥10% ahorro),
    "Oferta aceptable" (0%–10%), "Oferta válida sin ahorro" (0% ahorro).
  - Fórmula de ahorro documentada y verificada por prueba unitaria.

---

## Épica D — Niveles de aprobación

### HU-09 — Administrar niveles de aprobación
**Como** administrador del sistema
**Quiero** definir rangos de monto y su aprobador
**Para** que el sistema determine automáticamente quién aprueba cada licitación

- **Prioridad:** Media
- **Estimación:** 5
- **Criterios de aceptación:**
  - CRUD completo de rangos (mínimo, máximo nullable, aprobador).
  - Los rangos no pueden traslaparse.
  - Solo puede existir un rango abierto (sin monto máximo).
  - El aprobador se obtiene por consulta a la tabla, nunca por if/else fijo.

---

## Épica E — Tipo de cambio y conversión

### HU-10 — Administrar tipo de cambio
**Como** administrador del sistema
**Quiero** registrar tipos de cambio CRC/USD con vigencia
**Para** habilitar la conversión referencial de montos

- **Prioridad:** Media
- **Estimación:** 3
- **Criterios de aceptación:**
  - CRUD completo; solo un tipo de cambio puede estar activo a la vez.
  - Activar uno desactiva automáticamente el anterior.
  - El sistema opera sin Internet usando el tipo de cambio local administrado.

### HU-11 — Alternar CRC/USD en la interfaz
**Como** usuario del sistema
**Quiero** alternar la visualización de montos entre CRC y USD
**Para** interpretar los valores en la moneda que prefiera

- **Prioridad:** Media
- **Estimación:** 3
- **Criterios de aceptación:**
  - Los valores oficiales permanecen almacenados solo en CRC.
  - La conversión es una representación calculada (Monto CRC / tipo de cambio).
  - Se muestra la fecha del tipo de cambio utilizado.

---

## Épica F — Interfaz general y navegación

### HU-12 — Landing page y navegación
**Como** usuario nuevo del sistema
**Quiero** entender el propósito de la aplicación desde la página inicial
**Para** orientarme antes de operar el sistema

- **Prioridad:** Media
- **Estimación:** 3
- **Criterios de aceptación:**
  - Explica el flujo de licitación, ofertas, mejor oferta, aprobación y conversión.
  - Menú visible con acceso a todos los módulos y documentación de la API.
  - Diseño adaptable para escritorio y móvil.

### HU-13 — Modo claro/oscuro
**Como** usuario del sistema
**Quiero** alternar entre modo claro y oscuro
**Para** trabajar cómodamente según mis preferencias

- **Prioridad:** Baja
- **Estimación:** 2
- **Criterios de aceptación:**
  - Control visible; la preferencia persiste entre sesiones.

---

## Épica G — API REST

### HU-14 — Exponer operaciones vía API REST
**Como** sistema externo o integrador
**Quiero** ejecutar las operaciones principales mediante endpoints versionados
**Para** integrar el sistema con otras plataformas

- **Prioridad:** Alta
- **Estimación:** 8
- **Criterios de aceptación:**
  - Endpoints listados en `api.md`, con DTO, versionado (`/api/v1`) y OpenAPI/Swagger.
  - Códigos HTTP correctos y respuestas `ProblemDetails` sin exponer detalles internos.
  - No se exponen entidades de Entity Framework Core directamente.

---

## Resumen de priorización (Planning Game)

| Prioridad | Historias |
|---|---|
| Alta | HU-01, HU-03, HU-04, HU-06, HU-07, HU-08, HU-14 |
| Media | HU-02, HU-05, HU-09, HU-10, HU-11, HU-12 |
| Baja | HU-13 |

**Total de puntos estimados:** 63
