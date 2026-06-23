# Projectvoorstel: Fitnessclub Applicatie

Dit project is een desktopapplicatie ontwikkeld in C# met .NET voor het beheren van een fitnessclub zowel in web versie als mobiele versie. De applicatie ondersteunt verschillende rollen: Admin, Medewerker en Lid. Elke rol heeft zijn eigen dashboard en functies.

## Interne logica van de applicatie

De applicatie biedt een compleet systeem voor het beheren van leden, lessen, abonnementen, inschrijvingen, gebruikersrollen en toegangsrechten. Ze maakt gebruik van Entity Framework Core en ASP.NET Identity voor veilige authenticatie en autorisatie op basis van rollen.

De admin heeft volledige controle over de fitnessclub. De admin kan lessen toevoegen, bewerken en verwijderen, leden koppelen aan lessen, abonnementen beheren door prijzen aan te passen of te verwijderen, ledenprofielen bekijken en via het dashboard een overzicht krijgen van alle leden, lessen en abonnementen.

Het lid kan beschikbare lessen bekijken via het lessenoverzicht, eigen abonnementen raadplegen en persoonlijke informatie zien via het profielgedeelte.

De belangrijkste vensters in de applicatie zijn het loginvenster, het registratievenster, het dashboard, het lessenoverzicht voor leden, het venster om nieuwe lessen toe te voegen en het venster om leden te bewerken. Het loginvenster wordt gebruikt voor het aanmelden via ASP.NET Identity, het registratievenster voor het aanmaken van een nieuwe gebruiker met keuze van abonnement, en het dashboard toont verschillende tabbladen afhankelijk van de gebruikersrol.

De gebruikte technologieën zijn .NET 9 met  Class Library, Entity Framework Core, ASP.NET Identity, SQL Server en het MVVM-ontwerppatroon. De applicatie ondersteunt soft delete-functionaliteit en toegangsbeheer op basis van rollen.

## Installatie

1. Download of kloon de repository
2. Open het project in Visual Studio
3. Controleer de SQL Server-verbinding in `FitnessClubDbContext.cs`
4. Voer de migraties uit via de Package Manager Console:
5. Start de applicatie met **F5**

Bij het eerste opstarten worden standaardrollen en testgebruikers aangemaakt via de `SeedService`.

| Rol | E-mailadres | Wachtwoord |
| Admin | admin@fitness.com | Admin123 |
| Lid | lid@fitness.com | Lid123 |
| Trainer | Trainer@fitness.com | Trainer123 |

> Deze gegevens kunnen worden aangepast in de seeddata.

## Screenshots applicatie

> Voor de screenshots werd de gebruiker "Wesley" gebruikt, die hiervoor aangemaakt was. Het standaard testaccount `lid@fitness.com` werkt uiteraard ook.

### Welkomstvenster
<!-- screenshot hier -->

### Loginvenster
<!-- screenshot hier -->

### Registratieformulier
<!-- screenshot hier -->

### Dashboard admin
<!-- screenshot hier -->

### Pop-up vensters
<!-- screenshot hier -->

### Les toevoegen
<!-- screenshot hier -->

### Abonnement bewerken
<!-- screenshot hier -->

### Nieuwe inschrijving toevoegen
<!-- screenshot hier -->

### Dashboard gebruiker
<!-- screenshot hier -->

---

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

> ⚠️ *Alle AI-gegenereerde inhoud werd kritisch nagelezen, begrepen en waar nodig aangepast door de student zelf.*

*© 2026 Lina Benhaj — FitnessClub project*
