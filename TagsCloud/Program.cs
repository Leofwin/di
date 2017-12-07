using System;
using System.Drawing;
using System.Drawing.Imaging;
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
			var fontFamily = FontFamily.GenericMonospace;

			var containter = GetContainer(options);
			var cloudMaker = containter.Resolve<CloudMaker>();

			var fontSizeByWords = cloudMaker.GetFontSizeByWords(options.InputFileName, options.WordsCount);

			var rectanglesByWords = cloudMaker.GetRectanglesByWords(fontSizeByWords);

			using (var bitmap = cloudMaker.GenerateImage(size, Color.FromName(options.ColorName), 
				fontFamily, fontSizeByWords, rectanglesByWords))
			{
				try
				{
					bitmap.Save(options.OutputFileName, ImageFormat.Png);
				}
				catch (ArgumentException)
				{
					containter.Resolve<IErrorInformator>()
						.PrintErrorMessage("Can't write output file: incorrect argument");
				}
			}
		}

		private static IContainer GetContainer(GenerateOptions options)
		{
			var builder = new ContainerBuilder();
			builder.RegisterInstance(new ConsoleErrorInformator()).As<IErrorInformator>();
			builder.RegisterType<TextReader>().As<IWordsReader>();
			builder.Register(c => new WordFilter(c.Resolve<IErrorInformator>(), options.BoredWordsFile))
				.As<IWordFilter>();
			builder.RegisterType<WordFrequencySaver>().As<IWordFrequencySaver>();
			builder.RegisterInstance(new FontNormalizer(10, 60)).As<IFontNormalizer>();
			builder.RegisterInstance(new AcrhimedeCircularCloudLayouter(
					new Point(options.Width / 2, options.Height / 2))
				)
				.As<ICircularCloudLayouter>();
			builder.RegisterInstance(new SizeDetector()).As<ISizeDetector>();
			builder.RegisterType<CloudMaker>();

			var containter = builder.Build();
			return containter;
		}
	}
}
