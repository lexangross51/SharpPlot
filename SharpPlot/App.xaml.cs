using System.Globalization;
using System.Threading;

namespace SharpPlot;

public partial class App
{
    public App()
    {
        Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
    }
}