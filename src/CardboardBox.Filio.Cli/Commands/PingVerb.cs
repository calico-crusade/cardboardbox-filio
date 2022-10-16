using CommandLine;

namespace CardboardBox.Filio.Cli.Commands
{
	[Verb("ping", HelpText = "Checks to see if the CLI is working as intended")]
	public class PingVerbOptions { }

	public class PingVerb : SyncVerb<PingVerbOptions>
	{
		private readonly ILogger _logger;

		public PingVerb(ILogger<PingVerb> logger)
		{
			_logger = logger;
		}

		public override int RunSync(PingVerbOptions options)
		{
			_logger.LogInformation("Pong at {0}", DateTime.Now.ToString());
			return 0;
		}
	}
}
