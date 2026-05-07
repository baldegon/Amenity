# Engineering Roadmap — Property Analytics SaaS

**Project Type:** Portfolio / Production-Style SaaS Simulation
**Role:** Backend / Fullstack Engineer
**Prepared By:** Tech Lead Simulation
**Objective:** Deliver a production-grade SaaS platform demonstrating engineering maturity equivalent to junior+/mid backend expectations.

---

# 1. Product Vision

Build a multi-tenant SaaS platform enabling property owners/managers to:

* Manage rental properties
* Track reservations
* Analyze occupancy and profitability
* View operational dashboards
* Manage users securely

This project must be treated as if it were shipping to production.

---

# 2. Engineering Goals

By completion, the engineer should demonstrate proficiency in:

* Production-grade API architecture
* Domain modeling
* Business rule implementation
* Authentication / Authorization
* Data persistence / ORM usage
* SQL relational design
* Frontend/backend integration
* Deployment & DevOps fundamentals
* Engineering communication/documentation

---

# 3. Technical Requirements

## Backend

* ASP.NET Core Web API
* Entity Framework Core
* PostgreSQL or SQL Server

## Frontend

* Astro / React / Preferred Frontend Framework

## Infrastructure

* Docker
* Docker Compose
* Cloud Deployment Target

---

# 4. Architectural Constraints

Project MUST follow layered architecture:

```plaintext id="t6q91j"
Presentation Layer   → Controllers
Application Layer    → Services
Domain Layer         → Entities / Business Rules
Infrastructure Layer → Repositories / Persistence
```

Mandatory principles:

* Separation of Concerns
* Dependency Injection
* DTO Mapping Layer
* Repository Abstraction
* Service-Oriented Business Logic

---

# 5. Functional Scope (MVP)

---

## EPIC 1 — Authentication & User Management

### Deliverables

* User Registration
* User Login
* JWT Access Token Generation
* Password Hashing
* Role-Based Authorization

### Acceptance Criteria

* Users can register and authenticate securely
* Protected endpoints reject unauthorized requests
* Users can only access owned resources

---

## EPIC 2 — Property Management

### Deliverables

* Create Property
* Update Property
* Delete Property
* List User Properties

### Property Fields

* Title
* Description
* Address / Location
* Capacity
* PricePerNight
* OwnerId

### Acceptance Criteria

* CRUD fully operational
* Ownership validation enforced

---

## EPIC 3 — Reservation Engine

### Deliverables

* Create Reservation
* Cancel Reservation
* List Reservations by Property

### Business Rules

* No overlapping reservations
* StartDate < EndDate
* Reservation must belong to valid property

### Acceptance Criteria

* Invalid reservations rejected with proper errors
* Reservation lifecycle tracked

---

## EPIC 4 — Financial Metrics Engine

### Deliverables

* Reservation TotalPrice Calculation
* Monthly Revenue Calculation
* Occupancy Rate Calculation
* Revenue Per Property Calculation

### Acceptance Criteria

* Metrics return accurate computed values
* Business formulas encapsulated in domain/service layer

---

## EPIC 5 — Dashboard Analytics

### Deliverables

* Global Summary Endpoint
* Per-Property Metrics Endpoint

### Metrics

* Total Revenue
* Active Reservations
* Occupancy %
* Avg Reservation Value

---

# 6. Non-Functional Requirements

---

## Error Handling

Implement centralized exception middleware.

Must support:

* Validation errors
* Business rule violations
* Unexpected server errors

---

## Validation

All incoming requests must be validated.

Examples:

* Required fields
* Price > 0
* Valid email format
* Date consistency

---

## Logging

Implement structured logging for:

* Auth events
* Reservation creation
* Critical failures

---

## Testing

Minimum required:

* Unit tests for business rules
* Reservation overlap logic coverage
* Financial calculations coverage

---

# 7. Database Design Expectations

Minimum entities:

```plaintext id="pl3djw"
Users
Properties
Reservations
```

Relationships must be normalized and use foreign keys properly.

Engineer is responsible for:

* Schema design
* Constraints
* Index recommendations where applicable

---

# 8. Deployment Requirements

Application must be deployable via Docker.

Required:

* Dockerfile
* docker-compose.yml
* Environment variable configuration

Deployment target should include:

* Hosted API
* Hosted Database
* Hosted Frontend

---

# 9. Engineering Standards

---

## Git Workflow

* Feature branches per epic
* Pull Request style commits preferred
* Semantic commit messages encouraged

Example:

```plaintext id="85yj2w"
feat(auth): implement JWT authentication
fix(reservations): prevent overlapping bookings
refactor(properties): extract validation service
```

---

## Documentation

Repository must contain:

* Complete README
* Architecture Overview
* ER Diagram
* Setup Instructions
* API Usage Examples

---

# 10. Delivery Milestones

---

## Milestone 1 — Foundation

* Project Setup
* Architecture Setup
* Database Config
* Initial Entities

---

## Milestone 2 — Auth Complete

* Registration/Login
* JWT Security

---

## Milestone 3 — Property Domain Complete

* Full CRUD
* Ownership Rules

---

## Milestone 4 — Reservation Engine Complete

* Booking Logic
* Overlap Validation

---

## Milestone 5 — Metrics Engine Complete

* Revenue / Occupancy / Analytics

---

## Milestone 6 — Frontend Integration

* Basic UI for all flows

---

## Milestone 7 — Production Readiness

* Docker
* Tests
* Logging
* Deploy

---

# 11. Success Criteria

Project is considered complete when:

* All functional requirements are delivered
* Codebase is production-structured
* Business rules are enforced correctly
* Application is deployed publicly
* Engineer can defend architectural decisions in technical interview

---

# Final Tech Lead Note

> Treat every implementation decision as if another engineer will inherit your code tomorrow.
> Optimize for clarity, maintainability, and correctness over speed.

> "Working code" is not enough.
> Production-ready engineering is the standard.
