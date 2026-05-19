FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build

WORKDIR /src

COPY ["src/DotNet.Api/DotNet.Api.csproj", "DotNet.Api/"]

RUN dotnet restore "DotNet.Api/DotNet.Api.csproj"

COPY src/DotNet.Api/ DotNet.Api/

WORKDIR /src/DotNet.Api

RUN dotnet publish "DotNet.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false


FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final

WORKDIR /app

COPY --from=build /app/publish .

EXPOSE 8080

ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production

USER 1654

ENTRYPOINT ["dotnet", "DotNet.Api.dll"]