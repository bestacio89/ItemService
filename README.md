# 👤 UserService — MOBA Player Domain Service

<p align="center">
  <img width="180" src="./Docs/assets/UserService.png" alt="User Service"/>
</p>

<p align="center">
  <b>Event-Driven Player Domain for a Deterministic MOBA Ecosystem</b>
</p>

<p align="center">
  <img src="https://img.shields.io/badge/.NET-10%2B-blueviolet" />
  <img src="https://img.shields.io/badge/Architecture-DDD%20%7C%20Event%20Sourcing%20%7C%20CQRS-brightgreen" />
  <img src="https://img.shields.io/badge/Messaging-Kafka%20%7C%20RabbitMQ-orange" />
  <img src="https://img.shields.io/badge/Consistency-Event%20Driven-success" />
</p>

---

# ⚡ Overview

**UserService is the core player domain service** of the MOBA platform.

It is responsible for managing the full lifecycle and progression state of a player across the ecosystem.

This service is built using the **Franz runtime architecture**, providing:

* Event-sourced aggregates
* Outbox / Inbox reliability patterns
* Deterministic state reconstruction
* Distributed event propagation (Kafka / RabbitMQ)
* Replayable player state

---

# 🎮 Domain Responsibilities

UserService is NOT a CRUD identity service.

It is a **player state engine**.

## 👤 Identity Layer

* Player creation
* Email / identity binding
* Account lifecycle state (active, suspended, deleted)

---

## 🧬 Progression System

* Hero mastery tracking
* Class proficiency (tank, assassin, mage, etc.)
* XP progression (if enabled in game mode rules)

---

## 🎒 Ownership System

* Hero unlocks
* Skin ownership
* Cosmetic inventory tracking

---

## 📊 Player State Aggregation

* Consolidates all player-related gameplay state
* Serves as the authoritative source for player progression

---

# 📡 Domain Events

UserService produces **game-relevant integration events**:

* `UserCreated`
* `UserProfileUpdated`
* `HeroUnlocked`
* `SkinPurchased`
* `HeroMasteryIncreased`
* `PlayerClassMasteryUpdated`

These events are consumed by:

* HeroService (unlock validation / balancing)
* MatchmakingService (skill & ranking signals)
* InventoryService (cosmetics & ownership sync)
* RankingService (MMR evolution)
* AnalyticsService (player behavior modeling)

---

# 🔄 Event Flow Model

```text
Player Action
     ↓
User Aggregate (Event Sourcing)
     ↓
Domain Event Raised
     ↓
Outbox Persistence (reliable storage)
     ↓
Message Broker (Kafka / RabbitMQ)
     ↓
Downstream Services Consume
     ↓
Inbox prevents duplicate processing
```

---

# 🧠 Core Architecture

UserService is built on:

## 🧬 Event Sourcing

* User state is derived from events
* Full replay capability
* Audit-safe history

## 📦 Aggregate Model

All player mutations occur inside the `User` aggregate:

```csharp
public class User : AggregateRoot<UserEvent>
{
    public string Email { get; private set; }

    public void Create(string email)
    {
        RaiseEvent(new UserCreatedEvent(email));
    }

    public void UnlockHero(Guid heroId)
    {
        RaiseEvent(new HeroUnlockedEvent(heroId));
    }

    public void IncreaseHeroMastery(Guid heroId, int xp)
    {
        RaiseEvent(new HeroMasteryIncreasedEvent(heroId, xp));
    }
}
```

---

# 🔐 Reliability Guarantees

UserService enforces deterministic consistency through:

## 📤 Outbox Pattern

* Events are never lost
* Written alongside state changes

## 📥 Inbox Pattern

* Prevents duplicate processing
* Ensures idempotent event handling

## 🔁 Replay Capability

* Entire user state can be rebuilt from event history
* Supports debugging, rollback, analytics

---

# 🎯 System Role in MOBA Ecosystem

UserService acts as:

> The **source of truth for player progression and ownership**

It is upstream of:

| Service            | Dependency            |
| ------------------ | --------------------- |
| HeroService        | unlock validation     |
| MatchmakingService | skill signals         |
| InventoryService   | cosmetic ownership    |
| RankingService     | progression influence |
| AnalyticsService   | behavioral data       |

---

# 🧩 Example Use Cases

## 🎮 Player unlocks a hero

1. Command hits UserService
2. Aggregate validates rules
3. `HeroUnlocked` event emitted
4. HeroService updates availability
5. Matchmaking recalculates eligibility

---

## 🎮 Player buys a skin

1. Purchase processed in UserService
2. Ownership updated via event
3. InventoryService syncs state
4. Analytics logs purchase pattern

---

# 🧬 Design Philosophy

> This service is not a database wrapper.

It is:

* A deterministic **player simulation engine**
* A **source of truth for progression**
* A **producer of gameplay-relevant events**

Rules:

* State is derived from events
* No hidden mutations
* No cross-service direct writes
* Everything is traceable

---

# 🚀 Tech Stack

* .NET 10+
* Event Sourcing (Franz.Aggregates)
* Kafka / RabbitMQ
* MongoDB / SQL hybrid persistence
* Outbox / Inbox reliability model
* OpenTelemetry tracing

---

# 🐳 Deployment

```bash
docker build -t userservice .
docker run -p 8080:80 userservice
```

Includes:

* Health checks
* Structured logging
* Message broker integration
* Deterministic startup

---

# 📊 Observability

* OpenTelemetry tracing
* Correlation ID propagation
* Structured logs (Serilog)
* Event-level traceability

---

# 🧠 Architectural Constraint Model

UserService enforces:

* No direct cross-service writes
* No domain logic outside aggregates
* No event emission without persistence
* No side effects outside outbox pipeline

---

# 🔥 Summary

UserService is the **core player state engine** of the MOBA platform.

It ensures:

> Every player action is deterministic, replayable, and consistently propagated across the entire ecosystem.

---

