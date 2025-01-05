dotnet new tool-manifest
dotnet tool install dotnet-ef
dotnet tool restore

dotnet ef migrations add CoreDbContext_InitialMigration --context CoreDbContext
dotnet ef database update --context CoreDbContext
