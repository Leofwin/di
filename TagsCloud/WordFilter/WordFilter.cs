using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace TagsCloud
{
	public class WordFilter : IWordFilter
	{
		private readonly HashSet<string> boredWords;
		private readonly List<Func<string, bool>> filtersToApply;

		public WordFilter(IErrorInformator errorInformator, string fileWithBoringWordsName, string[] filters)
		{
			this.filtersToApply = filters.Select(FiltersKeeper.GetFilterByName).ToList();

			try
			{
				boredWords = new HashSet<string>(File.ReadAllLines(fileWithBoringWordsName));
			}
			catch (ArgumentException)
			{
				errorInformator.PrintInfoMessage("File is not specified. Will check without bored words list");	
			}
			catch (FileNotFoundException)
			{
				errorInformator.PrintInfoMessage($"Can't find a file \"{fileWithBoringWordsName}\". " +
				                                 "Will check without bored words list");
			}

			if (boredWords is null)
				boredWords = new HashSet<string>();
		}

		public bool IsValidateWord(string word)
		{
			if (filtersToApply.Any(filter => !filter(word)))
				return false;

			return !boredWords.Contains(word);
		}

		public string GetFormatWord(string word)
		{
			return word.ToLower();
		}
	}
}