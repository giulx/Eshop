# Eshop Backend

Questo modulo implementa il **Backend** del sistema e-commerce sviluppato come **progetto di tesi**.  
Il backend è realizzato in **.NET (C#)** ed espone **API REST** consumate dal frontend Angular.

L’intero backend è progettato per essere **eseguito esclusivamente tramite Docker**, insieme al database MySQL, così da garantire un ambiente riproducibile, coerente e facilmente avviabile senza configurazioni manuali locali.

L’architettura segue una struttura **multi-layer** ispirata al **Domain-Driven Design (DDD)**, con una separazione netta tra dominio, casi d’uso applicativi, layer di esposizione (API) e infrastruttura (persistenza e servizi tecnici).

---

## Ruolo nel sistema (Multi-Tier)

Nel sistema complessivo containerizzato:

- il **frontend** (Presentation Tier) è un’applicazione Angular eseguita in un container dedicato
- il **backend** (Application Tier) espone API REST ed è eseguito in un container .NET
- il **database MySQL** (Data Tier) è eseguito in un container separato

La comunicazione avviene tramite **rete Docker interna**:
- il frontend comunica con il backend tramite API REST
- il backend accede al database esclusivamente tramite MySQL
- il database non è mai esposto direttamente al frontend

---

## Architettura backend (DDD & multi-layer)

Il backend è suddiviso in quattro progetti principali.

### Eshop.Server.Domain
Contiene il **modello di dominio** e la logica core:
- entità di dominio
- value objects
- eventi di dominio

È il layer più stabile e completamente indipendente da framework, database o dettagli infrastrutturali.

### Eshop.Server.Application
Contiene i **casi d’uso applicativi**:
- application services (orchestrazione delle operazioni)
- DTO (contratti di input/output)
- interfacce verso repository e servizi tecnici

Definisce **cosa fa il sistema**, senza conoscere **come è implementato** a livello tecnico.

### Eshop.Server.Infrastructure
Contiene le **implementazioni tecniche**:
- persistenza e repository (Entity Framework Core + MySQL)
- configurazione database e migrations
- servizi tecnici (JWT, autenticazione, pagamenti simulati)

Questo layer realizza concretamente le interfacce definite nell’Application Layer.

### Eshop.Server.Api
Espone le funzionalità tramite **API REST**:
- controller HTTP
- routing e versioning delle API
- configurazione dependency injection
- configurazione CORS, JWT, Swagger

È il **punto di ingresso del backend** ed è il progetto avviato all’interno del container Docker.

---

## Dipendenze tra i layer (regola di dipendenza)

La struttura rispetta le regole di **Clean Architecture / DDD**:

- `Eshop.Server.Application` → dipende da `Eshop.Server.Domain`
- `Eshop.Server.Infrastructure` → dipende da `Eshop.Server.Application` (e dal Domain se necessario)
- `Eshop.Server.Api` → dipende da `Eshop.Server.Application` e `Eshop.Server.Infrastructure`
- `Eshop.Server.Domain` → non dipende da nessun altro layer

Il dominio rimane isolato dai dettagli infrastrutturali.

---

## Struttura del modulo

Eshop.Backend/  
├─ Eshop.Server.Api/  
│  ├─ Controllers/  
│  ├─ Extensions/  
│  ├─ Mapping/  
│  └─ Properties/  
├─ Eshop.Server.Application/  
│  ├─ ApplicationServices/  
│  ├─ DTOs/  
│  │  ├─ Auth/  
│  │  ├─ Cart/  
│  │  ├─ Order/  
│  │  ├─ Product/  
│  │  └─ User/  
│  └─ Interfaces/  
├─ Eshop.Server.Domain/  
│  ├─ Entities/  
│  ├─ Events/  
│  └─ ValueObjects/  
└─ Eshop.Server.Infrastructure/  
   ├─ Auth/  
   ├─ Migrations/  
   ├─ Payments/  
   └─ Persistence/  
      └─ Repositories/

Le cartelle `bin/` e `obj/` non fanno parte del codice sorgente e sono generate durante la build Docker.

---

## Avvio del backend (Docker)

Il backend **non è pensato per essere avviato manualmente** con `dotnet run`.  
L’esecuzione avviene esclusivamente tramite **Docker Compose**, insieme a frontend e database.

Dalla root del repository:

docker compose up --build

Questo comando:
- costruisce l’immagine Docker del backend
- avvia il container API
- collega il backend al container MySQL
- applica automaticamente le migrations
- esegue il seeding iniziale dei dati

---

## Database (MySQL, migrations e seeding)

Il backend utilizza **MySQL** come Data Tier, eseguito in un container dedicato.

Caratteristiche:
- schema versionato tramite **Entity Framework Core migrations**
- migrations applicate automaticamente all’avvio del container
- **seeding automatico** di utenti, carrelli e prodotti demo
- persistenza garantita tramite **volume Docker**

Il database non contiene dati sensibili ed è ricostruibile integralmente.

---

## Note

- Il sistema di pagamento è **simulato** e non utilizza provider reali
- Il progetto è sviluppato a scopo **didattico e accademico**
- L’obiettivo è dimostrare:
  - progettazione architetturale
  - separazione delle responsabilità
  - uso di DDD e Clean Architecture
  - containerizzazione completa dell’intero stack
