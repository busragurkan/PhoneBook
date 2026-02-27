# PhoneBook

Mikroservis mimarisinde basit bir telefon rehberi uygulamasi. 2 servis var: biri rehber islemleri (Contact.API), digeri konum bazli rapor olusturma (Report.API).

## Kullanilan Teknolojiler

- .NET 8
- PostgreSQL
- RabbitMQ + MassTransit
- Entity Framework Core
- Docker
- xUnit + Moq + FluentAssertions

## Nasil Calistirilir

Docker kurulu olmasi yeterli.

```bash
git clone https://github.com/busragurkan/PhoneBook.git
cd PhoneBook
docker-compose up --build
```

Ayaga kalktiktan sonra:
- Contact.API: http://localhost:5001/swagger
- Report.API: http://localhost:5002/swagger
- RabbitMQ: http://localhost:15672 (guest/guest)

### Docker olmadan calistirmak icin

PostgreSQL (5432) ve RabbitMQ (5672) lokal olarak calisiyor olmali, sonra:

```bash
dotnet run --project src/Services/Contact/Contact.API
dotnet run --project src/Services/Report/Report.API
```

### Testler

```bash
dotnet test
```

## Mimari

Contact.API rehber islemlerini yapar (kisi ekleme/silme, iletisim bilgisi ekleme/silme vs). Report.API ise konum bazli rapor talebi olusturur.

Rapor istegi geldiginde Report.API once RabbitMQ'ya event firlatir, sonra consumer bu eventi alip Contact.API'nin REST endpointini cagirarak o konumdaki kisi ve telefon sayisini alir. Sonucu rapora yazar.

Yani servisler arasi hem REST (senkron) hem RabbitMQ (asenkron) iletisim var.

## API Endpointleri

### Contact.API

- `GET /api/contacts` - Tum kisileri listele
- `GET /api/contacts/{id}` - Kisi detayi (iletisim bilgileri dahil)
- `POST /api/contacts` - Yeni kisi olustur
- `DELETE /api/contacts/{id}` - Kisi sil
- `POST /api/contacts/{id}/contact-informations` - Kisiye iletisim bilgisi ekle
- `DELETE /api/contacts/contact-informations/{id}` - Iletisim bilgisi sil
- `GET /api/contacts/statistics?location=X` - Konum istatistigi (Report.API tarafindan kullanilir)

### Report.API

- `POST /api/reports` - Rapor talebi olustur
- `GET /api/reports` - Tum raporlari listele
- `GET /api/reports/{id}` - Rapor detayi

## Veri Modeli

**Contact:** Id (UUID), Name, Surname, Company

**ContactInformation** (ayri tablo): Id, ContactId (FK), InfoType (Phone/Email/Location), InfoContent

## Ornek Kullanim

Kisi olustur:
```
POST /api/contacts
{ "name": "Ali", "surname": "Yilmaz", "company": "Arvento" }
```

Telefon ekle:
```
POST /api/contacts/{id}/contact-informations
{ "infoType": 0, "infoContent": "+905551234567" }
```

infoType: 0 = Telefon, 1 = Email, 2 = Konum

Rapor iste:
```
POST /api/reports
{ "location": "Ankara" }
```

## Proje Yapisi

```
src/
  Services/
    Contact/
      Contact.API/        -> Rehber servisi
      Contact.UnitTests/  -> 30 test
    Report/
      Report.API/         -> Rapor servisi
      Report.UnitTests/   -> 9 test
  Shared/
    PhoneBook.Shared/     -> Ortak DTO, enum, event sinflari
```

## Branch Stratejisi

master <- development <- feature/* branchleri

Versiyon tagleri: v0.1.0, v0.2.0, v0.3.0, v1.0.0
