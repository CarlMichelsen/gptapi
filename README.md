---install tool
dotnet tool install --global dotnet-ef

---create initial migration
dotnet ef migrations add InitialCreate
dotnet ef database update