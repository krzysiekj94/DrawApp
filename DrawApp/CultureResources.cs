
using DrawApp.Properties;
using System.Globalization;
using System.Windows.Data;

namespace DrawApp
{
    public class CultureResources
    {
        private static ObjectDataProvider resourceProvider =
            (ObjectDataProvider) App.Current.FindResource("Resources");

        public Properties.Resources GetResourceInstance()
        {
            return new Properties.Resources();
        }

        public static void ChangeCulture(CultureInfo culture)
        {
            DrawApp.Properties.Resources.Culture = culture;
            resourceProvider.Refresh();
        }
    }
}
