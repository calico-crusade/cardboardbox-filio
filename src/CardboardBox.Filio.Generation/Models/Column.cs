using CsvHelper.Configuration.Attributes;

namespace CardboardBox.Filio.Generation.Models
{
	/// <summary>
	/// Represents a single columns configuration in the file comparitor
	/// </summary>
	public class Column
	{
		/// <summary>
		/// The index of the column in the in-bound file
		/// </summary>
		[Index(0), Name("Index")]
		public int Index { get; set; } = -1;

		/// <summary>
		/// The name of the column in the in-bound file
		/// </summary>
		[Index(1), Name("Name")]
		public string Name { get; set; } = string.Empty;

		/// <summary>
		/// An optional description of the column in the in-bound file
		/// </summary>
		[Index(2), Name("Description")]
		public string? Description { get; set; }

		/// <summary>
		/// The <see cref="DataType"/> of the column in the in-bound file
		/// </summary>
		[Index(3), Name("Data Type")]
		public DataType Type { get; set; } = DataType.String;

		/// <summary>
		/// The format of the column in the in-bound file
		/// </summary>
		[Index(4), Name("Data Format")]
		public string? Format { get; set; }
	}
}