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
Po kliknutí na libovolný modul se zobrazí **přehledová tabulka** se všemi uloženými záznamy.
* Pokud je databáze prázdná, aplikace uživatele informuje textem (např. *"Zatím nejsou uloženy žádné smlouvy."*).
* V pravé části každého řádku se nachází tlačítka pro práci s konkrétním záznamem.
* Tlačítko **Detail** otevře kartu s kompletními informacemi. Z detailu klienta se např. lze přesunout na detail jeho smlouvy a z detailu smlouvy rovnou na kartu jejího správce.

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

### Architektura a vrstvy
* **Modely a ViewModels:** Aplikace odděluje databázové doménové modely (`Client`, `Advisor`, `Contract`) od view modelů (`ClientFormViewModel`, `AdvisorFormViewModel`, `ContractFormViewModel`). Doménové modely definují strukturu pro SQL databázi, ViewModely slouží jako mezivrstva mezi formuláři a databázovými modely. Modely pro osoby navíc sdílejí logiku pomocí dědičnosti ze společné abstraktní třídy `PersonFormViewModel`.
* **Služby (Services):** Obsahují hlavní logiku aplikace (`IClientService`, `IAdvisorService`, `IContractService`). Zajišťují veškeré CRUD operace a oddělují kontrolery od přímého přístupu k datům.
* **Kontrolery (Controllers):** Zajišťují HTTP směrování. Zpracovávají vstupy, kontrolují validitu dat (`ModelState.IsValid`) a komunikují se službami. `ErrorsController` slouží pro globální odchytávání chyb v produkčním prostředí.
* **Databáze (Data):** Využívá **Entity Framework Core** napojený na **MS SQL Server Express**. Využívá EF Migrací a seeding dat.

### Testování
Testy jsou postaveny na frameworku **xUnit** ve spojení s knihovnou **Moq** a slouží k vývoji pomocí metody TDD (Test-Driven Development).