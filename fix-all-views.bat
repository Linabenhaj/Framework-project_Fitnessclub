@echo off
echo =========================================
echo FIXING ALL VIEW FILES
echo =========================================

cd /d "C:\Users\Benha\source\repos\FitnessClub_Final_Solution"

echo 1. Fixing Gebruikers views...
echo @model FitnessClub.Models.Models.Gebruiker > "FitnessClub.Web\Views\Gebruikers\Create.cshtml.new"
type "FitnessClub.Web\Views\Gebruikers\Create.cshtml" | findstr /v "@model" >> "FitnessClub.Web\Views\Gebruikers\Create.cshtml.new" 2>nul
move /y "FitnessClub.Web\Views\Gebruikers\Create.cshtml.new" "FitnessClub.Web\Views\Gebruikers\Create.cshtml" > nul

echo @model FitnessClub.Models.Models.Gebruiker > "FitnessClub.Web\Views\Gebruikers\Edit.cshtml.new"
type "FitnessClub.Web\Views\Gebruikers\Edit.cshtml" | findstr /v "@model" >> "FitnessClub.Web\Views\Gebruikers\Edit.cshtml.new" 2>nul
move /y "FitnessClub.Web\Views\Gebruikers\Edit.cshtml.new" "FitnessClub.Web\Views\Gebruikers\Edit.cshtml" > nul

echo @model FitnessClub.Models.Models.Gebruiker > "FitnessClub.Web\Views\Gebruikers\Details.cshtml.new"
type "FitnessClub.Web\Views\Gebruikers\Details.cshtml" | findstr /v "@model" >> "FitnessClub.Web\Views\Gebruikers\Details.cshtml.new" 2>nul
move /y "FitnessClub.Web\Views\Gebruikers\Details.cshtml.new" "FitnessClub.Web\Views\Gebruikers\Details.cshtml" > nul

echo 2. Fixing Lessen views...
echo @model FitnessClub.Models.Models.Les > "FitnessClub.Web\Views\Lessen\Create.cshtml.new"
type "FitnessClub.Web\Views\Lessen\Create.cshtml" | findstr /v "@model" >> "FitnessClub.Web\Views\Lessen\Create.cshtml.new" 2>nul
move /y "FitnessClub.Web\Views\Lessen\Create.cshtml.new" "FitnessClub.Web\Views\Lessen\Create.cshtml" > nul

echo @model FitnessClub.Models.Models.Les > "FitnessClub.Web\Views\Lessen\Details.cshtml.new"
type "FitnessClub.Web\Views\Lessen\Details.cshtml" | findstr /v "@model" >> "FitnessClub.Web\Views\Lessen\Details.cshtml.new" 2>nul
move /y "FitnessClub.Web\Views\Lessen\Details.cshtml.new" "FitnessClub.Web\Views\Lessen\Details.cshtml" > nul

echo 3. Fixing WPF using statements...
for %%f in ("FitnessClub_WPF\Windows\*.xaml.cs") do (
    echo Adding using statement to %%f...
    powershell -Command "'using FitnessClub.Models.Models;' + \"`n`n\" + (Get-Content '%%f') | Set-Content '%%f'"
)

echo 4. Fixing MAUI...
if exist "FitnessClub.MAUI\MauiProgram.cs" (
    powershell -Command "(Get-Content 'FitnessClub.MAUI\MauiProgram.cs') -replace '\.AddDebug\(\)', '' | Set-Content 'FitnessClub.MAUI\MauiProgram.cs'"
    powershell -Command "(Get-Content 'FitnessClub.MAUI\MauiProgram.cs') -replace '\.UseMauiCommunityToolkit\(\)', '' | Set-Content 'FitnessClub.MAUI\MauiProgram.cs'"
    powershell -Command "(Get-Content 'FitnessClub.MAUI\MauiProgram.cs') -replace '\.UseMauiApp<App>\(\)', '.UseMauiApp<App>().UseMauiCommunityToolkit()' | Set-Content 'FitnessClub.MAUI\MauiProgram.cs'"
)

echo 5. Cleaning...
rmdir /s /q FitnessClub.Web\bin 2>nul
rmdir /s /q FitnessClub.Web\obj 2>nul

echo.
echo =========================================
echo ALL VIEWS FIXED!
echo Now run:
echo.
echo 1. cd FitnessClub.Models
echo 2. dotnet build
echo 3. cd ..\FitnessClub.Web
echo 4. dotnet build
echo =========================================
pause