using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using CivilizedConversation.Services.Models;

namespace CivilizedConversation.Services
{
    class Translator
    {
        public static Dictionary<string, string[]> Dictionary = new Dictionary<string, string[]>();
        public static Dictionary<string, string[]> Replacements = new Dictionary<string, string[]>();
        public static Phrase[] Phrases;
        private static readonly Random Random = new Random();

        static Translator()
        {
            var assetPath = Path.Combine(BepInEx.Paths.PluginPath, "CivilizedChat/Assets");
            LoadDictionary(Path.Combine(assetPath, "dictionary.csv"));
            LoadModifiers(Path.Combine(assetPath, "phrases.json"));
        }

        private static void LoadDictionary(string path)
        {
            using (var reader = new StreamReader(path))
            {
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    Dictionary<string, string[]> target;
                    if (line.StartsWith("*"))
                    {
                        target = Replacements;
                        line = line.Substring(1);
                    }
                    else
                    {
                        target = Dictionary;
                    }
                    var values = line.Split('|');
                    var inputs = values[0].Split(';');
                    var outputs = values[1].Split(';');
                    foreach (var input in inputs)
                    {
                        target.Add(input.ToLower(), outputs);
                    }
                }
            }
        }



        private static void LoadModifiers(string path)
        {
            var phraseCollection = SimpleJson.SimpleJson.DeserializeObject<PhraseCollection>(File.ReadAllText(path));
            Phrases = phraseCollection.Phrases.ToArray();
        }

        public static string Translate(string text)
        {
            if (!Mod.Enabled.Value || text.Length == 0) return text;
            int selectionIndex;

            // replace full text if it matches a replacement string
            if (Replacements.ContainsKey(text.ToLower()))
            {
                var choices = Replacements[text.ToLower()];
                selectionIndex = Random.Next(0, choices.Length);
                return choices[selectionIndex];
            }

            // apply dictionary
            foreach (var entry in Dictionary)
            {
                var matches = Regex.Matches(text.ToLower(), $@"\b{entry.Key}\b");
                foreach (Match match in matches)
                {
                    var phrase = match.Value;
                    var choices = entry.Value;
                    selectionIndex = Random.Next(0, choices.Length);
                    var replacement = choices[selectionIndex];
                    if (char.IsUpper(phrase.First())) char.ToUpper(replacement.First());
                    text = text.Replace(phrase, replacement);
                }
            }

            // fix sentence structure
            if (char.IsLower(text.First())) text = char.ToUpper(text.First()) + text.Substring(1);
            if (!char.IsPunctuation(text.Last())) text += ".";

            // add a random modifier
            selectionIndex = Random.Next(0, Phrases.Length);
            var modifier = Phrases[selectionIndex];
            string output;
            switch (modifier.Type.ToLower())
            {
                case "start":
                    output = $"{modifier.Value} {char.ToLower(text[0])}{text.Substring(1)}";
                    break;
                case "end":
                    output = $"{text} {modifier.Value}";
                    break;
                default:
                    output = text;
                    break;
            }

            return output;
        }
    }
}
