# Task Management System

## Overview

This project is a simple task management application with user authentication. It allows users to create, update, delete, and view tasks associated with their accounts. The project is implemented using .NET 8, Entity Framework Core, and JWT for authentication.

## Requirements

1. **Language & Framework:** .NET 8, Entity Framework Core
2. **Database:** SQL Server Express
3. **Version Control:** Git

## Project Setup

1. Clone the repository:
   ```bash
   git clone <Your_Repository_URL>
   cd <Project_Name>
2. Install dependencies:
   ```bash
   dotnet restore
3. Configure settings: Update a file appsettings.Development.json in the root directory of the project with the following content:
  ```json
  {
    "Jwt": {
      "Token": "MyVeryLongSuperPuperSecureAndRandomTokenHereForThisTaskManager!!!!!!"
    },
    "ConnectionStrings": {
      "DefaultConnection": "Server=.\\SQLExpress;Database=TaskManagerDB;Trusted_Connection=true;TrustServerCertificate=true;"
    }
  }
  ```
4. Apply database migrations:
   ```bash
   dotnet ef database update
5. Run the project:
   ```bash
   dotnet run
---
## Architecture

The project uses a 3-tier architecture with the following layers:
- **Controllers**: Handles HTTP requests and responses.
- **Service Layer**: Implements business logic.
- **Data Layer**: Data access via Entity Framework Core.

## API Endpoints

### Authentication

#### Register User

- **POST** `/api/users/register`
- **Request Body**: `UserDto` (username, email, password)
- **Response**: `UserServiceModel`

#### Login

- **POST** `/api/users/login`
- **Request Body**: `UserDto` (username, email, password)
- **Response**: `JWT token`

### Task Management (authentication required)

#### Create Task

- **POST** `/api/tasks`
- **Request Body**: `TaskDto` (title, description, dueDate, status, priority)
- **Response**: `TaskServiceModel`

#### Get Task by ID

- **GET** `/api/tasks/{id}`
- **Query Parameters**: `id`
- **Response**: `TaskServiceModel`

#### Get All Tasks

- **GET** `/api/tasks`
- **Query Parameters**: `status`, `dueDate`, `priority`, `sortField`, `sortOrder`, `pageNumber`, `pageSize`
- **Response**: `IEnumerable<TaskServiceModel>`

#### Update Task

- **PUT** `/api/tasks/{id}`
- **Request Body**: `TaskDto` (title, description, dueDate, status, priority)
- **Response**: `TaskServiceModel`

#### Delete Task

- **DELETE** `/api/tasks/{id}`
- **Query Parameters**: `id`
- **Response**: `result`

## Best Practices

- Using JWT for authentication
- Storing passwords in hashed form
- Logging critical operations
- Using Dependency Injection for services and repositories

## Documentation

For more detailed information on the architecture and project implementation, please refer to the code comments and the API documentation above.

## Additional Information

- **Unit Testing**: Unit tests are implemented for key components.
- **Docker**: Dockerfile and Docker Compose configuration are provided to run the application and database in containers.
- **CI/CD**: A basic CI/CD pipeline is set up using GitHub Actions for build and test automation.
