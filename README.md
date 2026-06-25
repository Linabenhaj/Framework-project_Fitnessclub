# Projectvoorstel: FitnessClub Applicatie

Dit project is een volledig systeem voor het beheren van een fitnessclub, bestaande uit een
ASP.NET Core Web-applicatie voor browser-gebruikers en een MAUI mobiele applicatie voor Android.
Beide applicaties zitten in dezelfde Visual Studio Solution samen met een gedeelde Class Library
en een aparte REST API met JWT-authenticatie die door de mobiele app geconsumeerd wordt. De
applicatie ondersteunt drie rollen: Admin, Trainer en Lid. Elke rol heeft zijn eigen dashboard,
menu en functies op basis van toegangsrechten.

# Interne logica van de applicatie

De applicatie biedt een compleet systeem voor het beheren van leden, lessen, abonnementen,
inschrijvingen, gebruikersrollen en toegangsrechten. Ze maakt gebruik van Entity Framework Core
en ASP.NET Identity Framework voor veilige authenticatie en autorisatie op basis van rollen.

De Admin heeft volledige controle over de fitnessclub. Hij kan lessen toevoegen, bewerken en
soft-deleten, abonnementen beheren door prijzen aan te passen of te verwijderen, ledenprofielen
bekijken en beheren, gebruikersrollen wijzigen en via het dashboard een overzicht krijgen van
alle leden, lessen, inschrijvingen en abonnementen.

De Trainer kan zichzelf aangeven als trainer voor een les, of een bestaande trainer-naam wijzigen.
Hij kan ook lessen bewerken.

Het Lid kan beschikbare lessen bekijken via het lessenoverzicht, zich inschrijven en uitschrijven,
zijn eigen abonnement raadplegen en wijzigen, en zijn persoonlijke informatie zien via het
profielgedeelte.

De belangrijkste vensters in de applicatie zijn het loginvenster, het registratievenster, het
dashboard, het lessenoverzicht, het venster om nieuwe lessen toe te voegen en het venster om
leden of trainers te bewerken. Het loginvenster wordt gebruikt voor het aanmelden via ASP.NET
Identity, het registratievenster voor het aanmaken van een nieuwe gebruiker met keuze van
abonnement, en het dashboard toont verschillende tabbladen afhankelijk van de gebruikersrol.

## Web-applicatie (ASP.NET Core MVC + Razor)

De Web-applicatie gebruikt het MVC-patroon met Razor-views. Belangrijkste features:
- Meertaligheid in Nederlands, Engels en Frans via .resx resource-files met IHtmlLocalizer
- Eigen RequestLoggingMiddleware die elke HTTP-request logt met statuscode en duur
- Generieke PaginatedList<T> klasse voor server-side paginatie via Skip en Take
- Unobtrusive AJAX voor het filteren van lessen en inschrijvingen zonder volledige pagina-reload
- AntiForgeryToken op alle POST-acties voor CSRF-beveiliging
- Data Annotations voor automatische client- en server-side validatie
- Soft delete via abstracte BasisEntiteit klasse (IsVerwijderd flag)

## MAUI-applicatie (Mobile)

De MAUI-app gebruikt het MVVM-patroon met source-generators uit CommunityToolkit.Mvvm. Ze
consumeert de REST API via HttpClient met JWT Bearer-tokens. Een lokale SQLite database via
LocalDbContext zorgt voor offline-werking via een cache-first patroon: data wordt eerst uit
SQLite gelezen en daarna door de API ververst. Een auto re-login flow met Preferences zorgt
dat de gebruiker ingelogd blijft tussen sessies. De UI is rol-gebaseerd via een Flyout-menu
in AppShell. Er worden afgeleide modellen (LocalLes, LocalUser, LocalInschrijving) gebruikt
voor de lokale SQLite cache.

## Gebruikte technologieën

.NET 9, Class Library voor gedeelde modellen, Entity Framework Core met SQL Server (Web + API)
en SQLite (MAUI cache), ASP.NET Identity Framework, JWT-authenticatie (API), MVC-patroon (Web),
MVVM-patroon (MAUI), Bootstrap 5 voor styling, unobtrusive AJAX, soft delete patroon en
meertaligheid via .resx files.

# Installatie

Om de applicatie te installeren moet de repository worden gedownload of gekloond, vervolgens
geopend in Visual Studio. Controleer de SQL Server-verbinding in appsettings.json van zowel
FitnessClub.Web als FitnessClub.API. Voer de migraties uit via de Package Manager Console met
het commando Update-Database, of laat de seeding automatisch lopen bij het opstarten. Daarna
kun je de drie projecten starten:

1. Start eerst FitnessClub.API (port 5000)
2. Start dan FitnessClub.Web (port 5062 of 7152)
3. Start als laatste FitnessClub.MAUI op een Pixel 7 Android-emulator

Bij het eerste opstarten worden standaardrollen, drie abonnementen (Basic, Medium en Pro) en
testgebruikers aangemaakt via de seeding in Program.cs.

# Testgebruikers

| Rol     | E-mail                       | Wachtwoord   |
|---------|------------------------------|--------------|
| Admin   | admin@fitnessclub.be         | Admin123!    |
| Trainer | trainer@fitnessclub.be       | Trainer123!  |
| Lid     | user@fitnessclub.be          | User123!     |

Het Lid-account heeft automatisch een Pro-abonnement en een ingevuld telefoonnummer.

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

### Homepagina/Inlogpagina
<img width="458" height="1000" alt="image" src="https://github.com/user-attachments/assets/d47b8b5a-7ed2-439e-8235-09e969f3ef62" />

### Registreren
<img width="367" height="828" alt="image" src="https://github.com/user-attachments/assets/cc54e2f0-715d-4ae9-803c-2dc697f962f0" />
<img width="366" height="772" alt="image" src="https://github.com/user-attachments/assets/7b3d08c0-b306-4533-9ab7-38fd96fd8b7f" />


## Admin
### Admin dashboard
<img width="468" height="965" alt="image" src="https://github.com/user-attachments/assets/6463f433-2319-4166-9541-6c68f61d07e2" />

## Lessen overzicht
<img width="467" height="970" alt="image" src="https://github.com/user-attachments/assets/dd33d401-2d34-4f07-9bfe-1783a69b9354" />

## Inschrijvingen overzicht
<img width="465" height="900" alt="image" src="https://github.com/user-attachments/assets/22629fcd-2814-4f33-bb24-84423c415ab3" />

## Gebruikerspagina
<img width="463" height="968" alt="image" src="https://github.com/user-attachments/assets/93829e41-2e6c-496c-b535-4935a9feb9ac" />

# Trainer
## Profielpagina
<img width="472" height="965" alt="image" src="https://github.com/user-attachments/assets/17ce8249-5ad4-488a-a65e-d63d7bcdbff0" />

## Lessenoverzicht (claims)
<img width="357" height="737" alt="image" src="https://github.com/user-attachments/assets/4c13fe57-819c-4f8e-9eb3-49c9b7c6469f" />

# Lid
## Profielpagina
<img width="362" height="811" alt="image" src="https://github.com/user-attachments/assets/8a463235-3f45-4f3f-9c19-bf7edcb77b02" />

## Lessenoverzicht
<img width="362" height="817" alt="image" src="https://github.com/user-attachments/assets/3f58c4c7-7aeb-4f3f-aca9-a0bcad9461c4" />

## Inschrijvingoverzichten
<img width="363" height="808" alt="image" src="https://github.com/user-attachments/assets/ec478724-0fb8-4d48-bc68-7cd81823f8d2" />

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
