using static Bogus.DataSets.Name;

namespace CardboardBox.Filio.Cli.FakeData
{
	public class FakeUser : IFake
	{
		public int Id { get; set; }
		public string FirstName { get; set; } = string.Empty;
		public string LastName { get; set; } = string.Empty;
		public string UserName { get; set; } = string.Empty;
		public string Email { get; set; } = string.Empty;
		public string Avatar { get; set; } = string.Empty;
		public Gender Gender { get; set; }
	}
}
