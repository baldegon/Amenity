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
