namespace CardboardBox.Filio.Generation
{
	using Models;

	public interface IGenerator
	{
		string Name { get; }

		Task WriteToStream(Stream output, FileConfig file);

		Task<string> WriteToString(FileConfig file);

		Task WriteToFile(string filename, FileConfig file);
	}

	public abstract class Generator : IGenerator
	{
		public abstract string Name { get; }

		public async Task WriteToFile(string filename, FileConfig file)
		{
			using var io = File.Create(filename);
			await WriteToStream(io, file);
		}

		public async Task<string> WriteToString(FileConfig file)
		{
			using var io = new MemoryStream();
			await WriteToStream(io, file);

			io.Position = 0;
			var bytes = io.ToArray();
			return Encoding.UTF8.GetString(bytes);
		}

		public abstract Task WriteToStream(Stream output, FileConfig file);
	}
}