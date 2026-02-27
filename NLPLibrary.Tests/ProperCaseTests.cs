using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NLPLibrary.Tests
{

    [TestFixture]
    public class ProperCaseTests : IoCSupportedTest<BusinessLogicModule>
    {
        private IProperCaseFormatter formatter;
        [SetUp]
        public void Init()
        {
            try
            {
                this.formatter = Resolve<IProperCaseFormatter>();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error");
                var error = ex.InnerException.Message;
                throw;
            }        
        }
        [Test]
        public void Acronyms()
        {
            using (formatter)
            {
                var result = formatter.ToTitleCase("Robbie J. Harris III");
                Assert.AreEqual("Robbie J. Harris III", result);
                Assert.That(AccurateInvalidWords(result, formatter.invalidWords));
            }
        }
        [Test]
        public void HandlesPossesives()
        {
            using (formatter)
            {
                var result = formatter.ToTitleCase("Test's wonderful thing!");
                Assert.AreEqual("Test's Wonderful Thing!", result);
                Assert.That(AccurateInvalidWords(result, formatter.invalidWords));

                result = formatter.ToTitleCase("That test should work's");
                Assert.AreEqual("That Test Should Work's", result);
                Assert.That(AccurateInvalidWords(result, formatter.invalidWords));
            }
        }
        [Test]
        public void DoesNotCapitalizeAfterSymbolIfLowercase()
        {
            using (formatter)
            {
                var result = formatter.ToTitleCase("C-moore Entertainment, Inc.");
                Assert.That(result == "C-moore Entertainment, Inc.");
                Assert.That(AccurateInvalidWords(result, formatter.invalidWords));
            }
        }
        [Test]
        public void DoesCapitalizeAfterSymbolIfUppercase()
        {
            using (formatter)
            {
                var result = formatter.ToTitleCase("C-MOOre Entertainment, Inc.");
                Assert.AreEqual("C-Moore Entertainment, Inc.", result);
                Assert.That(AccurateInvalidWords(result, formatter.invalidWords));
            }
        }
        [Test]
        public void ProperCapitalizationAfterSymbolAndSurname()
        {
            using (formatter)
            {
                var result = formatter.ToTitleCase("C-MOOre MacDonald");
                Assert.AreEqual("C-Moore MacDonald", result);
                Assert.That(AccurateInvalidWords(result, formatter.invalidWords));
            }
        }
        [Test]
        public void HandlesNumericValues()
        {
            using (formatter)
            {
                var result = formatter.ToTitleCase("the nest at the Eagles 741 LLC");
                Assert.That(result == "The Nest at the Eagles 741 LLC");
                Assert.That(AccurateInvalidWords(result, formatter.invalidWords));

                result = formatter.ToTitleCase("Marilyn I. Mumm Declaration of Trust Uad 9/3/04");
                Assert.AreEqual("Marilyn I. Mumm Declaration of Trust Uad 9/3/04", result);
                Assert.That(AccurateInvalidWords(result, formatter.invalidWords));

                result = formatter.ToTitleCase("Marilyn I. Mumm 9/3/04 Declaration of Trust Uad ");
                Assert.AreEqual("Marilyn I. Mumm 9/3/04 Declaration of Trust Uad", result);
                Assert.That(AccurateInvalidWords(result, formatter.invalidWords));

                result = formatter.ToTitleCase("9/3/04 Marilyn I. Mumm Declaration of Trust Uad ");
                Assert.AreEqual("9/3/04 Marilyn I. Mumm Declaration of Trust Uad", result);
                Assert.That(AccurateInvalidWords(result, formatter.invalidWords));

                result = formatter.ToTitleCase("Marilyn I. Mumm Declaration of Trust Uad 9/3/04 ");
                Assert.AreEqual("Marilyn I. Mumm Declaration of Trust Uad 9/3/04", result);
                Assert.That(AccurateInvalidWords(result, formatter.invalidWords));
            }
        }
        [Test]
        public void DoesNotAddSpaceToFirstNumeric()
        {
            using (formatter)
            {
                var result = formatter.ToTitleCase(" 7334 the nest at the Eagles LLC");
                Assert.AreEqual("7334 the Nest at the Eagles LLC", result);
                Assert.That(AccurateInvalidWords(result, formatter.invalidWords));
            }
        }
        [Test]
        public void DoesNotUncapitalizeOMalia()
        {
            using (formatter)
            {
                var result = formatter.ToTitleCase("O'Malia's Hard Surface Restoration");
                Assert.AreEqual("O'Malia's Hard Surface Restoration", result);
            
                result = formatter.ToTitleCase("O'malia's Hard Surface Restoration");
                Assert.AreEqual("O'malia's Hard Surface Restoration", result);
                Assert.That(AccurateInvalidWords(result, formatter.invalidWords));
            }
        }
        [Test]
        public void UncapitalizesIfAcronymWithinWord()
        {
            using (formatter)
            {
                var result = formatter.ToTitleCase("ASDtest name TestASD");
                Assert.That(result == "Asdtest Name Testasd");
                Assert.That(AccurateInvalidWords(result, formatter.invalidWords));
            }
        }
        [Test]
        public void HandlesAmpersand()
        {
            using (formatter)
            {
                var result = formatter.ToTitleCase("Stephen H. & Diane P. Rodriguez");
                Assert.That(result == "Stephen H. & Diane P. Rodriguez");
                Assert.That(AccurateInvalidWords(result, formatter.invalidWords));
            }
        }
        [Test]
        public void DetectsInvalidAllCaps()
        {
            using (formatter)
            {
                var result = formatter.ToTitleCase("STATE FINANCE");
                Assert.That(formatter.invalidWords.SequenceEqual(new List<string> { "STATE", "FINANCE" }));
                Assert.That(AccurateInvalidWords(result, formatter.invalidWords));
            }
        }
        [Test]
        public void ChecksForAcronymsWithDashes()
        {
            using (formatter)
            {
                var result = formatter.ToTitleCase("4-D Farms, Inc.");
                Assert.AreEqual("4-D Farms, Inc.", result);
                Assert.That(AccurateInvalidWords(result, formatter.invalidWords));

                result = formatter.ToTitleCase("Mariselle Melendez-Jordan & Christopher N. Jordan");
                Assert.AreEqual("Mariselle Melendez-Jordan & Christopher N. Jordan", result);
                Assert.That(AccurateInvalidWords(result, formatter.invalidWords));

                result = formatter.ToTitleCase("Mariselle & Christopher N. Jordan Melendez-Jordan");
                Assert.AreEqual("Mariselle & Christopher N. Jordan Melendez-Jordan", result);
                Assert.That(AccurateInvalidWords(result, formatter.invalidWords));
            }
        }
        [Test]
        public void ChecksForNumericAcronyms()
        {
            using (formatter)
            {
                var result = formatter.ToTitleCase("Label Technology 401K Plan");
                Assert.That(result == "Label Technology 401K Plan");
                Assert.That(AccurateInvalidWords(result, formatter.invalidWords));
            }
        }      
        [Test]
        public void HandlesParentheses()
        {
            using (formatter)
            {
                var result = formatter.ToTitleCase("Alles, Mary Frances (Estate)");
                Assert.AreEqual("Alles, Mary Frances (Estate)", result);
                Assert.That(AccurateInvalidWords(result, formatter.invalidWords));
            }
        }
        [Test]
        public void AddsSpaceToLeftParentheses()
        {
            using (formatter)
            {
                var result = formatter.ToTitleCase("Label Technology 401(K) Plan");
                Assert.AreEqual("Label Technology 401 (K) Plan", result); 
                Assert.That(AccurateInvalidWords(result, formatter.invalidWords));
            }
        }
        [Test]
        public void DoesNotChangeFirstLetterAfterMc()
        {
            using (formatter)
            {
                var result = formatter.ToTitleCase("McDaniel, James R. McDaniel Family Trust");
                Assert.AreEqual("McDaniel, James R. McDaniel Family Trust", result);
                Assert.That(AccurateInvalidWords(result, formatter.invalidWords));

                result = formatter.ToTitleCase("MacDONald, James R. DePOUNte Family Trust");
                Assert.AreEqual("MacDonald, James R. DePounte Family Trust", result);
                Assert.That(AccurateInvalidWords(result, formatter.invalidWords));

                result = formatter.ToTitleCase("Tom McKay");
                Assert.AreEqual("Tom McKay", result);
                Assert.That(AccurateInvalidWords(result, formatter.invalidWords));

                result = formatter.ToTitleCase("Macdonald, James");
                Assert.AreEqual("Macdonald, James", result);
                Assert.That(AccurateInvalidWords(result, formatter.invalidWords));
            }
        }
        [Test]
        public void AllowCapitalAfterForwardSlash()
        {
            using (formatter)
            {
                var result = formatter.ToTitleCase("Precast/Prestressed Concrete Institute of Illinois & Wisconsin");
                Assert.AreEqual("Precast/Prestressed Concrete Institute of Illinois & Wisconsin", result);
                Assert.That(AccurateInvalidWords(result, formatter.invalidWords));

                result = formatter.ToTitleCase("Elisabeth Adkins D/ B/ A Bosso's Grill");
                Assert.AreEqual("Elisabeth Adkins D/B/A Bosso's Grill", result);
                Assert.That(AccurateInvalidWords(result, formatter.invalidWords));

                result = formatter.ToTitleCase("Test/Thingy");
                Assert.AreEqual("Test/Thingy", result);
                Assert.That(AccurateInvalidWords(result, formatter.invalidWords));

                result = formatter.ToTitleCase("Mwr Investments LP/Water Street Partners LLC Daycare & Lot JV");
                Assert.AreEqual("Mwr Investments LP/Water Street Partners LLC Daycare & Lot JV", result);
                Assert.That(AccurateInvalidWords(result, formatter.invalidWords));
            }
        }
        [Test]
        public void AllowCapitalAfterDigit()
        {
            using (formatter)
            {
                var result = formatter.ToTitleCase("C-Design2Improve (MacDonald)");
                Assert.AreEqual("C-Design2Improve (MacDonald)", result);
                Assert.That(AccurateInvalidWords(result, formatter.invalidWords));

                result = formatter.ToTitleCase("Design277Improve");
                Assert.AreEqual("Design277Improve", result);
                Assert.That(AccurateInvalidWords(result, formatter.invalidWords));

                result = formatter.ToTitleCase("Design2-7-improve");
                Assert.AreEqual("Design2-7-improve", result);
                Assert.That(AccurateInvalidWords(result, formatter.invalidWords));

                result = formatter.ToTitleCase("123Test");
                Assert.AreEqual("123Test", result);
                Assert.That(AccurateInvalidWords(result, formatter.invalidWords));
            }
        }
        [Test]
        public void AllowPeriodsBetweenLetters()
        {
            using (formatter)
            {
                var result = formatter.ToTitleCase("D.M.E. Anson Farms Inc.");
                Assert.AreEqual("D.M.E. Anson Farms Inc.", result);
                Assert.That(AccurateInvalidWords(result, formatter.invalidWords));

                result = formatter.ToTitleCase("Crystal S. Oliver, O.D.P LLC");
                Assert.AreEqual("Crystal S. Oliver, O.D.P LLC", result);
                Assert.That(AccurateInvalidWords(result, formatter.invalidWords));
            }
        }
        [Test]
        public void MidWordCapitalIfAcronym()
        {
            using (formatter)
            {
                var result = formatter.ToTitleCase("iAPX");
                Assert.AreEqual("iAPX", result);
                Assert.That(AccurateInvalidWords(result, formatter.invalidWords));

                result = formatter.ToTitleCase("CPsychol");
                Assert.AreEqual("CPsychol", result);
                Assert.That(AccurateInvalidWords(result, formatter.invalidWords));

                result = formatter.ToTitleCase("I Really lIkE FedEx");
                Assert.AreEqual("I Really Like FedEx", result);
                Assert.That(AccurateInvalidWords(result, formatter.invalidWords));

                result = formatter.ToTitleCase("I am the TESTer");
                Assert.AreEqual("I Am the TESTer", result);
                Assert.AreEqual(0, formatter.invalidWords.Count());
                Assert.That(AccurateInvalidWords(result, formatter.invalidWords));

                result = formatter.ToTitleCase("I am the TESTeR");
                Assert.AreEqual("I Am the Tester", result);
                Assert.AreEqual(1, formatter.invalidWords.Count());
                Assert.That(AccurateInvalidWords(result, formatter.invalidWords));

                result = formatter.ToTitleCase("Reformmated Title Casing TESTEr");
                Assert.AreEqual(1, formatter.invalidWords.Count());
                Assert.That(AccurateInvalidWords(result, formatter.invalidWords));
            }
        }
        [Test]
        public void LowercaseCapitalIfContainsSymbol()
        {
            using (formatter)
            {
                var result = formatter.ToTitleCase("This acronym is lowercase a-z");
                Assert.AreEqual("This Acronym Is Lowercase a-z", result);
                Assert.AreEqual(0, formatter.invalidWords.Count());
                Assert.That(AccurateInvalidWords(result, formatter.invalidWords));
            }
        }
        [Test]
        public void DoesNotConcatAcronym()
        {
            using (formatter)
            {
                var result = formatter.ToTitleCase("LWA Enterprises LLC Dba the Jungle Island Test");
                Assert.AreEqual("LWA Enterprises LLC Dba the Jungle Island Test", result);
                Assert.That(AccurateInvalidWords(result, formatter.invalidWords));
            }
        }
        [Test]
        public void AllowsQuotes()
        {
            using (formatter)
            {
                var result = formatter.ToTitleCase(@"Magdaleno ""Max"" Barbosa");
                Assert.AreEqual(@"Magdaleno ""Max"" Barbosa", result);
                Assert.That(AccurateInvalidWords(result, formatter.invalidWords));

                result = formatter.ToTitleCase(@"Magdaleno 'Max' Barbosa");
                Assert.AreEqual(@"Magdaleno 'Max' Barbosa", result);
                Assert.That(AccurateInvalidWords(result, formatter.invalidWords));

                result = formatter.ToTitleCase(@"""Max"" Barbosa");
                Assert.AreEqual(@"""Max"" Barbosa", result);
                Assert.That(AccurateInvalidWords(result, formatter.invalidWords));
            }
        }
        [Test]
        public void InvalidWordCount()
        {
            using (formatter)
            {
                var result = formatter.ToTitleCase("Note Receivable - Dr. Crawford");
                Assert.AreEqual(formatter.invalidWords.Count(), 0);
                Assert.That(AccurateInvalidWords(result, formatter.invalidWords));
                Assert.AreEqual("Note Receivable - Dr. Crawford", result);
            }
        }
        [Test]
        public void AllowsSpacedHypens()
        {
            using (formatter)
            {
                var result = formatter.ToTitleCase("Bennett - Kluesner - Winkler");
                Assert.AreEqual("Bennett - Kluesner - Winkler", result);
                result = formatter.ToTitleCase("Bennett - Kluesner-Winkler");
                Assert.AreEqual("Bennett - Kluesner-Winkler", result);
                result = formatter.ToTitleCase("Bennett- Kluesner- Winkler");
                Assert.AreEqual("Bennett - Kluesner - Winkler", result);
            }
        }
        [Test]
        public void AllowsSpacedColons()
        {
            using (formatter)
            {
                var result = formatter.ToTitleCase("Deepthi S. Saxena MD PC DBA: Avant-Garde Medicine");
                Assert.AreEqual("Deepthi S. Saxena MD PC DBA: Avant-Garde Medicine", result);
                result = formatter.ToTitleCase("Deepthi S. Saxena MD PC DBA : Avant-Garde Medicine");
                Assert.AreEqual("Deepthi S. Saxena MD PC DBA : Avant-Garde Medicine", result);
                result = formatter.ToTitleCase("Deepthi S. Saxena MD PC DBA; Avant-Garde Medicine");
                Assert.AreEqual("Deepthi S. Saxena MD PC DBA; Avant-Garde Medicine", result);
                result = formatter.ToTitleCase("Deepthi S. Saxena MD PC DBA ; Avant-Garde Medicine");
                Assert.AreEqual("Deepthi S. Saxena MD PC DBA; Avant-Garde Medicine", result);
            }
        }
        [Test]
        public void DoesntSplitLLC()
        {
            using (formatter)
            {
                var result = formatter.ToTitleCase("S2 LLC");
                Assert.AreEqual("S2 LLC", result);
            }
        }
        [Test]
        public void HandlesII()
        {
            using (formatter)
            {
                var result = formatter.ToTitleCase("Mr. Cordell Sylvie II");
                Assert.AreEqual("Mr. Cordell Sylvie II", result);
            }
        }
        [Test]
        public void HandlesPoundSignNumeric()
        {
            using (formatter)
            {
                var result = formatter.ToTitleCase("251 E. Ohio Street # 1100");
                Assert.AreEqual("251 E. Ohio Street #1100", result);
            }
        }
        [Test]
        public void HandlesPoundSignNumericAfterPeriod()
        {
            using (formatter)
            {
                var result = formatter.ToTitleCase("One Courthouse Plaza, Ste.#205");
                Assert.AreEqual("One Courthouse Plaza, Ste. #205", result);
            }
        }
        [Test]
        public void HandlesTheWordAllCaps()
        {
            using (formatter)
            {
                var result = formatter.ToTitleCase("ALL CAPS");
                Assert.AreEqual("All Caps", result);
            }
        }
        //not necessary, just add more acronyms
        //[Test]
        //public void AllowsAcronymsInHypens()
        //{
        //    using (formatter)
        //    {
        //        var result = formatter.ToTitleCase("Test ASD-Thing");
        //        Assert.AreEqual("Test ASD-Thing", result);
        //        Assert.That(AccurateInvalidWords(result, formatter.invalidWords));
        //    }
        //}
        private bool AccurateInvalidWords(string result, IEnumerable<string> invalidWords)
        {
            foreach (string invalidWord in invalidWords)
            {
                if (result.Contains(invalidWord))
                {
                    return false;
                }
            }
            return true;
        }
    }
}
