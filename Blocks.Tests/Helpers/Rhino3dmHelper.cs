using Rhino.FileIO;
using System;
using System.IO;

namespace Blocks.Tests.Helpers
{
    public class Rhino3dmHelper: IDisposable
    {
        private string _fileName;
        private string _debugDirectory => GetDebugDirectory();
        private File3dm _rhinoFile = new File3dm();

        public Rhino3dmHelper(string fileName)
        {
            _fileName = fileName;
        }

        private string GetDebugDirectory()
        {
            var codeBase = System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase;
            var directoryPath = Path.GetDirectoryName(codeBase);
            return Directory.GetParent(directoryPath.Substring(6)).ToString();
        }

        public File3dmObjectTable Objects => _rhinoFile.Objects;

        public string WriteFile()
        {
            var fileWriteOptions = new File3dmWriteOptions();
            var filePath = Path.Combine(_debugDirectory, _fileName);
            var writeStatus = _rhinoFile.Write(filePath, fileWriteOptions);
            return writeStatus ? filePath : null;
        }

        public void Dispose() => _rhinoFile.Dispose();
    }
}
