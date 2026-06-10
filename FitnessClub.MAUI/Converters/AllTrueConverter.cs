using System.Globalization;

namespace FitnessClub.MAUI.Converters
{
   
    /// MultiBinding converter: geeft true terug als ALLE input-waarden true zijn
    /// Gebruik in XAML voor het combineren van meerdere bool-bindings (AND-logica)
 
    public class AllTrueConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object? parameter, CultureInfo culture)
        {
            if (values == null) return false;
            foreach (var v in values)
            {
                if (v is bool b)
                {
                    if (!b) return false;
                }
                else
                {
                    return false;
                }
            }
            return true;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object? parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }
}
