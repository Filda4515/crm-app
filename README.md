# CrmApp

Základní CRM (Customer Relationship Management) systém navržený pro správu klientů, finančních poradců a jejich smluv. 

---

## Uživatelská dokumentace

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