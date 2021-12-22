using Microsoft.Win32;
using System;
using System.IO;
using System.Reflection;
using Xunit;

namespace Blocks.Tests.Helpers
{
    /// <summary>
    /// Shared test context across unit tests that loads rhinocommon.dll and grasshopper.dll
    /// </summary>
    /// <remarks>See https://github.com/tmakin/RhinoCommonUnitTesting for details</remarks>
    public class RhinoTestingHelper : IDisposable
    {
        private bool _initialized = false;
        private static string _rhinoDir;
        private Rhino.Runtime.InProcess.RhinoCore _rhinoCore;

        /// <summary>
        /// Empty Constuctor
        /// </summary>
        public RhinoTestingHelper()
        {
            //get the correct rhino 7 installation directory
            _rhinoDir = Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\McNeel\Rhinoceros\7.0\Install", "Path", null) as string ?? string.Empty;
            Assert.True(Directory.Exists(_rhinoDir), String.Format("Rhino system dir not found: {0}", _rhinoDir));

            // Make sure we are running the tests as 64x
            Assert.True(Environment.Is64BitProcess, "Tests must be run as x64");

            if (_initialized)
            {
                throw new InvalidOperationException("Initialize Rhino.Inside once");
            }
            else
            {
                RhinoInside.Resolver.Initialize();
                _initialized = true;
            }

            // Set path to rhino system directory
            string envPath = Environment.GetEnvironmentVariable("path");
            Environment.SetEnvironmentVariable("path", envPath + ";" + _rhinoDir);

            // Start a headless rhino instance using Rhino.Inside
            StartRhino();

            // We have to load grasshopper.dll on the current AppDomain manually for some reason
            AppDomain.CurrentDomain.AssemblyResolve += ResolveGrasshopper;

        }

        /// <summary>
        /// Starting Rhino - loading the relevant libraries
        /// </summary>
        [STAThread]
        public void StartRhino()
        {
            _rhinoCore = new Rhino.Runtime.InProcess.RhinoCore(null, Rhino.Runtime.InProcess.WindowStyle.NoWindow);
        }

        /// <summary>
        /// Add Grasshopper.dll to the current Appdomain
        /// </summary>
        private Assembly ResolveGrasshopper(object sender, ResolveEventArgs args)
        {
            var name = args.Name;

            if (!name.StartsWith("Grasshopper"))
            {
                return null;
            }

            var path = Path.Combine(Path.GetFullPath(Path.Combine(_rhinoDir, @"..\")), "Plug-ins\\Grasshopper\\Grasshopper.dll");
            return Assembly.LoadFrom(path);
        }

        /// <summary>
        /// Disposing the context after running all the tests
        /// </summary>
        public void Dispose()
        {
            // do nothing or...
            _rhinoCore?.Dispose();
            _rhinoCore = null;
        }
    }
}
