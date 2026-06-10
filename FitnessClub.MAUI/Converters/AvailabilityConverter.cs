using System.Globalization;

namespace FitnessClub.MAUI.Converters
{

    /// Converteert IsVol (bool) naar een kleur voor de badge
    ///   true  → rood  (vol)
    ///   false → groen (plaatsen beschikbaar)
   
    public class AvailabilityConverter : IValueConverter
    {
        public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is bool isVol)
                return isVol ? Color.FromArgb("#F44336") : Color.FromArgb("#4CAF50");

            return Color.FromArgb("#9E9E9E");
        }

        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }
}