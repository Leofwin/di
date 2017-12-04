using System;

namespace TagsCloud
{
	public class ConsoleErrorInformator : IErrorInformator
	{
		public void PrintInfoMessage(string message)
		{
			Console.WriteLine($"INFO: {message}");
		}

		public void PrintErrorMessage(string message)
		{
			Console.WriteLine($"ERROR: {message}");
		}

		public void Exit()
		{
			Environment.Exit(0);
		}
	}
}