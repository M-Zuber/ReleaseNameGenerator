using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using Underscore;

namespace ReleaseNameGenerator
{
    public enum AnimalType
    {
        Birds,
        Fish,
        Lizards,
        Reptiles,
        Mammals
    }

    public class Generator
    {
        private static string PREVIOUS_NAME_FILE = Path.Combine(Directory.GetCurrentDirectory(), ".release-name-generator", "previousNames.json");
        private static string LIB_DIRECTORY_PATH = Path.Combine(Directory.GetCurrentDirectory(), "Lib");
        private static string ADJECTIVES_FILE = Path.Combine(LIB_DIRECTORY_PATH, "adjectives.json");
        private static string ANIMAL_DIRECTORY_PATH = Path.Combine(LIB_DIRECTORY_PATH, "Animals");
        private static Dictionary<AnimalType, string> ANIMAL_FILES = new Dictionary<AnimalType, string>
        {
            [AnimalType.Birds] = Path.Combine(ANIMAL_DIRECTORY_PATH, "birds.json"),
            [AnimalType.Fish] = Path.Combine(ANIMAL_DIRECTORY_PATH, "fish.json"),
            [AnimalType.Lizards] = Path.Combine(ANIMAL_DIRECTORY_PATH, "lizards.json"),
            [AnimalType.Reptiles] = Path.Combine(ANIMAL_DIRECTORY_PATH, "reptiles.json"),
            [AnimalType.Mammals] = Path.Combine(ANIMAL_DIRECTORY_PATH, "mammals.json")
        };
        private static int MAX_RECURSION_COUNT = 1024;

        public IList<string> PreviousNames { get; private set; } = new List<string>();
        public IList<string> Adjectives { get; private set; } = new List<string>();
        public Dictionary<string, IList<string>> Animals { get; private set; } = new Dictionary<string, IList<string>>();

        public Generator()
        {
            PreviousNames = JsonConvert.DeserializeObject<List<string>>(File.ReadAllText(PREVIOUS_NAME_FILE)) ?? new List<string>();
            Adjectives = JsonConvert.DeserializeObject<List<string>>(File.ReadAllText(ADJECTIVES_FILE)) ?? new List<string>();

            foreach (var animalFile in ANIMAL_FILES)
            {
                Animals.Add(animalFile.Key.ToString(), JsonConvert.DeserializeObject<List<string>>(File.ReadAllText(animalFile.Value)) ?? new List<string>());
            }
        }

        public static void InitDirectories()
        {
            var filesToCreate = new List<string>
            {
                PREVIOUS_NAME_FILE,
                ADJECTIVES_FILE,
            };

            filesToCreate.AddRange(ANIMAL_FILES.Select(kev => kev.Value));

            foreach (var file in filesToCreate)
            {
                if (!File.Exists(file))
                {
                    FileInfo f = new FileInfo(file);
                    Directory.CreateDirectory(f.DirectoryName);
#pragma warning disable CS0642 // Possible mistaken empty statement
                    using (File.Create(file)) ;
#pragma warning restore CS0642 // Possible mistaken empty statement
                }
            }
        }

        public void SavePreviousNames()
        {
            File.WriteAllText(PREVIOUS_NAME_FILE, JsonConvert.SerializeObject(PreviousNames));
        }

        public string Generate(string category)
        {
            if (string.IsNullOrWhiteSpace(category))
            {
                var categories = Animals.Keys.ToList();
                category = _.List.Shuffle(categories).First();
            }
            else if (!Animals.ContainsKey(category))
            {
                throw new ArgumentException($"Invalid Argument Specified: {category}");
            }

            var name = GenerateName(category, 0);
            SavePreviousNames();

            return name;
        }

        private string GenerateName(string category, int recursionCount)
        {
            if (recursionCount >= MAX_RECURSION_COUNT)
            {
                throw new StackOverflowException("No name can be generated. Choosing another category may help");
            }

            Adjectives = _.List.Shuffle(Adjectives);
            var adjective = Adjectives.First();

            // Take only animals that the first letter is the same as the first letter of the adjective
            var filteredAnimals = Animals[category].Where(a => a.ToUpper()[0] == adjective.ToUpper()[0]).ToList();

            // If we didn't find any matching animals - try again
            if (!filteredAnimals.Any())
            {
                return GenerateName(category, ++recursionCount);
            }

            filteredAnimals = _.List.Shuffle(filteredAnimals).ToList();

            var name = CultureInfo.CurrentCulture.TextInfo.ToTitleCase($"{adjective} {filteredAnimals.First()}");

            // If the name was already given - try again
            if (PreviousNames.Contains(name))
            {
                return GenerateName(category, ++recursionCount);
            }

            UpdatePreviousNames(name);
            return name;
        }

        private void UpdatePreviousNames(string name)
        {
            PreviousNames.Add(name);
        }
    }
}
