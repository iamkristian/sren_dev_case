# Use the official ASP.NET Core runtime as the base image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80

# Use the official ASP.NET Core SDK as the build image
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["BriefingService.csproj", "BriefingService/"]
RUN dotnet restore "BriefingService/BriefingService.csproj"
COPY . BriefingService/
WORKDIR "/src/BriefingService"
RUN dotnet build "BriefingService.csproj" -c Release -o /app/build

# Publish the application
FROM build AS publish
RUN dotnet publish "BriefingService.csproj" -c Release -o /app/publish

# Final stage/image
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "BriefingService.dll"]
