using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using Autofac;
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
			builder.Register(c => new WordFilter(
					ReadBoredWordsFromFile(options.BoredWordsFile),
					GetFilters(options)))
				.As<IWordFilter>();
			builder.RegisterType<WordFrequencySaver>().As<IWordFrequencySaver>();
			builder.Register(c => new FontNormalizer(
					options.MinFontSize, 
					options.MaxFontSize)
				)
				.As<IFontNormalizer>();
			builder.RegisterInstance(new AcrhimedeCircularCloudLayouter(
					new Point(options.Width / 2, options.Height / 2))
				)
				.As<ICircularCloudLayouter>();
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
