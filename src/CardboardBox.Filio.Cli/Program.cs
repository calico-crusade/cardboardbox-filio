using CardboardBox;
using CardboardBox.Filio;
using CardboardBox.Filio.Cli.Commands;
using CardboardBox.Filio.Cli.FakeData;

return await new ServiceCollection()
	.AddSerilog()
	.AddCore()
	.AddTransient<IFakeDataService, FakeDataService>()
	.Cli(c =>
	{
		c.Add<PingVerb, PingVerbOptions>()
		 .Add<FakeVerb, FakeVerbOptions>()
		 .Add<ShuffleVerb, ShuffleVerbOptions>()
		 .Add<TestVerb, TestVerbOptions>();
	});