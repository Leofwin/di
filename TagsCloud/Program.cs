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
			args = new[] { "-h", "1000", "-w", "1000", "-i", "../../texts/Master.txt", "-o", "result.png"};
			var options = new GenerateOptions();
			if (!Parser.Default.ParseArguments(args, options))
				return;

			var size = new Size(options.Width, options.Height);
			var fontFamily = FontFamily.GenericMonospace;

			var builder = new ContainerBuilder();
			builder.RegisterInstance(new TextReader()).As<IWordsReader>();
			builder.RegisterInstance(new WordFormatter()).As<IWordFormatter>();
			builder.RegisterInstance(new WordValidator()).As<IWordValidator>();
			builder.RegisterType<WordFrequencySaver>().As<IWordFrequencySaver>();
			builder.RegisterInstance(new FontNormalizer(10, 60)).As<IFontNormalizer>();
			builder.RegisterInstance(new AcrhimedeCircularCloudLayouter(
					new Point(options.Width / 2, options.Height / 2))
				)
				.As<ICircularCloudLayouter>();
			builder.RegisterInstance(new SizeDetector()).As<ISizeDetector>();
			builder.RegisterType<CloudMaker>();

			var containter = builder.Build();
			var cloudMaker = containter.Resolve<CloudMaker>();
			var bitmap = cloudMaker.GenerateImage(options.InputFileName,
				size, Color.FromName(options.ColorName), fontFamily, options.WordsCount);
			bitmap.Save(options.OutputFileName, ImageFormat.Png);
		}
	}
}
