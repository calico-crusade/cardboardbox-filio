namespace CardboardBox.Filio.Core
{
	public class FilioConfig
	{
		public string Procedure { get; set; }
		public string TvpName { get; set; }
		public string TvpParamName { get; set; }
		public object? DatabaseParameters { get; set; }
		public string ArchiveDirectory { get; set; }
		public string ArchiveMask { get; set; }

		public bool CsvHasHeader { get; set; } = true;
		public bool MoveFileToArchive { get; set; } = true;

		public Dictionary<string, object> FormatParameters { get; set; } = new();

		public FilioConfig(string procedure, 
			string tvpName, 
			string tvpParamName,
			string archiveDir,
			string archiveMask,
			object? parameters = null, 
			bool csvHasHeader = true)
		{
			Procedure = procedure;
			TvpName = tvpName;
			TvpParamName = tvpParamName;
			DatabaseParameters = parameters;
			CsvHasHeader = csvHasHeader;
			ArchiveDirectory = archiveDir;
			ArchiveMask = archiveMask;
		}
	}
}
