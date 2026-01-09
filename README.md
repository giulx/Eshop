# E-commerce Web Application – Multi-Tier Architecture

## Overview
Questo repository contiene un sistema **e-commerce web full-stack** sviluppato come **progetto di tesi in Reti di Calcolatori**.  
Il lavoro si concentra sulla progettazione e implementazione di un’**architettura multi-tier** modulare e scalabile, con una chiara separazione tra livelli di presentazione, logica applicativa e persistenza dei dati.

L’applicazione consente la comunicazione client-server tramite una web application e API REST, simulando un contesto reale di utilizzo di sistemi distribuiti.

---

## System Description
Il sistema è composto da tre componenti principali:

- **Frontend**: web application sviluppata in Angular (TypeScript), responsabile dell’interazione con l’utente
- **Backend**: applicazione server-side sviluppata in .NET (C#), che implementa la logica di business ed espone API REST
- **Database**: sistema di gestione MySQL per la memorizzazione persistente dei dati applicativi

L’architettura è progettata per favorire manutenibilità, estendibilità e scalabilità del sistema.

---

## Architectural Model
L’applicazione segue un modello **multi-tier**, articolato come segue:

### Presentation Layer
Livello di presentazione realizzato tramite Angular, accessibile via browser, che gestisce l’interfaccia utente e l’invio delle richieste HTTP al backend.

### Application / Business Layer
Livello applicativo implementato in .NET che gestisce:
- logica di business
- validazione dei dati
- esposizione delle API REST
- controllo dei flussi applicativi

### Data Layer
Livello di persistenza basato su MySQL, utilizzato per la gestione di utenti, prodotti, carrelli e ordini.

Questa architettura consente una netta separazione delle responsabilità e un’evoluzione indipendente dei singoli livelli.

---

## Backend Structure
Il backend è organizzato secondo una suddivisione a **moduli/layer**:

- **API**: gestione delle richieste HTTP e degli endpoint REST
- **Application**: servizi applicativi, casi d’uso e DTO
- **Domain**: entità di dominio e logica core
- **Infrastructure**: accesso ai dati, persistenza MySQL e servizi di supporto

Questa struttura migliora la leggibilità del codice e riduce l’accoppiamento tra i componenti.

---

## Functionalities

### User Features
- Registrazione e autenticazione
- Consultazione del catalogo prodotti
- Gestione del carrello
- Conferma degli ordini (pagamento simulato)
- Visualizzazione dello storico ordini

### Administrator Features
- Gestione degli utenti
- Gestione dei prodotti
- Gestione degli ordini

L’amministratore è implementato come **utente con privilegi estesi**.

---

## Technologies

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
├─ backend/          # Backend .NET (C#)
├─ frontend/         # Frontend Angular (TypeScript)
├─ README.md
└─ .gitignore
```

---

## Execution
Il progetto è strutturato per essere eseguito separatamente lato frontend e backend, consentendo una chiara distinzione tra client e server.

---

## Notes
- Il sistema di pagamento è **simulato** e non utilizza servizi di pagamento reali.
- Il progetto è stato sviluppato esclusivamente a scopo **didattico e accademico**.

---

## Author
Progetto di tesi in Reti di Calcolatori  
**Giulia Nuzzi**
