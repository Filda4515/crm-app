# CrmApp

Základní CRM (Customer Relationship Management) systém navržený pro správu klientů, finančních poradců a jejich smluv. 

---

## Uživatelská dokumentace

Webová aplikace. Uživatelské rozhraní se skládá z hlavní horní navigační lišty a pracovního prostoru pro správu jednotlivých záznamů.

### Hlavní navigace
V horní tmavé liště (Navbar) se nachází odkazy na tři hlavní moduly aplikace:
* **Klienti:** Správa osobních údajů klientů.
* **Poradci:** Správa osobních údajů poradců.
* **Smlouvy:** Správa smluv a jejich provázání s klienty a poradci.

### Práce se záznamy (Seznamy a Detaily)
Po kliknutí na libovolný modul se zobrazí **přehledová tabulka** s uloženými záznamy. Aplikace poskytuje pokročilé nástroje pro snadnou orientaci v datech:
* **Vyhledávání:** Nad každou tabulkou se nachází textové pole pro vyhledávání (podle jména, příjmení, rodného čísla, instituce či evidenčního čísla). Vyhledávání **ignoruje velikost písmen i diakritiku** (např. dotaz "bezny" bez problémů najde záznam "Běžný").
* **Řazení (Sorting):** Záznamy lze interaktivně řadit kliknutím na názvy sloupců v hlavičce tabulky. Opakovaným kliknutím se mění směr řazení (vzestupně ▲ / sestupně ▼).
* **Filtrování smluv:** V modulu Smlouvy je navíc k dispozici speciální filtr *„Skrýt neaktivní (vypršelé)“*, který vyfiltruje smlouvy, jejichž datum konce platnosti již uplynulo.
* **Export do CSV:** Zobrazená data lze jedním kliknutím exportovat do souboru CSV. Export respektuje aktuálně nastavené filtry i řazení, takže se stáhnou přesně ta data, která uživatel vidí na obrazovce.
* Pokud je databáze prázdná, nebo zadaným filtrům neodpovídá žádný záznam, aplikace na to uživatele upozorní.
* V pravé části každého řádku se nachází akční tlačítka. Tlačítko **Detail** otevře kartu s kompletními informacemi. Díky provázanosti databáze se lze např. z detailu klienta prokliknout rovnou na detail jeho smlouvy a odtud přímo na kartu správce dané smlouvy.

### Vytváření a úprava záznamů
Formuláře pro vytvoření (tlačítko **Přidat...**) i pro úpravu záznamů obsahují okamžitou validaci (kontrolu chyb). Pokud uživatel nezadá povinný údaj nebo vloží text ve špatném formátu (např. neplatný e-mail), systém jej na to upozorní červeným textem přímo pod konkrétním políčkem a zamezí uložení.

**Specifika pro smlouvy:**
Při vytváření a úpravě smluv uživatel vybírá existující osoby ze seznamů:
* **Klient a Správce smlouvy:** Vybírají se z menu (dropdown).
* **Účastníci (Poradci):** Umožňují vícenásobný výběr (multi-select). Pro výběr nebo zrušení výběru více osob najednou stačí držet klávesu `CTRL` a zvolit jednotlivá jména.
Systém striktně vyžaduje, aby osoba zvolená jako **Správce smlouvy** byla zároveň označena i mezi jejími **Účastníky**. 

### Bezpečné mazání
Pro smazání záznamu slouží červené tlačítko **Smazat** v přehledových tabulkách. Aplikace využívá modální (vyskakovací) okno. Pokud se uživatel pokusí smazat klienta nebo poradce, na kterého jsou aktuálně navázány aktivní smlouvy, systém jej na to varovně upozorní. Pro smazání takové osoby musí uživatel explicitně zaškrtnout potvrzovací políčko, čímž dojde ke kaskádovému smazání souvisejících smluv a je tak zachována referenční integrita databáze.

---

## Technická dokumentace

Aplikace je postavena na platformě **ASP.NET Core MVC**.

### Směrování a Endopointy (Routes)
Aplikace obsluhuje CRUD operace pro každou z hlavních entit (`Clients`, `Advisors`, `Contracts`):

* **`GET /{Entita}`:** Zobrazení hlavního seznamu záznamů.
* **`GET /{Entita}/Details/{id}`:** Zobrazení detailu konkrétního záznamu.
* **`GET /{Entita}/Create`:** Zobrazení formuláře pro vytvoření nového záznamu.
* **`POST /{Entita}/Create`:** Přijetí a validace dat z formuláře, uložení nového záznamu.
* **`GET /{Entita}/Edit/{id}`:** Zobrazení formuláře pro úpravu existujícího záznamu.
* **`POST /{Entita}/Edit/{id}`:** Přijetí, validace a uložení upravených dat.
* **`POST /{Entita}/Delete/{id}`:** Smazání záznamu.
* **`GET /{Entita}/ExportCsv`:** Zpracování aktuálních parametrů dotazu (Query) a vrácení vyfiltrovaných dat ve formátu CSV (`FileContentResult`).

### Architektura a vrstvy
* **Modely a ViewModels:** Aplikace odděluje databázové doménové modely (`Client`, `Advisor`, `Contract`) od view modelů (`ClientFormViewModel`, `AdvisorFormViewModel` atd.). ViewModely slouží jako bezpečená mezivrstva mezi UI a databází. Pro zapouzdření parametrů vyhledávání, řazení a filtrování jsou navíc využívány dedikované Query objekty (`PersonQuery`, `ContractQuery`).
* **Služby (Services):** Obsahují hlavní byznys logiku aplikace (`IClientService`, `IAdvisorService`, `IContractService`). Zajišťují veškeré CRUD operace, aplikují filtry, řeší bezpečné řazení dat a oddělují kontrolery od přímého přístupu k databázi.
* **Kontrolery (Controllers):** Zajišťují HTTP směrování. Zpracovávají vstupy z pohledů, kontrolují validitu dat (`ModelState.IsValid`) a delegují práci na služby. `ErrorsController` slouží pro globální odchytávání chyb v produkčním prostředí.
* **Databáze (Data):** Využívá **Entity Framework Core** napojený na **MS SQL Server Express**. Kód využívá EF Migrace a seeding dat pro konzistenci. Pro zajištění komfortního vyhledávání je na úrovni databáze globálně vynucena kolace `Latin1_General_CI_AI`, která zajišťuje ignorování diakritiky a velikosti písmen při databázových dotazech.

### Testování
Testy jsou postaveny na frameworku **xUnit** ve spojení s knihovnou **Moq** a slouží k vývoji pomocí metody TDD (Test-Driven Development).