using System.Globalization;

namespace FitnessClub.MAUI.Converters
{
    public class StatusColorConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is string status)
            {
                return status.ToLower() switch
                {
                    "actief" => Colors.Green,
                    "geannuleerd" => Colors.Red,
                    "geweest" => Colors.Gray,
                    _ => Colors.Black
                };
            }
            return Colors.Black;
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}