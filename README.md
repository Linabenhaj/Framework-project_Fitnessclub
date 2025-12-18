# Projectvoorstel:  Fitnessclub Applicatie

Dit project is een desktopapplicatie ontwikkeld in C Sharp met .NET WPF voor het beheren van een fitnessclub. De applicatie ondersteunt verschillende rollen: Admin, Medewerker en Lid. Elke rol heeft zijn eigen dashboard en functies.

# Interne logica van de applicatie

De applicatie biedt een compleet systeem voor het beheren van leden, lessen, abonnementen, inschrijvingen, gebruikersrollen en toegangsrechten. Ze maakt gebruik van Entity Framework Core en ASP.NET Identity voor veilige authenticatie en autorisatie op basis van rollen.

De admin heeft volledige controle over de fitnessclub. De admin kan lessen toevoegen, bewerken en verwijderen, leden koppelen aan lessen, abonnementen beheren door prijzen aan te passen of te verwijderen, ledenprofielen bekijken en via het dashboard een overzicht krijgen van alle leden, lessen en abonnementen.

Het lid kan beschikbare lessen bekijken via het lessenoverzicht voor leden, kan eigen abonnementen bekijken en persoonlijke informatie zien via het profielgedeelte.

De belangrijkste vensters in de applicatie zijn het loginvenster, het registratievenster, het dashboard, het lessenoverzicht voor leden, het venster om nieuwe lessen toe te voegen en het venster om leden te bewerken. Het loginvenster wordt gebruikt voor het aanmelden via ASP.NET Identity, het registratievenster voor het aanmaken van een nieuwe gebruiker met keuze van abonnement, en het dashboard toont verschillende tabbladen afhankelijk van de gebruikersrol. Het lessenoverzicht voor leden laat alle beschikbare lessen zien, het venster voor nieuwe lessen laat de admin nieuwe lessen aanmaken en het bewerkvenster laat de admin leden aanpassen.

De gebruikte technologieën in dit project zijn .NET 9 met WPF,Class Library, Entity Framework Core, ASP.NET Identity, SQL Server en het MVVM-ontwerppatroon. De applicatie ondersteunt soft delete functionaliteit en toegangsbeheer op basis van rollen.

Om de applicatie te installeren moet de repository worden gedownload of gekloond, vervolgens geopend in Visual Studio, daarna moet de SQL Server-verbinding in FitnessClubDbContext.cs gecontroleerd worden. Vervolgens voer je de migraties uit via de Package Manager Console met het commando Update-Database en kun je de applicatie starten met F5.

Bij het eerste opstarten worden standaardrollen en testgebruikers aangemaakt via de SeedService. De admin gebruikt het e-mailadres [admin@fitness.com](mailto:admin@fitness.com) met wachtwoord Admin123 en een standaard lid gebruikt het e-mailadres [lid@fitness.com](mailto:lid@fitness.com) met wachtwoord Lid123. Deze gegevens kunnen worden aangepast in de seeddata.


# Screenshots applicatie
Voor de screenshots van de applicatie gebruikte ik de user "Wesley" die hiervoor aangemaakt was en dus even niet lid@fitness.com. Maar deze werkt uiteraard ook.

## Welkom ventser

<img width="973" height="611" alt="image" src="https://github.com/user-attachments/assets/450203a8-cdd1-4158-99d6-cc00a3bd87d4" />

## loginvenster

<img width="581" height="657" alt="image" src="https://github.com/user-attachments/assets/0cf68fed-961e-4ebb-bbb7-e364f0d859a4" />

## registratieformulier

<img width="858" height="1017" alt="image" src="https://github.com/user-attachments/assets/eee411f7-12db-4643-b581-ec05054ec958" />

## Dashbaord admin
<img width="1099" height="733" alt="image" src="https://github.com/user-attachments/assets/ae6948f8-42f2-45f5-8fbc-df1d4c826c17" />

<img width="1105" height="728" alt="image" src="https://github.com/user-attachments/assets/8ba0593e-cc39-4c53-9ba9-6195f03582fe" />

<img width="1097" height="733" alt="image" src="https://github.com/user-attachments/assets/231a25b0-a033-44e6-a751-f08e1fbf8085" />

<img width="1094" height="733" alt="image" src="https://github.com/user-attachments/assets/6023c3e9-2459-4983-9994-0454cb32b540" />

<img width="1091" height="730" alt="image" src="https://github.com/user-attachments/assets/0c949e16-4cda-4a6f-8342-3078663390b8" />

## Pop-up vensters
## les toevoegen 
<img width="1918" height="1020" alt="image" src="https://github.com/user-attachments/assets/cdc63b19-3b9f-4637-a73a-e1018928ed4a" />
## abonnement bewerken
<img width="1916" height="1015" alt="image" src="https://github.com/user-attachments/assets/3d58022c-56d4-456b-91da-ecaaecfa6120" />
## Nieuwe inschrijving toevoegen
<img width="471" height="483" alt="image" src="https://github.com/user-attachments/assets/b69f2592-d2b5-4be8-937e-52de32f9fc1f" />


## Dashbaord user
<img width="1096" height="736" alt="image" src="https://github.com/user-attachments/assets/7509471f-f21a-4261-91ca-74181cbed608" />

<img width="1093" height="732" alt="image" src="https://github.com/user-attachments/assets/f2f7658d-0e27-4aa6-8abc-8504a955bcb9" />

<img width="1098" height="735" alt="image" src="https://github.com/user-attachments/assets/64f41bec-66ea-4e58-a2d5-162b031dad3e" />

<img width="1098" height="731" alt="image" src="https://github.com/user-attachments/assets/a5ace6c6-52b9-4504-9c42-a4aaf9bbe5e5" />

## Gebruik van AI

Github Copilot en ChatGPT technologieën werden gebruikt bij het debuggen van errors en hierdoor ook voor aanvulling om deze errors op te lossen als laatste optie. 

# MAUI APP

## Homepagina app
<img width="365" height="861" alt="image" src="https://github.com/user-attachments/assets/01a03cee-9961-4dca-81db-5fb4aec7bc8f" />

## Inlog pagina
<img width="357" height="858" alt="image" src="https://github.com/user-attachments/assets/971eaac2-9c2b-4984-b6b1-eef65f0fa8c7" />

## Regristreren pagina
<img width="367" height="783" alt="image" src="https://github.com/user-attachments/assets/ed1f31c3-5208-4cd5-bb61-df42ddcbcb47" />


## Admin dashboard
<img width="364" height="857" alt="image" src="https://github.com/user-attachments/assets/000cda93-cd42-413c-af39-eae2793b00e5" />


# .NET WEB 

## welkom pagina
<img width="1864" height="854" alt="image" src="https://github.com/user-attachments/assets/858a4d18-f6b9-47e2-814b-6869bb5839dc" />

## login pagina 

<img width="1866" height="849" alt="image" src="https://github.com/user-attachments/assets/bb56538e-5492-4962-942f-fbbf87425cf7" />

## registreren pagina

<img width="1814" height="855" alt="image" src="https://github.com/user-attachments/assets/ac6a05f5-498f-41ec-917d-d3ed1eb7cdaa" />

## user dashboard

<img width="1823" height="852" alt="image" src="https://github.com/user-attachments/assets/75ddd8c8-de91-4106-b94a-ebc58d5be71c" />

## logout pagina

<img width="1810" height="860" alt="image" src="https://github.com/user-attachments/assets/93e2b9c4-35eb-4ea5-9e96-a278123f1c9f" />
