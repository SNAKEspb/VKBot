using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MemeTextTranslator
{
    public class SentimentAnalyzer
    {
        static readonly char[] splitters = " «»,.! ?;:([\\]'\"¡¿)".ToCharArray();
        public static double getSentimentScore(string text)
        {
            //normalize and remove symbols \u0300-\u036f

            //text = text.Normalize(NormalizationForm.FormD);
            //  .replace(/[\u0300-\u036f] / g, '')
            //  .toLowerCase();
            //normalized.split(/[\s,.! ?;:([\]'"¡¿)/]+/).filter((x) => x);

            //split to separate words(tokenize) and stem
            var tokens = text.ToLowerInvariant()
                .Split(" «»,.! ?;:([\\]'\"¡¿)".ToCharArray())//split to separate words
                .Where(t => !string.IsNullOrWhiteSpace(t))//remove empty
                .Select(t => Porter.stemWord(t))//stam words
                //.Where(t => !SentimentVocabulary.stopWords.Contains(t))//remove "stop words"
                ;

            var numHits = 0;
            var score = calculateScore(tokens, out numHits);


            var result = new
            {
                score,
                numWords = tokens.Count(),
                numHits,
                average = score / tokens.Count(),
                type = "afinn",
                //locale = "ru",
                vote = score > 0 ? "positive" : score < 0 ? "negative" : "neutral"
            };

            return result.average;
        }

        private static double calculateScore(IEnumerable<string> tokens, out int numHits)
        {
            var score = 0.0;
            var negator = 1;
            numHits = 0;


            //optimization for empty negations list
            if (SentimentVocabulary.negations.Any())
            {
                foreach (var token in tokens)
                {
                    if (SentimentVocabulary.negations.Contains(token))
                    {
                        negator = -1;
                        numHits += 1;
                    }
                    else if (SentimentVocabulary.afinn.ContainsKey(token))
                    {
                        score += negator * SentimentVocabulary.afinn[token];
                        numHits += 1;
                    }
                }
            }
            else
            {

                foreach (var token in tokens)
                {
                    if (SentimentVocabulary.afinn.ContainsKey(token))
                    {
                        score += SentimentVocabulary.afinn[token];
                        numHits += 1;
                    }
                }
            }
            return score;
        }
    }
}
