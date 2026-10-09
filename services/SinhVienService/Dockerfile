FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["services/SinhVienService/SinhVienService.csproj", "services/SinhVienService/"]
RUN dotnet restore "services/SinhVienService/SinhVienService.csproj"
COPY . .
WORKDIR "/src/services/SinhVienService"
RUN dotnet build "SinhVienService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "SinhVienService.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "SinhVienService.dll"]
