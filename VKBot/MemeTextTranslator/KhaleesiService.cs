using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace MemeTextTranslator
{
    public class KhaleesiService
    {
        private static readonly Regex regex = new Regex(string.Join('|', KhaleesiVocabulary.replaces.Keys), RegexOptions.IgnoreCase | RegexOptions.Multiline);
        static Random _random = new Random();
        public static string generate(string text) {
            return regex.Replace(text, match => KhaleesiVocabulary.replaces[match.Groups[0].Value]);
        }

        public static bool calculateChance(string text, int chanceModifier = 100) {
            int textLen = text.Length;
            int minLen = 100;
            var sentimentScore = SentimentAnalyzer.getSentimentScore(text);
            var lengthMultiplier = textLen > minLen ? 1 : textLen / minLen;
            var chance = (chanceModifier * (Math.Abs(sentimentScore) + 1) * lengthMultiplier);
            return _random.Next(0, 100) < chance;
        }
    }
}
