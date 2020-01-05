using System;
using System.IO;
using System.Collections.Generic;
using System.Globalization;

namespace DLSiteParser.Resources
{
    public class Translator
    {
        public Dictionary<string, string> Dictionary { get; set; }
        private readonly string _fileLocation;

        public Translator(string fileLocation)
        {
            _fileLocation = fileLocation;
            Dictionary = new Dictionary<string, string>();
            FillDictionary(fileLocation);
        }

        private void FillDictionary(string fileLocation)
        {
            var data = File.ReadAllLines(fileLocation);
            foreach (var line in data)
            {
                var japanese = line.Split(',')[0];
                var english = line.Split(',')[1];

                Dictionary.Add(japanese, english);
            }
        }

        public void AddTranslation(string newJapaneseWord)
        {
            Console.WriteLine($"Translation for {newJapaneseWord} not found. Please enter a translation for {newJapaneseWord}:");
            var translation = Console.ReadLine();

            var cultureInfo = new CultureInfo("en-US", false).TextInfo;
            translation = cultureInfo.ToTitleCase(translation.ToLower());

            Dictionary.Add(newJapaneseWord, translation);
        }

        public void WriteToFile()
        {
            using (var file = new StreamWriter(_fileLocation))
            foreach (var item in Dictionary)
                file.WriteLine("{0},{1}", item.Key, item.Value);
        }
    }
}
