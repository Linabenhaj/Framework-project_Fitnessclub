using System.Globalization;

namespace FitnessClub.MAUI.Converters
{
    public class BooleanToColorConverter : IValueConverter
    {
        // Converteert boolean waarde naar kleur op basis van parameter
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is bool boolValue && parameter is string param)
            {
                var parts = param.Split('|');  // Format: "trueKleur|falseKleur"
                var colorString = boolValue ? parts[0] : parts[1];

                if (Color.TryParse(colorString, out var color))
                    return color;  // Retourneert geparse kleur
            }
            return Colors.Gray;  // Standaard bij fout
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();  // One-way conversie
        }
    }
}