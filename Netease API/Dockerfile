FROM mcr.microsoft.com/dotnet/core/aspnet:3.1.0 AS base
WORKDIR /app
EXPOSE 2003

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Netease API","Netease API"]
COPY ["DataAbstract","DataAbstract"]

RUN dotnet restore "Netease API/Netease API/Netease API.csproj"
# COPY . .
WORKDIR "/src/Netease API/Netease API"

RUN dotnet build "Netease API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Netease API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app

ENV ASPNETCORE_ENVIRONMENT=Production

COPY --from=publish /app/publish .

ENTRYPOINT ["dotnet", "Netease API.dll"]