FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY ["identity/RubroX.Identity/RubroX.Identity.csproj", "identity/RubroX.Identity/"]
RUN dotnet restore "identity/RubroX.Identity/RubroX.Identity.csproj"
COPY . .
RUN dotnet publish "identity/RubroX.Identity/RubroX.Identity.csproj" -c Release -o /app/publish 

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "RubroX.Identity.dll"]
