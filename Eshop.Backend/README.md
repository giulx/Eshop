# Eshop Backend

Questo modulo implementa l’**Application Tier** del sistema e-commerce sviluppato come progetto di tesi.  
Il backend è realizzato in **.NET (C#)** ed espone **API REST** consumate dal frontend Angular.

Il backend è progettato secondo una struttura **multi-layer** ispirata al **Domain-Driven Design (DDD)**, con separazione netta tra dominio, casi d’uso applicativi, layer di esposizione (API) e infrastruttura (persistenza e servizi tecnici).

---

## Ruolo nel sistema (Multi-Tier)
Nel sistema complessivo:
- il **frontend** (Presentation Tier) gestisce UI e interazione utente
- il **backend** (Application Tier) implementa logica applicativa ed espone endpoint REST
- il **database MySQL** (Data Tier) garantisce la persistenza dei dati

La comunicazione avviene tramite richieste HTTP tra frontend e backend; l’accesso ai dati è gestito esclusivamente dal backend.

---

## Architettura backend (DDD & multi-layer)

Il backend è suddiviso in quattro progetti principali:

### Eshop.Server.Domain
Contiene il **modello di dominio** e la logica core:
- entità di dominio
- value objects
- eventi di dominio

È il layer più stabile e indipendente da framework e dettagli tecnici.

### Eshop.Server.Application
Contiene i **casi d’uso** e i servizi applicativi:
- application services (orchestrazione delle operazioni)
- DTO (contratti di input/output)
- interfacce (astrazioni) verso repository e servizi tecnici

Non dipende dall’infrastruttura: definisce “cosa serve”, non “come è implementato”.

### Eshop.Server.Infrastructure
Contiene le **implementazioni tecniche**:
- persistenza e repository (accesso dati)
- integrazione con MySQL
- servizi tecnici (es. autenticazione/JWT, pagamenti simulati)
- migrations del database

### Eshop.Server.Api
Espone le funzionalità tramite **API REST**:
- controllers e routing HTTP
- gestione richieste/risposte
- configurazione e dependency injection
- mapping tra DTO e modelli

È il **punto di ingresso** del backend (entry point) e il progetto che va avviato in esecuzione.

---

## Dipendenze tra i layer (regola di dipendenza)
Schema consigliato (coerente con DDD / Clean Architecture):

- `Eshop.Server.Application` → dipende da `Eshop.Server.Domain`
- `Eshop.Server.Infrastructure` → dipende da `Eshop.Server.Application` (e, se necessario, da `Eshop.Server.Domain`)
- `Eshop.Server.Api` → dipende da `Eshop.Server.Application` e `Eshop.Server.Infrastructure`
- `Eshop.Server.Domain` → non dipende da altri layer

In questo modo il dominio resta isolato dai dettagli infrastrutturali.

---

## Struttura del modulo

```text
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
│  │  └─ Utente/
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
```

Nota: le cartelle `bin/` e `obj/` sono artefatti di build e non fanno parte del codice sorgente.

---

## Avvio del backend (sviluppo)

Il progetto da avviare è **Eshop.Server.Api**.

### Avvio da terminale
Dalla root del repository (o dalla root del backend), eseguire:

```bash
dotnet run --project Eshop.Backend/Eshop.Server.Api/Eshop.Server.Api.csproj
```

Il backend espone gli endpoint REST e può essere utilizzato dal frontend durante lo sviluppo.

### Avvio da Visual Studio
Aprire la solution e impostare `Eshop.Server.Api` come **Startup Project**, quindi avviare l’esecuzione.

---

## Database (MySQL) e migrations

Il sistema utilizza **MySQL** come livello di persistenza (Data Tier).  
La struttura del database è versionata tramite **migrations** (presenti nel progetto `Eshop.Server.Infrastructure/Migrations`).

Questo significa che:
- il database “fisico” non viene caricato su GitHub
- lo schema è ricostruibile applicando le migrations (ambiente di sviluppo)

---

## Note
- Il pagamento è **simulato** e non utilizza provider reali.
- Il progetto è stato sviluppato a scopo **didattico e accademico**, nell’ambito di una tesi in Reti di Calcolatori.
