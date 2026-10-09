FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["services/DangKyService/DangKyService.csproj", "services/DangKyService/"]
RUN dotnet restore "services/DangKyService/DangKyService.csproj"
COPY . .
WORKDIR "/src/services/DangKyService"
RUN dotnet build "DangKyService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "DangKyService.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "DangKyService.dll"]
