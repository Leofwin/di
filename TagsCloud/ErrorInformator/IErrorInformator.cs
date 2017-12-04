namespace TagsCloud
{
	public interface IErrorInformator
	{
		void PrintInfoMessage(string message);

		void PrintErrorMessage(string message);

		void Exit();
	}
}
