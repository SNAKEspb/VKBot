using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MemeTextTranslator
{
    public class CheemsService
    {
        static readonly List<char> consonats = new List<char> { 'б', 'в', 'г', 'д', 'ж', 'з', 'к', 'л', 'н', 'п', 'с', 'т', 'ф', 'х', 'ц', 'ч', 'ш', 'щ' };
        static readonly List<char> vowels = new List<char> { 'а', 'е', 'ё', 'и', 'о', 'у', 'ы', 'э', 'ю', 'я' };
        static readonly char[] splitters = "/[.,:;-_– \n]+".ToCharArray();
        static Dictionary<string, string> cache = new Dictionary<string, string>();

        public static string generate(string text)
        {
            var dict = text.ToLowerInvariant().Split(splitters).Where(t => !string.IsNullOrWhiteSpace(t)).Distinct().ToDictionary(t => t, t => cheemsWord(t));
            return dict.Aggregate(text, (res, t) => res.Replace(t.Key, t.Value));
        }

        private static string cheemsWord(string word)
        {
            if (!cache.ContainsKey(word))
            {
                var result = transformingWord(word);
                cache.Add(word, result);
                return result;
            }
            return cache[word];

        }

        private static string transformingWord(string word)
        {
            var letters = word.ToCharArray();
            var result = "";

            var mCount = 0;
            var mChance = 1;

            for (int i = 0; i < letters.Length; i++)
            {
                var skip = false;

                if (i > 1 && i < letters.Length - 1)
                {
                    //if ((letters[i + 1] == 'н' && letters[i] == 'н') || (letters[i] == 'н' && consonats.Contains(letters[i + 1])))
                    if (letters[i] == 'н' && (letters[i + 1] == 'н' || consonats.Contains(letters[i + 1])))
                    {
                        skip = true;
                    }
                }
                if (!skip)
                {
                    result += letters[i];
                }
                if ( letters.Length > 2 && ((mCount == 0 && i < letters.Length - 1) || (mCount > 0 && i < letters.Length - 2)))
                {
                    if (consonats.Contains(letters[i + 1]) && vowels.Contains(letters[i])
                    // && Math.random() <= mChance / (mCount + 1)
                    )
                    {
                        result += "м";
                        mCount++;
                    }
                }
            }
            return result;
        }
    }
}
