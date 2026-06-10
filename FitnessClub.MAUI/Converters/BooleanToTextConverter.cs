using System.Globalization;

namespace FitnessClub.MAUI.Converters
{
  
    public class BooleanToTextConverter : IValueConverter
    {
        public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            // Wordt gebruikt met Status (string) voor Uitschrijven-knop
            if (value is string status)
                return status == "Actief";

            /// Voor de "Uitschrijven" knop:
            ///   Status == "Actief" → knop zichtbaar/enabled (true)
            ///   anders             → knop verborgen (false)

            // Wordt gebruikt met IsVol (bool) voor Inschrijven-knop inverteer
            if (value is bool isVol)
                return !isVol;

            return true;

            /// Voor de "Inschrijven" knop op LessenPage:
            ///   IsVol == false → knop enabled (true, er zijn plaatsen)
            ///   IsVol == true  → knop disabled (false)
        }

        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }
}