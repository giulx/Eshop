# Eshop Frontend

Questo modulo rappresenta il **Frontend** (Presentation Tier) del sistema e-commerce sviluppato come **progetto di tesi**.

Il frontend è una **web application Angular** eseguita **esclusivamente tramite Docker**, progettata per interagire con il backend attraverso API REST e fornire all’utente finale le funzionalità di navigazione del catalogo, gestione del carrello e visualizzazione degli ordini.

Il frontend è un componente **completamente indipendente** dal backend dal punto di vista architetturale, ma integrato a livello di comunicazione tramite rete Docker interna, in accordo con un’architettura **multi-tier containerizzata**.

---

## Ruolo nel sistema (Presentation Tier)

Nel sistema complessivo basato su Docker:

- il **frontend** gestisce esclusivamente l’interfaccia utente
- il **backend** implementa tutta la logica applicativa e di dominio
- il **database** gestisce la persistenza dei dati

Il frontend:
- riceve input dall’utente tramite browser
- invia richieste HTTP al backend
- visualizza i dati restituiti dalle API

Non contiene **alcuna logica di business**, che è interamente demandata al backend.

---

## Tecnologie utilizzate

- Angular
- TypeScript
- HTML / CSS
- Vite (development server)
- Docker

---

## Comunicazione con il backend

La comunicazione con il backend avviene tramite **API REST**.

In ambiente Docker:
- il frontend utilizza un **proxy HTTP Angular** per inoltrare le richieste `/api/**`
- il proxy reindirizza le chiamate verso il container backend (`backend:8080`)
- non sono presenti URL hard-coded con `localhost` nei servizi Angular

Questo approccio consente:
- indipendenza dall’ambiente di esecuzione
- stesso codice frontend per sviluppo e deploy
- integrazione trasparente tra container

---

## Avvio del frontend (Docker)

Il frontend **non è pensato per essere avviato manualmente** con `ng serve`.

L’esecuzione avviene tramite **Docker Compose**, insieme a backend e database.

Dalla root del repository:

docker compose up --build

Questo comando:
- costruisce l’immagine Docker del frontend
- avvia il development server Angular nel container
- configura automaticamente il proxy verso il backend
- espone l’applicazione sulla porta `4200`

L’applicazione è accessibile da browser all’indirizzo:

http://localhost:4200/

---

## Hot reload e sviluppo

Durante l’esecuzione in Docker:
- il frontend supporta **hot reload**
- le modifiche al codice sorgente vengono riflesse automaticamente
- non è necessario ricostruire manualmente l’immagine per ogni modifica

---

## Note

- Il frontend è sviluppato a scopo **didattico e accademico**
- Il progetto dimostra:
  - separazione Presentation / Application / Data Tier
  - comunicazione frontend-backend tramite API REST
  - utilizzo di proxy per l’astrazione dell’infrastruttura
  - containerizzazione completa dell’intero stack applicativo
