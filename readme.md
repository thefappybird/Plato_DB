# Backend Architecture - .NET C# API

## Overview
This backend is designed to support various applications, including the **Plato Recipe App**. It is built using **.NET C#** and **MS SQL** and provides secure and efficient handling of data. The backend includes **public and private routes**, robust security features, image processing, and is deployed on **Azure DevOps**.

Visit PLATO here: 03-recipe-planner.vercel.app
---

## Tech Stack
- **Backend Framework**: .NET C#
- **Database**: MS SQL Server
- **Authentication & Security**: JWT tokens, password hashing, GUIDs for unique IDs
- **Image Management**: Cloudinary for processing image uploads
- **Deployment**: Azure DevOps

---

## Security Features
- **JWT Tokens**: Ensures stateless, secure authentication for private routes
- **Password Hashing**: User passwords are stored securely using hashing algorithms
- **GUIDs**: Unique identifiers for all database entities to prevent collisions
- **Route Protection**: Private routes are accessible only to authenticated users

---

## API Features
- **Public Routes**: 
  - Registration and login
  - Viewing publicly available data such as recipes or listings

- **Private Routes**:
  - Creating, editing, and deleting user-specific data
  - Managing user favorites or profile data
  - Image upload endpoints using Cloudinary

- **Image Handling**:
  - Cloudinary is integrated to handle storage, optimization, and retrieval of images

- **Database Design**:
  - MS SQL Server handles all persistent data
  - Entities use GUIDs as primary keys for uniqueness and security

---

## Data Flow Diagram

```text
+-------------+          +------------------+          +----------------+
|             |          |                  |          |                |
|   Client    |  ---->   |   Public /       |  ---->   | Controllers    |
| (Front-End) |          |   Private Routes |          |                |
+-------------+          +------------------+          +--------+-------+
                                                                 |
                                                                 v
                                                      +----------------+
                                                      |                |
                                                      |  Services /    |
                                                      |  Utilities     |
                                                      +--------+-------+
                                                               |
                                                               v
                                                      +----------------+
                                                      |                |
                                                      | Cloudinary /   |
                                                      |  File Storage  |
                                                      +--------+-------+
                                                               |
                                                               v
                                                      +----------------+
                                                      |                |
                                                      |  MS SQL Server |
                                                      |  Database      |
                                                      +----------------+
```

---

## Deployment
- The project is deployed via **Azure DevOps**, enabling CI/CD pipelines for automated build, test, and deployment processes.

---

## Summary
This backend architecture emphasizes **security, scalability, and maintainability**. JWT tokens and password hashing provide strong authentication, GUIDs ensure unique identification, and Cloudinary handles image uploads efficiently. The backend supports both public and private routes and integrates seamlessly with front-end applications, while deployment via Azure DevOps ensures robust and automated delivery.
