using System;
using System.ComponentModel;
using System.Net;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.HttpOverrides;
using Serilog;
using Service.Models.Builder.Extensions;
using Service.Models.ComponentModel;
using Service.Models.DependencyInjection.Extensions;

Log.Logger = new LoggerConfiguration().WriteTo.Console().CreateBootstrapLogger();
Log.Information("Starting host ...");

try
{
	TypeDescriptor.AddAttributes(typeof(IPAddress), new TypeConverterAttribute(typeof(IpAddressTypeConverter)));
	TypeDescriptor.AddAttributes(typeof(IPNetwork), new TypeConverterAttribute(typeof(IpNetworkTypeConverter)));

	var builder = WebApplication.CreateBuilder(args);

	builder.Host.UseSerilog((hostBuilderContext, serviceProvider, loggerConfiguration) =>
	{
		loggerConfiguration.ReadFrom.Configuration(hostBuilderContext.Configuration);
		loggerConfiguration.ReadFrom.Services(serviceProvider);
	});

	builder.Services.Add(builder.Configuration, builder.Environment);

	var application = builder.Build();
	application.Use();
	application.Run();
}
catch(Exception exception)
{
	Log.Fatal(exception, "Host terminated unexpectedly.");
}
finally
{
	Log.Information("Stopping host ...");
	Log.CloseAndFlush();
}