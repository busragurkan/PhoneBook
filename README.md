# PhoneBook

Simple phone book app with microservice architecture. There are 2 services: Contact.API handles contact CRUD operations, Report.API generates location-based reports.

## Tech

- .NET 8
- PostgreSQL
- RabbitMQ + MassTransit
- Entity Framework Core
- Docker
- xUnit, Moq, FluentAssertions

## How to Run

Just need Docker installed.

```bash
git clone https://github.com/busragurkan/PhoneBook.git
cd PhoneBook
docker-compose up --build
```

After everything is up:
- Contact.API: http://localhost:5001/swagger
- Report.API: http://localhost:5002/swagger
- RabbitMQ: http://localhost:15672 (guest/guest)

### Without Docker

Make sure PostgreSQL (5432) and RabbitMQ (5672) are running locally, then:

```bash
dotnet run --project src/Services/Contact/Contact.API
dotnet run --project src/Services/Report/Report.API
```

### Tests

```bash
dotnet test
```

## How it Works

Contact.API is the main service for managing contacts and their info (phone, email, location). Report.API handles report requests for a given location.

When a report is requested, Report.API publishes an event to RabbitMQ. The consumer picks it up and calls Contact.API's REST endpoint to get the contact/phone count for that location, then updates the report.

So there's both REST (sync) and RabbitMQ (async) communication between services.

## Endpoints

### Contact.API

- `GET /api/contacts` - list all contacts
- `GET /api/contacts/{id}` - contact detail with contact info
- `POST /api/contacts` - create contact
- `DELETE /api/contacts/{id}` - delete contact
- `POST /api/contacts/{id}/contact-informations` - add contact info
- `DELETE /api/contacts/contact-informations/{id}` - remove contact info
- `GET /api/contacts/statistics?location=X` - location stats (used internally by Report.API)

### Report.API

- `POST /api/reports` - request a report
- `GET /api/reports` - list reports
- `GET /api/reports/{id}` - report detail

## Data Model

**Contact:** Id (UUID), Name, Surname, Company

**ContactInformation** (separate table): Id, ContactId (FK), InfoType (Phone/Email/Location), InfoContent

## Examples

Create contact:
```
POST /api/contacts
{ "name": "Ali", "surname": "Yilmaz", "company": "Arvento" }
```

Add phone number:
```
POST /api/contacts/{id}/contact-informations
{ "infoType": 0, "infoContent": "+905551234567" }
```

infoType: 0 = Phone, 1 = Email, 2 = Location

Request report:
```
POST /api/reports
{ "location": "Ankara" }
```

## Project Structure

```
src/
  Services/
    Contact/
      Contact.API/        -> contact service
      Contact.UnitTests/  -> 30 tests
    Report/
      Report.API/         -> report service
      Report.UnitTests/   -> 9 tests
  Shared/
    PhoneBook.Shared/     -> shared DTOs, enums, events
```

## Branches

master <- development <- feature/* branches

Tags: v0.1.0, v0.2.0, v0.3.0, v1.0.0
