# File Manager — Backend (ASP.NET Core)

A layered ASP.NET Core Web API for managing folders and files (create, list, upload, rename, delete). The solution follows a clean architecture/CQRS approach:

- `Domain` — entities and core rules  
- `Application` — use cases (MediatR handlers), DTOs  
- `Infrastructure` — EF Core, persistence & file storage  
- `Web.API` — HTTP endpoints, DI, middleware

> Target runtime: **.NET 8 (LTS)**  
> Database: **PostgreSQL** (default; can be swapped)  
> Patterns: **CQRS with MediatR**, EF Core, DTO mapping

---

## Table of Contents

- [Quick start](#quick-start)
- [Configuration](#configuration)
- [API overview](#api-overview)
- [Architecture](#architecture)
- [Development](#development)

---

## Quick start

### Prerequisites

- .NET 8 SDK  
- PostgreSQL 13+ (local or remote)
- (Optional) PowerShell/bash to run commands below

### 1) Clone & restore

```bash
git clone https://github.com/LeDu98/File-Manager-Backend.git
cd File-Manager-Backend
dotnet restore
```

### 2) Configure environment

Create `Web.API/appsettings.Development.json` (or use environment variables):

```json
{
  "ConnectionStrings": {
    "Default": "Host=localhost;Port=5432;Database=filemanager;Username=postgres;Password=postgres"
  },
  "Storage": {
    "RootPath": "./storage"
  },
  "Cors": {
    "AllowedOrigins": [ "http://localhost:4200" ]
  },
  "Upload": {
    "MaxFileSizeMB": 50
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  }
}
```

### 3) Create database & apply migrations

```bash
dotnet tool update --global dotnet-ef
dotnet ef database update --project Infrastructure --startup-project Web.API
```

### 4) Run the API

```bash
dotnet run --project Web.API
```

By default: `https://localhost:5001` or `http://localhost:5000`

---

## Configuration

| Setting | Env var | Example |
|---------|---------|---------|
| Connection string | `ConnectionStrings__Default` | `Host=...;Database=...;Username=...;Password=...` |
| Storage root | `Storage__RootPath` | `/var/app/storage` |
| CORS allowed origins | `Cors__AllowedOrigins__0` | `http://localhost:4200` |
| Max upload size | `Upload__MaxFileSizeMB` | `50` |
| ASP.NET env | `ASPNETCORE_ENVIRONMENT` | `Development` |

---

## API overview

### Folders

- **POST** `/api/folders/create` — create folder  
- **POST** `/api/folders/rename` — rename folder  

### Files

- **POST** `/api/files/upload` — upload (multipart)  
- **GET** `/api/files/{id}/content` — get file content  
- **PUT** `/api/files/rename` — rename file (extension preserved)

### File Manager
- **POST** /api/file-manager/batch/delete - delete list of items
- **GET** /api/file-manager/breadcrumb/{id} - get breadcrumbs
- **GET** /api/file-manager/{id} - get folder content

---

## Architecture

```
FileManagerProject.sln
├─ Domain
├─ Application
│  ├─ Commands
│  └─ Queries
├─ Infrastructure
└─ Web.API
```

**Decisions:** CQRS with MediatR, EF Core + PostgreSQL, local file storage (pluggable).

---

## Development

```bash
dotnet test
dotnet ef migrations add <Name> --project Infrastructure --startup-project Web.API
```
