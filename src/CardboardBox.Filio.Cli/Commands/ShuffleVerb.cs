using CommandLine;

namespace CardboardBox.Filio.Cli.Commands
{
	using Core.Utilities;
	using FakeData;

	[Verb("shuffle", HelpText = "Shuffles the data within a fake data set")]
	public class ShuffleVerbOptions
	{
		[Option('c', "count", Default = 2, HelpText = "How many properties to shuffle")]
		public int Count { get; set; }

		[Option('t', "type", Required = true, HelpText = "The type of fake data in the csv (Supports: address, user)")]
		public string Type { get; set; } = string.Empty;

		[Option('i', "input", HelpText = "The target input file", Default = "output.csv")]
		public string InputPath { get; set; } = string.Empty;

		[Value(0, HelpText = "The file path to save the CSV to", Default = "output-mod.csv")]
		public string OutputPath { get; set; } = string.Empty;
	}

	public class ShuffleVerb : IVerb<ShuffleVerbOptions>
	{
		private Random _rnd => LinqExtensions.RandomInstance;
		private readonly ICsvService _csv;
		private readonly ILogger _logger;
		private readonly IFakeDataService _fake;

		public ShuffleVerb(
			ICsvService csv,
			ILogger<ShuffleVerb> logger,
			IFakeDataService fake)
		{
			_csv = csv;
			_logger = logger;
			_fake = fake;
		}

		public async Task<int> Run(ShuffleVerbOptions options)
		{
			var src = _fake.DetermineFaker(options.Type);
			if (src == null)
			{
				_logger.LogError("{0} is not a valid type", options.Type);
				return 1;
			}

			Func<ShuffleVerbOptions, FakeData, Task> task = options.Type switch
			{
				"address" => Handle<FakeAddress>,
				"user" => Handle<FakeUser>,
				_ => throw new NotImplementedException("Unknown data type")
			};

			await task(options, src);
			return 0;
		}

		public async Task Handle<T>(ShuffleVerbOptions opts, FakeData source) where T : IFake
		{
			var data = _csv.ReadFile<T>(opts.InputPath);
			using var io = File.Create(opts.OutputPath);
			var modData = Shuffle(data, source, opts.Count);
			await _csv.Write(io, modData);
		}

		public async IAsyncEnumerable<T> Shuffle<T>(IAsyncEnumerable<T> data, FakeData source, int count) where T: IFake
		{
			await foreach(var record in data)
			{
				var shouldMod = _rnd.Next(0, 100) > 50;
				if (shouldMod) _fake.Shuffle(source.Name, record, count);
				yield return record;
			}
		}
	}
}
