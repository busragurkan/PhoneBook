# PhoneBook - Microservice Phone Book Application

A simple phone book application built with a microservice architecture using .NET 8, PostgreSQL, and RabbitMQ. The system consists of two independent services communicating both synchronously (REST) and asynchronously (RabbitMQ).

## Architecture

```
┌─────────────────┐    REST/HTTP     ┌──────────────────┐
│                 │ ◄──────────────► │                  │
│  Contact.API    │                  │  Report.API      │
│  (Port: 5001)   │    RabbitMQ      │  (Port: 5002)    │
│                 │ ──────────────►  │                  │
└────────┬────────┘   (async)        └────────┬─────────┘
         │                                     │
         ▼                                     ▼
   ┌───────────┐                         ┌───────────┐
   │ PostgreSQL │                         │ PostgreSQL │
   │ ContactDb  │                         │ ReportDb   │
   └───────────┘                         └───────────┘
```

- **Contact.API** - Manages contacts and contact information (CRUD operations)
- **Report.API** - Handles report generation requests asynchronously

### Communication Flow

1. User requests a report via Report.API (POST /api/reports)
2. Report.API creates a report record with "Preparing" status and publishes `ReportRequestedEvent` to RabbitMQ
3. Report.API's consumer picks up the event and calls Contact.API via **REST HTTP** to get location statistics
4. Report.API updates the report with the results and sets status to "Completed"

### Tech Stack

- .NET 8 (ASP.NET Core Web API)
- PostgreSQL 16
- RabbitMQ 3 (with MassTransit library)
- Entity Framework Core 8
- Docker & Docker Compose
- xUnit, Moq, FluentAssertions (Unit Testing)

## Getting Started

### Prerequisites

- [Docker](https://www.docker.com/get-started) and Docker Compose installed on your machine

### Running the Application

1. Clone the repository:
```bash
git clone https://github.com/busragurkan/PhoneBook.git
cd PhoneBook
```

2. Run with Docker Compose:
```bash
docker-compose up --build
```

3. Access the services:
   - **Contact.API Swagger**: http://localhost:5001/swagger
   - **Report.API Swagger**: http://localhost:5002/swagger
   - **RabbitMQ Management UI**: http://localhost:15672 (guest/guest)

### Running Without Docker

1. Ensure PostgreSQL (port 5432) and RabbitMQ (port 5672) are running locally
2. Update connection strings in `appsettings.json` files if needed
3. Run each service in separate terminals:
```bash
dotnet run --project src/Services/Contact/Contact.API
dotnet run --project src/Services/Report/Report.API
```

### Running Tests

Run all unit tests:
```bash
dotnet test
```

Run tests with detailed output:
```bash
dotnet test --verbosity normal
```

## Data Model

### Contact
| Field | Type | Description |
|-------|------|-------------|
| Id | UUID | Primary key |
| Name | string | First name (required, max 100) |
| Surname | string | Last name (required, max 100) |
| Company | string | Company name (max 200) |

### ContactInformation (separate table)
| Field | Type | Description |
|-------|------|-------------|
| Id | UUID | Primary key |
| ContactId | UUID | Foreign key to Contact |
| InfoType | enum | PhoneNumber, Email, Location |
| InfoContent | string | The actual information content |

## API Endpoints

### Contact.API (http://localhost:5001)

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | /api/contacts | List all contacts |
| GET | /api/contacts/{id} | Get contact detail with all contact information |
| POST | /api/contacts | Create a new contact |
| DELETE | /api/contacts/{id} | Delete a contact (cascades to contact information) |
| POST | /api/contacts/{id}/contact-informations | Add contact information to a contact |
| DELETE | /api/contacts/contact-informations/{id} | Remove a specific contact information |
| GET | /api/contacts/statistics?location={loc} | Get location statistics (used by Report.API) |

### Report.API (http://localhost:5002)

| Method | Endpoint | Description |
|--------|----------|-------------|
| POST | /api/reports | Request a new location-based report (async) |
| GET | /api/reports | List all reports |
| GET | /api/reports/{id} | Get report detail with status |

### Sample Requests

**Create a contact:**
```json
POST /api/contacts
{
  "name": "Ali",
  "surname": "Yilmaz",
  "company": "Arvento"
}
```

**Add phone number to contact:**
```json
POST /api/contacts/{contactId}/contact-informations
{
  "infoType": 0,
  "infoContent": "+905551234567"
}
```
> InfoType values: 0 = PhoneNumber, 1 = Email, 2 = Location

**Request a report:**
```json
POST /api/reports
{
  "location": "Ankara"
}
```

## Project Structure

```
PhoneBook/
├── src/
│   ├── Services/
│   │   ├── Contact/
│   │   │   ├── Contact.API/          # Contact microservice
│   │   │   │   ├── Controllers/      # API endpoints
│   │   │   │   ├── Data/             # DbContext & Migrations
│   │   │   │   ├── DTOs/             # Data transfer objects
│   │   │   │   ├── Entities/         # Domain models
│   │   │   │   ├── Repositories/     # Data access layer
│   │   │   │   ├── Services/         # Business logic layer
│   │   │   │   └── Dockerfile
│   │   │   └── Contact.UnitTests/    # Unit tests (30 tests)
│   │   └── Report/
│   │       ├── Report.API/           # Report microservice
│   │       │   ├── Controllers/
│   │       │   ├── Consumers/
│   │       │   ├── Data/
│   │       │   ├── DTOs/
│   │       │   ├── Entities/
│   │       │   ├── Repositories/
│   │       │   ├── Services/
│   │       │   └── Dockerfile
│   │       └── Report.UnitTests/     # Unit tests (9 tests)
│   └── Shared/
│       └── PhoneBook.Shared/         # Shared models, enums, events
├── docker-compose.yml                # Docker orchestration
├── init-databases.sh                 # PostgreSQL multi-db init script
├── PhoneBook.slnx                    # Solution file
└── README.md
```

## Git Branching Strategy

- `master` - Production-ready code
- `development` - Integration branch
- `feature/*` - Feature branches (merged into development)
- Version tags: v0.1.0, v0.2.0, v0.3.0, v1.0.0
