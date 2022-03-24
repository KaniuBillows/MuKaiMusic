FROM mcr.microsoft.com/dotnet/core/aspnet:3.1.0 AS base
WORKDIR /app
EXPOSE 2002
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build

WORKDIR /src
COPY ["Kuwo API","Kuwo API"]
COPY ["DataAbstract","DataAbstract"]

RUN dotnet restore "Kuwo API/Kuwo API/Kuwo API.csproj"
# COPY . .
WORKDIR "/src/Kuwo API/Kuwo API"

RUN dotnet build "Kuwo API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Kuwo API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app

ENV ASPNETCORE_ENVIRONMENT=Production

COPY --from=publish /app/publish .


ENTRYPOINT ["dotnet", "Kuwo API.dll"]