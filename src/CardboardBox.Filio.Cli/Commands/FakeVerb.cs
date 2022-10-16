using CommandLine;

namespace CardboardBox.Filio.Cli.Commands
{
	using Core.Utilities;
	using FakeData;

	[Verb("fake", HelpText = "Generates a fake CSV full of address data")]
	public class FakeVerbOptions
	{
		[Option('c', "count", HelpText = "How many records to populate the CSV with", Default = 1000)]
		public int Count { get; set; }

		[Option('t', "type", HelpText = "The type of data to generate (Supports: address, user). Defaults to: address", Default = "address")]
		public string Type { get; set; } = string.Empty;

		[Value(0, HelpText = "The file path to save the CSV to", Default = "output.csv")]
		public string Path { get; set; } = string.Empty;
	}

	public class FakeVerb : IVerb<FakeVerbOptions>
	{
		private readonly ICsvService _csv;
		private readonly ILogger _logger;
		private readonly IFakeDataService _fake;

		public FakeVerb(
			ICsvService csv, 
			ILogger<FakeVerb> logger,
			IFakeDataService fake)
		{
			_csv = csv;
			_logger = logger;
			_fake = fake;
		}

		public async Task<int> Run(FakeVerbOptions options)
		{
			var src = _fake.DetermineFaker(options.Type);
			var data = _fake.Generate(options.Type, options.Count);
			if (src == null || data == null)
			{
				_logger.LogError("{0} is not a valid type", options.Type);
				return 1;
			}

			switch(data)
			{
				case FakeUser[] users:
					await _csv.Write(options.Path, users);
					break;

				case FakeAddress[] addresses:
					await _csv.Write(options.Path, addresses);
					break;

				default:
					_logger.LogError("Unknown data type");
					return 1;
			}

			return 0;
		}
	}
}
