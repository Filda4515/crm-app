# CrmApp

Webová aplikace vytvořená v ASP.NET Core MVC pro správu klientů, poradců a smluv. 

---

# Uživatelská dokumentace

### Přihlášení
Po spuštění aplikace je uživatel přesměrován na přihlašovací stránku.

Přístup do aplikace je umožněn po zadání správného uživatelského jména a hesla.

### Hlavní navigace
Aplikace obsahuje tři hlavní moduly v horní navigaci:
- **Klienti**
- **Poradci**
- **Smlouvy**

### Vyhledávání
Vyhledávání je dostupné napříč seznamy a podporuje:
- jméno
- příjmení
- rodné číslo
- instituci
- evidenční číslo smlouvy

Vyhledávání ignoruje velikost písmen i diakritiku.

### Řazení dat
Záznamy lze řadit kliknutím na hlavičku sloupce. Opakovaným kliknutím se mění směr řazení (vzestupně ▲ / sestupně ▼).

### Export do CSV
Aktuálně zobrazená data lze exportovat do CSV.

Export respektuje aktivní filtr.

### Filtrování smluv
Možnost skrýt neaktivní (vypršelé) smlouvy (datum konce platnosti již uplynulo). Filtr lze kombinovat s vyhledáváním i řazením. 

Pokud nejsou nalezeny žádné záznamy, aplikace zobrazí informační hlášku.

### Detailní pohledy
Každý záznam obsahuje akci **Detail**, která zobrazí kompletní informace.

Aplikace umožňuje provázání entit:
- klient → smlouva → poradce

### Vytváření a úprava záznamů
Formuláře Formuláře obsahují validaci v reálném čase.

Pokud uživatel nezadá povinné pole, nebo vloží text ve špatném formátu, zobrazí se chybová hláška pod konkrétním polem a uložení je zablokováno.

**Specifika smluv:**
Při vytváření a editaci smluv uživatel vybírá:
* **Klient a Správce smlouvy:** výběr z dropdown menu.
* **Účastníci (Poradci):** multi-select (`CTRL` pro výběr více osob).

Business pravidlo:
- **Správce smlouvy** musí být vždy mezi **Účastníky**. 

### Bezpečné mazání
Mazání probíhá přes modální dialog.

Systém:
- kontroluje navázané smlouvy
- umožňuje kaskádové smazání po potvrzení uživatelem
- chrání referenční integritu databáze

---

# Technická dokumentace

### Použité technologie
- ASP.NET Core MVC
- Razor Views
- Entity Framework Core
- MS SQL Server Express
- Bootstrap
- xUnit
- Moq


### Architektura aplikace
Aplikace je postavena na MVC architektuře se separací do více vrstev.

**Controllers**
- ClientsController
- AdvisorsController
- ContractsController
- AccountController
- ErrorsController

Controllers zajišťují:
- HTTP routing
- validace vstupů (ModelState)
- delegace logiky do service vrstvy
- globální error handling


**Services** (Business vrstva)
- ClientService
- AdvisorService
- ContractService

Services zajišťují:
- CRUD operace
- implementace business logiky
- filtrování a řazení dat
- izolace databázové logiky od controllerů
- testovatelnost backendu

**Models**
- Client
- Advisor
- Contract
- Person

**ViewModels (`Models/ViewModels`)**
- ClientIndexViewModel
- ContractIndexViewModel
- PersonFormViewModel
- ContractFormViewModel
- LoginViewModel

ViewModels zajišťují:
- oddělení databázových entit od UI
- overposting prevence
- bezpečný datový přenos mezi vrstvami

**Query objekty (`Models/Queries`)**
- PersonQuery
- ContractQuery

Query objekty zajišťují:
- centralizace filtrů
- řízení řazení
- enkapsulace vyhledávacích parametrů
- čisté a explicitní service API bez nutnosti předávání velkého množství parametrů

**Datová vrstva**
- Entity Framework Core
- SQL Server Express
- Code-first migrace
- seed dat pro testování
- kolace Latin1_General_CI_AI (case-insensitive + diacritics-insensitive vyhledávání)

**Extensions**
- CsvExtensions → export dat do CSV
- BirthNumberExtensions → validace a práce s rodnými čísly

**Views**

UI frontend využívající **Razor Views** s použitím knihovny **Bootstrap**.

### Směrování a Endpointy (Routes)
Aplikace obsluhuje CRUD operace pro každou z hlavních entit (`Clients`, `Advisors`, `Contracts`):

- **`GET /{Entita}`** → hlavní seznam záznamů
- **`GET /{Entita}/Details/{id}`** → detail záznamu
- **`GET /{Entita}/Create`** → formulář pro vytvoření nového záznamu
- **`POST /{Entita}/Create`** → vytvoření nového záznamu (validace a uložení nových dat).
- **`GET /{Entita}/Edit/{id}`** → formulář pro editaci existujícího záznamu
- **`POST /{Entita}/Edit/{id}`** → update záznamu (validace a uložení upravených dat).
- **`POST /{Entita}/Delete/{id}`** → smazání záznamu
- **`GET /{Entita}/ExportCsv`** → export filtrovaných dat do Csv

### Testování
Testovací projekt: `CrmApp.Tests`

Testy jsou postaveny na frameworku **xUnit** ve spojení s knihovnou **Moq**. Test-Driven Development (TDD) přístup se zaměřením na izolaci business logiky od UI.

### Security
- Cookie Authentication middleware
- `[Authorize]` na všech kontrolerech
- CSRF ochrana (`[ValidateAntiForgeryToken]` pro POST requesty)
- User Secrets pro ukonfiguraci přihlášení

## Spuštění aplikace

Před spuštěním aplikace si nastavte přihlašovací údaje do User Secrets projektu. Aplikace očekává sekci `AdminCredentials` s poli `Username` a `Password`. Bez této konfigurace se nelze přihlásit.

Konfigurace:
```json
{
  "AdminCredentials": {
    "Username": "vaše_jméno",
    "Password": "vaše_heslo"
  }
}
```

#### Požadavky

- .NET SDK
- SQL Server Express

#### Vytvoření databáze

```bash
dotnet ef database update
```

#### Spuštění

```bash
dotnet run
```

#### Spuštění testů

```bash
dotnet test
```