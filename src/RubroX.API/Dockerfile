FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY ["src/RubroX.API/RubroX.API.csproj", "src/RubroX.API/"]
COPY ["src/RubroX.Application/RubroX.Application.csproj", "src/RubroX.Application/"]
COPY ["src/RubroX.Infrastructure/RubroX.Infrastructure.csproj", "src/RubroX.Infrastructure/"]
COPY ["src/RubroX.Domain/RubroX.Domain.csproj", "src/RubroX.Domain/"]
RUN dotnet restore "src/RubroX.API/RubroX.API.csproj"
COPY . .
RUN dotnet publish "src/RubroX.API/RubroX.API.csproj" -c Release -o /app/publish 

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "RubroX.API.dll"]
