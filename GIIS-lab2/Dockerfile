FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["GIIS-lab2/GIIS-lab2.csproj", "GIIS-lab2/"]
RUN dotnet restore "GIIS-lab2/GIIS-lab2.csproj"
COPY . .
WORKDIR "/src/GIIS-lab2"
RUN dotnet build "GIIS-lab2.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "GIIS-lab2.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "GIIS-lab2.dll"]