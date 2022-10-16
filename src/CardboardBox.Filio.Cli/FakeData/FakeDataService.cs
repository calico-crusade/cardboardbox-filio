using Bogus;
using static Bogus.DataSets.Name;

namespace CardboardBox.Filio.Cli.FakeData
{
	public interface IFakeDataService
	{
		FakeData? DetermineFaker(string name);
		IFake[]? Generate(string name, int count);
		IFake? Generate(string name);
		bool Shuffle(string name, IFake[] data, int count);
		bool Shuffle(string name, IFake data, int count);
	}

	public class FakeDataService : IFakeDataService
	{
		private static Random _rnd => LinqExtensions.RandomInstance;

		public FakeData[] Fakers => new[]
		{
			new FakeData("address", 
				typeof(FakeAddress), 
				() => AddressFaker().Generate(), 
				(c) => AddressFaker().Generate(c).ToArray()),

			new FakeData("user", 
				typeof(FakeUser), 
				() => UserFaker().Generate(), 
				(c) => UserFaker().Generate(c).ToArray()),
		};

		public FakeData? DetermineFaker(string name)
		{
			return Fakers.FirstOrDefault(t => t.Name == name.ToLower());
		}

		public IFake[]? Generate(string name, int count)
		{
			var srs = DetermineFaker(name);
			if (srs == null) return null;

			return srs.GenerateMultiple(count);
		}

		public IFake? Generate(string name)
		{
			var srs = DetermineFaker(name);
			if (srs == null) return null;

			return srs.GenerateSingle();
		}

		public bool Shuffle(string name, IFake[] data, int count)
		{
			var src = DetermineFaker(name);
			if (src == null) return false;

			var (_, type, single, _) = src;

			var props = type.GetProperties()
				.Where(t => t.Name != "Id")
				.ToArray();

			for(var i = 0; i < count; i++)
			{
				var propModCount = _rnd.Next(0, props.Length);
				var update = single();
				var target = data.Random();

				for(var p = 0; p < propModCount; p++)
				{
					var prop = props.Random();
					var getVal = prop.GetValue(update);
					prop.SetValue(target, getVal);
				}
			}

			return true;
		}

		public bool Shuffle(string name, IFake data, int count)
		{
			var src = DetermineFaker(name);
			if (src == null) return false;

			var (_, type, single, _) = src;
			var props = type.GetProperties()
				.Where(t => t.Name != "Id")
				.ToArray();

			var update = single();

			for(var i = 0; i < count; i++)
			{
				var prop = props.Random();
				var getVal = prop.GetValue(update);
				prop.SetValue(data, getVal);
			}

			return true;
		}

		public Faker<FakeAddress> AddressFaker() => new Faker<FakeAddress>()
				.RuleFor(u => u.Id, f => f.UniqueIndex)
				.RuleFor(u => u.Line1, f => f.Address.StreetAddress())
				.RuleFor(u => u.Country, f => f.Address.Country())
				.RuleFor(u => u.City, f => f.Address.City())
				.RuleFor(u => u.Zip, f => f.Address.ZipCode())
				.RuleFor(u => u.State, f => f.Address.State())
				.RuleFor(u => u.Latitude, f => f.Address.Latitude())
				.RuleFor(u => u.Longitude, f => f.Address.Longitude());

		public Faker<FakeUser> UserFaker() => new Faker<FakeUser>()
			.RuleFor(u => u.Id, f => f.UniqueIndex)
			.RuleFor(u => u.Gender, f => f.PickRandom<Gender>())
			.RuleFor(u => u.FirstName, (f, u) => f.Name.FirstName(u.Gender))
			.RuleFor(u => u.LastName, (f, u) => f.Name.LastName(u.Gender))
			.RuleFor(u => u.Avatar, f => f.Internet.Avatar())
			.RuleFor(u => u.UserName, (f, u) => f.Internet.UserName(u.FirstName, u.LastName))
			.RuleFor(u => u.Email, (f, u) => f.Internet.Email(u.FirstName, u.LastName));
	}

	public record class FakeData(string Name, Type Type, Func<IFake> GenerateSingle, Func<int, IFake[]> GenerateMultiple);
}
