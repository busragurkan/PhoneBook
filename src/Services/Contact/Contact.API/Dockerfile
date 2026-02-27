FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy project files and restore
COPY ["src/Services/Contact/Contact.API/Contact.API.csproj", "src/Services/Contact/Contact.API/"]
COPY ["src/Shared/PhoneBook.Shared/PhoneBook.Shared.csproj", "src/Shared/PhoneBook.Shared/"]
RUN dotnet restore "src/Services/Contact/Contact.API/Contact.API.csproj"

# Copy source code and build
COPY . .
WORKDIR "/src/src/Services/Contact/Contact.API"
RUN dotnet build "Contact.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Contact.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Contact.API.dll"]
