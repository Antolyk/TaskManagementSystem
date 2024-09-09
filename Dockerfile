# Базовий образ для побудови
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Копіювання всіх проектів і відновлення залежностей
COPY *.sln .
COPY ["TaskManagementSystem/TaskManagementSystem.csproj", "TaskManagementSystem/"]
COPY ["TaskManagementSystem.Data/TaskManagementSystem.Data.csproj", "TaskManagementSystem.Data/"]
COPY ["TaskManagementSystem.Service/TaskManagementSystem.Service.csproj", "TaskManagementSystem.Service/"]
COPY ["TaskManagementSystem.Tests/TaskManagementSystem.Tests.csproj", "TaskManagementSystem.Tests/"]
RUN dotnet restore "TaskManagementSystem/TaskManagementSystem.csproj"

# Копіювання решти файлів і збірка проєктів
COPY . .
WORKDIR "/src/TaskManagementSystem"
RUN apt-get update && apt-get install -y libgssapi-krb5-2
RUN dotnet build "TaskManagementSystem.csproj" -c Release -o /app/build

# Публікація проєкту
FROM build AS publish
RUN dotnet publish "TaskManagementSystem.csproj" -c Release -o /app/publish

# Фінальний образ для запуску
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
EXPOSE 5000
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TaskManagementSystem.dll"]
