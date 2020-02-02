#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app
EXPOSE 5001


FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src

# If you actually do: COPY . ., it is easier to maintain, yet slower.
COPY ["DistributedHashTable/DistributedHashTable.csproj", "DistributedHashTable/"]
COPY ["Library/Library.csproj", "Library/"]
RUN dotnet restore "DistributedHashTable/DistributedHashTable.csproj"
RUN ls -la
COPY . .
WORKDIR "/src/DistributedHashTable"
RUN dotnet build "DistributedHashTable.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "DistributedHashTable.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "DistributedHashTable.dll"]