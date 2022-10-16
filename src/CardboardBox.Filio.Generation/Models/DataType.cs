namespace CardboardBox.Filio.Generation.Models
{
	/// <summary>
	/// Represents the data type for a specific column
	/// </summary>
	public enum DataType
	{
		#region Characters & Strings (1 - 9)
		/// <summary>
		/// A single character
		/// </summary>
		Char = 1,
		/// <summary>
		/// A collection of characters
		/// </summary>
		String = 2,
		#endregion

		#region Numerics (10-19)
		/// <summary>
		/// Int (<see cref="int"/>)
		/// </summary>
		Number = 10,
		/// <summary>
		/// Long (<see cref="long"/>)
		/// </summary>
		BigNumber = 11,
		/// <summary>
		/// Short (<see cref="short"/>)
		/// </summary>
		ShortNumber = 12,
		/// <summary>
		/// Decimal number (<see cref="double"/>)
		/// </summary>
		Decimal = 13,
		#endregion

		#region DateTimes (20-29)
		/// <summary>
		/// Date only portion of (<see cref="System.DateTime"/>) - Treated as (<see cref="System.DateTime"/>)
		/// </summary>
		Date = 20,
		/// <summary>
		/// Time only portion of (<see cref="System.DateTime"/>) - Treated as (<see cref="System.DateTime"/>)
		/// </summary>
		Time = 21,
		/// <summary>
		/// Date & time (<see cref="System.DateTime"/>)
		/// </summary>
		DateTime = 22,
		#endregion

		#region Bit / Boolean (30-39)
		/// <summary>
		/// True / False (also represents a bit)
		/// </summary>
		Boolean = 30,
		#endregion

		#region Collections (40-49)
		/// <summary>
		/// Represents an array of <see cref="Char"/>s or <see cref="String"/>s  - Treated as (<see cref="String"/>)
		/// </summary>
		StringArray = 40,
		/// <summary>
		/// Represents an array of <see cref="Number"/>s, <see cref="BigNumber"/>s, <see cref="ShortNumber"/>s, or <see cref="Decimal"/>s - Treated as (<see cref="Decimal"/>)
		/// </summary>
		NumericArray = 41,
		/// <summary>
		/// Represents an array of <see cref="Date"/>s, <see cref="Time"/>s, or <see cref="DateTime"/>s - Treated as (<see cref="DateTime"/>)
		/// </summary>
		DateTimeArray = 42,
		/// <summary>
		/// Represents an array of <see cref="Boolean"/>s - Treated as (<see cref="Boolean"/>)
		/// </summary>
		BooleanArray = 43,
		#endregion
	}
}
