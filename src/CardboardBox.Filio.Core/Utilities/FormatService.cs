namespace CardboardBox.Filio.Core.Utilities
{
	using Utilities;

	public interface IFormatService
	{
		string Format(string input,
			Dictionary<string, Func<object?>>? arguments = null,
			TokenParserConfig? parserConfig = null);

		Dictionary<string, Func<object?>> ToArgFormat(Dictionary<string, object> dic);
	}

	public class FormatService : IFormatService
	{
		public string Format(string input,
			Dictionary<string, Func<object?>>? arguments = null,
			TokenParserConfig? parserConfig = null)
		{
			var args = CombineArguments(arguments);
			var parser = new TokenParser(input, parserConfig);
			var tokens = parser.Tokens();

			var output = input;
			foreach (var (tag, opt, start, length) in tokens)
			{
				if (!args.ContainsKey(tag.ToLower())) continue;

				var actualTag = input.Substring(start, length);
				var value = args[tag.ToLower()]() ?? "";
				var strval = !string.IsNullOrEmpty(opt) &&
					value is IFormattable form ?
						form.ToString(opt, null) :
						value.ToString();

				output = output.Replace(actualTag, strval);
			}

			return output;
		}

		public Dictionary<string, Func<object?>> DefaultArguments()
		{
			var dic = new Dictionary<string, Func<object?>>()
			{
				["now"] = () => DateTime.Now,

				//Environment.*
				["env.Is64BitOperatingSystem"] = () => Environment.Is64BitOperatingSystem,
				["env.Is64BitProcess"] = () => Environment.Is64BitProcess,
				["env.OSVersion"] = () => Environment.OSVersion,
				["env.HasShutdownStarted"] = () => Environment.HasShutdownStarted,
				["env.ExitCode"] = () => Environment.ExitCode,
				["env.CurrentManagedThreadId"] = () => Environment.CurrentManagedThreadId,
				["env.CurrentDirectory"] = () => Environment.CurrentDirectory,
				["env.CommandLine"] = () => Environment.CommandLine,
				["env.ProcessPath"] = () => Environment.ProcessPath,
				["env.ProcessId"] = () => Environment.ProcessId,
				["env.MachineName"] = () => Environment.MachineName,
				["env.ProcessorCount"] = () => Environment.ProcessorCount,
				["env.SystemDirectory"] = () => Environment.SystemDirectory,
				["env.SystemPageSize"] = () => Environment.SystemPageSize,
				["env.TickCount"] = () => Environment.TickCount,
				["env.TickCount64"] = () => Environment.TickCount64,
				["env.UserDomainName"] = () => Environment.UserDomainName,
				["env.UserInteractive"] = () => Environment.UserInteractive,
				["env.UserName"] = () => Environment.UserName,
				["env.Version"] = () => Environment.Version,
				["env.WorkingSet"] = () => Environment.WorkingSet,
				["env.StackTrace"] = () => Environment.StackTrace,
				["env.NewLine"] = () => Environment.NewLine,
			};

			var flags = Enum.GetValues<Environment.SpecialFolder>() ?? Array.Empty<Environment.SpecialFolder>();

			foreach (var flag in flags)
			{
				var key = "folder." + flag.ToString();
				if (dic.ContainsKey(key)) continue;

				dic.Add(key, () => Environment.GetFolderPath(flag, Environment.SpecialFolderOption.None));
			}
			return dic;
		}

		public Dictionary<string, Func<object?>> CombineArguments(params Dictionary<string, Func<object?>>?[] args)
		{
			var defaults = DefaultArguments();
			foreach (var dic in args)
			{
				if (dic == null) continue;

				foreach (var (key, value) in dic)
				{
					if (!defaults.ContainsKey(key))
					{
						defaults.Add(key, value);
						continue;
					}

					defaults[key] = value;
				}
			}

			return defaults.ToDictionary(t => t.Key.ToLower(), t => t.Value);
		}

		public Dictionary<string, Func<object?>> ToArgFormat(Dictionary<string, object> dic)
		{
			return dic.ToDictionary(t => t.Key, t =>
			{
				object? val() => t.Value;
				return (Func<object?>)val;
			});
		}
	}
}
