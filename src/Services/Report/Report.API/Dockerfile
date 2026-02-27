FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy project files and restore
COPY ["src/Services/Report/Report.API/Report.API.csproj", "src/Services/Report/Report.API/"]
COPY ["src/Shared/PhoneBook.Shared/PhoneBook.Shared.csproj", "src/Shared/PhoneBook.Shared/"]
RUN dotnet restore "src/Services/Report/Report.API/Report.API.csproj"

# Copy source code and build
COPY . .
WORKDIR "/src/src/Services/Report/Report.API"
RUN dotnet build "Report.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Report.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Report.API.dll"]
