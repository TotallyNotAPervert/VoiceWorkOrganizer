using System;
using System.Text;
using System.IO;
using DLSiteParser.Resources;
using System.Linq;
using System.Collections.Generic;
using HtmlAgilityPack;

namespace DLSiteParser
{
    public class Runner
    {
        private Translator tagTranslator = new Translator("tags.txt");
        private Translator voiceActorTranslator = new Translator("voiceActors.txt");
        private ParsedWorkChecker parsedWorkChecker = new ParsedWorkChecker("parsedWorks.txt");

        private string parentDirectory;
        private string tagOutputDirectory;
        private string voiceActorOutputDirectory;
        private string circleNameOutPutDirectory;

        private bool translateTags;
        private bool translateVoiceActors;

        public void Run()
        {
            SetUp();

            var folders = Directory.GetDirectories(parentDirectory);

            foreach (var folder in folders)
            {
                var translatedTags = new List<string>();
                var translatedVoiceActors = new List<string>();

                var workNumber = RegexMatcher.FindWorkNumber(folder);
                if (workNumber.Equals(string.Empty))
                    continue;

                if (parsedWorkChecker.ParsedWorks.Contains(workNumber))
                    continue;

                var url = $@"https://www.dlsite.com/maniax/work/=/product_id/{workNumber}.html";

                var parser = new Parser(url);

                var errorNode = parser.GetSingleNode("div", "class", "error_box_inner");
                if (errorNode != null)
                    continue;

                parsedWorkChecker.ParsedWorks.Add(workNumber);

                Console.WriteLine($"Parsing data for {folder}...");
                Console.WriteLine();

                var circleNode = parser.GetSingleNode("span", "class", "maker_name");
                var circleName = RegexMatcher.CleanUpText(circleNode.InnerText);
                var circleNameList = new List<string> { CleanUpCircleName(circleName) };

                var outlineNodes = parser.GetNodes("table", "id", "work_outline");
                var voiceActorNames = FindVoiceActors(outlineNodes);

                var genreNodes = parser.GetNodes("div", "class", "main_genre");
                var tags = genreNodes.Select(x => x.InnerText);

                if (translateTags)
                    AddTranslationToList(tags, translatedTags, tagTranslator);
                else
                    translatedTags = tags.Where(x => !x.Equals("\n")).ToList();

                if (translateVoiceActors)
                    AddTranslationToList(voiceActorNames, translatedVoiceActors, voiceActorTranslator);
                else
                    translatedVoiceActors = voiceActorNames.ToList();

                ShortcutCreator.CreateShortcuts(tagOutputDirectory, translatedTags, folder, workNumber);
                ShortcutCreator.CreateShortcuts(voiceActorOutputDirectory, translatedVoiceActors, folder, workNumber);
                ShortcutCreator.CreateShortcuts(circleNameOutPutDirectory, circleNameList, folder, workNumber);

                tagTranslator.WriteToFile();
                voiceActorTranslator.WriteToFile();
                parsedWorkChecker.WriteToFile();
            }
        }

        private void SetUp()
        {
            Console.WriteLine("Please enter the full path of the main folder.");
            parentDirectory = Console.ReadLine();

            Console.WriteLine("Please enter the full path of the output directory. Three folders will be made here.");
            var outputDirectory = Console.ReadLine();

            tagOutputDirectory = Path.Combine(outputDirectory, "Tag");
            voiceActorOutputDirectory = Path.Combine(outputDirectory, "Voice Actor");
            circleNameOutPutDirectory = Path.Combine(outputDirectory, "Circle");

            Console.WriteLine("Do you want to translate tags? Y/N");
            translateTags = Console.ReadLine().Equals("Y");

            Console.WriteLine("Do you want to translate voice actor names? Y/N");
            translateVoiceActors = Console.ReadLine().Equals("Y");
        }

        public void AddTranslationToList(IEnumerable<string> rawData, List<string> translatedList, Translator translator)
        {
            foreach (var data in rawData)
            {
                var dataText = RegexMatcher.CleanUpText(data);

                if (!dataText.Any())
                    continue;

                if (!translator.Dictionary.ContainsKey(dataText))
                    translator.AddTranslation(dataText);

                translatedList.Add($"{dataText.Replace("/", " ∕ ")} - {translator.Dictionary[dataText].Replace("/", " ∕ ")}");
            }
        }

        private IEnumerable<string> FindVoiceActors(HtmlNodeCollection outlineNodes)
        {
            var voiceActorNode = outlineNodes.FirstOrDefault(x => x.InnerText.Contains("\n  声優\n"));
            if (voiceActorNode == null)
            {
                yield return "Unknown Voice Actor";
                yield break;
            }

            var voiceActors = RegexMatcher.CleanUpText(voiceActorNode.InnerText).Substring(2).Split("/");

            foreach (var voiceActor in voiceActors)
                yield return voiceActor;
        }

        private string CleanUpCircleName(string circleName)
        {
            var sb = new StringBuilder();

            foreach (var c in circleName.ToCharArray())
            {
                if (char.IsLetterOrDigit(c))
                    sb.Append(c);
            }

            return sb.ToString();
        }
    }
}
