using CommandLine;

namespace CardboardBox.Filio.Cli.Commands
{
	using Core.Utilities;

	[Verb("test")]
	public class TestVerbOptions { }
	public class TestVerb : SyncVerb<TestVerbOptions>
	{
		private readonly IFormatService _format;

		public TestVerb(IFormatService format)
		{
			_format = format;
		}

		public override int RunSync(TestVerbOptions options)
		{
			var args = new Dictionary<string, Func<object?>>
			{
				["me"] = () => "Cardboard",
				["test"] = () => 999.9999
			};

			var tests = new[]
			{
				"You owe me: ${test:0.00}. That's right, I'm talking to you, {me}. And you owed it to me {now} ({now:yyyy/MM/dd} <-> {now:HH:mm}).",
				"This one is \\{escaped} though! {me} you better get me my money! Or are you actually \"{env.osversion}\"?"
			};

			foreach (var test in tests)
				Console.WriteLine(_format.Format(test, args));

			return 0;
		}
	}
}
