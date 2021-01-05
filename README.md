# CSRO


Set up cache 

https://github.com/Azure-Samples/active-directory-aspnetcore-webapp-openidconnect-v2/tree/master/2-WebApp-graph-user/2-2-TokenCache

-create db CSRO.TokenCacheDb in (localdb)\MSSQLLocalDB

- run in cmd
dotnet tool install --global dotnet-sql-cache 
dotnet sql-cache create "Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=CSRO.TokenCacheDb;Integrated Security=True;" dbo TokenCache


- statup.cs
services.AddDistributedSqlServerCache(options =>
{
    options.ConnectionString = Configuration.GetConnectionString("TokenCacheDbConnStr");
    options.SchemaName = "dbo";
    options.TableName = "TokenCache";
});


- app settings
"ConnectionStrings": {
"TokenCacheDbConnStr": "Data Source=(LocalDb)\\MSSQLLocalDB;Database=CSRO.TokenCacheDb;Trusted_Connection=True;"
},
