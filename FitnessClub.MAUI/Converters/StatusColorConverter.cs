using System.Globalization;

namespace FitnessClub.MAUI.Converters
{
    public class StatusColorConverter : IValueConverter
    {
        // Converteert status string naar bijpassende kleur
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is string status)
            {
                return status.ToLower() switch
                {
                    "actief" => Colors.Green,      // Groen voor actief
                    "geannuleerd" => Colors.Red,   // Rood voor geannuleerd
                    "geweest" => Colors.Gray,      // Grijs voor verleden
                    _ => Colors.Black               // Zwart voor onbekend
                };
            }
            return Colors.Black;  // Standaard zwart
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();  // Niet geïmplementeerd voor terugconversie
        }
    }
}