# Eshop Frontend

Questo modulo implementa il **livello di presentazione (Presentation Tier)** del sistema e-commerce sviluppato come progetto di tesi.

Si tratta di una **single-page web application** realizzata con **Angular**, progettata per interagire esclusivamente con il backend tramite **API REST**, senza includere logica di business o accesso diretto ai dati.

Il frontend è concepito come componente **indipendente e sostituibile**, in linea con i principi di un’architettura **multi-tier**.

---

## Contesto architetturale

Nel sistema complessivo:

- il **frontend** gestisce l’interazione con l’utente e la presentazione dei dati
- il **backend** espone le funzionalità applicative e la logica di dominio
- la comunicazione avviene tramite richieste HTTP verso endpoint REST

Questa separazione consente l’evoluzione indipendente dei componenti e favorisce scalabilità e manutenibilità.

---

## Tecnologie utilizzate
- Angular
- TypeScript
- HTML / CSS

---

## Struttura del modulo

```text
Eshop.Frontend/
├─ src/
│  ├─ app/            # Componenti e logica dell’applicazione
│  ├─ assets/         # Risorse statiche
│  └─ environments/   # Configurazioni per ambiente
├─ angular.json
├─ package.json
└─ README.md
```

La struttura segue le convenzioni standard Angular ed è mantenuta intenzionalmente semplice, poiché l’obiettivo del progetto non è la sperimentazione architetturale lato frontend.

---

## Prerequisiti

Per l’esecuzione in ambiente di sviluppo sono richiesti:
- Node.js
- npm
- Angular CLI

---

## Avvio rapido (sviluppo)

Per avviare il frontend localmente:

1. Accedere alla cartella del modulo
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

Durante lo sviluppo, l’applicazione viene ricaricata automaticamente a ogni modifica del codice sorgente.

---

## Integrazione con il backend

Il frontend comunica con il backend tramite **API REST** esposte dal modulo `Eshop.Server.Api`.

Le chiamate API sono utilizzate per:
- autenticazione e gestione degli utenti
- consultazione dei prodotti
- gestione del carrello
- creazione e visualizzazione degli ordini

Il frontend non mantiene stato applicativo persistente lato client oltre a quanto necessario per la sessione utente.

---

## Note
Questo modulo è stato sviluppato a scopo **didattico e sperimentale**, nell’ambito di un progetto di tesi in Reti di Calcolatori.
