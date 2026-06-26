# FitnessClub — Mobiele applicatie (.NET MAUI)

Een cross-platform mobiele applicatie gebouwd met **.NET MAUI** die de mobiele tegenhanger is van de FitnessClub web-applicatie. Werkt op Android, iOS én Windows Desktop.

Deze app **consumeert de RESTful API** uit het project `FitnessClub.API` en gebruikt **dezelfde modellen** uit de Class Library `FitnessClub.Models`.

---

## 📋 Inhoudstafel

1. [Doel van de app](#doel-van-de-app)
2. [Architectuur](#architectuur)
3. [Functionaliteiten per rol](#functionaliteiten-per-rol)
4. [Folder-structuur](#folder-structuur)
5. [Installatie en starten](#installatie-en-starten)
6. [Demo-accounts](#demo-accounts)
7. [Online/Offline werking](#onlineoffline-werking)
8. [GDPR](#gdpr)
9. [Gebruikte technologieën en bibliotheken](#gebruikte-technologieën-en-bibliotheken)
10. [Bronvermelding](#bronvermelding)

---

## Doel van de app

De mobiele versie biedt leden en trainers toegang tot de fitnessclub via hun telefoon. De klant-georiënteerde functionaliteit (lessen bekijken, inschrijven, profiel) staat centraal. Administratieve taken (gebruikersbeheer, abonnement-CRUD) gebeuren via de web-applicatie.

Belangrijke kenmerken:
- Volledig **MVVM** met `CommunityToolkit.Mvvm`
- **Bindings** in XAML voor alle UI-update
- **JWT-authenticatie** via de API
- **Auto re-login** op basis van lokaal opgeslagen token
- **SQLite** voor offline cache
- **GDPR-compliant**: privacymelding bij eerste opstart

---

## Architectuur

```
   ┌─────────────────────────┐
   │  XAML Views             │  ← gebruikerinterface
   └────────┬────────────────┘
            │  {Binding}
            ↓
   ┌─────────────────────────┐
   │  ViewModels             │  ← presentatielaag (MVVM)
   │  [ObservableProperty]   │
   │  [RelayCommand]         │
   └────────┬────────────────┘
            │
            ↓
   ┌─────────────────────────┐    ┌──────────────┐
   │  ApiService             │ ←→ │ FitnessClub  │
   │  (HttpClient + JWT)     │    │ .API (REST)  │
   └────────┬────────────────┘    └──────────────┘
            │
            ↓
   ┌─────────────────────────┐
   │  LocalDbContext         │  ← offline cache
   │  (SQLite)               │
   └─────────────────────────┘
```

---

## Functionaliteiten per rol

### 👤 Bezoeker (niet ingelogd)
- **HomePage** met inlog- en registratie-knoppen
- **RegisterPage** voor nieuwe leden (met abonnement-keuze)
- **LoginPage** voor authenticatie

### 🧑 Lid
- Welkomstpagina met komende lessen
- Lessen-overzicht met filter, zoek en pull-to-refresh
- "Inschrijven"-knop die verandert naar "✓ Ingeschreven" na inschrijving
- Eigen inschrijvingen bekijken + uitschrijven (24u-regel)
- Profielpagina met eigen gegevens en abonnement
- Contact klantendienst voor gegevenswijzigingen

### 🏃 Trainer
- Lessen-overzicht (zonder Inschrijven-knop)
- **Claim**-knop om zichzelf als trainer aan een les toe te wijzen
- Eigen profielpagina (zonder klantendienst-info)

### 🔑 Admin
- AdminDashboardPage met snelle acties
- Lessen aanmaken (zonder trainer — die claimt later zelf)
- Lessen bewerken en verwijderen
- Gebruikersbeheer met verwijder-knop (admin/trainer worden beschermd)
- "← Beheer"-knop in toolbar van detailpagina's

---

## Folder-structuur

```
FitnessClub.MAUI/
├── Views/                       # XAML-pagina's
│   ├── HomePage.xaml            # Landingspagina (logo + buttons)
│   ├── LoginPage.xaml           # Inloggen
│   ├── RegisterPage.xaml        # Registratie met abonnementkeuze
│   ├── DashboardPage.xaml       # Welkomstscherm voor Lid/Trainer
│   ├── AdminDashboardPage.xaml  # Admin-startscherm
│   ├── LessenPage.xaml          # Lessen-overzicht
│   ├── InschrijvingenPage.xaml  # Eigen / alle inschrijvingen
│   ├── ProfielPage.xaml         # Mijn profiel
│   ├── GebruikersPage.xaml      # Gebruikerslijst (Admin)
│   ├── SettingsPage.xaml        # Instellingen + sync
│   └── AboutPage.xaml           # Over deze app
├── ViewModels/                  # MVVM ViewModels (één per Page)
│   ├── BaseViewModel.cs         # Gemeenschappelijke logica (IsBusy, Title)
│   ├── LoginViewModel.cs
│   ├── RegisterViewModel.cs
│   ├── DashboardViewModel.cs
│   ├── AdminDashboardViewModel.cs
│   ├── LessenViewModel.cs
│   ├── InschrijvingenViewModel.cs
│   ├── ProfielViewModel.cs
│   ├── GebruikersViewModel.cs
│   └── SettingsViewModel.cs
├── Services/                    # Services (DI)
│   ├── ApiService.cs            # Alle HTTP-calls naar de API
│   ├── AuthService.cs           # Authenticatie-hulp
│   └── Synchronizer.cs          # Online ↔ offline sync
├── Converters/                  # XAML value-converters
│   ├── AvailabilityConverter.cs    # IsVol → kleur
│   ├── StatusColorConverter.cs     # Status → kleur
│   ├── AllTrueConverter.cs         # MultiBinding AND-logica
│   └── BooleanToTextConverter.cs   # Bool → tekst
├── Resources/
│   ├── Styles/Colors.xaml       # Kleurenpalet
│   └── Styles/Styles.xaml       # Globale styles
├── Platforms/                   # Platform-specifieke code
│   ├── Android/
│   ├── iOS/
│   └── Windows/
├── AppShell.xaml + .cs          # Navigatiestructuur (flyout-menu)
├── App.xaml + .cs               # Opstartlogica + GDPR-melding
├── MauiProgram.cs               # Dependency Injection-registratie
├── General.cs                   # Globale user-info (Preferences)
└── FitnessClub.MAUI.csproj      # Project-configuratie
```

---

## Installatie en starten

### Voorwaarden
- **Visual Studio 2022** (17.8 of hoger) met de workload "**.NET Multi-platform App UI development**"
- **.NET 9 SDK**
- **Android SDK** (komt mee met VS) — voor de emulator
- Een Android-emulator (bv. **Pixel 7 - API 35**)
- Optioneel: een fysieke Android-phone met USB-debugging

### Stappen

1. **Open de Solution** `FitnessClub_Final_Solution.sln` in Visual Studio
2. **Restore NuGet-packages**: rechtsklik op de Solution → *Restore NuGet Packages*
3. **Start eerst de API** (verplicht — MAUI heeft hem nodig):
   - Open een PowerShell-venster
   - ```
     cd FitnessClub.API
     dotnet run
     ```
   - Wacht tot je `Now listening on: http://localhost:5000` ziet
4. **Selecteer Pixel 7 emulator** in de dropdown naast de Start-knop in Visual Studio
5. **Stel `FitnessClub.MAUI` in als Startup Project**
6. **Druk F5** — eerste deploy duurt 2-5 minuten

### Fysieke Android-phone (optioneel)
- Schakel USB-debugging in op de phone
- Sluit aan via USB → kies "Bestandsoverdracht" als USB-modus
- Phone verschijnt in VS dropdown
- F5

---

## Demo-accounts

| Rol | Email | Wachtwoord |
|---|---|---|
| **Admin** | `admin@fitnessclub.be` | `Admin123!` |
| **Trainer** | `trainer@fitnessclub.be` | `Trainer123!` |
| **Lid** | `user@fitnessclub.be` | `User123!` |

---

## Online/Offline werking

De app werkt zowel online als offline:

1. **Bij opstart**: token wordt gevalideerd. Geldig → auto re-login. Verlopen → naar LoginPage.
2. **Lessen ophalen**: eerst uit lokale SQLite (snel, ook offline). Daarna van API. Bij conflict wint de API.
3. **Inschrijven**: gaat via API. Bij offline wordt een melding getoond.
4. **Synchronisatie**: gebeurt automatisch bij login en kan handmatig via Settings.

---

## GDPR

Bij de eerste opstart van de app verschijnt een **privacymelding** waarin uitgelegd wordt welke data lokaal wordt opgeslagen (e-mail, JWT-token in `Preferences`). De gebruiker moet expliciet **akkoord** geven. Niets wordt gedeeld met derden.

In **SettingsPage** is er een knop om **alle lokale data te wissen**.

---

## Gebruikte technologieën en bibliotheken

### Framework
- **.NET MAUI 9** — cross-platform UI framework
- **XAML** als front-end taal
- **C# 12** als programmeertaal

### NuGet-packages

| Package | Versie | Doel | Bron |
|---|---|---|---|
| `Microsoft.Maui.Controls` | 9.0.0 | MAUI core | NuGet officieel |
| `Microsoft.Maui.Controls.Compatibility` | 9.0.0 | Xamarin-compatibiliteit | NuGet officieel |
| `CommunityToolkit.Maui` | 9.0.0 | Extra MAUI-controls + helpers | NuGet officieel |
| `CommunityToolkit.Mvvm` | 8.2.2 | MVVM source generators ([ObservableProperty], [RelayCommand]) | NuGet officieel |
| `Microsoft.EntityFrameworkCore.Sqlite` | 9.0.0 | SQLite EF Core provider voor lokale cache | NuGet officieel |
| `SQLitePCLRaw.bundle_green` | 2.1.10 | SQLite native bibliotheek (verplicht voor Android) | NuGet officieel |
| `Microsoft.Extensions.Http` | 9.0.0 | HttpClientFactory voor API-calls | NuGet officieel |
| `Microsoft.Extensions.Logging.Debug` | 9.0.0 | Debug-logging | NuGet officieel |

---

## Bronvermelding

### Inspiratiebron — klasvoorbeeld

De architectuur van deze MAUI-app (MVVM met CommunityToolkit, gedeelde modellen via Class Library, online/offline sync met SQLite) is geïnspireerd op de **klasvoorbeelden en richtlijnen van docent Waldo Heudens**.

Specifiek:
- Het **MVVM-patroon** met `BaseViewModel`, `[ObservableProperty]` en `[RelayCommand]` volgt zijn aanpak
- Het patroon **`LocalDbContext` + afgeleide `Local*` modellen** (waarbij modellen zoals `LocalLes` apart bestaan voor SQLite naast de server-modellen) is gebaseerd op zijn aanpak in het Agenda-voorbeeldproject
- De **Synchronizer-laag** tussen API en SQLite is geïnspireerd op zijn patroon
- De **opzet van het AppShell-menu** met flyout-navigatie

De code is **niet één-op-één gekopieerd**:
- Het thema is anders (FitnessClub i.p.v. Agenda)
- De entiteiten verschillen (Les + Gebruiker + Abonnement + Inschrijving)
- De business-logica (claim-flow voor trainers, 24u-uitschrijfregel, abonnementkeuze bij registratie, GDPR-popup) is uniek aan dit project
- De UI is volledig opnieuw ontworpen voor het FitnessClub-thema

### Officiële documentatie geraadpleegd
- [.NET MAUI documentatie](https://learn.microsoft.com/en-us/dotnet/maui/)
- [CommunityToolkit.Mvvm documentatie](https://learn.microsoft.com/en-us/dotnet/communitytoolkit/mvvm/)
- [Shell navigation in MAUI](https://learn.microsoft.com/en-us/dotnet/maui/fundamentals/shell/)
- [HttpClient + JWT Bearer](https://learn.microsoft.com/en-us/dotnet/api/system.net.http.headers.authenticationheadervalue)
- [EF Core SQLite provider](https://learn.microsoft.com/en-us/ef/core/providers/sqlite/)

---

## Versie

Versie 1.0 — Juni 2026
Auteur: Lina Benhaj
Platform: Android (primary), iOS, Windows Desktop
