FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["NewsAPI.csproj", "."]
RUN dotnet restore
COPY . .
RUN dotnet publish -c Release -o /app

FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app .
RUN mkdir uploads && chmod 777 uploads
EXPOSE 80
ENV ASPNETCORE_URLS=http://+:${PORT}
ENTRYPOINT ["dotnet", "NewsAPI.dll"]