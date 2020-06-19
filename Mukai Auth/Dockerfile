FROM registry.cn-hangzhou.aliyuncs.com/yoyosoft/dotnet/core/aspnet:3.1 AS base
WORKDIR /app
EXPOSE 3000

FROM registry.cn-hangzhou.aliyuncs.com/yoyosoft/dotnet/core/sdk AS build
WORKDIR /src
COPY ["Mukai Auth/Mukai Auth.csproj", "Mukai Auth/"]
COPY ["DataAbstract/DataAbstract.csproj", "DataAbstract/"]
RUN dotnet restore "Mukai Auth/Mukai Auth.csproj"
COPY . .
WORKDIR "/src/Mukai Auth"
RUN dotnet build "Mukai Auth.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Mukai Auth.csproj" -c Release -o /app/publish


FROM base AS final
WORKDIR /app

ENV ASPNETCORE_ENVIRONMENT=Production

COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Mukai Auth.dll"]