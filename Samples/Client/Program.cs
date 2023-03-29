using Client.Models.Configuration;
using Client.Models.ServiceClients;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpClient<ServiceClient>();
builder.Services.AddRazorPages();
builder.Services.AddSingleton<IServiceConnectionOptionsParser, ServiceConnectionOptionsParser>();
builder.Services.AddTransient<IServiceClient>(serviceProvider => serviceProvider.GetRequiredService<ServiceClient>());

var app = builder.Build();

app.UseStaticFiles();
app.UseRouting();
app.MapRazorPages();

app.Run();