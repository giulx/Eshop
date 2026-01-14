# Eshop – Web E-commerce Application

Questo repository contiene un **sistema e-commerce web** sviluppato come **progetto di tesi**, progettato secondo un’architettura **multi-tier** con una netta separazione delle responsabilità tra i diversi livelli applicativi.

L’obiettivo del progetto è la realizzazione di un sistema distribuito coerente, manutenibile ed estendibile, applicando principi di **ingegneria del software**, **clean architecture** e **architettura a livelli**.

---

## Panoramica del sistema

Il sistema è composto da due macro-componenti **indipendenti**, orchestrati tramite **Docker**:

- **Frontend**  
  Single-page web application responsabile della presentazione dei dati e dell’interazione con l’utente.

- **Backend**  
  Sistema server-side strutturato in più livelli (API, Application, Domain, Infrastructure) che implementa la logica applicativa, il dominio e la persistenza dei dati.

La comunicazione tra frontend e backend avviene **esclusivamente tramite API REST** su protocollo HTTP.

---

## Architettura

L’architettura complessiva segue un modello **multi-tier**, in cui ogni livello ha responsabilità ben definite e comunica tramite interfacce esplicite.

Schema concettuale:

```
[ Browser ]
     |
     | HTTP / REST
     v
[ Frontend (Angular) ]
     |
     | HTTP / REST
     v
[ Backend API ]
     |
     v
[ Application Layer ]
     |
     v
[ Domain Layer ]
     |
     v
[ Infrastructure / Database ]
```

Questa organizzazione consente:
- isolamento della logica di dominio
- maggiore testabilità
- riduzione dell’accoppiamento tra componenti
- evoluzione indipendente dei livelli

---

## Containerizzazione e orchestrazione

L’intero sistema è **containerizzato tramite Docker** ed eseguito mediante **Docker Compose**.

Ogni componente del sistema è eseguito in un container dedicato:
- frontend
- backend API
- database relazionale

Questo approccio garantisce:
- riproducibilità dell’ambiente di esecuzione
- isolamento dei componenti
- semplificazione dell’avvio e del deployment
- coerenza tra ambienti di sviluppo e test

---

## Avvio del progetto

### Prerequisiti
- Docker
- Docker Compose

### Avvio

Dalla root del repository eseguire:

```
docker compose up --build
```

Al termine dell’avvio:
- frontend disponibile su `http://localhost:4200`
- backend API disponibile su `http://localhost:8080`
- documentazione Swagger disponibile su `http://localhost:8080/swagger`

Il database viene inizializzato automaticamente tramite **migrazioni** e **seeding dei dati** all’avvio dei container.

---

## Struttura del repository

```
.
├─ Eshop.Backend/
│  ├─ Dockerfile
│  └─ README.md
│
├─ Eshop.Frontend/
│  ├─ Dockerfile
│  ├─ proxy.conf.json
│  ├─ proxy.local.json
│  └─ README.md
│
├─ docker-compose.yml
├─ .gitignore
└─ README.md
```

Ogni modulo include un README dedicato che descrive:
- il ruolo architetturale del componente
- le tecnologie utilizzate
- la struttura interna
- i dettagli di esecuzione all’interno dei container

---

## Tecnologie utilizzate

Frontend:
- Angular
- TypeScript
- HTML / CSS

Backend:
- .NET
- ASP.NET Core Web API
- Entity Framework Core
- Database relazionale (MySQL)

Infrastruttura:
- Docker
- Docker Compose

---

## Scopo del progetto

Il progetto è stato sviluppato a scopo **didattico e sperimentale**, nell’ambito di un lavoro di **tesi universitaria** in area **architetture software e sistemi distribuiti**.

L’attenzione è focalizzata su:
- progettazione architetturale
- separazione delle responsabilità
- comunicazione tra componenti distribuiti
- utilizzo di tecniche di containerizzazione

Il sistema non è concepito come prodotto commerciale, ma come **dimostrazione tecnica e progettuale**.

---

## Note

- il repository non contiene dati sensibili
- i container sono progettati per essere autonomi e sostituibili
- l’intero sistema è eseguibile tramite un singolo comando Docker
