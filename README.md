# Projectvoorstel: Fitnessclub Applicatie

Dit project is een volledig systeem voor het beheren van een
fitnessclub, bestaande uit een ASP.NET Core Web-applicatie voor
browser-gebruikers en een MAUI mobiele applicatie voor Android.
Beide applicaties zitten in dezelfde Visual Studio Solution
samen met een gedeelde Class Library en een aparte REST API met
JWT-authenticatie die door de mobile-app wordt geconsumeerd.

## Interne logica van de applicatie

De applicatie ondersteunt drie rollen: Admin, Trainer en Lid.
Elke rol heeft zijn eigen dashboard, menu en functionaliteiten
op basis van toegangsrechten. De applicatie maakt gebruik van
Entity Framework Core en ASP.NET Identity Framework voor veilige
authenticatie en autorisatie op basis van rollen.

De Admin heeft volledige controle over de fitnessclub. Hij kan
lessen toevoegen, bewerken en (soft) verwijderen, abonnementen
beheren door prijzen aan te passen of te verwijderen, leden-
profielen bekijken en beheren, gebruikersrollen wijzigen, en via
het dashboard een overzicht krijgen van alle leden, lessen,
inschrijvingen en abonnementen.

De Trainer kan zichzelf aangeven als trainer voor een les of de
bestaande trainer-naam wijzigen. Hij kan ook lessen bewerken.

Het Lid kan beschikbare lessen bekijken, zich inschrijven en
uitschrijven, zijn eigen abonnement raadplegen en wijzigen, en
zijn profielgegevens bekijken.

## Web-applicatie (ASP.NET Core MVC + Razor)

De Web-applicatie gebruikt het MVC-patroon met Razor-views.
Belangrijke features zijn meertaligheid in Nederlands, Engels
en Frans via .resx resource-files met IHtmlLocalizer, een eigen
RequestLoggingMiddleware die elke request logt met statuscode
en duur, een generieke PaginatedList<T> klasse voor server-side
paginatie via Skip en Take, en unobtrusive AJAX voor het filteren
van lessen en inschrijvingen zonder de pagina te herladen.

## MAUI-applicatie (Mobile)

De MAUI-app gebruikt het MVVM-patroon met source-generators uit
CommunityToolkit.Mvvm. Ze consumeert de REST API via HttpClient
met JWT Bearer-tokens. Een lokale SQLite database via LocalDbContext
zorgt voor offline-werking via een cache-first patroon: data wordt
eerst uit SQLite gelezen en daarna door de API ververst. Een
auto re-login flow met Preferences zorgt dat de gebruiker ingelogd
blijft tussen sessies. De UI is rol-gebaseerd via een Flyout-menu
in AppShell.

## Gebruikte technologieën

.NET 9, Class Library voor gedeelde modellen, Entity Framework
Core met SQL Server (Web + API) en SQLite (MAUI cache), ASP.NET
Identity Framework, JWT-authenticatie (API), MVC-patroon (Web),
MVVM-patroon (MAUI), unobtrusive AJAX, soft delete via abstract
BasisEntiteit klasse, meertaligheid via .resx files.

## Installatie

1. Download of kloon de repository
2. Open het project in Visual Studio
3. Controleer de SQL Server-verbinding in `FitnessClubDbContext.cs`
4. Voer de migraties uit via de Package Manager Console:
5. Start de applicatie met **F5**

Bij het eerste opstarten worden standaardrollen en testgebruikers aangemaakt via de `SeedService`.

| Rol | E-mailadres | Wachtwoord |
| Admin | admin@fitness.com | Admin123! |
| User | User@fitness.com | User123! |
| Trainer | Trainer@fitness.com | Trainer123! |

> Deze gegevens kunnen worden aangepast in de seeddata.

# Web applicatie
## Screenshots applicatie

### Welkomstvenster
<img width="1908" height="911" alt="image" src="https://github.com/user-attachments/assets/f1406a9b-e493-4f71-93ff-b7823a9a0f3b" />

### Loginvenster
<img width="1907" height="906" alt="image" src="https://github.com/user-attachments/assets/18078a8f-4454-4491-b28a-adfd5b5c5413" />

### Registratieformulier
<img width="1918" height="968" alt="image" src="https://github.com/user-attachments/assets/d901f87b-cc4f-41af-a008-53bddfc54e3b" />


### Dashboard admin
<img width="1908" height="907" alt="image" src="https://github.com/user-attachments/assets/9e63b067-43e1-4f41-b2cd-0a0bff449927" />


### Lessen venster
<img width="1918" height="903" alt="image" src="https://github.com/user-attachments/assets/1d817c31-c798-4233-a292-34dc63b9915e" />


### Les toevoegen/bewerken/filteren
<img width="1918" height="907" alt="image" src="https://github.com/user-attachments/assets/1bc9acc8-1386-4222-9322-6746a284ac70" />
<img width="1917" height="908" alt="image" src="https://github.com/user-attachments/assets/d3fb9dd1-4232-476d-bb22-78f22b3a6d2f" />
<img width="1917" height="903" alt="image" src="https://github.com/user-attachments/assets/d1eb4886-97a8-46a9-bf86-24b91d91cb16" />

### Abonnement/bewerken/details bekijken
<img width="1913" height="905" alt="image" src="https://github.com/user-attachments/assets/82f4293d-e8e4-4b42-b4af-2cb2c367b63e" />
<img width="1912" height="902" alt="image" src="https://github.com/user-attachments/assets/f838fbcb-2e24-4f30-a49a-8c0b02edf95f" />
<img width="1907" height="957" alt="image" src="https://github.com/user-attachments/assets/fcb96afe-11a1-4db6-a0dd-1c0725270717" />

### Dashboard gebruiker/bewerken
<img width="1915" height="976" alt="image" src="https://github.com/user-attachments/assets/bfb51edd-1033-475f-82f0-69203e7b7dc3" />
<img width="1918" height="965" alt="image" src="https://github.com/user-attachments/assets/15470fa0-b6b9-4333-b5e5-8d4cf0cb12b5" />

## Talen
<img width="1362" height="517" alt="image" src="https://github.com/user-attachments/assets/f99384cd-0937-43ff-ab00-b16f9e8bdc49" />

## Gebruiker
### lessen inschrijven/overzicht/details
<img width="1918" height="976" alt="image" src="https://github.com/user-attachments/assets/1face579-1d93-493f-8dd6-6baa61d8548d" />
<img width="1902" height="960" alt="image" src="https://github.com/user-attachments/assets/9201dda7-cb8d-4b6d-a609-1082320f9a4c" />


### inschrijvingen overzicht
<img width="1912" height="963" alt="image" src="https://github.com/user-attachments/assets/cd5d05c9-e8a0-43b8-acd6-d31cf74e42b2" />

### Mijn profiel
<img width="1882" height="960" alt="image" src="https://github.com/user-attachments/assets/30959bb2-c5bc-46bd-aa86-46ae87058eff" />

## Trainer
### Dashboard
<img width="1913" height="903" alt="image" src="https://github.com/user-attachments/assets/81ec3257-cf3e-4019-97ef-832948ca080d" />

### Lessenbeheer/bewerken/overzicht
<img width="1896" height="953" alt="image" src="https://github.com/user-attachments/assets/2b1130cc-ff7d-43ec-a9f1-fb96bb41f7df" />
<img width="1911" height="971" alt="image" src="https://github.com/user-attachments/assets/193547de-ac27-4fa4-9b55-2315086c05bb" />

### Profielpagina
<img width="1892" height="958" alt="image" src="https://github.com/user-attachments/assets/bc6bc411-d78f-4255-a6bd-08594522eba2" />



# MAUI App

### Homepagina
<!-- screenshot hier -->

### Inlogpagina
<!-- screenshot hier -->

### Registreren
<!-- screenshot hier -->

### Admin dashboard
<!-- screenshot hier -->

---

# .NET Web

### Welkomstpagina
<!-- screenshot hier -->

### Loginpagina
<!-- screenshot hier -->

### Registreren
<!-- screenshot hier -->

### Gebruikersdashboard
<!-- screenshot hier -->

### Uitlogpagina
<!-- screenshot hier -->

---

# Bronnen & Inspiratie

## Inspirerend project

- **Agenda app van Waldo Heudens** — structuur van MAUI-navigatie, API-integratie en MVVM-patroon dienden als referentie voor de opbouw van de MAUI app.

## Officiële documentatie

- [ASP.NET Core MVC documentatie](https://learn.microsoft.com/en-us/aspnet/core/mvc/)
- [.NET MAUI documentatie](https://learn.microsoft.com/en-us/dotnet/maui/)
- [Localisatie in ASP.NET Core](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/localization)
- [JWT authenticatie in ASP.NET Core](https://learn.microsoft.com/en-us/aspnet/core/security/authentication/jwt-authn)
- [Entity Framework Core](https://learn.microsoft.com/en-us/ef/core/)
  
# AI-gebruik

Tijdens dit project werd AI ingezet als hulpmiddel voor de volgende doeleinden:

| Doel | Tool |

| Design inspiratie & UI-structuur | DeepSeek, Claude |
| Vragen rond build-errors & debugging | DeepSeek, Claude |
| Vertaling van views (NL → EN / FR) | DeepSeek, Claude |
| Grote commit berichten opstellen | DeepSeek |
| Schrijffouten corrigeren | DeepSeek, Claude |
| README opstellen | Claude (Cowork) |

## Gebruikte AI-gesprekken

**DeepSeek — technische hulp & vertalingen:**
- [Gesprek 1](https://chat.deepseek.com/share/sf2e5ues75abm79nbq)
- [Gesprek 2](https://chat.deepseek.com/share/nxdx3at5io1vosvrhf)
- [Gesprek 3](https://chat.deepseek.com/share/wivn1c348y9kkltycs)
- [Gesprek 4](https://chat.deepseek.com/share/imxeaw9b06jkfxgqid)
- [Gesprek 5](https://chat.deepseek.com/share/l5grt7z1aky33gryg7)
- [Gesprek 6](https://chat.deepseek.com/share/hmgk2pitngj6z75ho0)

**Claude — meertaligheid, errors & README:**
- [Gesprek 1](https://claude.ai/share/3df3d947-ac99-4aef-9d7e-684d73c1f730)

> ⚠️ *Alle AI-gegenereerde inhoud werd kritisch nagelezen, begrepen en waar nodig aangepast door mezelf.*
> Ongeveer 30% AI assistentie

*© 2026 Lina Benhaj — FitnessClub project*
