namespace CardboardBox.Filio.Cli.FakeData
{
	public class FakeAddress : IFake
	{
		public int Id { get; set; }
		public string Line1 { get; set; } = string.Empty;
		public string Country { get; set; } = string.Empty;
		public string City { get; set; } = string.Empty;
		public string Zip { get; set; } = string.Empty;
		public string State { get; set; } = string.Empty;
		public double Latitude { get; set; }
		public double Longitude { get; set; }
	}
}
