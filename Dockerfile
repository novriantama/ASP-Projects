# Build stage
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy csproj and restore dependencies
COPY ["ASPProjects.csproj", "./"]
RUN dotnet restore "ASPProjects.csproj"

# Copy source code and build
COPY . .
RUN dotnet publish "ASPProjects.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

EXPOSE 5000

ENTRYPOINT ["dotnet", "ASPProjects.dll"]
