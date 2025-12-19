using System.Globalization;

namespace FitnessClub.MAUI.Converters
{
    public class AvailabilityConverter : IValueConverter
    {
        // Converteert bezettingsgraad naar kleur voor visualisatie
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is int currentCount && parameter is string param)
            {
                var parts = param.Split('/');  // Split parameter zoals "huidig/totaal"
                if (parts.Length == 2 && int.TryParse(parts[1], out int max))
                {
                    // Rood als vol
                    if (currentCount >= max)
                        return Colors.Red;
                    // Oranje als bijna vol (80% of meer)
                    if (currentCount >= max * 0.8)
                        return Colors.Orange;
                    // Groen als nog voldoende plaats
                    return Colors.Green;
                }
            }
            return Colors.Gray;  // Standaard grijs bij fout
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();  // Niet nodig voor one-way binding
        }
    }
}