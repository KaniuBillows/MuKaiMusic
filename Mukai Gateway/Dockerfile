FROM registry.cn-hangzhou.aliyuncs.com/yoyosoft/dotnet/core/aspnet:3.1 AS base
WORKDIR /app
EXPOSE 9999

FROM registry.cn-hangzhou.aliyuncs.com/yoyosoft/dotnet/core/sdk AS build
WORKDIR /src
COPY ["Mukai Gateway/Mukai Gateway.csproj","Mukai Gateway/"]

RUN dotnet restore "Mukai Gateway/Mukai Gateway.csproj"
COPY . .
WORKDIR "/src/Mukai Gateway/"
RUN dotnet build "Mukai Gateway.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Mukai Gateway.csproj" -c Release -o /app/publish


FROM base AS final
WORKDIR /app

ENV ASPNETCORE_ENVIRONMENT=Production

COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Mukai Gateway.dll"]