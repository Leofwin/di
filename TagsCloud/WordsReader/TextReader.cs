using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace TagsCloud
{
	public class TextReader : IWordsReader
	{
		private readonly char[] delimiters = { ' ', ',', '.', '!', '?', ':', ';', '-', '"', '\'' };
		public List<string> ReadAllWords(string fileName)
		{
			return File.ReadAllLines(fileName)
				.SelectMany(line => line.Split(delimiters, StringSplitOptions.RemoveEmptyEntries))
				.ToList();
		}
	}
}
