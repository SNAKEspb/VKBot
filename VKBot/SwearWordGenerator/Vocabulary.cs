using System.Collections.Generic;

namespace SwearWordGenerator
{
    class Vocabulary
    {
        public class Word
        {
            public Type type;
            public Sex sex;
            public string root;
            public Dictionary<Case, string> cases;

            public Word(Type type, Sex sex, string root, string i, string r, string d, string v, string t, string p)
            {
                this.type = type;
                this.sex = sex;
                this.root = root;
                this.cases = new Dictionary<Case, string>() {
                    { Case.I, i },
                    { Case.R, r },
                    { Case.D, d },
                    { Case.V, v },
                    { Case.T, t },
                    { Case.P, p },
                };
            }

            public string getWord(Case wordCase = Case.I)
            {
                return root + cases[wordCase];
            }
        }

        public class Noun : Word
        {
            public Noun(Sex sex, string root, string i, string r, string d, string v, string t, string p) : base(Type.Noun, sex, root, i, r, d, v, t, p)
            {
            }
        }

        public class Adjective : Word
        {
            public Adjective(Sex sex, string root, string i, string r, string d, string v, string t, string p) : base(Type.Adjective, sex, root, i, r, d, v, t, p)
            {
            }
        }

        public class Verb : Word
        {
            public Verb(string root) : base(Type.Verb, Sex.N, root, null, null, null, null, null, null)
            {
            }
        }

        public static List<string> prefix = new List<string>()
        {
            "блядско",
            "бляхо",
            "вафле",
            "гондо",
            "дерьмо",
            "долбо",
            "ебли",
            "ебло",
            "зло",
            "косо",
            "лохо",
            "манда",
            "мудло",
            "мудо",
            "недо",
            "пере",
            "пидо",
            "пиздо",
            "плеше",
            "под",
            "подлихо",
            "подлохо",
            "свино",
            "срако",
            "страхо",
            "суче",
            "трое",
            "херо",
            "хуе",
            "хуйло",
        };

        public static List<Noun> nounsMale = new List<Noun>()
        {
            new Noun(Sex.M, "глаз", "", "a", "у", "", "ом" , "е"),
            new Noun(Sex.M, "ебальник", "", "a", "у", "", "ом" , "е"),
            new Noun(Sex.M, "ебар", "ь", "я", "ю", "я", "ём" , "е"),
            new Noun(Sex.M, "еблан", "", "a", "у", "а", "ом" , "е"),
            new Noun(Sex.M, "лох", "", "a", "у", "а", "ом" , "е"),
            new Noun(Sex.M, "мудак", "", "a", "у", "а", "ом" , "е"),
            new Noun(Sex.M, "отстойник", "", "a", "у", "", "ом" , "е"),
            new Noun(Sex.M, "пидор", "", "a", "у", "а", "ом" , "е"),
            new Noun(Sex.M, "пидорас", "", "a", "у", "а", "ом" , "е"),
            new Noun(Sex.M, "сран", "", "a", "у", "а", "ом" , "е"),
            new Noun(Sex.M, "уебан", "", "a", "у", "", "ом" , "е"),
            new Noun(Sex.M, "фан", "", "a", "у", "а", "ом" , "е"),
            new Noun(Sex.M, "хер", "", "a", "у", "", "ом" , "е"),
            new Noun(Sex.M, "ху", "й", "я", "ю", "й", "ем" , "е"),
        };

        public static List<Noun> nounsFemale = new List<Noun>()
        {
            new Noun(Sex.F, "блядин", "а" , "ой" , "ой" , "у" , "ой" , "ой"),
            new Noun(Sex.F, "бляд", "ь", "и", "и", "ь", "ью" , "и"),
            new Noun(Sex.F, "залуп", "а", "ы", "е", "у", "ой" , "е"),
            new Noun(Sex.F, "манд", "а", "ы", "е", "у", "ой" , "е"),
            new Noun(Sex.F, "мандавох", "а", "и", "е", "у", "ой" , "е"),
            new Noun(Sex.F, "мандавошин", "а", "ой", "ой", "у", "ой" , "ой"),
            new Noun(Sex.F, "морда", "а", "ы", "е", "у", "ой" , "е"),
            new Noun(Sex.F, "пизд", "а", "ы", "е", "у", "ой" , "е"),
            new Noun(Sex.F, "проушин", "а", "ы", "е", "у", "ой" , "е"),
            new Noun(Sex.F, "рожищ", "а", "и", "е", "у", "ой" , "е"),
            new Noun(Sex.F, "срак", "а", "и", "е", "у", "ой" , "е"),
            new Noun(Sex.F, "уебин", "а" , "ой" , "ой" , "у" , "ой" , "ой"),
            new Noun(Sex.F, "фанк", "а", "и", "е", "у", "ой" , "е"),
            new Noun(Sex.F, "хуетень", "ь", "я", "ю", "я", "ем" , "е"),
            new Noun(Sex.F, "хуяра",  "а", "ы", "е", "у", "ой" , "е"),
        };

        public static List<Noun> nounsNeuter = new List<Noun>()
        {
            new Noun(Sex.F, "блядищ", "е" , "а" , "у" , "е" , "ем" , "е"),
            new Noun(Sex.F, "видл", "о", "а", "у", "о", "ом" , "е"),
            new Noun(Sex.F, "еблищ", "е" , "а" , "у" , "е" , "ем" , "е"),
            new Noun(Sex.F, "ебл", "о", "а", "у", "о", "ом" , "е"),
            new Noun(Sex.F, "залупищ", "а", "и", "е", "у", "ой" , "е"),//
            new Noun(Sex.F, "мондил", "о", "а", "у", "о", "ом" , "е"),
            new Noun(Sex.F, "мудачищ", "е" , "а" , "у" , "е" , "ем" , "е"),
            new Noun(Sex.F, "мудищ", "е" , "а" , "у" , "е" , "ем" , "е"),
            new Noun(Sex.F, "пиздищ", "е" , "а" , "у" , "е" , "ем" , "е"),
            new Noun(Sex.F, "уебищ", "е" , "а" , "у" , "е" , "ем" , "е"),
            new Noun(Sex.F, "фанищ", "е" , "а" , "у" , "е" , "ем" , "е"),
            new Noun(Sex.F, "хуищ", "е" , "а" , "у" , "е" , "ем" , "е"),
            new Noun(Sex.F, "хуярищ", "е" , "а" , "у" , "е" , "ем" , "е"),
        };

        public static List<Adjective> adjectivesMale = new List<Adjective>()
        {
            new Adjective(Sex.M, "блядск", "ий", "ого", "ому", "ий", "им" , "ом"),
            new Adjective(Sex.M, "вафельн", "ый", "ого", "ому", "ый", "ым" , "ом"),
            new Adjective(Sex.M, "гондонн", "ый", "ого", "ому", "ый", "ым" , "ом"),
            new Adjective(Sex.M, "двупизд", "ый", "ого", "ому", "ый", "ым" , "ом"),
            new Adjective(Sex.M, "ебан", "ый", "ого", "ому", "ый", "ым" , "ом"),
            new Adjective(Sex.M, "еблив", "ый", "ого", "ому", "ый", "ым" , "ом"),
            new Adjective(Sex.M, "ебуч", "ий", "ого", "ому", "ий", "им" , "ом"),
            new Adjective(Sex.M, "залупн", "ый", "ого", "ому", "ый", "ым" , "ом"),
            new Adjective(Sex.M, "кос", "ой", "ого", "ому", "ого", "ым" , "ом"),
            new Adjective(Sex.M, "ногий", "ий", "ого", "ому", "ий", "им" , "ом"),
            new Adjective(Sex.M, "отстойн", "ый", "ого", "ому", "ый", "ым" , "ом"),
            new Adjective(Sex.M, "отхуяренн", "ый", "ого", "ому", "ый", "ым" , "ом"),
            new Adjective(Sex.M, "рог", "ий", "ого", "ому", "ий", "им" , "ом"),
            new Adjective(Sex.M, "суч", "ий", "его", "ему", "ий", "им" , "ем"),
        };

        public static List<Adjective> adjectivesFemale = new List<Adjective>
        {
            new Adjective(Sex.F, "блядск", "ая", "ой", "ой", "ую", "ой" , "ой"),
            new Adjective(Sex.F, "вафельн", "ая", "ой", "ой", "ую", "ой" , "ой"),
            new Adjective(Sex.F, "гондонн", "ая", "ой", "ой", "ую", "ой" , "ой"),
            new Adjective(Sex.F, "двупизд", "ая", "ой", "ой", "ую", "ой" , "ой"),
            new Adjective(Sex.F, "ебан", "ая", "ой", "ой", "ую", "ой" , "ой"),
            new Adjective(Sex.F, "еблив", "ая", "ой", "ой", "ую", "ой" , "ой"),
            new Adjective(Sex.F, "ебуч", "ая", "ей", "ей", "ую", "ей" , "ей"),
            new Adjective(Sex.F, "залупн", "ая", "ой", "ой", "ую", "ой" , "ой"),
            new Adjective(Sex.F, "кос", "ая", "ой", "ой", "ую", "ой" , "ой"),
            new Adjective(Sex.F, "ног","ая", "ой", "ой", "ую", "ой" , "ой"),
            new Adjective(Sex.F, "отстойн", "ая", "ой", "ой", "ую", "ой" , "ой"),
            new Adjective(Sex.F, "отхуяренн", "ая", "ой", "ой", "ую", "ой" , "ой"),
            new Adjective(Sex.F, "рог", "ая", "ой", "ой", "ую", "ой" , "ой"),
            new Adjective(Sex.F, "сран", "ая", "ой", "ой", "ую", "ой" , "ой"),
            new Adjective(Sex.F, "сучь", "я", "ей", "ей", "ю", "ей" , "ей"),
        };

        public static List<Adjective> adjectivesNeuter = new List<Adjective>
        {
            new Adjective(Sex.F, "блядск", "ое", "ого", "ому", "ое", "им" , "ом"),
            new Adjective(Sex.F, "вафельн", "ое", "ого", "ому", "ое", "ым" , "ом"),
            new Adjective(Sex.F, "гондонн", "ое", "ого", "ому", "ое", "ым" , "ом"),
            new Adjective(Sex.F, "двупизд", "ое", "ого", "ому", "ое", "ым" , "ом"),
            new Adjective(Sex.F, "ебан", "ое", "ого", "ому", "ое", "ым" , "ом"),
            new Adjective(Sex.F, "еблив", "ое", "ого", "ому", "ое", "ым" , "ом"),
            new Adjective(Sex.F, "ебуч", "ее", "его", "ему", "ее", "им" , "ем"),
            new Adjective(Sex.F, "залупн", "ое", "ого", "ому", "ое", "ым" , "ом"),
            new Adjective(Sex.F, "кос", "ое", "ого", "ому", "ое", "ым" , "ом"),
            new Adjective(Sex.F, "ног", "ое", "ого", "ому", "ое", "им" , "ом"),
            new Adjective(Sex.F, "отстойн", "ое", "ого", "ому", "ое", "ым" , "ом"),
            new Adjective(Sex.F, "отхуяренн", "ое", "ого", "ому", "ое", "ым" , "ом"),
            new Adjective(Sex.F, "рог", "ое", "ого", "ому", "ое", "им" , "ом"),
            new Adjective(Sex.F, "сран", "ое", "ого", "ому", "ое", "ым" , "ом"),
            new Adjective(Sex.F, "сучь", "е", "его", "ему", "е", "им" , "ем"),
        };

        public static List<Verb> verbs = new List<Verb>()
        {
            new Verb("еби"),
            new Verb("заеби"),
            new Verb("залупи"),
            new Verb("запизди"),
            new Verb("захуярь"),
            new Verb("захуячь"),
            new Verb("медузь"),
            new Verb("пизди"),
            new Verb("ухуярь"),
            new Verb("ухуячь"),
            new Verb("хуярь"),
            new Verb("хуячь"),
        };

        public static Dictionary<Type, Dictionary<Sex, IEnumerable<Word>>> words = new Dictionary<Type, Dictionary<Sex, IEnumerable<Word>>>()
        {
            { Type.Adjective, new Dictionary<Sex, IEnumerable<Word>>() { { Sex.M, adjectivesMale }, { Sex.F, adjectivesFemale }, { Sex.N, adjectivesNeuter } } },
            { Type.Noun, new Dictionary<Sex, IEnumerable<Word>>() { { Sex.M, nounsMale }, { Sex.F, nounsFemale }, { Sex.N, nounsNeuter } } },
            { Type.Verb, new Dictionary<Sex, IEnumerable<Word>>() { { Sex.N, verbs } } },
        };
    }
}
