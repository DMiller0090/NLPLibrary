using NLPLibrary.Extensions;

namespace NLPLibrary
{
    public class POS
    {
        private POS(string value) { Value = value; }
        public string Value { get; set; }
        public static POS CoordinatingConjunction { get { return new POS("CC"); } }
        public static POS CardinalNumber { get { return new POS("CD"); } }
        public static POS Determiner { get { return new POS("DT"); } }
        public static POS ExistentialThere { get { return new POS("EX"); } }
        public static POS ForeignWord { get { return new POS("FW"); } }
        public static POS Preposition { get { return new POS("IN"); } }
        public static POS SubordinatingConjunction { get { return new POS("IN"); } }
        public static POS Adjective { get { return new POS("JJ"); } }
        public static POS AdjectiveComparative { get { return new POS("JJR"); } }
        public static POS ListItemMarker { get { return new POS("LS"); } }
        public static POS Modal { get { return new POS("MD"); } }
        public static POS NounSingular { get { return new POS("NN"); } }
        public static POS NounPlural { get { return new POS("NNS"); } }
        public static POS ProperNounSingular { get { return new POS("NNP"); } }
        public static POS ProperNounPlural { get { return new POS("NNPS"); } }
        public static POS Predeterminer { get { return new POS("PDT"); } }
        public static POS PossessiveEnding { get { return new POS("POS"); } }
        public static POS PersonalPronoun { get { return new POS("PRP"); } }
        public static POS PossessivePronoun { get { return new POS("PRP$"); } }
        public static POS Adverb { get { return new POS("RB"); } }
        public static POS AdverbComparative { get { return new POS("RBR"); } }
        public static POS AdverbSuperlative { get { return new POS("RBS"); } }
        public static POS Particle { get { return new POS("RP"); } }
        public static POS Symbol { get { return new POS("SYM"); } }
        public static POS To { get { return new POS("TO"); } }
        public static POS Interjection { get { return new POS("UH"); } }
        public static POS Verb { get { return new POS("VB"); } }
        public static POS VerbPastTense { get { return new POS("VBD"); } }
        public static POS VerbGerundOrPresentParticiple { get { return new POS("VBG"); } }
        public static POS VerbPastParticiple { get { return new POS("VBN"); } }
        public static POS VerbNon3rdPersonSingularPresent { get { return new POS("VBP"); } }
        public static POS Verb3rdPersonSingularPresent { get { return new POS("VBZ"); } }
        public static POS Whdeterminer { get { return new POS("WDT"); } }
        public static POS Whpronoun { get { return new POS("WP"); } }
        public static POS PossessiveWhpronoun { get { return new POS("WP$"); } }
        public static POS Whadverb { get { return new POS("WRB"); } }
        public static POS LeftParenthesis { get { return new POS("-LRB-"); } }
        public static POS RightParenthesis { get { return new POS("-RRB-"); } }
        public static POS OpenQuote { get { return new POS("``"); } }
        public static POS EndQuote { get { return new POS("''"); } }
        public static POS MidSentencePunctuation { get { return new POS(":"); } }
        public static POS Period { get { return new POS("."); } }
        public static POS Comma { get { return new POS(","); } }
        public static POS Date { get { return new POS("DATE"); } }
 
        public static POS GetValue(string pos)
        {
            //var t = typeof(POS);
            //var methods = t.GetMethods().ToList().Where(x => x.IsStatic && x.ReturnType == typeof(POS) && x.GetParameters().Count() == 0).ToArray();
            //for (int i = 0; i < methods.Length; i++)
            //{
            //    var method = methods[i];
            //    POS p = (POS)method.Invoke(null, null);
            //    return p;
            //}
            return new POS(pos);
        }
        public static POS GetValue(string pos, string word)
        {
            POS partOfSpeech = POS.GetValue(pos);
            if (word == "'" && partOfSpeech.Value != POS.EndQuote.Value) //NLP occasionally assigns "'" as possesive, reassign as open quote
            {
                partOfSpeech = POS.OpenQuote;
            }
            else if (partOfSpeech.Value == Period.Value && word.Length > 1)
            {
                if (word.IsDate())     //OpenNLP assigns date values as period. Need to check if date for correct spacing.
                {
                    partOfSpeech = POS.Date;
                }
            }
            return partOfSpeech;
        }
    }
}
