# --- BUILD STAGE ---
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copio i csproj prima per sfruttare la cache Docker (restore più veloce)
COPY Eshop.Server.Api/Eshop.Server.Api.csproj Eshop.Server.Api/
COPY Eshop.Server.Application/Eshop.Server.Application.csproj Eshop.Server.Application/
COPY Eshop.Server.Domain/Eshop.Server.Domain.csproj Eshop.Server.Domain/
COPY Eshop.Server.Infrastructure/Eshop.Server.Infrastructure.csproj Eshop.Server.Infrastructure/

# Se hai anche una solution file (.sln), copiala (opzionale ma utile)
# COPY Eshop.sln ./

RUN dotnet restore Eshop.Server.Api/Eshop.Server.Api.csproj

# Ora copio tutto il resto del codice
COPY . .

# Pubblico l'API (include automaticamente gli altri progetti)
RUN dotnet publish Eshop.Server.Api/Eshop.Server.Api.csproj -c Release -o /app/publish /p:UseAppHost=false

# --- RUNTIME STAGE ---
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app

# La tua API ascolta su 8080 dentro il container
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Eshop.Server.Api.dll"]
