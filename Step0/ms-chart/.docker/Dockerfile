FROM mcr.microsoft.com/dotnet/sdk:5.0-alpine3.12 AS build-env

WORKDIR /app

COPY . .

RUN dotnet publish -c Release -o ./out

#----------------
FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS runtime

ENV APP_HOME=/app
WORKDIR $APP_HOME
ENV ASPNETCORE_URLS=http://*:5000

ARG ENV
ENV ASPNETCORE_ENVIRONMENT Development
COPY --from=build-env /app/out $APP_HOME
EXPOSE 5000

ENTRYPOINT ["dotnet", "ms-chart.dll"]