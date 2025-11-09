# Projectvoorstel:  Fitnessclub Applicatie

Dit project is een desktopapplicatie ontwikkeld in C Sharp met .NET WPF voor het beheren van een fitnessclub. De applicatie ondersteunt verschillende rollen: Admin, Medewerker en Lid. Elke rol heeft zijn eigen dashboard en functies.

# Interne logica van de applicatie

De applicatie biedt een compleet systeem voor het beheren van leden, lessen, abonnementen, inschrijvingen, gebruikersrollen en toegangsrechten. Ze maakt gebruik van Entity Framework Core en ASP.NET Identity voor veilige authenticatie en autorisatie op basis van rollen.

De admin heeft volledige controle over de fitnessclub. De admin kan lessen toevoegen, bewerken en verwijderen, leden koppelen aan lessen, abonnementen beheren door prijzen aan te passen of te verwijderen, ledenprofielen bekijken en aanpassen en via het dashboard een overzicht krijgen van alle leden, lessen en abonnementen.

De medewerker kan leden inschrijven voor lessen en lessen en inschrijvingen beheren, maar heeft geen toegang tot systeeminstellingen of rolbeheer.

Het lid kan beschikbare lessen bekijken via het lessenoverzicht voor leden, kan eigen abonnementen bekijken en persoonlijke informatie inzien via het profielgedeelte.

De belangrijkste vensters in de applicatie zijn het loginvenster, het registratievenster, het dashboard, het lessenoverzicht voor leden, het venster om nieuwe lessen toe te voegen en het venster om leden te bewerken. Het loginvenster wordt gebruikt voor het aanmelden via ASP.NET Identity, het registratievenster voor het aanmaken van een nieuwe gebruiker met keuze van abonnement, en het dashboard toont verschillende tabbladen afhankelijk van de gebruikersrol. Het lessenoverzicht voor leden laat alle beschikbare lessen zien, het venster voor nieuwe lessen laat de admin nieuwe lessen aanmaken en het bewerkvenster laat de admin leden aanpassen.

De gebruikte technologieën in dit project zijn .NET 8 met WPF, Entity Framework Core, ASP.NET Identity, SQL Server en het MVVM-ontwerppatroon. De applicatie ondersteunt soft delete functionaliteit en toegangsbeheer op basis van rollen.

Om de applicatie te installeren moet de repository worden gedownload of gekloond, vervolgens geopend in Visual Studio, daarna moet de SQL Server-verbinding in FitnessClubDbContext.cs gecontroleerd worden. Vervolgens voer je de migraties uit via de Package Manager Console met het commando Update-Database en kun je de applicatie starten met F5.

Bij het eerste opstarten worden standaardrollen en testgebruikers aangemaakt via de SeedService. De admin gebruikt het e-mailadres [admin@fitness.com](mailto:admin@fitness.com) met wachtwoord Admin123 en een standaard lid gebruikt het e-mailadres [lid@fitness.com](mailto:lid@fitness.com) met wachtwoord Lid123. Deze gegevens kunnen worden aangepast in de seeddata.


# Screenshots applicatie


## Welkom ventser


## loginvenster


## Dashbaord admin


## Dashbaord user