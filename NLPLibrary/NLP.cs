using OpenNLP.Tools.Tokenize;
using OpenNLP.Tools.PosTagger;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.IO;
using System;
using System.Text.RegularExpressions;
using NLPLibrary.Extensions;

namespace NLPLibrary
{
    public class NLP : IProperCaseFormatter
    {    
        private EnglishRuleBasedTokenizer _tokenizer;
        private EnglishMaximumEntropyPosTagger _posTagger;
        private WordPropertyList _words;
        
        private readonly IAcronymDetector _acronymDetector;
        public IEnumerable<string> invalidWords { get { return _words.invalidWords; } }

        public NLP(IAcronymDetector acronymDetector)
        {
            if(acronymDetector == null)
            {
                throw new ArgumentNullException("acronymDetector");
            }
            _acronymDetector = acronymDetector;
            IEmbeddedResource posFile = new PosFile();
            IEmbeddedResource tagDict = new TagDict();
            _tokenizer = new EnglishRuleBasedTokenizer(false);
            _posTagger = new EnglishMaximumEntropyPosTagger(posFile.resourceFilePath, tagDict.resourceFilePath);
        }
        public string ToTitleCase(string data)
        {
            var tokens = _tokenizer.Tokenize(data);
            var partsOfSpeech = _posTagger.Tag(tokens);
            _words = new WordPropertyList(tokens, partsOfSpeech, data, _acronymDetector);
            WordProperty wordProperty = _words.FirstOrDefault();
            wordProperty.word = wordProperty.ToUpperFirstChar();
            string formattedString = wordProperty.word;
            int lastWordIndex = LastWordIndex();
            for (int i = 1; i < _words.Count; i++)
            {
                wordProperty = _words[i];
                if (wordProperty.ShouldCapitalize())//_words[i-1]))
                {
                    wordProperty.word = wordProperty.ToUpperFirstChar();
                }
                else
                {
                    wordProperty.word = wordProperty.word.ToLower();
                }
                if (wordProperty.ShouldAddSpace())//wordProperty, _words[i-1]))
                {
                    formattedString += " ";
                }
                formattedString += wordProperty.word;
            }
            return formattedString;
        }
        /// <summary>
        /// Returns the index of the last valid word in the array of words.
        /// </summary>
        /// <returns></returns>
        private int LastWordIndex()
        {
            for (int i = _words.Count - 1; i >= 0; i--)
            {
                if (_words[i].word.ContainsLetters() && !_words[i].word.StartsWith("'"))
                {
                    return i;
                }
            }
            return 0;
        }

        public void Dispose()
        {
            _acronymDetector.Dispose();
        }
    }
}