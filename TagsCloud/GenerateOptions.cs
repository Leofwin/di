using System.Text;
using CommandLine;
using CommandLine.Text;

namespace TagsCloud
{
	public class GenerateOptions
	{
		[Option('w', "width", Required = true,
			HelpText = "Width of output file.")]
		public int Width { get; set; }

		[Option('h', "height", Required = true,
			HelpText = "Height of output file.")]
		public int Height { get; set; }

		[Option('i', "input", Required = true,
			HelpText = "Name of input file")]
		public string InputFileName { get; set; }

		[Option('o', "output", Required = true,
			HelpText = "Name of output file")]
		public string OutputFileName { get; set; }

		[Option('c', "color", DefaultValue = "Blue",
			HelpText = "Color of text in output file")]
		public string ColorName { get; set; }

		[Option('l', "limit", DefaultValue = 100,
			HelpText = "Words count")]
		public int WordsCount { get; set; }

		[Option('b', "bored", DefaultValue = null,
			HelpText = "File, containing words which should be exlcuded")]
		public string BoredWordsFile { get; set; }

		[Option('u', "max-font", DefaultValue = 60,
			HelpText = "Max font size")]
		public int MaxFontSize { get; set; }

		[Option('d', "min-font", DefaultValue = 10,
			HelpText = "Min font size")]
		public int MinFontSize { get; set; }

		[OptionArray('f', "filters", DefaultValue = new string[0], 
			HelpText = "Validating filters of words")]
		public string[] Filters { get; set; }

		[ParserState]
		public IParserState LastParserState { get; set; }

		[HelpOption]
		public string GetUsage()
		{
			var result = new StringBuilder();

			result.Append(HelpText.AutoBuild(this,
				current => HelpText.DefaultParsingErrorsHandler(this, current)));

			result.AppendLine("-f[--filters] possible arguments:");
			foreach (var pair in FiltersKeeper.GetFilterDescription())
				result.AppendLine($"\t{pair.Key} - {pair.Value}");
			result.AppendLine();

			return result.ToString();
		}
	}
}
