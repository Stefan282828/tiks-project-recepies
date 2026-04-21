# FoodExplorer Recipe Project

## What this project does
FoodExplorer is a recipe management application built around three core entities:
- **Kategorija** (Category)
- **Podkategorija** (Subcategory)
- **Recept** (Recipe)

It provides CRUD APIs for all three entities, keeps their relationships consistent, and includes a simple browser-based client for managing data.

## What this project is meant to do
The project is meant to be a practical full-stack learning and portfolio application that demonstrates:
- Building REST-style backend services with layered architecture (controllers + services + data access)
- Modeling relational data with Entity Framework Core
- Connecting a lightweight frontend to backend APIs
- Implementing and testing business logic for category/subcategory/recipe workflows

## Main functionality
- Create, read, update, and delete categories, subcategories, and recipes
- Get subcategories by category (`Podkategorija/ZaKategoriju/{kategorijaId}`)
- Get recipes by subcategory (`Recept/ZaPodkategoriju/{podkategorijaId}`)
- Validate model inputs and persist data in PostgreSQL
- Explore API endpoints via Swagger UI

## Architecture at a glance
- **Backend**: ASP.NET Core Web API (`Recepti_back`)
- **Data layer**: `FoodExplorerContext` with EF Core and Npgsql provider
- **Service layer**: `KategorijaService`, `PodkategorijaService`, `ReceptEfService`
- **Frontend**: Static HTML/CSS/JavaScript pages under `Recepti_back/Client`
- **Tests**: NUnit test project (`Recepti.Tests`) for service-layer behavior

## Languages and tools used
- **Languages**: C#, JavaScript, HTML, CSS
- **Frameworks/Libraries**:
  - ASP.NET Core (.NET 7)
  - Entity Framework Core
  - Npgsql (PostgreSQL provider)
  - Swagger / Swashbuckle
  - NUnit
- **Database**: PostgreSQL (configured through `DefaultConnection`)
- **Additional package references in project**: Neo4j.Driver, Neo4jClient, MemoryCache
  - Neo4j packages are currently tied to an extra/legacy service module (`ReceptService.cs`), while the active API CRUD flow uses EF Core + PostgreSQL (`ReceptEfService`).

## Quick start
1. Configure PostgreSQL and update connection string in:
   - `Recepti_back/appsettings.json`
2. Run backend API:
   - `dotnet run --project Recepti_back/Recepti.csproj`
3. Open Swagger UI:
   - `http://localhost:5121/swagger`
4. Optional frontend:
   - Open HTML pages in `Recepti_back/Client/html` (or serve them with a local static server)

## Build and test
- Build:
  - `dotnet build Recepti_back/Recepti.csproj`
- Tests:
  - `dotnet test Recepti.Tests/Recepti.Tests.csproj`

> Note: This project targets .NET 7. If your environment only has newer runtimes (for example .NET 8+), install the .NET 7 runtime first: https://dotnet.microsoft.com/en-us/download/dotnet/7.0

## Project description (portfolio-ready)
Designed and implemented a recipe-management web application with ASP.NET Core and Entity Framework Core, delivering CRUD workflows for categories, subcategories, and recipes through a clean controller/service/data architecture. Modeled relational data in PostgreSQL with enforced entity relationships and validation-aware DTO-based request handling. Integrated Swagger for API exploration, added service-layer tests with NUnit, and built a lightweight JavaScript + HTML/CSS client for browsing and managing recipe structures end to end.
