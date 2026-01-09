# Eshop – Web E-commerce Application

Questo repository contiene un **sistema e-commerce web** sviluppato come **progetto di tesi**, progettato secondo un’architettura **multi-tier** con una netta separazione delle responsabilità tra i diversi livelli applicativi.

L’obiettivo del progetto è la realizzazione di un sistema distribuito coerente, manutenibile ed estendibile, applicando principi di **ingegneria del software**, **clean architecture** e **architettura a livelli**.

---

## Panoramica del sistema

Il sistema è composto da due macro-componenti **indipendenti**:

- **Frontend**  
  Single-page web application responsabile della presentazione dei dati e dell’interazione con l’utente.

- **Backend**  
  Sistema server-side strutturato in più livelli (API, Application, Domain, Infrastructure) che implementa la logica applicativa, il dominio e la persistenza dei dati.

La comunicazione tra frontend e backend avviene **esclusivamente tramite API REST** su protocollo HTTP.

---

## Architettura

L’architettura complessiva segue un modello **multi-tier**, in cui ogni livello ha responsabilità ben definite e non accede direttamente ai livelli inferiori.

Schema concettuale:

```text
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

## Struttura del repository

```text
.
├─ Eshop.Backend/        # Backend (API, Application, Domain, Infrastructure)
│  └─ README.md
│
├─ Eshop.Frontend/       # Frontend Angular (Presentation Tier)
│  └─ README.md
│
├─ .gitignore
└─ README.md             # Documento principale
```

Ogni modulo include un README dedicato che descrive:
- il ruolo architetturale del componente
- le tecnologie utilizzate
- la struttura interna
- le modalità di esecuzione in ambiente di sviluppo

---

## Tecnologie utilizzate

Frontend:
- Angular
- TypeScript
- HTML / CSS

Backend:
- .NET
- ASP.NET Core (Web API)
- Entity Framework Core
- Database relazionale

---

## Avvio del progetto

Frontend e backend possono essere avviati **in modo indipendente**.

Per le istruzioni dettagliate fare riferimento ai README dei singoli moduli:
- `Eshop.Frontend/README.md`
- `Eshop.Backend/README.md`

---

## Scopo del progetto

Il progetto è stato sviluppato a scopo **didattico e sperimentale**, nell’ambito di un lavoro di **tesi universitaria** in area **sistemi distribuiti / reti di calcolatori**.

L’attenzione è focalizzata su:
- progettazione architetturale
- separazione delle responsabilità
- comunicazione tra componenti distribuiti
- applicazione di buone pratiche di sviluppo software

Il sistema non è concepito come prodotto commerciale, ma come **dimostrazione tecnica e progettuale**.

---

## Note

- Il repository non contiene file privati o dati sensibili
- I file di configurazione specifici per ambiente sono esclusi tramite `.gitignore`
- Ogni modulo è progettato per essere **autonomo e sostituibile**
