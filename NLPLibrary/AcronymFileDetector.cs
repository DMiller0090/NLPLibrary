using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace NLPLibrary
{
    public class AcronymFileDetector : IAcronymDetector
    {
        private readonly IReadable _reader;
        public AcronymFileDetector(IReadable reader)
        {
            if(reader == null)
            {
                throw new ArgumentNullException("reader");
            }
            this._reader = reader;
        }
        public void Dispose()
        {
            _reader.Dispose();
        }

        public bool IsAcronym(string word)
        {
            try
            {
                //using(StreamReader acronymReader = new StreamReader(filePath))

                //using (_reader)
                //{
                using (_reader)
                {
                    string prevLine;
                    string line;
                    while ((line = _reader.Read()?.TrimEnd()) != null)
                    {
                        prevLine = line;
                        if (word == line)
                        {
                            return true;
                        }
                    }
                    return false;
                }

                //}
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
        }
    }
}
