using ObjCRuntime;
using UIKit;

namespace FitnessClub.MAUI
{
    public class Program
    {
        // Startpunt van de applicatie op MacCatalyst
        static void Main(string[] args)
        {
            // Gebruik een ander Application Delegate type door dit hier aan te passen
            UIApplication.Main(args, null, typeof(AppDelegate));
        }
    }
}
