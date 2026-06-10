using System.Globalization;

namespace FitnessClub.MAUI.Converters
{

    /// Converteert een Status-string naar een achtergrondkleur voor de badge
    ///   "Actief"      → groen
    ///   "Geannuleerd" → rood
    ///   overige       → grijs
   
    public class StatusColorConverter : IValueConverter
    {
        public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            return value?.ToString() switch
            {
                "Actief" => Color.FromArgb("#4CAF50"),
                "Geannuleerd" => Color.FromArgb("#F44336"),
                _ => Color.FromArgb("#9E9E9E")
            };
        }

        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }
}