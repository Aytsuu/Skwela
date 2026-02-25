# 🎓 Skwela

<div align="center">

[![.NET](https://img.shields.io/badge/.NET-512BD4?logo=dotnet&logoColor=white)](https://dotnet.microsoft.com/)
[![Next.js](https://img.shields.io/badge/Next.js-000000?logo=nextdotjs&logoColor=white)](https://nextjs.org/)
[![TypeScript](https://img.shields.io/badge/TypeScript-3178C6?logo=typescript&logoColor=white)](https://www.typescriptlang.org/)
[![React](https://img.shields.io/badge/React-61DAFB?logo=react&logoColor=black)](https://react.dev/)
[![License](https://img.shields.io/badge/License-Proprietary-red)](LICENSE)

A comprehensive full-stack application for classroom management and educational administration. Built with ASP.NET Core on the backend and Next.js on the frontend.

</div>

## Overview

Skwela is a modern, scalable platform designed to streamline classroom operations, student enrollment, and assignment management. The application follows clean architecture principles with a well-structured separation of concerns.

### 🛠️ Tech Stack

**Backend:**
- 🔵 **ASP.NET Core** (.NET) - Web API Framework
- 🗄️ **Entity Framework Core** - ORM & Database Access
- 💾 **SQL Server** - Relational Database
- 🔐 **JWT Authentication** - Secure Token-Based Auth

**Frontend:**
- ⚡ **Next.js 16+** - React Meta-Framework
- 📘 **TypeScript** - Type-Safe JavaScript
- 🎨 **TailwindCSS** - Utility-First CSS Framework
- 🔄 **React Query** - Server State Management
- 🧩 **Radix UI** - Headless Component Library

## Project Structure

```
Skwela/
├── backend/                          # .NET Backend
│   ├── Skwela.API/                   # REST API & Controllers
│   ├── Skwela.Application/           # Business Logic & Use Cases
│   ├── Skwela.Domain/                # Domain Entities & Models
│   ├── Skwela.Infrastructure/        # Data Access & Services
│   ├── Skwela.sln                    # Solution File
│   ├── docker-compose.yml            # Docker Compose Configuration
│   ├── Dockerfile                    # Docker Image Definition
│   └── init_docker.sql               # Database Initialization
│
└── frontend/                         # Next.js Frontend
    ├── src/
    │   ├── app/                      # Application Pages & Layout
    │   ├── components/               # Reusable React Components
    │   ├── hooks/                    # Custom React Hooks
    │   ├── lib/                      # Utility Functions
    │   ├── schemas/                  # Data Validation Schemas
    │   ├── services/                 # API Services
    │   └── types/                    # TypeScript Type Definitions
    ├── package.json                  # Dependencies & Scripts
    ├── tsconfig.json                 # TypeScript Configuration
    └── next.config.ts                # Next.js Configuration
```

## Architecture

### Backend Architecture (Clean Architecture)

The backend is organized into four main layers:

1. **Domain Layer** (`Skwela.Domain`)
   - Core business entities (User, Classroom, Enrollment, Assignment, Submission)
   - Business logic enums (UserRole)
   - Custom exceptions

2. **Application Layer** (`Skwela.Application`)
   - Business use cases and orchestration
   - DTOs (Data Transfer Objects)
   - Service interfaces
   - Repository interfaces

3. **Infrastructure Layer** (`Skwela.Infrastructure`)
   - Entity Framework Core DbContext
   - Database migrations
   - Repository implementations
   - External service integrations

4. **API Layer** (`Skwela.API`)
   - REST API controllers
   - Request/response handling
   - Authentication/Authorization
   - Dependency injection setup

### Frontend Architecture

The frontend uses a feature-based organization:
- **Pages & Routing**: Next.js App Router for navigation
- **Components**: Reusable UI components using Radix UI primitives
- **Services**: Centralized API communication layer
- **Hooks**: Custom React hooks for stateful logic
- **Types**: TypeScript definitions for type safety

## Getting Started

### Prerequisites

- .NET 8.0+ SDK
- Node.js 18+ & npm
- SQL Server
- Docker & Docker Compose (optional)

### Backend Setup

1. **Navigate to backend directory:**
   ```bash
   cd backend
   ```

2. **Restore NuGet packages:**
   ```bash
   dotnet restore
   ```

3. **Configure connections string:**
   - Update `appsettings.json` with your database connection string and JWT key
   ```json
   {
     "ConnectionStrings": {
       "DefaultConnection": "your-connection-string"
     },
     "Jwt": {
       "Key": "your-secret-key"
     }
   }
   ```

4. **Run database migrations:**
   ```bash
   dotnet ef database update
   ```

5. **Build the solution:**
   ```bash
   dotnet build
   ```

6. **Run the API:**
   ```bash
   dotnet run --project Skwela.API
   ```

   The API will be available at `https://localhost:5001` (or configured port)

### Frontend Setup

1. **Navigate to frontend directory:**
   ```bash
   cd frontend
   ```

2. **Install dependencies:**
   ```bash
   npm install
   ```

3. **Configure API endpoint:**
   - Update `proxy.ts` or environment variables with your backend API URL

4. **Run development server:**
   ```bash
   npm run dev
   ```

   The frontend will be available at `http://localhost:3000`

### Docker Setup

To run the entire application using Docker:

1. **From the project root, build and start containers:**
   ```bash
   docker-compose -f backend/docker-compose.yml up -d
   ```

This will:
- Start the SQL Server database with initial schema
- Build and run the ASP.NET Core API
- Expose the API on the configured port

## Key Features

- **User Management**: Authentication and authorization with JWT
- **Classroom Management**: Create and manage classrooms
- **Student Enrollment**: Manage student enrollment in classrooms
- **Assignment Tracking**: Create and track assignments
- **Submission Management**: Students submit assignments and receive feedback

## API Documentation

The API follows RESTful conventions with the following main endpoints:

- **Authentication**: `/api/auth/*`
- **Classrooms**: `/api/classrooms/*`
- **Enrollments**: `/api/enrollments/*`
- **Users**: `/api/users/*`

For detailed API documentation, check the HTTP files in `Skwela.API/` or import them into your API client (Postman, Insomnia, etc.)

## Development

### Building

**Backend:**
```bash
cd backend
dotnet build
```

**Frontend:**
```bash
cd frontend
npm run build
```

### Running Tests

```bash
cd backend
dotnet test
```

### Code Style & Linting

**Frontend:**
```bash
cd frontend
npm run lint
```

## Environment Variables

### Backend (`appsettings.json`)

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=SkwelDB;User Id=sa;Password=YourPassword123!;"
  },
  "Jwt": {
    "Key": "your-very-secret-key-min-32-characters-long",
    "Issuer": "SkwelAuth",
    "Audience": "SkwelAPI"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information"
    }
  }
}
```

### Frontend

Create a `.env.local` file:
```
NEXT_PUBLIC_API_URL=http://localhost:5001
```

## Contributing

1. Create a feature branch (`git checkout -b feature/AmazingFeature`)
2. Commit your changes (`git commit -m 'Add some AmazingFeature'`)
3. Push to the branch (`git push origin feature/AmazingFeature`)
4. Open a Pull Request

## License

This project is proprietary. Unauthorized copying, distribution, or use is prohibited.

## Support

For issues, questions, or suggestions, please open an issue or contact the development team.
