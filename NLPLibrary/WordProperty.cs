using NLPLibrary.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Collections;

namespace NLPLibrary
{
    internal class WordPropertyList : IEnumerable<WordProperty>
    {
        private List<WordProperty> _words { get; set; }
        public int Count { get { return _words.Count(); } }
        public IEnumerable<string> invalidWords { get { return _words.Where(w => !w.isValid).Select(w => w.originalWord); } }
        public WordProperty this[int i]
        {
            get {
                if(i > this.Count - 1 && i < 0)
                {
                    return null;
                }
                return _words[i];
            }
            set {
                _words[i] = value;
                _words[i].index = i;
            }
        }
        public void Add(WordProperty word)
        {
            _words.Add(word);
            int lastIndex = this.Count - 1;
            _words.Last().index = lastIndex;
        }
        public WordPropertyList(string[] tokens, string[] partsOfSpeech, string originalData, IAcronymDetector acronymDetector)
        {
            _words = new List<WordProperty>();
            if (tokens.Length == partsOfSpeech.Length)
            {
                for (int i = 0; i < tokens.Length; i++)
                {
                    WordProperty word = new WordProperty(tokens[i], partsOfSpeech[i], this, originalData, acronymDetector);
                    this.Add(word);
                }
            }
            else
            {
                throw new ArgumentException("tokens and partsOfSpeech must be same length");
            }
        }
        public IEnumerator<WordProperty> GetEnumerator()
        {
            return _words.GetEnumerator();
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return _words.GetEnumerator();
        }
    }
    /// <summary>
    /// TAKEN FROM PENN TREEBANK TAGSET http://erwinkomen.ruhosting.nl/eng/2014_Longdale-Labels.htm
    ///CC Coordinating conjunction
    /// CD Cardinal number
    /// DT Determiner
    /// EX Existential there
    /// FW Foreign word
    /// IN Preposition or subordinating conjunction
    /// JJ Adjective
    /// JJR Adjective, comparative
    /// JJS Adjective, superlative
    /// LS List item marker
    /// MD Modal
    /// NN Noun, singular or mass
    /// NNS Noun, plural
    /// NNP Proper noun, singular
    /// NNPS Proper noun, plural
    /// PDT Predeterminer
    /// POS Possessive ending
    /// PRP Personal pronoun
    /// PRP$ Possessive pronoun
    /// RB Adverb
    /// RBR Adverb, comparative
    /// RBS Adverb, superlative
    /// RP Particle
    /// SYM Symbol
    /// TO to
    /// UH Interjection
    /// VB Verb, base form
    /// VBD Verb, past tense
    /// VBG Verb, gerund or present participle
    /// VBN Verb, past participle
    /// VBP Verb, non­3rd person singular present
    /// VBZ Verb, 3rd person singular present
    /// WDT Wh­determiner
    /// WP Wh­pronoun
    /// WP$ Possessive wh­pronoun
    /// WRB Wh­adverb
    /// -LRB- Left Parentheses
    /// -RRB- Right Parentheses
    /// </summary>
    internal class WordProperty
    {
        private WordPropertyList _list;
        public int index = -1;
        public WordProperty prevWord { get { return _list[index - 1]; } }
        public WordProperty nextWord { get { return _list[index + 1]; } }
        public bool isValid = true;
        public bool isAcronym = false;
        public bool isSpaced = false; //defines whether or not a word originally had a space before it
        public string originalWord;
        public string word;
        public POS partOfSpeech;
        public WordProperty(string word, string partOfSpeech, WordPropertyList list, string originalData, IAcronymDetector acronymDetector)
        {
            this.originalWord = word;
            this.word = word;
            this.partOfSpeech = POS.GetValue(partOfSpeech, word);
            this.isSpaced = CheckIfSpaced(word, originalData, list);
            this._list = list;
            if (this.word.Length > 1 && this.ContainsUppercaseLetterOrSymbol() && this.PotentialAcronym())//&& word.ContainsUppercaseLetterOrSymbol())
            {
                this.isAcronym = acronymDetector.IsAcronym(this.word);
                this.isValid = this.isAcronym;
            }

            if (this.partOfSpeech == null)
            {
                throw new ArgumentNullException("Invalid part of speech");
            }
        }
        private bool CheckIfSpaced(string word, string data, WordPropertyList list)
        {
            //remove already processed words
            foreach (WordProperty wp in list)
            {
                if(wp.word == word)
                {
                    int startIdx = data.IndexOf(word);
                    int endIdx = startIdx + word.Length;
                    data = data.Substring(0, startIdx) + data.Substring(endIdx, data.Length - endIdx);
                }
            }
            int potentialSpaceIndex = data.IndexOf(word) - 1;
            if(potentialSpaceIndex >= 0)
            {
                return data[potentialSpaceIndex] == ' ';
            }
            else
            {
                return false;
            }
        }
        public bool ShouldCapitalize()/*WordProperty prevWordProperty*/
        {
            if (!this.word.ContainsLetters()) { return false; }
            string testWord = this.word.ToLower();
            if (this.prevWord.word.Contains("/"))
            {
                return Char.IsUpper(this.word.First());
            }
            if (IsPartOfSpeech(POS.Particle, POS.To, POS.CoordinatingConjunction, POS.PossessiveEnding))
            {
                return false;
            }
            else if (IsPartOfSpeech(POS.Preposition)) // if a preposition, only capitalize if more than 3 letters
            {
                return (testWord.Length > 3) ? true : false;
            }
            else if (IsPartOfSpeech(POS.Determiner))
            {
                return !(testWord == "a" | testWord == "an" | testWord == "the") ? true : false;
            }
            else
            {
                return true;
            }
        }
        public bool ShouldAddSpace()//WordProperty wordProperty, WordProperty prevWordProperty)
        {
            bool addSpace = false;
            if (this.IsPartOfSpeech(POS.PossessiveEnding)) // Posessive endings like 's
            {
                if (this.word.ContainsLetters()) // Likely the end of word, do not add space
                {
                    addSpace = false;
                }
                else // Misidentified most likely, add space
                {
                    addSpace = true;
                }
            }
            //else if (this.word.ContainsLetters())
            //{
            //    addSpace = true;
            //}
            //else
            //{
            //    if (this.IsPartOfSpeech(POS.MidSentencePunctuation))
            //    {
            //        if (this.word == ":")
            //        {
            //            addSpace = this.isSpaced; //If colon, let original spacing determine
            //        }
            //        else if (this.word == ";")
            //        {
            //            addSpace = false; //Semi-colons should never be spaced
            //        }
            //        else //If hyphen (most likely case to trigger here)
            //        {
            //            if (!this.isSpaced)
            //            {
            //                addSpace = this.nextWord.isSpaced; //If hypen isn't spaced, let next words original spacing determine if this word should be spaced
            //            }
            //            else
            //            {
            //                addSpace = this.isSpaced; //Allow spacing if user originally spaced the hypen
            //            }
            //        }
            //    }
            //}
            else
            {
                if (this.word.ContainsLetters() || !this.IsPartOfSpeech(POS.MidSentencePunctuation, POS.Period, POS.Comma))//!(wordProperty.partOfSpeech == ":" || wordProperty.partOfSpeech == "." || wordProperty.partOfSpeech == ","))
                {
                    if (!this.prevWord.word.StartsWith("/"))//|| !prevWordProperty.IsPartOfSpeech(POS.)
                    {
                        if (this.word.ContainsLetters() || this.IsPartOfSpeech(POS.CardinalNumber, POS.LeftParenthesis, POS.OpenQuote, POS.Date))
                        {
                            if(this.prevWord.word == "#")
                            {
                                addSpace = false;
                            }
                            else if (!this.prevWord.IsPartOfSpeech(POS.LeftParenthesis, POS.OpenQuote))
                            {
                                addSpace = true;
                            }
                            else
                            {
                                addSpace = this.isSpaced;
                            }
                        }
                        else if (this.partOfSpeech.Value == "#" && this.nextWord.IsPartOfSpeech(POS.CardinalNumber))
                        {
                            addSpace = this.isSpaced;
                        }
                    }
                }
                else if (this.IsPartOfSpeech(POS.MidSentencePunctuation))
                {
                    if (this.word == ":")
                    {
                        addSpace = this.isSpaced; //If colon, let original spacing determine
                    }
                    else if (this.word == ";")
                    {
                        addSpace = false; //Semi-colons should never be spaced
                    }
                    else //If hyphen (most likely case to trigger here)
                    {
                        if (!this.isSpaced)
                        {
                            addSpace = this.nextWord.isSpaced; //If hypen isn't spaced, let next words original spacing determine if this word should be spaced
                        }
                        else
                        {
                            addSpace = this.isSpaced; //Allow spacing if user originally spaced the hypen
                        }
                    }
                }
            }
            return addSpace;
        }

        /// <summary>
        /// Combines array of strings into mixed case.
        /// Rule:
        /// If first element in list, first char MUST be capitalized
        /// If not first element, first char matches what user entered
        /// </summary>
        /// <param name="strList"></param>
        /// <returns></returns>
        public string CombineToMixedCase(params string[] strList)
        {
            string result = string.Empty;
            for (int i = 0; i < strList.Length; i++)
            {
                string str = strList[i];
                if (!string.IsNullOrEmpty(str))
                {
                    char firstChar = str.First();
                    if (i == 0)
                    {
                        result += firstChar.ToString().ToUpper() + str.Substring(1).ToLower();
                    }
                    else
                    {
                        if (Char.IsUpper(firstChar))
                        {
                            result += firstChar.ToString().ToUpper() + str.Substring(1).ToLower();
                        }
                        else
                        {
                            result += str.ToLower();
                        }
                    }
                }
            }
            return result;
        }
        public string ToUpperFirstChar()
        {
            if (this.isAcronym) { return this.word; }
            string newWord = string.Empty;
            string[] matches = Regex.Split(this.word, @"(\W+)")     //Split by non alphanumeric characters
                .SelectMany(w => Regex.Split(w, @"(?<=Mc|Mac|De|Da|Du)"))   //Split by common surname exceptions (ie MacDonald, McDaniel)
                .SelectMany(w => Regex.Split(w, @"(\d+)")).ToArray();       //Split by numeric (Learn2Drive)

            newWord = CombineToMixedCase(matches);
            return newWord;
        }

        public bool IsPartOfSpeech(params POS[] tests)
        {
            foreach (var test in tests)
            {
                if (this.partOfSpeech.Value == test.Value)
                {
                    return true;
                }
            }
            return false;
        }
        public bool PotentialAcronym()
        {
            return ToUpperFirstChar() != word;
        }
        public bool ContainsUppercaseLetterOrSymbol()
        {
            if (word.ContainsLetters())
            {
                for (int i = 1; i < word.Length; i++) //skip first character
                {
                    if ((Char.IsUpper(word[i]) || !Char.IsLetterOrDigit(word[i])))
                        return true;
                }
                return false;
            }
            else
            {
                return false;
            }
        }
    }
}
