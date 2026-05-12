# Plan MVP 4 días — Amenity

Este plan existe para una sola cosa: **llegar a un MVP usable en 4 días para validarlo con personas reales**.

No vamos a optimizar por elegancia ni por features lindas.
Vamos a optimizar por:

- flujo principal completo
- reglas críticas correctas
- tiempo de entrega
- facilidad de prueba por usuarios reales

---

## Quick path

1. Cerrar backend mínimo funcional.
2. Proteger reglas críticas: auth, ownership, reservas sin solapamiento.
3. Exponer métricas mínimas para validar valor.
4. Dejar el sistema corrible por Docker.

---

## Regla de alcance

### Sí entra al MVP

- registro/login
- JWT
- CRUD de propiedades
- crear/cancelar reservas
- no overlapping reservations
- total price
- métricas básicas
- dashboard backend mínimo
- Docker

### No entra al MVP

- frontend fancy
- roles complejos
- refresh tokens
- notificaciones
- pagos
- auditoría avanzada
- dashboards visuales complejos
- observabilidad avanzada

Si aparece algo fuera de esta lista, se posterga.

---

## Definición de MVP lanzable

El MVP está listo cuando una persona puede:

1. registrarse e iniciar sesión
2. crear una propiedad
3. ver sus propiedades
4. crear una reserva válida
5. no poder crear una reserva solapada
6. cancelar una reserva
7. ver métricas básicas del negocio

Si eso funciona, podemos validar la idea.

---

## Flujo crítico a proteger

```text
Usuario se registra -> inicia sesión -> crea propiedad -> crea reserva -> consulta métricas
```

Si ese flujo falla, el MVP no sirve.

---

## Día 1 — Base sólida + Auth

### Objetivo del día

Dejar autenticación real y estructura lista para crecer sin reescribir.

### Tareas

- [x] cerrar refactor base de `Property` a capas si queda algo pendiente
- [x] crear entidad `User`
- [x] definir modelo mínimo de usuario
- [x] agregar tabla/migración de usuarios
- [x] implementar hashing de password
- [x] implementar register
- [x] implementar login
- [x] implementar generación de JWT
- [x] proteger endpoints con authentication/authorization base
- [x] documentar en `LEARNING_LOG.md`

### Resultado esperado

- un usuario puede registrarse
- un usuario puede loguearse
- la API entrega JWT válido
- ya podemos empezar a hablar de ownership real

### Riesgo del día

Perder tiempo en auth "enterprise".

### Límite

Nada de refresh token, confirmación por mail, recuperación de contraseña ni roles sofisticados.

---

## Día 2 — Property completo

### Objetivo del día

Cerrar el módulo de propiedades con modelo correcto para el MVP.

### Tareas

- [ ] completar entidad `Property`
- [ ] agregar campos faltantes:
  - [ ] `Description`
  - [ ] `Address`
  - [ ] `Capacity`
  - [ ] `OwnerId` (o mantener transición desde `UserId` y corregirla)
- [ ] actualizar DTOs
- [ ] implementar update property
- [ ] implementar delete property
- [ ] listar propiedades del usuario autenticado
- [ ] aplicar ownership validation
- [ ] actualizar migración/schema
- [ ] documentar decisiones

### Resultado esperado

- el usuario administra solo sus propiedades
- el CRUD de propiedades queda funcional

### Riesgo del día

Mezclar auth con lógica de negocio de propiedades.

### Límite

No construir filtros avanzados ni búsqueda sofisticada.

---

## Día 3 — Reservation Engine

### Objetivo del día

Implementar el núcleo del producto.

### Tareas

- [ ] crear entidad `Reservation`
- [ ] crear tabla/migración
- [ ] modelar estado mínimo de reserva
- [ ] implementar create reservation
- [ ] implementar cancel reservation
- [ ] implementar list reservations by property
- [ ] validar `StartDate < EndDate`
- [ ] validar propiedad existente
- [ ] impedir overlapping reservations
- [ ] calcular `TotalPrice`
- [ ] agregar tests de reglas críticas
- [ ] documentar reglas en `LEARNING_LOG.md`

### Resultado esperado

- ya existe una reserva válida de punta a punta
- no se permiten reservas incorrectas

### Riesgo del día

Subestimar la lógica de solapamiento.

### Límite

No implementar flujos complejos de calendario.

---

## Día 4 — Metrics + Hardening + Lanzamiento

### Objetivo del día

Dejar el MVP listo para ser probado por usuarios reales.

### Tareas

- [ ] implementar métricas mínimas:
  - [ ] total revenue
  - [ ] active reservations
  - [ ] occupancy %
  - [ ] avg reservation value
- [ ] crear endpoint de resumen global
- [ ] crear endpoint por propiedad
- [ ] agregar middleware centralizado de errores
- [ ] revisar respuestas y errores básicos
- [ ] agregar Dockerfile de la API
- [ ] completar `docker-compose.yml`
- [ ] parametrizar configuración por variables de entorno
- [ ] dejar README mínimo de ejecución
- [ ] smoke test manual del flujo crítico

### Resultado esperado

- sistema corrible
- flujo principal usable
- demo lista para validación

### Riesgo del día

Intentar embellecer en vez de cerrar.

### Límite

Nada de frontend complejo si el backend no está sólido.

---

## Orden obligatorio de ejecución

1. Auth
2. Property
3. Reservation
4. Metrics
5. Docker + release básica

No vamos a alterar este orden salvo bloqueo técnico real.

---

## Checklist de lanzamiento

- [ ] `register` funciona
- [ ] `login` funciona
- [ ] JWT protege endpoints
- [ ] propiedad se crea correctamente
- [ ] propiedad solo la ve/modifica su dueño
- [ ] reserva válida se crea
- [ ] reserva solapada se rechaza
- [ ] reserva puede cancelarse
- [ ] métricas devuelven valores coherentes
- [ ] app levanta con Docker
- [ ] existe guía mínima para probar el sistema

---

## Criterio de decisión diario

Si algo compite entre:

- "queda más lindo"
- "llegamos a probar la idea"

gana siempre:

**llegar a probar la idea**

---

## Qué vamos a hacer ahora

Arrancar por el Día 1:

**User + Auth + JWT**

Sin eso, no existe ownership real y el resto del MVP queda mal apoyado.
