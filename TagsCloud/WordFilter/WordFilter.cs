using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;

namespace TagsCloud
{
	public class WordFilter : IWordFilter
	{
		private readonly List<string> boredWords;
		private readonly List<Func<string, bool>> filtersToApply;

		public WordFilter(List<string> boredWords, List<Func<string, bool>> filters)
		{
			this.boredWords = new List<string>(boredWords);
			filtersToApply = new List<Func<string, bool>>(filters);
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