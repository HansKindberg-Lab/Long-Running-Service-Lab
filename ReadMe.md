# Long-Running-Service-Lab

This is a solution for laborating with long running services.

## 1 Background

- [Long running REST API hosted on OpenShift – connection closed after 5 minutes](https://hanskindberg.wordpress.com/2023/03/23/long-running-rest-api-hosted-on-openshift-connection-closed-after-5-minutes/)

## 2 Development

### 2.1 Migrations

We might want to create/recreate/update migrations. If we can accept data-loss we can recreate the migrations otherwhise we will have to update them. For each update we need to bump the migration-name suffix:

- Initial migration: "Function"
- First update: "Function1"
- Second update: "Function2"

Copy all the commands below and run them in the Package Manager Console.

If you want more migration-information you can add the -Verbose parameter:

	Add-Migration Function -Context FunctionContext -OutputDir Some/Path/To/Your/Migrations -Project Service -StartupProject Service -Verbose;

#### 2.1.1 Create migrations

	Write-Host "Removing migrations...";
	Remove-Migration -Context SqliteOperationContext -Force -Project Service -StartupProject Service;
	Remove-Migration -Context SqlServerOperationContext -Force -Project Service -StartupProject Service;
	Write-Host "Removing current migrations-directory...";
	Remove-Item "Service\Models\Data\Migrations" -ErrorAction Ignore -Force -Recurse;
	Write-Host "Creating migrations...";
	Add-Migration Operation -Context SqliteOperationContext -OutputDir Models/Data/Migrations/Sqlite -Project Service -StartupProject Service;
	Add-Migration Operation -Context SqlServerOperationContext -OutputDir Models/Data/Migrations/SqlServer -Project Service -StartupProject Service;
	Write-Host "Finnished";

#### 2.1.2 Update migrations

	Write-Host "Updating migrations...";
	Add-Migration Operation1 -Context SqliteOperationContext -OutputDir Models/Data/Migrations/Sqlite -Project Service -StartupProject Service;
	Add-Migration Operation1 -Context SqlServerOperationContext -OutputDir Models/Data/Migrations/SqlServer -Project Service -StartupProject Service;
	Write-Host "Finnished";

#### 2.1.3 Notes

You may/will get [Fatal] logs in the output when you run the scripts above:

	[11:21:52 INF] Starting host ...
	2023-03-25 11:21:52.874 +01:00 - [Fatal] - : Host terminated unexpectedly.
	Microsoft.Extensions.Hosting.HostAbortedException: The host was aborted.
	   at Microsoft.Extensions.Hosting.HostFactoryResolver.HostingListener.ThrowHostAborted()
	   at Microsoft.Extensions.Hosting.HostFactoryResolver.HostingListener.OnNext(KeyValuePair`2 value)
	   at System.Diagnostics.DiagnosticListener.Write(String name, Object value)
	   at Microsoft.Extensions.Hosting.HostBuilder.ResolveHost(IServiceProvider serviceProvider, DiagnosticListener diagnosticListener)
	   at Microsoft.Extensions.Hosting.HostApplicationBuilder.Build()
	   at Microsoft.AspNetCore.Builder.WebApplicationBuilder.Build()

This is expected. In my opinion you can leave the logging and just ignore it. This happens when we create migrations and not when we run the application. If you want to avoid the log you can find more information here:

- [Using EntityFramework Core for configuration and operational data - Handle Expected Exception](https://docs.duendesoftware.com/identityserver/v6/quickstarts/4_ef/#handle-expected-exception)

## 3 Links

- [Migrations with Multiple Providers](https://docs.microsoft.com/en-us/ef/core/managing-schemas/migrations/providers/)
- [HTTP Status 202 (Accepted)](https://restfulapi.net/http-status-202-accepted/)
- [Long running ReST requests and status endpoints](https://dunnhq.com/posts/2021/long-running-rest-requests/)
- [REST API Best Practices — Decouple Long-running Tasks from HTTP Request Processing](https://medium.com/geekculture/rest-api-best-practices-decouple-long-running-tasks-from-http-request-processing-9fab2921ace8)