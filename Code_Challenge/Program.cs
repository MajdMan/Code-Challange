using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CodeChallenge
{
    /*Created By Majd Abu Libdeh: This is my solution for the challenge.
    *  This solution is able to fine the correct phrase for any given 3 word anagram as long as you have the hash for the correct word. 
   /I also could have created classes with additional behavior, but I decided not to. I just created this quick solution in a short period of time to solve the anagram. I tried to 
   make this as dynamic as I could but did not manage complete within my time frame. I wanted to be able to input any amount of words*/
    class Program
    {
        static void Main(string[] args)
        {
            //Here I start by adding all the variables that I need
            Stopwatch stopwatch = Stopwatch.StartNew();
            string anagram = "poultry outwits ants";
            string hash = "4624d200580677270a54ccff86b9610e";

            string trimmedAnagram = anagram.Trim();
            List<string> listOfWrittenWords = anagram.Split(' ').ToList();
            string characterNotAllowed = @"";
            string characterAllowed = "";
            string alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ'".ToLower();

            List<KeyValuePair<int, int>> myKeyValuePairs = new List<KeyValuePair<int, int>>();

            //I add to the key value pair. The key is the position of the string and the value is the length of the word. Trying to have it dynamic 
            for (int i = 0; i < listOfWrittenWords.Count; i++)
            {
                myKeyValuePairs.Add(new KeyValuePair<int, int>(i, listOfWrittenWords[i].Length));
            }


            // Here I loop through the characters in the phrase and separate them into Allowed characters characters"
            foreach (var character in trimmedAnagram)
            {
                if (character != ' ')
                {
                    int countFor = trimmedAnagram.Split(character).Length - 1;
                    if (countFor > 0)
                    {
                        if (!characterAllowed.Contains(character))
                        {
                            characterAllowed += character;
                        }
                    }

                }
            }
            // Here I loop through the characters in the phrase and separate them into Not allowed characters, I I add them to use for the Regen to filter the words containing the letters that
            //are not in the anagram "
            for (int i = 0; i < alphabet.Length; i++)
            {
                if (!characterAllowed.Contains(alphabet[i]))
                {
                    if (i == alphabet.Length - 1)
                    {
                        characterNotAllowed += alphabet[i];
                    }
                    else
                    {
                        characterNotAllowed += alphabet[i] + "|";
                    }
                }
            }

            //  just in case, to be sure the reg-ex is correct 
            if (characterNotAllowed[characterNotAllowed.Length - 1] == '|')
            {
                characterNotAllowed = characterNotAllowed.Remove(characterNotAllowed.Length - 1);
            }
            Console.WriteLine("Wait for it...");

            //Method for getting the right phrase
            var result = GetThePhrase(characterNotAllowed, hash, myKeyValuePairs);
            stopwatch.Stop();
            //write the result 
            Console.WriteLine(result + " " + "time:" + " " + stopwatch.Elapsed);
            Console.ReadKey();
        }
        public static string GetThePhrase(string notAllowdLetters, string hashPassed, List<KeyValuePair<int, int>> myKeyValuePairs)
        {
            //more variabels that I needed 
            Stopwatch stopwatch = Stopwatch.StartNew();
            MD5 hash = MD5.Create();
            // re
            List<string> tempList = new List<string>();

            // read from file and load into memory. BTW got this somewhere on Stack overflow(Read from file )
            string fileName = @"C:\Users\Majd\Downloads\word list";
            var streamReader = File.OpenText(fileName);
            List<string> wordList =
            streamReader.ReadToEnd().Split("\r\n".ToCharArray(), StringSplitOptions.RemoveEmptyEntries).ToList();
            //pass the not allowed words to the reg ex
            Regex regex = new Regex(notAllowdLetters);
            var combined = "";

            //loop and filter the word list... Use the key-value pair to filter the length of the words 
            foreach (var word in wordList)
            {
                bool contains = regex.IsMatch(word);

                if (!contains && (word.Length == myKeyValuePairs[0].Value || word.Length == myKeyValuePairs[1].Value || word.Length == myKeyValuePairs[2].Value))
                {
                    tempList.Add(word);
                }
            }
            Console.WriteLine("The amount of filtered words are:" + " " + tempList.Count);

            /*Now its the combinations of possible words. Again the key-value pair allows me to know the position and the length of each word. now I can run this on any 3 word combination
            if I know the hash and the anagram... :). Could also have used a while loop and a Boolean to stop the loop. return is not the best practice but I had already done it and published and I did not want to change it */

            for (int i = 0; i < tempList.Count; i++)
            {

                if (tempList[i].Length == myKeyValuePairs[0].Value)
                {
                    var first = tempList[i];

                    for (int j = 0; j < tempList.Count; j++)
                    {
                        if (tempList[j].Length == myKeyValuePairs[1].Value)
                        {
                            var second = tempList[j];
                            for (int k = 0; k < tempList.Count; k++)
                            {
                                if (tempList[k].Length == myKeyValuePairs[2].Value)
                                {
                                    combined = "";
                                    var third = tempList[k];
                                    combined = first + " " + second + " " + third;
                                    var actualHash = GetMd5Hash(hash, combined);
                                    if (actualHash.ToLower() == hashPassed)
                                    {
                                        stopwatch.Stop();
                                        return combined;

                                    }
                                }

                            }
                        }
                    }
                }
                Console.WriteLine(stopwatch.Elapsed);
            }
            stopwatch.Stop();
            return "Nothing Found";
        }

        //create hash for a certain string. Found this on-line. 
        public static string GetMd5Hash(MD5 md5Hash, string input)
        {

            // Convert the input string to a byte array and compute the hash.
            byte[] data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(input));

            // Create a new Stringbuilder to collect the bytes
            // and create a string.
            StringBuilder sBuilder = new StringBuilder();

            // Loop through each byte of the hashed data 
            // and format each one as a hexadecimal string.
            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }

            // Return the hexadecimal string.
            return sBuilder.ToString();
        }
    }
}
