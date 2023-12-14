# Database setup

## install entity framework cli tool
dotnet tool install --global dotnet-ef

## create migration if there are changes to the database
dotnet ef migrations add InitialCreate

## update database with latest migration
dotnet ef database update

## setup local docker database
make sure you have the *C:/database-data* folder on your pc

docker run -d --name development-database -e POSTGRES_PASSWORD=developer-password -v C:/database-data:/var/lib/postgresql/data --restart always -p 5432:5432 postgres