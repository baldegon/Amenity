# Bitácora técnica de aprendizaje — Amenity

> Este archivo existe para que el desarrollo NO quede como una caja negra.
> La idea es registrar qué se hizo, por qué se hizo, qué concepto hay detrás y qué tengo que estudiar.

---

## 1. Objetivo de esta bitácora

Este proyecto no se está construyendo solo para "que funcione".

Se está construyendo para:

- llegar al MVP pedido en `Requirements.md`
- aprender arquitectura backend real
- entender cada decisión importante
- mantener control sobre el sistema

### Regla de trabajo

Cada cambio importante debe dejar registro de:

1. **Qué se hizo**
2. **Por qué se hizo**
3. **Qué concepto arquitectónico hay detrás**
4. **Qué debería repasar/estudiar**

---

## 2. Estado actual del proyecto

### Estado inicial encontrado

Cuando se analizó el repo, se encontró:

- solución `.NET 8`
- proyecto `Reservas.Api`
- proyecto `Reservas.Domain`
- proyecto `Reservas.Infrastructure`
- tests básicos de dominio
- `docker-compose.yml` solo para PostgreSQL

### Problemas detectados

- se estaba usando **minimal API**
- `Program.cs` hablaba directo con `ApplicationDbContext`
- no existía capa `Application`
- solo existía una entidad `Property` incompleta
- no existían `User` ni `Reservation`
- no existía auth con JWT
- no existían services, repositories ni controllers tradicionales
- no existía todavía la arquitectura pedida por `Requirements.md`

### Conclusión

El repo servía como base inicial, pero no estaba listo para crecer correctamente hacia el MVP.

---

## 3. Decisión arquitectónica #1 — No usar Minimal APIs

### Qué se decidió

Se decidió abandonar el enfoque de minimal APIs y usar:

- **Controllers**
- **Services / casos de uso**
- **Repositorios**
- **DTOs**
- **Arquitectura en capas**

### Por qué se decidió

Porque el documento de requerimientos exige explícitamente:

- Presentation Layer → Controllers
- Application Layer → Services
- Domain Layer → Entities / Business Rules
- Infrastructure Layer → Repositories / Persistence

Además, el objetivo del proyecto es aprender y construir con claridad, no solo resolver rápido.

### Concepto detrás

**Separación de responsabilidades**.

Una API HTTP no debería:

- contener lógica de negocio
- validar reglas complejas inline
- hablar directamente con EF Core en cada endpoint

Si eso pasa, el sistema se vuelve difícil de mantener, testear y escalar.

### Qué estudiar

- diferencia entre Minimal API y Controllers
- Separation of Concerns
- Layered Architecture

---

## 4. Decisión arquitectónica #2 — Crear la capa `Reservas.Application`

### Qué se hizo

Se creó el proyecto:

- `src/Reservas.Application`

También se agregó a la solución y se conectó con las referencias correctas.

### Estructura actual de capas

- `Reservas.Api`
- `Reservas.Application`
- `Reservas.Domain`
- `Reservas.Infrastructure`

### Por qué se hizo

Porque faltaba el lugar donde deben vivir los **casos de uso**.

Si solo existieran `Api`, `Domain` e `Infrastructure`, la lógica terminaría otra vez mal ubicada:

- en `Program.cs`
- en controllers gordos
- o pegada a EF Core

La capa `Application` existe para coordinar acciones del sistema como, por ejemplo:

- crear una propiedad
- registrar un usuario
- crear una reserva
- cancelar una reserva
- calcular métricas

### Concepto detrás

**Application Layer** = orquestación del negocio.

No representa transporte HTTP.
No representa almacenamiento.
Representa el "qué hace el sistema".

### Qué estudiar

- diferencia entre Domain y Application
- casos de uso / use cases
- services vs entities

---

## 5. Dirección de dependencias

### Cómo quedó

- `Reservas.Application` → depende de `Reservas.Domain`
- `Reservas.Infrastructure` → depende de `Reservas.Application` y `Reservas.Domain`
- `Reservas.Api` → depende de `Reservas.Application` y `Reservas.Infrastructure`

### Por qué quedó así

Porque las dependencias tienen que apuntar desde lo más externo hacia lo más central del negocio.

### Explicación simple

#### `Domain`
Es el corazón del sistema.
No debería depender de web, base de datos ni swagger.

#### `Application`
Usa el dominio para ejecutar casos de uso.

#### `Infrastructure`
Implementa detalles técnicos concretos:

- EF Core
- PostgreSQL
- repositorios
- persistencia

#### `Api`
Expone endpoints HTTP y delega en Application.

### Concepto detrás

**Dependency direction**.

El negocio no debe depender de tecnología específica.
La tecnología debe adaptarse al negocio.

### Qué estudiar

- inversión de dependencias
- clean architecture vs layered architecture
- por qué Domain no debe depender de Infrastructure

---

## 6. Limpieza hecha en las capas

### Qué se hizo

Se eliminaron referencias innecesarias de OpenAPI/Swagger en:

- `Reservas.Domain`
- `Reservas.Infrastructure`

### Por qué

Porque Swagger pertenece a la capa web/API, no al dominio ni a infraestructura.

### Concepto detrás

**Aislar responsabilidades y evitar contaminación de capas**.

Una dependencia incorrecta no rompe solo compilación: rompe diseño.

### Qué estudiar

- acoplamiento
- cohesión
- contaminación de capas

---

## 7. Qué NO se hizo todavía

Todavía NO se hizo:

- refactor de `Program.cs` a controllers
- creación de DTOs
- creación de services
- creación de interfaces de repositorio
- creación de entidades `User` y `Reservation`
- implementación de JWT
- ownership real
- métricas financieras

### Por qué todavía no

Porque primero había que enderezar la estructura.

Si se construye sobre una base desordenada, después se reescribe todo.

---

## 8. Próximo paso planificado

El siguiente paso lógico es:

### Refactorizar `Property` correctamente

Eso implica:

1. sacar lógica de `Program.cs`
2. crear controller tradicional
3. crear DTOs
4. crear service de aplicación
5. crear contrato de repositorio
6. implementar repositorio en Infrastructure

### Por qué arrancar por `Property`

Porque ya existe una base mínima y sirve para aprender el patrón completo con un caso más chico antes de meternos con auth y reservations.

---

## 9. Mapa mental del sistema que queremos construir

### Api

Responsable de:

- recibir requests HTTP
- devolver responses HTTP
- autenticar/autorización a nivel web
- configurar middleware

### Application

Responsable de:

- ejecutar casos de uso
- validar reglas de aplicación
- coordinar repositorios
- mapear DTOs

### Domain

Responsable de:

- entidades
- reglas de negocio puras
- invariantes
- enums / excepciones de dominio

### Infrastructure

Responsable de:

- EF Core
- DbContext
- repositorios concretos
- persistencia
- integración con servicios técnicos

---

## 10. Dudas que conviene hacer siempre

Antes de aceptar un cambio, revisar:

1. ¿Esto pertenece a esta capa?
2. ¿Estoy mezclando HTTP con negocio?
3. ¿Estoy acoplando dominio a EF o a web?
4. ¿Esto va a ser fácil de testear?
5. ¿Entiendo por qué existe esta clase?

Si alguna respuesta es "no sé", hay que frenar y entender antes de seguir.

---

## 11. Convención para seguir documentando

Cada vez que hagamos un cambio importante, vamos a agregar una entrada nueva con este formato:

```md
## Decisión / Cambio

### Qué se hizo

### Por qué se hizo

### Qué concepto hay detrás

### Qué debería estudiar o repasar
```

---

## 12. Idea clave

El objetivo no es que yo "toque cosas" por vos.

El objetivo es que vos puedas decir:

> "Entiendo qué partes tiene el sistema, por qué existen y cómo se conectan."

ESO es tener control.

---

## 13. Refactor del flujo `Property` a capas reales

### Qué se hizo

Se sacó el flujo de `Property` de `Program.cs` y se repartió en piezas con responsabilidad clara:

- `PropertiesController` en `Reservas.Api`
- `PropertyService` en `Reservas.Application`
- `IPropertyRepository` como contrato en `Reservas.Application`
- `PropertyRepository` en `Reservas.Infrastructure`
- DTOs para create/list/get
- `Program.cs` quedó como composition root

También se movió la validación básica de creación a Application para que la regla no viva pegada al transporte HTTP.

### Por qué se hizo

Porque si `Program.cs` recibe HTTP, valida, consulta EF Core y persiste todo junto, entonces NO hay arquitectura en capas: hay un archivo haciendo demasiadas cosas.

Este refactor deja el primer flujo del sistema con el recorrido correcto:

**HTTP -> Controller -> Application Service -> Repository Contract -> Repository EF Core**

### Qué concepto hay detrás

**Separación de responsabilidades + dirección de dependencias**.

- el controller entiende HTTP
- el service entiende el caso de uso
- el repositorio abstracto define lo que Application necesita
- Infrastructure resuelve el detalle técnico con EF Core

Esto baja acoplamiento y hace más fácil crecer después con auth, ownership real y tests de aplicación.

### Qué debería estudiar o repasar

- diferencia entre DTO y entidad de dominio
- por qué Application no debe depender de EF Core
- por qué `Program.cs` debe ser composition root y no caso de uso
- `CreatedAtAction` y convenciones REST básicas

---

## 14. Auth base + ownership real de `Property`

### Qué se hizo

Se agregó la primera base real de identidad y seguridad del sistema:

- entidad `User`
- registro de usuario
- login
- hashing de contraseñas con PBKDF2
- generación de JWT
- autenticación bearer en la API
- relación real `User 1:N Properties`

Además, `Property` dejó de confiar en un `UserId` libre enviado por el cliente para crear y listar propiedades.

### Por qué se hizo

Porque antes el sistema tenía un problema serio: cualquier cliente podía mandar un `UserId` arbitrario y actuar como si fuera otro usuario.

Eso rompe:

- seguridad
- integridad de datos
- ownership real
- confianza del producto

La solución correcta fue que la identidad salga del usuario autenticado, no del body del request.

### Qué concepto hay detrás

#### Authentication
Responde: **quién sos**.

En este proyecto se resuelve con login + JWT.

#### Authorization
Responde: **qué podés hacer**.

En este proyecto se aplica al dejar que cada usuario opere solo sobre sus propios recursos.

#### Password Hashing
Las contraseñas no se guardan en texto plano.
Se guarda un hash seguro para reducir el riesgo en caso de exposición de la base.

#### Ownership
Una `Property` pertenece a un `User`.
Eso implica que el backend debe derivar el owner desde la identidad autenticada y no desde datos manipulables del request.

### Qué debería estudiar o repasar

- JWT en ASP.NET Core
- diferencia entre authentication y authorization
- claims
- PBKDF2 / password hashing
- relación `1:N` entre `User` y `Property`
- por qué no confiar datos sensibles enviados por el cliente

---

## 15. Endurecimiento de ownership en `Property`

### Qué se hizo

Se cerró la regla de ownership real también para:

- `get by id`
- `update`
- `delete`

La validación quedó alineada entre:

- controller
- service
- repository

También se agregaron tests y se verificó el comportamiento esperado.

### Por qué se hizo

Porque no alcanza con proteger solo el create o el list.

Si un usuario puede consultar, editar o borrar una propiedad ajena, entonces el sistema sigue estando roto desde el punto de vista de seguridad.

### Qué concepto hay detrás

#### Resource Ownership
No alcanza con estar autenticado.
También hay que verificar que el recurso sobre el que actuás te pertenezca.

#### Defense in Depth
La seguridad no debe descansar en una sola capa.
Por eso la regla se sostuvo de forma consistente en el flujo de aplicación, no como un parche aislado.

#### 404 vs 403
Se eligió devolver **404** cuando una property no existe o no pertenece al usuario autenticado.

Esto evita confirmar la existencia de recursos ajenos y reduce enumeración de IDs.

### Qué debería estudiar o repasar

- diferencia entre 404 y 403
- information leakage
- resource enumeration
- ownership checks en APIs REST
- por qué la seguridad no debe vivir solo en el controller

---

## 16. Estrategia de base de datos y despliegue inicial

### Qué se decidió

Se decidió continuar con **PostgreSQL** como base de datos del producto.

### Por qué se decidió

Porque el proyecto ya está orientado a Postgres y, para un MVP real con despliegue rápido, da un camino más simple y portable que volver a SQL Server.

Además, encaja mejor con un esquema moderno de despliegue cloud para una API .NET.

### Estrategia inicial

- desarrollo local con Docker + PostgreSQL
- API .NET corriendo localmente
- despliegue inicial sugerido:
  - API en Render
  - base de datos en Neon

### Qué concepto hay detrás

No se elige una base solo por costumbre local.
Se elige también por:

- portabilidad
- facilidad de despliegue
- costo operativo
- fricción de mantenimiento

### Qué debería estudiar o repasar

- PostgreSQL básico
- connection strings en ASP.NET Core
- variables de entorno
- estrategia de migraciones EF Core
- diferencia entre entorno local y producción

---

## 17. Plan de estudio recomendado hasta este punto

### Orden sugerido

1. entender el problema del producto en `Requirements.md`
2. repasar la arquitectura base en este `LEARNING_LOG.md`
3. entender el flujo de auth
4. entender ownership de `Property`
5. recién después pasar a `Reservation`

### Preguntas que debería poder responder

1. ¿Por qué `Program.cs` no debe contener lógica de negocio?
2. ¿Por qué se creó la capa `Application`?
3. ¿Qué diferencia hay entre authentication y authorization?
4. ¿Qué problema resuelve JWT en este proyecto?
5. ¿Por qué no se debe aceptar `UserId` libre en el request?
6. ¿Por qué se eligió 404 y no 403 para recursos ajenos?

### Recursos de apoyo

- Microsoft Learn: ASP.NET Core Web API
- Microsoft Learn: Authentication / Authorization
- Microsoft Learn: Entity Framework Core
- documentación oficial de JWT y claims en ASP.NET Core
- documentación oficial sobre password hashing
- Martin Fowler: Layered Architecture
- Clean Architecture / Dependency Direction
- Nick Chapsas
- Milan Jovanović

### Idea clave

No estudiar queriendo memorizar clases.
Estudiar queriendo entender:

- qué problema resuelve cada pieza
- por qué existe
- por qué vive en esa capa

Ese cambio mental es el que empieza a formar criterio profesional real.

---

## 14. Ownership real de `Property` para get/update/delete

### Qué se hizo

Se cerró el ownership real de `Property` para las operaciones sensibles del MVP backend:

- `GET /api/properties/{id}` sigue filtrando por usuario autenticado
- se agregó `PUT /api/properties/{id}` con validación en Application
- se agregó `DELETE /api/properties/{id}`
- repository, service y controller quedaron alineados con la misma regla de ownership
- se agregaron tests para demostrar que un usuario no puede actualizar ni borrar una property ajena

### Por qué se hizo

Porque ligar una property al usuario solo al crearla NO alcanza.

Si después `get-by-id`, `update` o `delete` no verifican ownership, el sistema queda roto desde seguridad: cualquier usuario autenticado podría intentar operar sobre IDs ajenos.

En un MVP serio, la regla no puede ser parcial. Tiene que ser consistente en TODO el recorrido.

### Qué concepto hay detrás

**Autorización por ownership + no enumeración de recursos**.

La decisión elegida fue responder **404** cuando la property no existe **o** cuando existe pero no pertenece al usuario autenticado.

¿Por qué 404 y no 403 en este MVP?

Porque `403` confirma implícitamente que el recurso existe pero no te pertenece.
Eso ayuda a enumerar recursos ajenos.

Con `404`, desde afuera el mensaje es el mismo:

> "para vos, este recurso no existe"

Eso es más seguro y además mantiene una política simple y coherente para `get-by-id`, `update` y `delete`.

También quedó una idea importante: la autorización NO debe depender solo del controller.
La capa Application y el repository tienen que colaborar para que la consulta/actualización llegue ya filtrada por `UserId`.

### Qué debería estudiar o repasar

- diferencia entre autenticación y autorización
- ownership-based access control
- cuándo usar `404` vs `403`
- por qué las reglas de acceso deben ser consistentes entre controller, service y repository

### Próximo paso sugerido

El siguiente batch lógico es endurecer todavía más el módulo `Property` antes de pasar a `Reservation`:

- tests más integrados de persistence/auth
- estandarizar resultados de aplicación para casos `validation/not-found`
- revisar si conviene ocultar `UserId` en ciertos response DTOs públicos del futuro

---

## 18. Reservation foundation con ownership real y regla anti-solapamiento

### Qué se hizo

Se agregó la primera base real del módulo de reservas:

- entidad `Reservation`
- relación `Property 1:N Reservations`
- DTOs de create/list/cancel
- contrato `IReservationRepository`
- `ReservationService` en Application
- `ReservationsController` anidado bajo `api/properties/{propertyId}/reservations`
- persistencia EF Core + migración
- tests de reglas críticas

Además, la reserva calcula `TotalPrice` en backend y no permite crear reservas activas solapadas para la misma property.

### Por qué se hizo

Porque ya no alcanzaba con tener auth y ownership de properties.

El MVP necesita empezar a cerrar el caso de negocio central: una property tiene disponibilidad implícita y el sistema debe impedir que el owner genere reservas inconsistentes.

Si el backend permitiera:

- fechas inválidas
- reservas solapadas
- operar reservas sobre properties ajenas

entonces el módulo nacería roto desde dominio y desde seguridad.

### Qué concepto hay detrás

#### Ownership transitivo
La reserva NO referencia todavía a `User`.

Se eligió el mínimo correcto para este batch: `Reservation` depende de `Property`, y el ownership se resuelve transitivamente por `Property.UserId`.

Eso alcanza para las operaciones owner-side actuales sin meter una relación extra que todavía no aporta una regla de negocio real.

#### Interval overlap
Dos reservas activas se consideran solapadas cuando:

- `existing.StartDate < new.EndDate`
- `new.StartDate < existing.EndDate`

Esta forma evita errores comunes de bordes y permite que una reserva termine el mismo día en que otra empieza, sin contar eso como conflicto.

#### Backend como fuente de verdad
`TotalPrice` se calcula en servidor usando:

- cantidad de noches = `EndDate - StartDate`
- precio por noche actual de la property

Eso evita confiar en un monto enviado por cliente, que sería manipulable.

#### Cancelar != borrar
Se eligió cancelación lógica con `Status` + `CancelledAtUtc`.

Esto conserva trazabilidad mínima y además permite que una reserva cancelada deje de bloquear disponibilidad futura.

### Qué debería estudiar o repasar

- modelado de intervalos de fechas
- diferencias entre cancelación lógica y borrado físico
- aggregate boundaries básicos
- cuándo una relación extra (`Reservation -> User`) agrega valor real y cuándo es ruido
- por qué el backend debe calcular datos derivados sensibles

### Preguntas que debería poder responder ahora

1. ¿Por qué `StartDate < EndDate` y no `<=`?
2. ¿Cómo se detecta un solapamiento de intervalos sin caer en casos borde?
3. ¿Por qué `TotalPrice` no debería venir del cliente?
4. ¿Por qué en este batch `Reservation` no necesita todavía un `UserId` propio?
5. ¿Qué ventaja da cancelar lógicamente en vez de borrar la fila?

---

## 19. Cierre de Día 1 MVP — auth mínima real y entendible

### Qué se hizo

Se terminó de dejar cerrada la base mínima de autenticación del MVP con estas piezas alineadas:

- `User` persistido con `Email`, `PasswordHash`, `Role` y `CreatedAtUtc`
- register/login por controller tradicional
- hashing PBKDF2 para no guardar contraseñas en texto plano
- JWT con claims de `user id`, `email` y `role`
- migración para persistir el rol simple del usuario

### Por qué se hizo

Porque el MVP no necesitaba un sistema de permisos complejo, pero SÍ necesitaba una identidad real sobre la cual apoyar ownership y autorización básica.

Se eligió un `Role` string simple porque enseña el concepto sin meter ruido innecesario:

- no agrega complejidad de roles jerárquicos
- deja la puerta abierta para crecer después
- mantiene el modelo fácil de leer para esta etapa

### Qué concepto hay detrás

#### Claims como contrato de identidad

El JWT no es solo un "ticket de acceso".
También es el contrato mínimo con el que la API entiende quién hace la request.

Por eso se incluyeron claims concretos y útiles para el MVP:

- identificador del usuario
- email
- role

#### Simplicidad intencional

Acá había una tentación clásica: empezar con permisos sofisticados "por si después hacen falta".

Eso para un MVP es una mala decisión.
Primero resolvemos la identidad REAL y el ownership básico.
Después, si el producto lo pide, escalamos autorización.

### Qué debería estudiar o repasar

- diferencia entre entidad de usuario y claims del token
- cuándo usar un `Role` simple y cuándo un sistema de permisos más rico
- por qué PBKDF2 sigue siendo una opción válida para un MVP backend en .NET
- cómo una migración acompaña cambios reales del dominio sin mezclar lógica HTTP
