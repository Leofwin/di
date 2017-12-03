using System.Drawing;
using System.Drawing.Imaging;
using Autofac;


namespace TagsCloud
{
	class Program
	{
		public static void Main(string[] args)
		{
			var size = new Size(1000, 1000);
			var fontFamily = FontFamily.GenericMonospace;

			var builder = new ContainerBuilder();
			builder.RegisterInstance(new TextReader()).As<IWordsReader>();
			builder.RegisterInstance(new WordFormatter()).As<IWordFormatter>();
			builder.RegisterInstance(new WordValidator()).As<IWordValidator>();
			builder.RegisterType<WordFrequencySaver>().As<IWordFrequencySaver>();
			builder.RegisterInstance(new FontNormalizer(10, 60)).As<IFontNormalizer>();
			builder.RegisterInstance(new AcrhimedeCircularCloudLayouter(new Point(500, 500)))
				.As<ICircularCloudLayouter>();
			builder.RegisterInstance(new SizeDetector()).As<ISizeDetector>();
			builder.RegisterType<CloudMaker>();

			var containter = builder.Build();
			var cloudMaker = containter.Resolve<CloudMaker>();
			var bitmap = cloudMaker.GenerateImage("../../texts/Sherlock_Holmes.txt", 
				size, Color.Blue, fontFamily, 400);
			bitmap.Save("result.png", ImageFormat.Png);
		}
	}
}
