# E-commerce Web Application  
## Multi-Tier Architecture with DDD-Based Backend

## Overview
Questo repository contiene un sistema **e-commerce web full-stack** sviluppato come **progetto di tesi in Reti di Calcolatori**.  
Il lavoro affronta la progettazione e l’implementazione di un’architettura **multi-tier**, affiancata da una struttura **multi-layer** del backend ispirata ai principi del **Domain-Driven Design (DDD)**.

L’obiettivo è analizzare e dimostrare come la separazione dei livelli di un sistema distribuito e la corretta organizzazione della logica applicativa consentano di realizzare applicazioni web **modulari, scalabili e manutenibili**.

---

## System Architecture
Il sistema è organizzato secondo una **architettura multi-tier**, in cui i livelli sono separati sia logicamente sia a livello di comunicazione:

- **Presentation Tier**  
  Web application sviluppata in **Angular (TypeScript)**, eseguita nel browser dell’utente e responsabile dell’interazione con il sistema.

- **Application Tier**  
  Backend sviluppato in **.NET (C#)** che espone **API REST** e gestisce la logica applicativa, fungendo da punto di comunicazione tra client e sistema informativo.

- **Data Tier**  
  Database **MySQL**, dedicato alla persistenza dei dati relativi a utenti, prodotti, carrelli e ordini.

La separazione dei tier consente una comunicazione client-server basata su rete, coerente con i principi dei sistemi distribuiti.

---

## Backend Architecture (DDD & Multi-Layer)
All’interno dell’Application Tier, il backend è ulteriormente strutturato secondo un’architettura **multi-layer** ispirata al **Domain-Driven Design**, con una chiara separazione delle responsabilità:

### Domain
Contiene il **modello di dominio**, includendo:
- entità
- regole di business fondamentali
- logica core dell’applicazione  

Questo layer è indipendente da framework, database e tecnologie esterne.

### Application
Raccoglie i **servizi applicativi** e i **casi d’uso**, coordinando le operazioni tra il dominio e gli strati esterni.  
Include DTO e logica applicativa, senza dipendenze dirette dall’infrastruttura.

### API (Controllers)
Espone le funzionalità del sistema tramite **API REST**.  
Gestisce le richieste HTTP, l’autenticazione e l’autorizzazione, delegando l’elaborazione ai servizi applicativi.

### Infrastructure
Contiene le **implementazioni tecniche**, tra cui:
- accesso al database MySQL
- implementazione dei repository
- servizi di supporto (autenticazione, pagamenti simulati)

Questa suddivisione migliora manutenibilità, testabilità ed estendibilità del backend.

---

## Modularity and Scalability
La **modularità** del sistema è garantita dalla suddivisione del backend in layer distinti, che riduce l’accoppiamento tra i componenti e consente l’evoluzione indipendente delle parti applicative.

La **scalabilità** è favorita da:
- architettura **multi-tier**, che permette di scalare frontend, backend e database in modo indipendente
- esposizione di API REST stateless
- separazione tra logica di dominio e dettagli infrastrutturali

Queste caratteristiche rendono il sistema adatto a scenari di crescita sia funzionale sia di carico.

---

## Functionalities

### User
- Registrazione e autenticazione
- Consultazione del catalogo prodotti
- Gestione del carrello
- Conferma degli ordini (pagamento simulato)
- Visualizzazione dello storico degli ordini

### Administrator
- Gestione degli utenti
- Gestione dei prodotti
- Gestione degli ordini

L’amministratore è implementato come **utente con privilegi estesi**, con accesso a funzionalità di gestione avanzate.

---

## Technologies Used

### Backend
- .NET (C#)
- RESTful APIs
- MySQL

### Frontend
- Angular
- TypeScript
- HTML / CSS

### Database
- MySQL

---

## Repository Structure

```text
my-project/
├─ backend/
│  ├─ Domain/           # Modello di dominio (DDD)
│  ├─ Application/      # Servizi applicativi e casi d’uso
│  ├─ API/              # Controller e API REST
│  └─ Infrastructure/   # Persistenza e servizi tecnici
├─ frontend/            # Frontend Angular
├─ README.md
└─ .gitignore
```

---

## Execution Model
Il frontend e il backend sono progettati come componenti separati, comunicanti tramite HTTP, in linea con i principi dell’architettura client-server e dei sistemi distribuiti.

---

## Notes
- Il sistema di pagamento è **simulato** e non utilizza servizi di pagamento reali.
- Il progetto è stato sviluppato esclusivamente a scopo **didattico e accademico**, nell’ambito di un lavoro di tesi.

---

## Author
Progetto di tesi in Reti di Calcolatori  
**Giulia Nuzzi**
