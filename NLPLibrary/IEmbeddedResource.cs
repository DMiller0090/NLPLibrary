using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NLPLibrary
{
    public interface IEmbeddedResource
    {
        string resourceFilePath { get; set; }
        void WriteResourceToFile();
    }
}
