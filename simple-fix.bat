@echo off
echo Quick Fix for FitnessClub Project

cd /d "C:\Users\Benha\source\repos\FitnessClub_Final_Solution"

echo Deleting bin/obj folders...
rmdir /s /q FitnessClub.Web\bin 2>nul
rmdir /s /q FitnessClub.Web\obj 2>nul
rmdir /s /q FitnessClub.Models\bin 2>nul
rmdir /s /q FitnessClub.Models\obj 2>nul

echo Please manually fix these files:
echo 1. Open GebruikersController.cs
echo    - Add: using FitnessClub.Models.Data;
echo    - Add: using FitnessClub.Models.Models;
echo 
echo 2. Open LessenController.cs
echo    - Add: using FitnessClub.Models.Data;
echo    - Add: using FitnessClub.Models.Models;
echo
echo 3. Open _Layout.cshtml
echo    - Remove all @using statements
echo    - Add only: @using FitnessClub.Models.Models
echo
echo 4. Check PaginatedList.cs is in FitnessClub.Web\ (not Controllers)
echo
pause