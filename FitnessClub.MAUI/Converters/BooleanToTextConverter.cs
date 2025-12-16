using System.Globalization;

namespace FitnessClub.MAUI.Converters
{
    public class BooleanToColorConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is bool boolValue && parameter is string param)
            {
                var parts = param.Split('|');
                var colorString = boolValue ? parts[0] : parts[1];

                if (Color.TryParse(colorString, out var color))
                    return color;
            }
            return Colors.Gray;
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}