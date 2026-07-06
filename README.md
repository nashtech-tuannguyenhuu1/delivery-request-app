# Delivery Request App

A sample enterprise application built with **ASP.NET Core 8** following a layered architecture and an event-driven approach.

---

# Overview

The application allows users to manage **Delivery Requests**, including:

* Login using Cookie Authentication
* Create Delivery Request
* Update Delivery Request
* Delete Delivery Request
* View Delivery Request list
* Upload attachments
* Track Delivery Request status
* Audit all user activities
* Synchronize Redis cache
* Generate reporting data asynchronously

---

# Architecture

```
+---------------------------+
| ASP.NET Core MVC (.NET 8) |
| Razor                     |
+------------+--------------+
             |
             |
             v
+---------------------------+
| ASP.NET Core Web API      |
+------------+--------------+
             |
             v
+---------------------------+
| Application Layer         |
+------------+--------------+
             |
             |
      +------+------+
      |             |
      |             |
      v             v
 SQL Server      Azure Service Bus
(MainDB)              |
                       |
        +--------------+----------------+
        |              |                |
        v              v                v
     Redis        ReportsDB       AuditLogDB

             |
             |
             v
      Azure Blob Storage
             |
             v
        DocumentsDB
```

---

# Technology Stack

| Component      | Technology                      |
| -------------- | ------------------------------- |
| Frontend       | ASP.NET Core MVC (.NET 8 Razor) |
| Backend        | ASP.NET Core Web API (.NET 8)   |
| Database       | SQL Server                      |
| Cache          | Redis                           |
| Message Broker | Azure Service Bus               |
| File Storage   | Azure Blob Storage              |
| Authentication | Cookie Authentication           |

---

# Databases

## MainDB

Stores application data.

### Tables

* DeliveryRequest
* User
* Role

---

## DocumentsDB

Stores attachment metadata.

### Tables

* Document

Example

| Column            |
| ----------------- |
| Id                |
| DeliveryRequestId |
| FileName          |
| BlobName          |
| BlobUrl           |
| ContentType       |
| FileSize          |
| CreatedDate       |

---

## ReportsDB

Stores reporting data generated asynchronously.

Example

* DeliveryRequestReport
* DailyDeliveryReport

---

## AuditLogDB

Stores all user activities.

Example

| Column      |
| ----------- |
| Id          |
| UserId      |
| Action      |
| Entity      |
| EntityId    |
| OldValue    |
| NewValue    |
| IPAddress   |
| UserAgent   |
| CreatedDate |

---

# Delivery Request Status

```
New
   │
   ▼
Assigned
   ├────────► Returned
   │
   ▼
Delivered
```

Available statuses

* New
* Assigned
* Delivered
* Returned

---

# Features

## Authentication

* Login
* Logout
* Cookie Authentication

---

## Delivery Request

* List Delivery Requests
* View Details
* Create
* Update
* Delete

---

## Attachment

* Upload file
* Download file
* Delete file

Files are stored in **Azure Blob Storage**.

Metadata is stored in **DocumentsDB**.

---

# Audit Logging

The system records all user activities.

Examples

* Login
* Logout
* Create Delivery Request
* Update Delivery Request
* Delete Delivery Request
* Upload Attachment
* Download Attachment
* Delete Attachment

---

# Event-Driven Workflow

## Create Delivery Request

```
User

↓

API

↓

MainDB

↓

Publish DeliveryRequestCreated Event

↓

Azure Service Bus

↓

Redis Consumer

↓

Reports Consumer

↓

AuditLog Consumer
```

---

## Update Delivery Request

```
User

↓

API

↓

MainDB

↓

Publish DeliveryRequestUpdated Event

↓

Azure Service Bus

↓

Redis Consumer

↓

Reports Consumer

↓

AuditLog Consumer
```

---

## Delete Delivery Request

```
User

↓

API

↓

MainDB

↓

Publish DeliveryRequestDeleted Event

↓

Azure Service Bus

↓

Redis Consumer

↓

Reports Consumer

↓

AuditLog Consumer
```

---

## Upload Attachment

```
User

↓

Upload File

↓

Azure Blob Storage

↓

DocumentsDB

↓

Publish DocumentUploaded Event

↓

AuditLog Consumer
```

---

# Redis Cache

The following data is cached:

* Delivery Request List
* Delivery Request Details
* Lookup Data

Cache is refreshed asynchronously whenever a Delivery Request is created, updated, or deleted.

---

# Azure Service Bus

Topics

```
deliveryrequest.created

deliveryrequest.updated

deliveryrequest.deleted

document.uploaded
```

Consumers

* Cache Consumer
* Reports Consumer
* AuditLog Consumer

---

# REST API

| Method | Endpoint                             | Description                  |
| ------ | ------------------------------------ | ---------------------------- |
| POST   | /api/login                           | Login                        |
| POST   | /api/logout                          | Logout                       |
| GET    | /api/deliveryrequests                | Get all Delivery Requests    |
| GET    | /api/deliveryrequests/{id}           | Get Delivery Request details |
| POST   | /api/deliveryrequests                | Create Delivery Request      |
| PUT    | /api/deliveryrequests/{id}           | Update Delivery Request      |
| DELETE | /api/deliveryrequests/{id}           | Delete Delivery Request      |
| POST   | /api/deliveryrequests/{id}/documents | Upload attachment            |
| GET    | /api/deliveryrequests/{id}/documents | Get attachments              |
| DELETE | /api/documents/{id}                  | Delete attachment            |

---

# Project Structure

```
delivery-request-app
│
├── src
│   ├── DeliveryRequest.Web
│   ├── DeliveryRequest.API
│   ├── DeliveryRequest.Application
│   ├── DeliveryRequest.Domain
│   ├── DeliveryRequest.Infrastructure
│   ├── DeliveryRequest.Persistence
│   ├── DeliveryRequest.EventBus
│   ├── DeliveryRequest.Cache
│   ├── DeliveryRequest.Document
│   ├── DeliveryRequest.Report
│   └── DeliveryRequest.Shared
│
├── tests
│
├── docs
│
└── README.md
```

---

# Future Enhancements

* Role-Based Authorization
* Email Notification
* Background Jobs
* Dashboard
* Elasticsearch Integration
* OpenTelemetry
* Distributed Tracing
* CI/CD Pipeline
* Docker & Kubernetes Deployment
