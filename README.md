# CSRO

This is .NET 5 (core) application
- Client C# Blazor Mudblazor components (all  UI client Code) 
- Server C# NET 5 (all Back end Microservices REST Api projects)
- Common C# (Sdk and Common libraries to be ported anywhere)

# How to run
1. Visual Studio: 
  - latest .NET 5 installed (Latest VS or VS Code)
  - local sql server MSSQLLocalDB
  - Set secrets in projects -> Right click on Project -> Manage users secrets (Client secrets, DB and Azure Service bus connection strings)
  - or using Azure keyvaul in appsettings.json set UseKeyVault : true
2. Command Prompt, Console(s):
  - Ensure all appsettings.json set UseKeyVault : true  
  - BE/Server Apis locate and launch runServer.cmd. 3 api projects cmd console should run.
    if not all 3 are running close and launch runServer.cmd again.
  - UI localte in src folder watchClient.cmd


