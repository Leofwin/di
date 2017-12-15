﻿using System;
using System.Collections.Generic;
using System.Drawing;
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
			filtersToApply = filters
				.Select(FiltersKeeper.GetFilterByName)
				.Where(r => r.IsSuccess)
				.Select(r => r.Value)
				.ToList();

			if (!string.IsNullOrEmpty(fileWithBoringWordsName) && File.Exists(fileWithBoringWordsName))
			{
				var readResult = ReadBoringWords(fileWithBoringWordsName);
				boredWords = readResult.IsSuccess 
					? new HashSet<string>(readResult.Value) 
					: new HashSet<string>();
			}
			else
				boredWords = new HashSet<string>();
		}

		private static Result<string[]> ReadBoringWords(string fileName)
		{
			return Result.Of(() => File.ReadAllLines(fileName));
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