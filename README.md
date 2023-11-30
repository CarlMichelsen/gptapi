# install tool
dotnet tool install --global dotnet-ef

# create initial migration
dotnet ef migrations add InitialCreate
dotnet ef database update

# local database
make sure you have the *C:/database-data* folder on your pc

docker run -d --name development-database -e POSTGRES_PASSWORD=developer-password -v C:/database-data:/var/lib/postgresql/data --restart always -p 5432:5432 postgres