﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace VKBot.VkBotLogic.Services
{
    public class DataService
    {
        public class User
        {
            public string id;
            public string name;
            public byte access;
        }
        public class Meme
        {
            public string Id;
            public bool isActive;
            public string title;
            public string description;
        }

        public static string memeFile = "memes.txt";
        public static string vityaMessagesFile = "VityaMessages.txt";
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
        public static List<string> vityaMessages = loadVityaMessages();
        public static List<string> loadVityaMessages()
        {
            return File.ReadAllLines(vityaMessagesFile, System.Text.Encoding.GetEncoding(1251))
                .Where(t => !string.IsNullOrWhiteSpace(t))
                .ToList();

        }

        public static List<string> vityaShortMessages { get; set; } = vityaMessages.Where(t => t.Length > 10 && t.Length < 30).GroupBy(t => t).Select(t => t.First()).ToList();

        public static List<string> vityaMediumMessages { get; set; } = vityaMessages.Where(t => t.Length < 100).GroupBy(t => t).Select(t => t.First()).ToList();

        public static Dictionary<string, string> pictures = new Dictionary<string, string>()
        {
            { "communityLike", "photo-179992947_456239019" },
            { "joke", "photo-179992947_456239020"},
            { "respect", "photo-179992947_456239021"},
        };

        public static List<User> users = new List<User>()
        {
            new User(){ name = "Vitya", id = vityaId, access = 2},
            new User(){ name = "Mitya", id = "1556462", access = 4},
            new User(){ name = "Sanya", id = "1500589", access = 4},
        };

        public static List<string> adminsIds => users.Where(t => t.access == 4).Select(t => t.id).ToList();

        public static string vityaId = "212515973";

        public static int userAccess(string id)
        {
            var access = users.Where(t => t.id == id).Select(t => t.access).FirstOrDefault();
            return access == 0 ? 1 : access;
        }

        public static Dictionary<string, string> peers = new Dictionary<string, string>()
        {
            { "2000000001", "test group" },
            { "2000000002", "main group"},
        };
    }
}
