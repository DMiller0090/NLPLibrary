using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NLPLibrary
{
    public interface IProperCaseFormatter : IDisposable
    {
        IEnumerable<string> invalidWords { get; } 
        string ToTitleCase(string data);
    }
}
