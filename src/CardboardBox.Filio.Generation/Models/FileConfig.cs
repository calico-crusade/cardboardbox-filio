namespace CardboardBox.Filio.Generation.Models
{
	/// <summary>
	/// Represents the configuration for a single file comparitor
	/// </summary>
	public class FileConfig
	{
		/// <summary>
		/// The name of the file
		/// </summary>
		public string Name { get; set; } = string.Empty;

		/// <summary>
		/// An optional description of the file
		/// </summary>
		public string? Description { get; set; }

		/// <summary>
		/// A collection of all of the columns in the file
		/// </summary>
		public List<Column> Columns { get; set; } = new();

		/// <summary>
		/// The file mask for the file-name on disk, to be used to scan for files.
		/// </summary>
		public string? SearchMask { get; set; }
	}
}
