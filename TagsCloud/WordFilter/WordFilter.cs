using System;
using System.Collections.Generic;
using System.IO;

namespace TagsCloud
{
	public class WordFilter : IWordFilter
	{
		private readonly HashSet<string> boredWords;

		public WordFilter(IErrorInformator errorInformator, string fileWithBoringWordsName)
		{
			try
			{
				var words = File.ReadAllLines(fileWithBoringWordsName);
				boredWords = new HashSet<string>(words);
			}
			catch (ArgumentException)
			{
				errorInformator.PrintInfoMessage("File is not specified. Will check without bored words list");
				if (boredWords is null)
					boredWords = new HashSet<string>();
			}
			catch (FileNotFoundException)
			{
				errorInformator.PrintInfoMessage($"Can't find a file \"{fileWithBoringWordsName}\". " +
				                                 "Will check without bored words list");
				if (boredWords is null)
					boredWords = new HashSet<string>();
			}
		}

		public bool IsValidateWord(string word)
		{
			return !boredWords.Contains(word) && word.Length > 4;
		}

		public string GetFormatWord(string word)
		{
			return word.ToLower();
		}
	}
}