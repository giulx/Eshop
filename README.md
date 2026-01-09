# E-commerce Web Multi-Tier

## Descrizione del progetto
Questo progetto è stato realizzato come **tesi in Reti di Calcolatori** e consiste nella progettazione e nello sviluppo di un sistema **e-commerce web** basato su un’**architettura multi-tier**, modulare e scalabile.

L’obiettivo del lavoro è dimostrare come una corretta separazione dei livelli applicativi consenta di realizzare applicazioni web robuste, manutenibili ed estendibili, sfruttando tecnologie moderne lato backend e frontend.

Il sistema è suddiviso in:
- **Backend** sviluppato in **.NET (C#)**, responsabile della logica di business e dell’accesso ai dati
- **Frontend** sviluppato in **Angular (TypeScript)**, che fornisce l’interfaccia utente tramite una web application
- **Database MySQL**, utilizzato per la persistenza dei dati

---

## Architettura
L’applicazione adotta un’architettura **multi-tier** composta da:

1. **Presentation Tier**  
   Web application Angular che consente all’utente di interagire con il sistema tramite browser.

2. **Application / Business Tier**  
   Backend .NET che espone API REST per la gestione delle funzionalità applicative (utenti, prodotti, carrello, ordini).

3. **Data Tier**  
   Database MySQL che memorizza utenti, prodotti, carrelli e ordini.

Questa suddivisione garantisce:
- Separazione delle responsabilità
- Maggiore scalabilità
- Facilità di manutenzione ed estensione

---

## Funzionalità principali

### Utente
- Registrazione e autenticazione
- Visualizzazione dei prodotti
- Gestione del carrello
- Effettuazione di ordini (pagamento simulato)
- Visualizzazione dello storico ordini

### Amministratore
- Visualizzazione e gestione degli utenti
- Visualizzazione e gestione dei prodotti
- Visualizzazione e gestione degli ordini

L’amministratore è implementato come **utente con privilegi speciali**.

---

## Tecnologie utilizzate

### Backend
- .NET (C#)
- API REST
- MySQL

### Frontend
- Angular
- TypeScript
- HTML / CSS

### Database
- MySQL

---

## Struttura del progetto

```text
my-project/
├─ backend/          # Backend .NET (C#)
├─ frontend/         # Frontend Angular (TypeScript)
├─ README.md
└─ .gitignore
```

---

## Avvio del progetto

### Backend
1. Accedere alla cartella backend
2. Configurare la connessione al database MySQL
3. Avviare l’applicazione backend

### Frontend
1. Accedere alla cartella frontend
2. Installare le dipendenze
3. Avviare il server di sviluppo Angular

---

## Note
- Il sistema di pagamento è **simulato** e non utilizza servizi di pagamento reali.
- Il progetto è stato sviluppato a scopo **didattico e sperimentale**.

---

## Autore
Progetto di tesi in Reti di Calcolatori  
Sviluppato da Giulia Nuzzi

