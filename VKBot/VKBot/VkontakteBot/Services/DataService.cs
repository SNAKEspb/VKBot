using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace VKBot.VkontakteBot.Services
{
    public class DataService
    {
        public class Meme
        {
            public string Id;
            public bool isActive;
            public string title;
            public string description;
        }
        public static string memeFile = "memes.txt";
        public static List<Meme> activeMemes = loadActiveMeme();
        public static List<Meme> loadActiveMeme()
        {
            return File.ReadAllLines(memeFile)
                .Where(t => !string.IsNullOrWhiteSpace(t))
                .Select(t => {
                    string[] values = t.Split(';');
                    return new Meme() { Id = values[0], isActive = int.Parse(values[1]) == 1, title = values[2], description = (values.Length > 3 ? values[3] : "") };
                })
                .Where(t => t.isActive)
                .ToList();

        }
    }
}
