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
Aplikace je postavena na MVC vícevrstvé architektuře (Clean Architecture) se striktním oddělení zodpovědností (Separation of Concerns).

Závislosti:
`Web` ➔ `Application` ➔ `Domain` ⇦ `Infrastructure`

### Architektura aplikace
Aplikace je postavena na MVC vícevrstvé architektuře (Clean Architecture) rozdělené do 4 nezávislých projektů. Toto rozdělení zpřehledňuje kód a odděluje odpovědnosti jednotlivých vrstev.

**1. CrmApp.Domain (Doménová vrstva)**
Základní entity aplikace. Neobsahuje žádné závislosti na externích knihovnách, databázi ani webovém rozhraní.
- **Entity:** `Client`, `Advisor`, `Contract`, `Person`
- **Doménová logika:** Extenze pro výpočet věku z rodného čísla (`BirthNumberExtensions`).

**2. CrmApp.Application (Aplikační vrstva)**
Obsahuje business logiku (Use Cases). Zná doménovou a infrastrukturní vrstvu.
- **Services:** `ClientService`, `AdvisorService`, `ContractService` (a jejich rozhraní pro Dependency Injection). Komunikují přímo s databázovým kontextem.
- **Queries:** Centralizace vyhledávacích a řadících parametrů (`PersonQuery`, `ContractQuery`).
- Zajišťuje CRUD operace a aplikuje business pravidla.

**3. CrmApp.Infrastructure (Datová vrstva)**
Implementuje komunikaci s databází a správu dat.
- **Entity Framework Core:** `CrmDbContext` a Code-first migrace.
- **Mapování a Seed dat:** Konfigurace entit, vazeb a výchozích databázových záznamů probíhá centrálně pomocí `OnModelCreating`.

**4. CrmApp (Prezentační vrstva / Web)**
Vstupní bod aplikace (ASP.NET Core MVC). Komunikuje s aplikační vrstvou pro zpracování požadavků a s infrastrukturou pro inicializaci aplikace.
- **Controllers:** Zajišťují HTTP routing, delegaci požadavků do Services a error handling.
- **ViewModels:** (`ContractFormViewModel`, atd.) Oddělení datových entit od UI, prevence overpostingu a řízení validací.
- **Views:** Uživatelské rozhraní postavené na Razor Views a Bootstrap 5.
- **Prezentační Extenze:** Generování CSV (`CsvExtensions`) a skloňování UI textů (`PluralizationExtensions`).

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

Testy jsou postaveny na frameworku **xUnit** ve spojení s knihovnou **Moq**. Slouží k psaní izolovaných unit testů (unit testing) se zaměřením na logiku service vrstvy a oddělení business pravidel od uživatelského rozhraní.

### Security
- Cookie Authentication middleware
- `[Authorize]` na všech kontrolerech
- CSRF ochrana (`[ValidateAntiForgeryToken]` pro POST requesty)
- User Secrets pro ukonfiguraci přihlášení

### Známá omezení (Known Limitations)
Aktuální verze aplikace má následující omezení:
- **Jednouživatelská autentizace:** Systém aktuálně využívá `User Secrets` pro přihlášení jediného administrátora, neobsahuje plnohodnotný Identity management.

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

- **.NET 10.0 SDK** (nutné pro kompilaci)
- **.NET EF Core Tools** (`dotnet tool install --global dotnet-ef`)
- **SQL Server Express** (lokální databázový server pro uložení dat)

#### Vytvoření databáze

Aby aplikace fungovala, musí mít připravenou strukturu databáze. Otevřete terminál v kořenové složce řešení (`CrmApp`) a spusťte migraci:

```bash
dotnet ef database update --project CrmApp.Infrastructure --startup-project CrmApp
```

#### Spuštění

```bash
dotnet run --project CrmApp
```

#### Spuštění testů

```bash
dotnet test
```
