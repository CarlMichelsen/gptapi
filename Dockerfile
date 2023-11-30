FROM mcr.microsoft.com/dotnet/sdk:8.0 AS base

WORKDIR /app

EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

# Install Node.js manually
RUN apt-get update && \
    apt-get install -y curl xz-utils && \
    curl -fsSL https://nodejs.org/dist/v20.0.0/node-v20.0.0-linux-x64.tar.xz | tar -xJ && \
    mv node-v20.0.0-linux-x64 /usr/local/lib/nodejs && \
    ln -s /usr/local/lib/nodejs/bin/node /usr/local/bin/node && \
    ln -s /usr/local/lib/nodejs/bin/npm /usr/local/bin/npm && \
    ln -s /usr/local/lib/nodejs/bin/npx /usr/local/bin/npx

# Verify the installation of Node.js and npm
RUN node --version
RUN npm --version

WORKDIR /src

COPY ["GptApi.sln", "./"]

COPY . .

RUN dotnet restore

RUN dotnet test

RUN dotnet build "./Api" -c Release --output /app/build

FROM build AS publish

RUN dotnet publish "./Api" -c Release --output /app/publish

FROM base AS final

WORKDIR /app

COPY --from=publish /app/publish .

ENTRYPOINT ["dotnet", "./Api.dll"]