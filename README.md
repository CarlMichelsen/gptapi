# Build for release
```bash
dotnet build -c Release
```

# Database setup

## install entity framework cli tool
```bash
dotnet tool install --global dotnet-ef
```

## create migration if there are changes to the database
```bash
dotnet ef migrations add InitialCreate
```

## update database with latest migration
```bash
dotnet ef database update
```

## setup local docker database
make sure you have the *C:/database-data* folder on your pc

```bash
docker run -d --name development-database -e POSTGRES_PASSWORD=developer-password -v C:/database-data:/var/lib/postgresql/data --restart always -p 5432:5432 postgres
```