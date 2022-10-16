using CardboardBox.Database;
using CardboardBox.Filio.Core.Utilities;
using Dapper;

namespace CardboardBox.Filio.Core
{
	public interface IFilioService
	{
		Task<bool> Process<T>(string source, FilioConfig config);
	}

	public class FilioService : IFilioService
	{
		private readonly ISqlService _sql;
		private readonly ILogger _logger;
		private readonly ICsvService _csv;
		private readonly IFormatService _format;

		public FilioService(
			ISqlService sql, 
			ILogger<FilioService> logger,
			ICsvService csv, 
			IFormatService format)
		{
			_sql = sql;
			_logger = logger;
			_csv = csv;
			_format = format;
		}

		public async Task<bool> Process<T>(string source, FilioConfig config)
		{
			var args = _format.ToArgFormat(config.FormatParameters);

			source = _format.Format(source, args);
			var archiveDir = _format.Format(config.ArchiveDirectory, args);
			var archiveMask = _format.Format(config.ArchiveMask, args);

			if (!File.Exists(source))
			{
				_logger.LogError("Could not find file: {0}", source);
				return false;
			}

			_logger.LogInformation("Starting load of: {0}", source);
			await LoadFile<T>(source, config);
			_logger.LogInformation("Finished load of: {0}", source);

			if (!config.MoveFileToArchive) return true;

			var path = Path.Combine(archiveDir, archiveMask);

			_logger.LogInformation("Moving source file to archive location: {0} --> {1}", source, path);
			if (!Directory.Exists(archiveDir))
				Directory.CreateDirectory(archiveDir);

			File.Move(source, path);
			_logger.LogInformation("Moved source file to archive location: {0} -> {1}", source, path);
			return true;
		}

		public async Task LoadFile<T>(string csv, FilioConfig config)
		{
			_logger.LogInformation("Starting to read CSV file: {0}", csv);
			var data = _csv.ReadFile<T>(csv, config.CsvHasHeader);

			using var con = _sql.CreateConnection();
			using var dt = await ToDataTable(data);
			_logger.LogInformation("CSV loaded! Starting DB Load Records: {0}", dt.Rows.Count);

			var pars = new DynamicParameters(config.DatabaseParameters);
			pars.Add(config.TvpParamName, dt.AsTableValuedParameter(config.TvpName));

			var count = await con.ExecuteAsync(config.Procedure, pars, commandType: CommandType.StoredProcedure);
			_logger.LogInformation("DB Load finished! Return Code: {0}", count);
		}

		public async Task<DataTable> ToDataTable<T>(IAsyncEnumerable<T> data)
		{
			var type = typeof(T);
			var properties = type.GetProperties()
				.Select(ColumnFromProp)
				.Where(t => t.ShouldUse)
				.ToArray();

			var dt = new DataTable();

			foreach(var prop in properties)
				dt.Columns.Add(prop.Name, prop.Type);

			await foreach(var record in data)
			{
				var row = properties
					.Select(t => t.Prop.GetValue(record) ?? (object?)DBNull.Value)
					.ToArray();

				dt.Rows.Add(row);
			}

			return dt;
		}
		
		public FilioColumn ColumnFromProp(PropertyInfo property)
		{
			return new(property.Name, property.PropertyType, property, true);
		}

		public record class FilioColumn(string Name, Type Type, PropertyInfo Prop, bool ShouldUse);
	}
}
