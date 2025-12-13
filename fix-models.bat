@echo off
echo =========================================
echo FIXING MODELS PROJECT NAMESPACE ERRORS
echo =========================================

cd /d "C:\Users\Benha\source\repos\FitnessClub_Final_Solution\FitnessClub.Models"

echo 1. Removing old classes from root...
del "Gebruiker.cs" 2>nul
del "Les.cs" 2>nul
del "Abonnement.cs" 2>nul
del "Inschrijving.cs" 2>nul
del "BasisEntiteit.cs" 2>nul

echo 2. Cleaning bin/obj...
rmdir /s /q bin 2>nul
rmdir /s /q obj 2>nul

echo 3. Fixing Abonnement.cs...
if exist "Models\Abonnement.cs" (
    powershell -Command "(Get-Content 'Models\Abonnement.cs') -replace 'public ICollection<Gebruiker> Gebruikers.*', '' | Set-Content 'Models\Abonnement.cs'"
)

echo 4. Fixing DbContext relationships...
if exist "Data\FitnessClubDbContext.cs" (
    powershell -Command "(Get-Content 'Data\FitnessClubDbContext.cs') -replace '\.HasOne\(g => g\.Abonnement\)\s*\.WithMany\(a => a\.Gebruikers\)', '.HasOne<Abonnement>().WithMany()' | Set-Content 'Data\FitnessClubDbContext.cs'"
)

echo 5. Restoring packages...
dotnet restore

echo 6. Building Models project...
dotnet build

echo.
if %errorlevel% equ 0 (
    echo =========================================
    echo MODELS PROJECT BUILT SUCCESSFULLY!
    echo =========================================
) else (
    echo =========================================
    echo MODELS PROJECT STILL HAS ERRORS
    echo =========================================
)

pause