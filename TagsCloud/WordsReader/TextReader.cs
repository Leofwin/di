using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace TagsCloud
{
	public class TextReader : IWordsReader
	{
		private readonly IErrorInformator errorInformator;
		private readonly char[] delimiters = { ' ', ',', '.', '!', '?', ':', ';', '-', '"', '\'' };

		public TextReader(IErrorInformator errorInformator)
		{
			this.errorInformator = errorInformator;
		}

		public List<string> ReadAllWords(string fileName)
		{
			if (string.IsNullOrEmpty(fileName))
			{
				errorInformator.PrintErrorMessage("Input file is not specified");
				errorInformator.Exit();
			}

			if (!File.Exists(fileName))
			{
				errorInformator.PrintErrorMessage($"Can't find input file \"{fileName}\"");
				errorInformator.Exit();
			}

			try
			{
				return File.ReadAllLines(fileName)
					.SelectMany(line => line.Split(delimiters, StringSplitOptions.RemoveEmptyEntries))
					.ToList();
			}
			catch (Exception)
			{
				errorInformator.PrintErrorMessage($"Unexpected error while reading \"{fileName}\"");
				errorInformator.Exit();
			}
			return null;
		}
	}
}
