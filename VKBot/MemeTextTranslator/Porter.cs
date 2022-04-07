using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

//Porter's algorithm for stemming for Russian language
//https://github.com/SergeiGalkovskii/Porter-s-algorithm-for-stemming-for-russian-language-csharp.git

namespace MemeTextTranslator
{
    public static class Porter
    {

        private static Regex PERFECTIVEGROUND = new Regex("((ив|ивши|ившись|ыв|ывши|ывшись)|((<;=[ая])(в|вши|вшись)))$");

        private static Regex REFLEXIVE = new Regex("(с[яь])$");

        private static Regex ADJECTIVE = new Regex("(ее|ие|ые|ое|ими|ыми|ей|ий|ый|ой|ем|им|ым|ом|его|ого|ему|ому|их|ых|ую|юю|ая|яя|ою|ею)$");

        private static Regex PARTICIPLE = new Regex("((ивш|ывш|ующ)|((?<=[ая])(ем|нн|вш|ющ|щ)))$");

        private static Regex VERB = new Regex("((ила|ыла|ена|ейте|уйте|ите|или|ыли|ей|уй|ил|ыл|им|ым|ен|ило|ыло|ено|ят|ует|уют|ит|ыт|ены|ить|ыть|ишь|ую|ю)|((?<=[ая])(ла|на|ете|йте|ли|й|л|ем|н|ло|но|ет|ют|ны|ть|ешь|нно)))$");

        private static Regex NOUN = new Regex("(а|ев|ов|ие|ье|е|иями|ями|ами|еи|ии|и|ией|ей|ой|ий|й|иям|ям|ием|ем|ам|ом|о|у|ах|иях|ях|ы|ь|ию|ью|ю|ия|ья|я)$");

        private static Regex RVRE = new Regex("^(.*?[аеиоуыэюя])(.*)$");

        private static Regex DERIVATIONAL = new Regex(".*[^аеиоуыэюя]+[аеиоуыэюя].*ость?$");

        private static Regex DER = new Regex("ость?$");

        private static Regex SUPERLATIVE = new Regex("(ейше|ейш)$");

        private static Regex I = new Regex("и$");
        private static Regex P = new Regex("ь$");
        private static Regex NN = new Regex("нн$");

        private static Dictionary<string, string> cache = new Dictionary<string, string>();


        public static string stemWord(string word)
        {
            if (!cache.ContainsKey(word))
            {
                var result = transformingWord(word).RemoveSpecialCharacters();
                cache.Add(word, result);
                return result;
            }
            return cache[word];

        }

        public static string RemoveSpecialCharacters(this string str)
        {
            StringBuilder sb = new StringBuilder();
            foreach (char c in str)
            {
                if ((c >= '0' && c <= '9') || (c >= 'А' && c <= 'Я') || (c >= 'а' && c <= 'я') || c == '.' || c == '_')
                {
                    sb.Append(c);
                }
            }
            return sb.ToString();
        }

        private static string transformingWord(string word)
        {
            word = word.ToLower();
            word = word.Replace('ё', 'е');
            MatchCollection m = RVRE.Matches(word);
            if (m.Count > 0)
            {
                Match match = m[0]; // only one match in this case 
                GroupCollection groupCollection = match.Groups;
                string pre = groupCollection[1].ToString();
                string rv = groupCollection[2].ToString();

                MatchCollection temp = PERFECTIVEGROUND.Matches(rv);
                string StringTemp = ReplaceFirst(temp, rv);


                if (StringTemp.Equals(rv))
                {
                    MatchCollection tempRV = REFLEXIVE.Matches(rv);
                    rv = ReplaceFirst(tempRV, rv);
                    temp = ADJECTIVE.Matches(rv);
                    StringTemp = ReplaceFirst(temp, rv);
                    if (!StringTemp.Equals(rv))
                    {
                        rv = StringTemp;
                        tempRV = PARTICIPLE.Matches(rv);
                        rv = ReplaceFirst(tempRV, rv);
                    }
                    else
                    {
                        temp = VERB.Matches(rv);
                        StringTemp = ReplaceFirst(temp, rv);
                        if (StringTemp.Equals(rv))
                        {
                            tempRV = NOUN.Matches(rv);
                            rv = ReplaceFirst(tempRV, rv);
                        }
                        else
                        {
                            rv = StringTemp;
                        }
                    }

                }
                else
                {
                    rv = StringTemp;
                }

                MatchCollection tempRv = I.Matches(rv);
                rv = ReplaceFirst(tempRv, rv);
                if (DERIVATIONAL.Matches(rv).Count > 0)
                {
                    tempRv = DER.Matches(rv);
                    rv = ReplaceFirst(tempRv, rv);
                }

                temp = P.Matches(rv);
                StringTemp = ReplaceFirst(temp, rv);
                if (StringTemp.Equals(rv))
                {
                    tempRv = SUPERLATIVE.Matches(rv);
                    rv = ReplaceFirst(tempRv, rv);
                    tempRv = NN.Matches(rv);
                    rv = ReplaceFirst(tempRv, rv);
                }
                else
                {
                    rv = StringTemp;
                }
                word = pre + rv;

            }

            return word;
        }

        public static string ReplaceFirst(MatchCollection collection, string part)
        {
            string StringTemp = "";
            if (collection.Count == 0)
            {
                return part;
            }
            /*else if(collection.Count == 1) 
            { 
            return StringTemp; 
            }*/
            else
            {
                StringTemp = part;
                for (int i = 0; i < collection.Count; i++)
                {
                    GroupCollection GroupCollection = collection[i].Groups;
                    if (StringTemp.Contains(GroupCollection[i].ToString()))
                    {
                        string deletePart = GroupCollection[i].ToString();
                        StringTemp = StringTemp.Replace(deletePart, "");
                    }

                }
            }
            return StringTemp;
        }
    }
}