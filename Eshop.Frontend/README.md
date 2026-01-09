# Eshop Frontend

Questo modulo rappresenta il **livello di presentazione (Presentation Tier)** del sistema e-commerce sviluppato nell’ambito del progetto di tesi.

Il frontend è una **web application Angular** che consente agli utenti di interagire con il backend tramite API REST, offrendo funzionalità di navigazione del catalogo, gestione del carrello e visualizzazione degli ordini.

Il frontend è progettato come componente **indipendente** rispetto al backend, in linea con i principi dell’architettura **multi-tier**.

---

## Ruolo nel sistema

Il frontend si occupa esclusivamente di:
- interazione con l’utente tramite browser
- invio di richieste HTTP al backend
- visualizzazione dei dati restituiti dalle API

Non contiene logica di business, che è demandata interamente al backend.

---

## Tecnologie utilizzate
- Angular
- TypeScript
- HTML / CSS

---

## Avvio dell’applicazione

Per avviare il frontend in ambiente di sviluppo:

1. Accedere alla cartella del frontend
```bash
cd Eshop.Frontend
```

2. Installare le dipendenze
```bash
npm install
```

3. Avviare il server di sviluppo
```bash
ng serve
```

L’applicazione sarà disponibile all’indirizzo:
```
http://localhost:4200/
```

Durante lo sviluppo, l’applicazione viene ricaricata automaticamente in seguito a modifiche del codice sorgente.

---

## Comunicazione con il backend

Il frontend comunica con il backend tramite **API REST** esposte dal modulo `Eshop.Server.Api`.  
Gli endpoint vengono utilizzati per la gestione di utenti, prodotti, carrello e ordini.

---

## Note
Questo modulo è stato sviluppato a scopo **didattico e sperimentale** nell’ambito di un progetto di tesi.
