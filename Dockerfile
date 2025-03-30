FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy csproj files and restore as distinct layers
COPY ["E_Commerce.API/E_Commerce.API.csproj", "E_Commerce.API/"]
COPY ["E_Commerce.Application/E_Commerce.Application.csproj", "E_Commerce.Application/"]
COPY ["E_Commerce.Domain/E_Commerce.Domain.csproj", "E_Commerce.Domain/"]
COPY ["E_Commerce.Infrastructure/E_Commerce.Infrastructure.csproj", "E_Commerce.Infrastructure/"]
RUN dotnet restore "E_Commerce.API/E_Commerce.API.csproj"

# Copy everything else and build app
COPY . .
WORKDIR "/src/E_Commerce.API"
RUN dotnet build "E_Commerce.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "E_Commerce.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .

ENV ASPNETCORE_ENVIRONMENT=Production
ENV ConnectionStrings__DefaultConnection="Server=sqlserver;Database=ECommerceDb;User Id=sa;Password=Strong_Password123!;TrustServerCertificate=True;MultipleActiveResultSets=true"

EXPOSE 80
EXPOSE 443

# Set the entry point
ENTRYPOINT ["dotnet", "E_Commerce.API.dll"] 