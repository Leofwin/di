using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace TagsCloud
{
	public class TextReader : IWordsReader
	{
		private readonly char[] delimiters = { ' ', ',', '.', '!', '?', ':', ';', '-', '"', '\'' };

		public Result<List<string>> ReadAllWords(string fileName)
		{
			if (string.IsNullOrEmpty(fileName))
				return Result.Fail<List<string>>("Input file is not specified");

			if (!File.Exists(fileName))
				return Result.Fail<List<string>>($"Can't find input file \"{fileName}\"");

			try
			{
				var words = File.ReadAllLines(fileName)
					.SelectMany(line => line.Split(delimiters, StringSplitOptions.RemoveEmptyEntries))
					.ToList();
				return Result.Ok(words);
			}
			catch (Exception e)
			{
				return Result.Fail<List<string>>($"Unexpected error while reading \"{fileName}\". {e.Message}");
			}
		}
	}
}
