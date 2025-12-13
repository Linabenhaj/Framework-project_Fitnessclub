@echo off
echo =========================================
echo FIXING ALL WEB PROJECT ERRORS
echo =========================================

cd /d "C:\Users\Benha\source\repos\FitnessClub_Final_Solution"

echo 1. Cleaning folders...
rmdir /s /q "FitnessClub.Web\bin" 2>nul
rmdir /s /q "FitnessClub.Web\obj" 2>nul
rmdir /s /q "FitnessClub.Models\bin" 2>nul
rmdir /s /q "FitnessClub.Models\obj" 2>nul

echo 2. Fixing using statements in controllers...

echo 3. Creating new _Layout.cshtml...
(
echo @using FitnessClub.Models.Models
echo @using Microsoft.AspNetCore.Identity
echo @inject SignInManager^<FitnessClub.Models.Models.Gebruiker^> SignInManager
echo @inject UserManager^<FitnessClub.Models.Models.Gebruiker^> UserManager
echo.
) > "FitnessClub.Web\Views\Shared\_Layout_new.cshtml"

REM Voeg de rest van het bestand toe zonder de oude @using statements
type "FitnessClub.Web\Views\Shared\_Layout.cshtml" | findstr /v "@using" | findstr /v "@inject" | findstr /v "^using " >> "FitnessClub.Web\Views\Shared\_Layout_new.cshtml"

move /y "FitnessClub.Web\Views\Shared\_Layout_new.cshtml" "FitnessClub.Web\Views\Shared\_Layout.cshtml" > nul

echo 4. Fixing Index.cshtml files...
if exist "FitnessClub.Web\Views\Lessen\Index.cshtml" (
    powershell -Command "(Get-Content 'FitnessClub.Web\Views\Lessen\Index.cshtml') -replace '@model.*', '@model FitnessClub.Web.PaginatedList<FitnessClub.Models.Models.Les>' | Set-Content 'FitnessClub.Web\Views\Lessen\Index.cshtml'"
)

if exist "FitnessClub.Web\Views\Gebruikers\Index.cshtml" (
    powershell -Command "(Get-Content 'FitnessClub.Web\Views\Gebruikers\Index.cshtml') -replace '@model.*', '@model FitnessClub.Web.PaginatedList<FitnessClub.Models.Models.Gebruiker>' | Set-Content 'FitnessClub.Web\Views\Gebruikers\Index.cshtml'"
)

echo 5. Removing PaginatedList from Controllers folder...
del "FitnessClub.Web\Controllers\PaginatedList.cs" 2>nul

echo 6. Checking PaginatedList in root...
if not exist "FitnessClub.Web\PaginatedList.cs" (
    echo Creating PaginatedList.cs in root...
    (
    echo using Microsoft.EntityFrameworkCore;
    echo using System;
    echo using System.Collections.Generic;
    echo using System.Linq;
    echo using System.Threading.Tasks;
    echo.
    echo namespace FitnessClub.Web
    echo {
    echo     public class PaginatedList^<T^> : List^<T^>
    echo     {
    echo         public int PageIndex { get; private set; }
    echo         public int TotalPages { get; private set; }
    echo.
    echo         public PaginatedList(List^<T^> items, int count, int pageIndex, int pageSize)
    echo         {
    echo             PageIndex = pageIndex;
    echo             TotalPages = ^(int^)Math.Ceiling(count / ^(double^)pageSize^);
    echo             this.AddRange(items^);
    echo         }
    echo.
    echo         public bool HasPreviousPage =^> PageIndex ^> 1;
    echo         public bool HasNextPage =^> PageIndex ^< TotalPages;
    echo.
    echo         public static async Task^<PaginatedList^<T^>^> CreateAsync^(IQueryable^<T^> source, int pageIndex, int pageSize^)
    echo         {
    echo             var count = await source.CountAsync^(^);
    echo             var items = await source.Skip^(
    echo                 ^(pageIndex - 1^) * pageSize^)
    echo                 .Take^(pageSize^).ToListAsync^(^);
    echo             return new PaginatedList^<T^>^(items, count, pageIndex, pageSize^);
    echo         }
    echo     }
    echo }
    ) > "FitnessClub.Web\PaginatedList.cs"
)

echo 7. Fixing controllers using statements...
if exist "FitnessClub.Web\Controllers\GebruikersController.cs" (
    echo Adding using statements to GebruikersController.cs...
    (
    echo using FitnessClub.Models.Data;
    echo using FitnessClub.Models.Models;
    echo using Microsoft.AspNetCore.Identity;
    echo using Microsoft.AspNetCore.Mvc;
    echo using Microsoft.EntityFrameworkCore;
    echo using System;
    echo using System.Linq;
    echo using System.Threading.Tasks;
    echo.
    ) > "FitnessClub.Web\Controllers\GebruikersController_new.cs"
    type "FitnessClub.Web\Controllers\GebruikersController.cs" | findstr /v "using " >> "FitnessClub.Web\Controllers\GebruikersController_new.cs" 2>nul
    move /y "FitnessClub.Web\Controllers\GebruikersController_new.cs" "FitnessClub.Web\Controllers\GebruikersController.cs" > nul
)

if exist "FitnessClub.Web\Controllers\LessenController.cs" (
    echo Adding using statements to LessenController.cs...
    (
    echo using FitnessClub.Models.Data;
    echo using FitnessClub.Models.Models;
    echo using Microsoft.AspNetCore.Mvc;
    echo using Microsoft.EntityFrameworkCore;
    echo using System;
    echo using System.Linq;
    echo using System.Threading.Tasks;
    echo.
    ) > "FitnessClub.Web\Controllers\LessenController_new.cs"
    type "FitnessClub.Web\Controllers\LessenController.cs" | findstr /v "using " >> "FitnessClub.Web\Controllers\LessenController_new.cs" 2>nul
    move /y "FitnessClub.Web\Controllers\LessenController_new.cs" "FitnessClub.Web\Controllers\LessenController.cs" > nul
)

echo 8. Restoring packages...
dotnet restore FitnessClub.Models
dotnet restore FitnessClub.Web

echo.
echo =========================================
echo FIXES COMPLETED!
echo Now run these commands:
echo cd FitnessClub.Models
echo dotnet build
echo cd ..\FitnessClub.Web
echo dotnet build
echo =========================================
echo.
pause