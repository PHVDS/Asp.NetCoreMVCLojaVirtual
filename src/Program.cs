using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Serilog;
using Serilog.Events;
using System;
using System.IO;

namespace LojaVirtual
{
	public class Program	
	{
		public static void Main(string[] args)

		{
			string caminhoLog = Path.Combine(Directory.GetCurrentDirectory(), "Logs.txt");

			Log.Logger = new LoggerConfiguration()
				.MinimumLevel
				.Debug().MinimumLevel.Override("Microsoft", LogEventLevel.Information)
				.Enrich.FromLogContext()
				.WriteTo.File(caminhoLog)
				.CreateLogger();

			try
			{
				Log.Information("--- SERVIDOR INICIANDO ---");
				CreateWebHostBuilder(args).Build().Run();
			}
			catch (Exception ex)
			{
				Log.Fatal(ex, "--- SERVIDOR ENCERROU INESPERADAMENTE ---");
			}
			finally
			{ 
				Log.CloseAndFlush();
			}
		}

		public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
			WebHost.CreateDefaultBuilder(args)
				.UseStartup<Startup>()
				.UseSerilog();
	}
}
