FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY ["PapeleriaDB.Api/PapeleriaDB.Api.csproj", "PapeleriaDB.Api/"]
COPY ["PapeleriaDB.Application/PapeleriaDB.Application.csproj", "PapeleriaDB.Application/"]
COPY ["PapeleriaDB.Domain/PapeleriaDB.Domain.csproj", "PapeleriaDB.Domain/"]
COPY ["PapeleriaDB.Infrastructure/PapeleriaDB.Infrastructure.csproj", "PapeleriaDB.Infrastructure/"]
COPY ["PapeleriaDB.Persistence/PapeleriaDB.Persistence.csproj", "PapeleriaDB.Persistence/"]
COPY ["PapeleriaDB.Shared/PapeleriaDB.Shared.csproj", "PapeleriaDB.Shared/"]
RUN dotnet restore "PapeleriaDB.Api/PapeleriaDB.Api.csproj"

COPY . .
RUN dotnet publish "PapeleriaDB.Api/PapeleriaDB.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
ENV ASPNETCORE_ENVIRONMENT=Production
ENV DOTNET_EnableDiagnostics=0
EXPOSE 10000
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "PapeleriaDB.Api.dll"]
