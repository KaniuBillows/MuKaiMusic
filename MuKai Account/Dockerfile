FROM registry.cn-hangzhou.aliyuncs.com/yoyosoft/dotnet/core/aspnet:3.1 AS base
WORKDIR /app
EXPOSE 2011

COPY ./dist /app


ENV ASPNETCORE_ENVIRONMENT=Production


ENTRYPOINT ["dotnet", "Mukai Account.dll"]