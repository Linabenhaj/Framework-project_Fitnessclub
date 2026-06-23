using System.Globalization;

namespace FitnessClub.MAUI.Converters
{
    // converter die meerdere bool waarden combineert via AND
    // wordt in XAML gebruikt om bv een knop alleen te tonen als 2 voorwaarden waar zijn
    public class AllTrueConverter : IMultiValueConverter
    {
        // ontvangt een array van waarden uit de MultiBinding
        public object Convert(object[] values, Type targetType, object? parameter, CultureInfo culture)
        {
            // als er geen waarden zijn return false
            if (values == null) return false;

            // overloop elke waarde
            foreach (var v in values)
            {
                // als de waarde geen bool is return false
                if (v is bool b)
                {
                    // als één waarde false is return false
                    if (!b) return false;
                }
                else
                {
                    return false;
                }
            }

            // alle waarden waren true dus return true
            return true;
        }

        // ConvertBack is verplicht door IMultiValueConverter maar wordt niet gebruikt
        public object[] ConvertBack(object value, Type[] targetTypes, object? parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }
}
