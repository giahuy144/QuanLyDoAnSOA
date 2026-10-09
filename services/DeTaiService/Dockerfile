FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["services/DeTaiService/DeTaiService.csproj", "services/DeTaiService/"]
RUN dotnet restore "services/DeTaiService/DeTaiService.csproj"
COPY . .
WORKDIR "/src/services/DeTaiService"
RUN dotnet build "DeTaiService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "DeTaiService.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "DeTaiService.dll"]
