FROM mcr.microsoft.com/dotnet/sdk:8.0 AS base

WORKDIR /app

EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

# Install Node.js
RUN apt-get update && apt-get install -y nodejs

WORKDIR /src

COPY ["GptApi.sln", "./"]

COPY . .

RUN dotnet restore

RUN dotnet test

RUN dotnet build "./Api" -c Release -o /app/build

FROM build AS publish

RUN dotnet publish "./Api" -c Release -o /app/publish

FROM base AS final

WORKDIR /app

COPY --from=publish /app/publish .

ENTRYPOINT ["dotnet", "./Api.dll"]