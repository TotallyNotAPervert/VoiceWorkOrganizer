using System;
using System.IO;
using System.Collections.Generic;
using System.Text;

namespace DLSiteParser.Resources
{
    public class ParsedWorkChecker
    {
        public List<string> ParsedWorks { get; set; }
        private readonly string _fileLocation;

        public ParsedWorkChecker(string fileLocation)
        {
            _fileLocation = fileLocation;
            ParsedWorks = new List<string>();
            FillList(fileLocation);
        }

        private void FillList(string fileLocation)
        {
            var data = File.ReadAllLines(fileLocation);
            foreach (var line in data)
            {
                ParsedWorks.Add(line);
            }
        }

        public void WriteToFile()
        {
            using (var file = new StreamWriter(_fileLocation))
                foreach (var item in ParsedWorks)
                    file.WriteLine(item);
        }
    }
}
