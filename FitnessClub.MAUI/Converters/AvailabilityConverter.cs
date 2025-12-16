using System.Globalization;

namespace FitnessClub.MAUI.Converters
{
    public class AvailabilityConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is int currentCount && parameter is string param)
            {
                var parts = param.Split('/');
                if (parts.Length == 2 && int.TryParse(parts[1], out int max))
                {
                    if (currentCount >= max)
                        return Colors.Red;
                    if (currentCount >= max * 0.8)
                        return Colors.Orange;
                    return Colors.Green;
                }
            }
            return Colors.Gray;
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}