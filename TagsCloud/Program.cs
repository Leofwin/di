using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using Autofac;
using Autofac.Core;
using CommandLine;

namespace TagsCloud
{
	class Program
	{
		public static void Main(string[] args)
		{
			var options = new GenerateOptions();
			if (!Parser.Default.ParseArguments(args, options))
				return;

			var size = new Size(options.Width, options.Height);
			var fontResult = Result.Of(() => new FontConverter().ConvertFromString(options.FontFamily) as Font);

			var containter = GetDiContainer(options);
			var cloudMaker = containter.Resolve<CloudMaker>();

			var fontSizeByWordsResult = cloudMaker.GetFontSizeByWords(options.InputFileName, options.WordsCount);
			var rectanglesByWordsResult = cloudMaker.GetRectanglesByWords(fontSizeByWordsResult);

			var bitmapResult = cloudMaker.GenerateImage(size, Color.FromName(options.ColorName),
				fontResult, fontSizeByWordsResult, rectanglesByWordsResult);

			var saveResult = containter.Resolve<ICloudWriter>().SaveCloud(bitmapResult, options.OutputFileName);

			Console.WriteLine(!saveResult.IsSuccess ? saveResult.Error : "Successfully done!");
		}

		private static IContainer GetDiContainer(GenerateOptions options)
		{
			var builder = new ContainerBuilder();
			builder.RegisterType<TextReader>().As<IWordsReader>();
			builder.RegisterType<WordFilter>()
				.As<IWordFilter>()
				.WithParameters(new List<Parameter>
				{
					new NamedParameter("boredWords", ReadBoredWordsFromFile(options.BoredWordsFile)),
					new NamedParameter("filters", GetFilters(options))	
				});

			builder.RegisterType<WordFrequencySaver>().As<IWordFrequencySaver>();
			builder.RegisterType<FontNormalizer>()
				.As<IFontNormalizer>()
				.WithParameters(new List<Parameter>
				{
					new NamedParameter("minFontSize", options.MinFontSize),
					new NamedParameter("maxFontSize", options.MaxFontSize)
				});

			builder.RegisterType<AcrhimedeCircularCloudLayouter>()
				.As<ICircularCloudLayouter>().WithParameter(
					"center",
					new Point(options.Width / 2, options.Height / 2)
				);
			builder.RegisterInstance(new SizeDetector()).As<ISizeDetector>();
			builder.RegisterType<CloudMaker>();
			builder.RegisterType<ImageCloudWriter>().As<ICloudWriter>();

			var containter = builder.Build();
			return containter;
		}

		private static List<Func<string, bool>> GetFilters(GenerateOptions options)
		{
			return options.Filters
				.Select(FiltersKeeper.GetFilterByName)
				.Where(r => r.IsSuccess)
				.Select(r => r.Value)
				.ToList();
		}

		private static List<string> ReadBoredWordsFromFile(string fileWithBoringWordsName)
		{
			var boredWords = new List<string>();
			if (!string.IsNullOrEmpty(fileWithBoringWordsName) && File.Exists(fileWithBoringWordsName))
			{
				var readResult = ReadBoringWords(fileWithBoringWordsName);
				if (readResult.IsSuccess)
					boredWords = new List<string>(readResult.Value);
			}

			return boredWords;
		}

		private static Result<string[]> ReadBoringWords(string fileWithBoringWordsName)
		{
			return Result.Of(() => File.ReadAllLines(fileWithBoringWordsName));
		}
	}
}
