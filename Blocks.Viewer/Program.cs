using System;

namespace Blocks.Viewer
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            RhinoInside.Resolver.Initialize();

            RunApplication();
        }

        static void RunApplication()
        {
            RhinoInstance.Run();
        }
    }
}
