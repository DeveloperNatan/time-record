FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /app

COPY *.csproj .
COPY *.sln .
RUN dotnet restore TimeRecord.sln

COPY . .
RUN dotnet publish TimeRecord.sln -c Release -o out

FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app

EXPOSE 8080
ENV ASPNETCORE_HTTP_PORTS=8080

COPY --from=build /app/out .
ENTRYPOINT ["dotnet", "TimeRecord.dll"]
