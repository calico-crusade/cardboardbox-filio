namespace CardboardBox.Filio
{
	using CardboardBox.Filio.Core.Utilities;
	using Core;

	public static class DiExtensions
	{
		public static IServiceCollection AddCore(this IServiceCollection services)
		{
			return services
				.AddTransient<ICsvService, CsvService>()
				.AddTransient<IFormatService, FormatService>()
				.AddTransient<IFilioService, FilioService>();
		}
	}
}
