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

		public WordFilter(string fileWithBoringWordsName, string[] filters)
		{
			filtersToApply = filters.Select(FiltersKeeper.GetFilterByName).ToList();

			if (!string.IsNullOrEmpty(fileWithBoringWordsName) && File.Exists(fileWithBoringWordsName))
				boredWords = new HashSet<string>(File.ReadAllLines(fileWithBoringWordsName));
			else
				boredWords = new HashSet<string>();
//			errorInformator.PrintInfoMessage("File is not specified. Will check without bored words list");
//			else if (!File.Exists(fileWithBoringWordsName))
//				errorInformator.PrintInfoMessage($"Can't find a file \"{fileWithBoringWordsName}\". " +
//				                                 "Will check without bored words list");
//			else
//				boredWords = new HashSet<string>(File.ReadAllLines(fileWithBoringWordsName));

//			if (boredWords is null)
//				boredWords = new HashSet<string>();
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