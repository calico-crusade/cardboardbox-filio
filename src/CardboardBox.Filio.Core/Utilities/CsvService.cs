using CsvHelper;

namespace CardboardBox.Filio.Core.Utilities
{
	public interface ICsvService
	{
		#region Readers
		/// <summary>
		/// Reads the records from the stream of csv data
		/// </summary>
		/// <typeparam name="T">The type to deserialize to</typeparam>
		/// <param name="source">The source stream for the CSV data</param>
		/// <param name="hasHeader">Whether or not the data has a header record</param>
		/// <param name="config">Any additional configuration necessary</param>
		/// <returns>Async collection of records</returns>
		IAsyncEnumerable<T?> Read<T>(Stream source, bool hasHeader = true, Action<CsvReader>? config = null);

		/// <summary>
		/// Reads the records from the string of csv data
		/// </summary>
		/// <typeparam name="T">The type to deserialize to</typeparam>
		/// <param name="data">The CSV data</param>
		/// <param name="hasHeader">Whether or not the data has a header record</param>
		/// <param name="config">Any additional configuration necessary</param>
		/// <param name="encoder">The encoding to use for the text reader (defaults to <see cref="Encoding.UTF8"/>)</param>
		/// <returns>Async collection of records</returns>
		IAsyncEnumerable<T> Read<T>(string data, bool hasHeader = true, Action<CsvReader>? config = null, Encoding? encoder = null);

		/// <summary>
		/// Reads the records from the csv file
		/// </summary>
		/// <typeparam name="T">The type to deserialize to</typeparam>
		/// <param name="path">The path to the csv file</param>
		/// <param name="hasHeader">Whether or not the data has a header record</param>
		/// <param name="config">Any additional configuration necessary</param>
		/// <returns>Async collection of records</returns>
		/// <exception cref="FileNotFoundException">Thrown if the specified path could not be found</exception>
		IAsyncEnumerable<T> ReadFile<T>(string path, bool hasHeader = true, Action<CsvReader>? config = null);
		#endregion

		#region Writers
		/// <summary>
		/// Writes the given records to the target stream
		/// </summary>
		/// <typeparam name="T">The type of records to write</typeparam>
		/// <param name="target">The target stream</param>
		/// <param name="data">The data to write</param>
		/// <param name="writeHeader">Whether or not to write the header record</param>
		/// <param name="config">Any additional configuration necessary</param>
		/// <returns>A task representing the completion of the writing operation</returns>
		Task Write<T>(TextWriter target, IEnumerable<T> data, bool writeHeader = true, Action<CsvWriter>? config = null);

		/// <summary>
		/// Writes the given records to the target stream
		/// </summary>
		/// <typeparam name="T">The type of records to write</typeparam>
		/// <param name="target">The target stream</param>
		/// <param name="data">The data to write</param>
		/// <param name="writeHeader">Whether or not to write the header record</param>
		/// <param name="config">Any additional configuration necessary</param>
		/// <returns>A task representing the completion of the writing operation</returns>
		Task Write<T>(Stream target, IEnumerable<T> data, bool writeHeader = true, Action<CsvWriter>? config = null);

		/// <summary>
		/// Writes the given records to a string and returns it
		/// </summary>
		/// <typeparam name="T">The type of records to write</typeparam>
		/// <param name="data">The data to write</param>
		/// <param name="writeHeader">Whether or not to write the header record</param>
		/// <param name="config">Any additional configuration necessary</param>
		/// <returns>A task representing the completion of the writing operation</returns>
		Task<string> Write<T>(IEnumerable<T> data, bool writeHeader = true, Action<CsvWriter>? config = null);

		/// <summary>
		/// Writes the given records to the specified file path
		/// </summary>
		/// <typeparam name="T">The type of records to write</typeparam>
		/// <param name="path">The path to the file to write</param>
		/// <param name="data">The data to write</param>
		/// <param name="writeHeader">Whether or not to write the header record</param>
		/// <param name="config">Any additional configuration necessary</param>
		/// <returns>A task representing the completion of the writing operation</returns>
		Task Write<T>(string path, IEnumerable<T> data, bool writeHeader = true, Action<CsvWriter>? config = null);
		#endregion

		#region Async Writers
		/// <summary>
		/// Writes the given async data to the target stream
		/// </summary>
		/// <typeparam name="T">The type of records to write</typeparam>
		/// <param name="target">The target stream</param>
		/// <param name="data">The data to write</param>
		/// <param name="writeHeader">Whether or not to write the header record</param>
		/// <param name="config">Any additional configuration necessary</param>
		/// <returns>A task representing the completion of the writing operation</returns>
		Task Write<T>(Stream target, IAsyncEnumerable<T> data, bool writeHeader = true, Action<CsvWriter>? config = null);
		#endregion
	}

	public class CsvService : ICsvService
	{
		#region Readers
		/// <summary>
		/// Reads the records from the string of csv data
		/// </summary>
		/// <typeparam name="T">The type to deserialize to</typeparam>
		/// <param name="data">The CSV data</param>
		/// <param name="hasHeader">Whether or not the data has a header record</param>
		/// <param name="config">Any additional configuration necessary</param>
		/// <param name="encoder">The encoding to use for the text reader (defaults to <see cref="Encoding.UTF8"/>)</param>
		/// <returns>Async collection of records</returns>
		public async IAsyncEnumerable<T> Read<T>(string data, bool hasHeader = true, Action<CsvReader>? config = null, Encoding? encoder = null)
		{
			encoder ??= Encoding.UTF8;
			var bytes = encoder.GetBytes(data);
			using var io = new MemoryStream(bytes);
			await foreach (var record in Read<T>(io, hasHeader, config))
				yield return record;
		}

		/// <summary>
		/// Reads the records from the stream of csv data
		/// </summary>
		/// <typeparam name="T">The type to deserialize to</typeparam>
		/// <param name="source">The source stream for the CSV data</param>
		/// <param name="hasHeader">Whether or not the data has a header record</param>
		/// <param name="config">Any additional configuration necessary</param>
		/// <returns>Async collection of records</returns>
		public async IAsyncEnumerable<T> Read<T>(Stream source, bool hasHeader = true, Action<CsvReader>? config = null)
		{
			using var reader = new StreamReader(source);
			using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);

			config?.Invoke(csv);

			if (hasHeader)
			{
				await csv.ReadAsync();
				csv.ReadHeader();
			}

			while (await csv.ReadAsync())
			{
				var record = csv.GetRecord<T>();
				if (record != null)
					yield return record;
			}
		}

		/// <summary>
		/// Reads the records from the csv file
		/// </summary>
		/// <typeparam name="T">The type to deserialize to</typeparam>
		/// <param name="path">The path to the csv file</param>
		/// <param name="hasHeader">Whether or not the data has a header record</param>
		/// <param name="config">Any additional configuration necessary</param>
		/// <returns>Async collection of records</returns>
		/// <exception cref="FileNotFoundException">Thrown if the specified path could not be found</exception>
		public async IAsyncEnumerable<T> ReadFile<T>(string path, bool hasHeader = true, Action<CsvReader>? config = null)
		{
			if (!File.Exists(path))
				throw new FileNotFoundException($"Could not find specified file: " + path);

			using var io = File.OpenRead(path);
			await foreach (var record in Read<T>(io, hasHeader, config))
				yield return record;
		}
		#endregion

		#region Writers
		/// <summary>
		/// Writes the given records to the target stream
		/// </summary>
		/// <typeparam name="T">The type of records to write</typeparam>
		/// <param name="target">The target stream</param>
		/// <param name="data">The data to write</param>
		/// <param name="writeHeader">Whether or not to write the header record</param>
		/// <param name="config">Any additional configuration necessary</param>
		/// <returns>A task representing the completion of the writing operation</returns>
		public async Task Write<T>(TextWriter target, IEnumerable<T> data, bool writeHeader = true, Action<CsvWriter>? config = null)
		{
			using var csv = new CsvWriter(target, CultureInfo.InvariantCulture);

			config?.Invoke(csv);

			if (writeHeader)
			{
				csv.WriteHeader<T>();
				await csv.NextRecordAsync();
			}

			foreach (var record in data)
			{
				csv.WriteRecord(record);
				await csv.NextRecordAsync();
			}
		}

		/// <summary>
		/// Writes the given records to the target stream
		/// </summary>
		/// <typeparam name="T">The type of records to write</typeparam>
		/// <param name="target">The target stream</param>
		/// <param name="data">The data to write</param>
		/// <param name="writeHeader">Whether or not to write the header record</param>
		/// <param name="config">Any additional configuration necessary</param>
		/// <returns>A task representing the completion of the writing operation</returns>
		public async Task Write<T>(Stream target, IEnumerable<T> data, bool writeHeader = true, Action<CsvWriter>? config = null)
		{
			using var writer = new StreamWriter(target);
			await Write(writer, data, writeHeader, config);
		}

		/// <summary>
		/// Writes the given records to a string and returns it
		/// </summary>
		/// <typeparam name="T">The type of records to write</typeparam>
		/// <param name="data">The data to write</param>
		/// <param name="writeHeader">Whether or not to write the header record</param>
		/// <param name="config">Any additional configuration necessary</param>
		/// <returns>A task representing the completion of the writing operation</returns>
		public async Task<string> Write<T>(IEnumerable<T> data, bool writeHeader = true, Action<CsvWriter>? config = null)
		{
			var bob = new StringBuilder();
			using var writer = new StringWriter(bob);
			await Write(writer, data, writeHeader, config);
			return bob.ToString();
		}

		/// <summary>
		/// Writes the given records to the specified file path
		/// </summary>
		/// <typeparam name="T">The type of records to write</typeparam>
		/// <param name="path">The path to the file to write</param>
		/// <param name="data">The data to write</param>
		/// <param name="writeHeader">Whether or not to write the header record</param>
		/// <param name="config">Any additional configuration necessary</param>
		/// <returns>A task representing the completion of the writing operation</returns>
		public async Task Write<T>(string path, IEnumerable<T> data, bool writeHeader = true, Action<CsvWriter>? config = null)
		{
			using var io = File.Create(path);
			await Write(io, data, writeHeader, config);
		}
		#endregion

		#region Async Writers
		/// <summary>
		/// Writes the given async data to the target stream
		/// </summary>
		/// <typeparam name="T">The type of records to write</typeparam>
		/// <param name="target">The target stream</param>
		/// <param name="data">The data to write</param>
		/// <param name="writeHeader">Whether or not to write the header record</param>
		/// <param name="config">Any additional configuration necessary</param>
		/// <returns>A task representing the completion of the writing operation</returns>
		public async Task Write<T>(Stream target, IAsyncEnumerable<T> data, bool writeHeader = true, Action<CsvWriter>? config = null)
		{
			using var writer = new StreamWriter(target);
			using var csv = new CsvWriter(writer, CultureInfo.InvariantCulture);

			config?.Invoke(csv);

			if (writeHeader)
			{
				csv.WriteHeader<T>();
				await csv.NextRecordAsync();
			}

			await foreach (var record in data)
			{
				csv.WriteRecord(record);
				await csv.NextRecordAsync();
			}
		}
		#endregion
	}
}
