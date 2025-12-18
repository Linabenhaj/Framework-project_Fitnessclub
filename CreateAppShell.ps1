# CompleteAppShell.ps1
Write-Host "Creating AppShell..." -ForegroundColor Yellow

# XAML content
$xaml = @"
<?xml version="1.0" encoding="UTF-8" ?>
<Shell xmlns="http://schemas.microsoft.com/dotnet/2021/maui"
       xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml"
       x:Class="FitnessClub.MAUI.AppShell">
    <ShellContent Title="Test">
        <ContentPage>
            <Label Text="FitnessClub" />
        </ContentPage>
    </ShellContent>
</Shell>
"@

# CS content
$cs = @"
namespace FitnessClub.MAUI
{
    public partial class AppShell : Shell
    {
        public AppShell() { InitializeComponent(); }
    }
}
"@

# Save files
$xml | Out-File "AppShell.xaml" -Encoding UTF8
$cs | Out-File "AppShell.xaml.cs" -Encoding UTF8

Write-Host "Done!" -ForegroundColor Green
