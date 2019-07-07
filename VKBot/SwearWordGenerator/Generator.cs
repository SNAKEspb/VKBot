using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SwearWordGenerator
{
    public class Generator
    {
        static Random rand = new Random();
        public static string generate(Type type, int amount, Sex sex = Sex.N, Case wordCase = Case.I)
        {
            //validate
            if (type == Type.Verb && sex != Sex.N && wordCase != Case.I)
            {
                throw new Exception($"Sex and case should not be specified for {type} type");
            }
            //generate prefixes
            var result = new StringBuilder();
            result.Append(getPrefix(amount));
            //generate root
            var words = Vocabulary.words[type][sex];
            result.Append(rand.Next(0, words.Count()));

            return result.ToString();
        }

        private static string getPrefix(int amount)
        {
            StringBuilder result = new StringBuilder();
            for (int i = 0; i < amount; i++)
            {
                result.Append(Vocabulary.prefix[rand.Next(0, Vocabulary.prefix.Count)]);
            }
            return result.ToString();
        }
    }
}
