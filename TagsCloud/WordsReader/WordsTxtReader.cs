using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace TagsCloud
{
	public class WordsTxtReader : IWordsReader
	{
		public Result<List<string>> ReadAllWords(string fileName)
		{
			if (string.IsNullOrEmpty(fileName))
				return Result.Fail<List<string>>("Input file is not specified");

			if (!File.Exists(fileName))
				return Result.Fail<List<string>>($"Can't find input file \"{fileName}\"");

			try
			{
				return Result.Ok(File.ReadAllLines(fileName).ToList());
			}
			catch (Exception e)
			{
				return Result.Fail<List<string>>($"Unexpected error while reading \"{fileName}\". {e.Message}");
			}
		}
	}
}