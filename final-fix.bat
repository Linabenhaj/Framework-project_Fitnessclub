@echo off
echo =========================================
echo FINAL FIX FOR ALL PROJECTS
echo =========================================

cd /d "C:\Users\Benha\source\repos\FitnessClub_Final_Solution"

echo 1. Adding missing properties to Les.cs...
(
echo using System;
echo using System.Collections.Generic;
echo using System.ComponentModel.DataAnnotations;
echo.
echo namespace FitnessClub.Models.Models
echo {
echo     public class Les : BasisEntiteit
echo     {
echo         [Required(ErrorMessage = "Naam is verplicht")]
echo         [Display(Name = "Naam")]
echo         [StringLength(100)]
echo         public string Naam { get; set; } = string.Empty;
echo.
echo         [Display(Name = "Beschrijving")]
echo         [StringLength(1000)]
echo         public string Beschrijving { get; set; } = string.Empty;
echo.
echo         [Required(ErrorMessage = "Starttijd is verplicht")]
echo         [Display(Name = "Starttijd")]
echo         [DataType(DataType.DateTime)]
echo         public DateTime StartTijd { get; set; }
echo.
echo         [Required(ErrorMessage = "Eindtijd is verplicht")]
echo         [Display(Name = "Eindtijd")]
echo         [DataType(DataType.DateTime)]
echo         public DateTime EindTijd { get; set; }
echo.
echo         [Display(Name = "Maximaal deelnemers")]
echo         [Range(1, 100, ErrorMessage = "Maximaal aantal deelnemers moet tussen 1 en 100 zijn")]
echo         public int MaxDeelnemers { get; set; } = 20;
echo.
echo         [Display(Name = "Locatie")]
echo         [StringLength(200)]
echo         public string Locatie { get; set; } = string.Empty;
echo.
echo         [Display(Name = "Trainer")]
echo         [StringLength(100)]
echo         public string Trainer { get; set; } = string.Empty;
echo.
echo         [Display(Name = "Actief")]
echo         public bool IsActief { get; set; } = true;
echo.
echo         public ICollection<Inschrijving> Inschrijvingen { get; set; } = new List<Inschrijving>();
echo.
echo         // Computed properties
echo         public string DisplayInfo => "^${"Naam} - ^{StartTijd:dd/MM/yyyy HH:mm} (^{Locatie})";
echo         public string KorteInfo => "^${"Naam} (^{StartTijd:HH:mm})";
echo         public int Duur => ^(int^)(EindTijd - StartTijd^).TotalMinutes;
echo         public bool IsToekomstig => StartTijd ^> DateTime.Now;
echo         public bool IsBezig => DateTime.Now ^>= StartTijd ^&^& DateTime.Now ^<= EindTijd;
echo         public bool IsVerleden => EindTijd ^< DateTime.Now;
echo         public int BeschikbarePlaatsen => MaxDeelnemers - ^(Inschrijvingen?.Count ?? 0^);
echo         public bool IsVol => BeschikbarePlaatsen ^<= 0;
echo         public string DagVanWeek => StartTijd.ToString("dddd");
echo         public string TijdRange => "^${"StartTijd:HH:mm} - ^{EindTijd:HH:mm}";
echo     }
echo }
) > "FitnessClub.Models\Models\Les.cs"

echo 2. Fixing DbContext Ignore statements...
if exist "FitnessClub.Models\Data\FitnessClubDbContext.cs" (
    powershell -Command "(Get-Content 'FitnessClub.Models\Data\FitnessClubDbContext.cs') -replace '\.Ignore\(l => l\.DagVanWeek\)', '' | Set-Content 'FitnessClub.Models\Data\FitnessClubDbContext.cs'"
    powershell -Command "(Get-Content 'FitnessClub.Models\Data\FitnessClubDbContext.cs') -replace '\.Ignore\(l => l\.TijdRange\)', '' | Set-Content 'FitnessClub.Models\Data\FitnessClubDbContext.cs'"
)

echo 3. Fixing Index.cshtml model directive...
if exist "FitnessClub.Web\Views\Gebruikers\Index.cshtml" (
    powershell -Command "(Get-Content 'FitnessClub.Web\Views\Gebruikers\Index.cshtml') | Select-Object -First 1 | Set-Content 'temp_first_line.txt'"
    set /p first_line=<temp_first_line.txt
    del temp_first_line.txt 2>nul
    
    if not "%first_line:~0,6%"=="@model" (
        echo Fixing model directive...
        powershell -Command "@'@model FitnessClub.Web.PaginatedList<FitnessClub.Models.Models.Gebruiker>'@ + \"`n`n\" + (Get-Content 'FitnessClub.Web\Views\Gebruikers\Index.cshtml' | Select-Object -Skip 1) | Set-Content 'FitnessClub.Web\Views\Gebruikers\Index.cshtml'"
    )
    
    REM Verwijder extra model directives
    powershell -Command "(Get-Content 'FitnessClub.Web\Views\Gebruikers\Index.cshtml') | Where-Object { ^$_-notmatch '^@model.*' -or ^$_-match '^@model FitnessClub\.Web\.PaginatedList<FitnessClub\.Models\.Models\.Gebruiker>' } | Set-Content 'FitnessClub.Web\Views\Gebruikers\Index.cshtml'"
)

echo 4. Cleaning all projects...
rmdir /s /q FitnessClub.Models\bin 2>nul
rmdir /s /q FitnessClub.Models\obj 2>nul
rmdir /s /q FitnessClub.Web\bin 2>nul
rmdir /s /q FitnessClub.Web\obj 2>nul

echo 5. Restoring packages...
dotnet restore FitnessClub.Models
dotnet restore FitnessClub.Web

echo.
echo =========================================
echo FIXES APPLIED!
echo Now run these commands:
echo.
echo 1. cd FitnessClub.Models
echo 2. dotnet build
echo 3. cd ..\FitnessClub.Web
echo 4. dotnet build
echo =========================================
pause