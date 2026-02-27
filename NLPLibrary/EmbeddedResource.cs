using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace NLPLibrary
{
    public class AcronymDict : EmbeddedResource, IReadable
    {
        private StreamReader _sr;
        private FileStream _reader;
        private const string FILE_NAME = "NLPLibrary.acronyms";
        public AcronymDict() : base(FILE_NAME)
        { }
        public string Read()
        {
            if (_reader == null)
            {
                _reader = new FileStream(resourceFilePath, FileMode.Open, FileAccess.Read);
                _sr = new StreamReader(_reader);
            }
            var result = _sr.ReadLine();
            return result;
        }

        public void Dispose()
        {
            if (_reader != null)
            {
                //_reader.Close();
                //_reader.Dispose();
                _reader.Position = 0;
                _sr.DiscardBufferedData();
            }
        }
    }
    public class TagDict : EmbeddedResource
    {
        private const string FILE_NAME = "NLPLibrary.tagdict";
        public TagDict() : base(FILE_NAME)
        { }
    }
    public class PosFile : EmbeddedResource
    {
        private const string FILE_NAME = "NLPLibrary.EnglishPOS.nbin";
        public PosFile() : base(FILE_NAME)
        { }
    }
    public class EmbeddedResource : IEmbeddedResource
    { 
        protected static Assembly _assembly { get; set; }
        protected static Assembly assembly
        {
            get
            {
                if (_assembly == null)
                {
                    _assembly = Assembly.GetExecutingAssembly();
                }
                return _assembly;
            }
        }
        public string resourceFilePath { get; set; }

        public EmbeddedResource(string fileName)
        {
            var dirPath = new Uri(Path.GetDirectoryName(assembly.CodeBase)).LocalPath + "\\";
            if (UnitTestDetector.IsRunningFromNUnit)
            {
                dirPath = dirPath.Replace(".Tests", "");
            }
            resourceFilePath = dirPath + fileName;
            if (!File.Exists(resourceFilePath))
            {
                WriteResourceToFile();
            }
        }
        public void WriteResourceToFile()
        {
            var resourceName = Path.GetFileName(resourceFilePath);
            using (Stream resource = _assembly.GetManifestResourceStream(resourceName))
            {
                using (Stream file = new FileStream(resourceFilePath, FileMode.Create, FileAccess.Write))
                {
                    resource.CopyTo(file);
                }
            }
        }
    }
}
