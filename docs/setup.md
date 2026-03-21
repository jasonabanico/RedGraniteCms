# Setup

## Prerequisites
- Node.js (version 14 or higher)
- .NET SDK (version 9)
- A code editor (e.g., Visual Studio Code)
- Git
- (Optional) [PostgreSQL](https://www.postgresql.org/download/) if using PostgreSQL instead of SQLite

## Installation
1. Clone the repository:
   ```bash
   git clone https://github.com/jasonabanico/redgranitecms.git
   ```
2. Navigate to the project directory:
   ```bash
   cd redgranitecms
   ```
3. Install dependencies:
   - For the client (frontend):
     ```bash
     cd src/Client/RedGraniteCms.Client.Web
     npm install
     ```
   - For the server (backend):
     ```bash
     cd src
     dotnet restore
     ```

## Database Configuration

The database provider is controlled by the `DatabaseProvider` setting in `appsettings.json`. Supported values: `Sqlite` (default), `PostgreSQL`.

Both the **Api** (`src/Server/RedGraniteCms.Server.Api/appsettings.json`) and **Web** (`src/Server/RedGraniteCms.Server.Web/appsettings.json`) projects have their own `appsettings.json`. Update the one for the project you are running. You can also override settings per environment in `appsettings.Development.json`.

The database schema is created automatically on startup via `EnsureCreatedAsync()` — no manual migrations are required.

### SQLite (default)

SQLite requires no external setup. The database file is created automatically in the project directory on first run.

**Configuration** (`appsettings.json`):
```json
{
    "DatabaseProvider": "Sqlite",
    "ConnectionStrings": {
        "DefaultConnection": "Data Source=RedGraniteCms.db"
    }
}
```

| Setting | Description |
|---------|-------------|
| `DatabaseProvider` | Set to `Sqlite` (this is also the default if omitted) |
| `ConnectionStrings:DefaultConnection` | Path to the SQLite database file. Relative to the project directory |

The `.db` file is git-ignored. To reset the database, stop the app and delete `RedGraniteCms.db`.

### PostgreSQL

To use PostgreSQL, you need a running PostgreSQL server.

#### 1. Install PostgreSQL

- **Windows**: Download from [postgresql.org](https://www.postgresql.org/download/windows/) or use `winget install PostgreSQL.PostgreSQL`
- **macOS**: `brew install postgresql@16`
- **Linux (Debian/Ubuntu)**: `sudo apt install postgresql`
- **Docker**:
  ```bash
  docker run --name redgranite-postgres -e POSTGRES_USER=postgres -e POSTGRES_PASSWORD=yourpassword -e POSTGRES_DB=RedGraniteCms -p 5432:5432 -d postgres:16
  ```

#### 2. Create the database

If you're not using the Docker command above (which creates the database automatically), create it manually:

```bash
psql -U postgres
CREATE DATABASE "RedGraniteCms";
\q
```

#### 3. Update configuration

Update `appsettings.json` (or `appsettings.Development.json`) for the project you are running:

```json
{
    "DatabaseProvider": "PostgreSQL",
    "ConnectionStrings": {
        "PostgreSQLConnection": "Host=localhost;Port=5432;Database=RedGraniteCms;Username=postgres;Password=yourpassword"
    }
}
```

| Setting | Description |
|---------|-------------|
| `DatabaseProvider` | Set to `PostgreSQL` |
| `ConnectionStrings:PostgreSQLConnection` | [Npgsql connection string](https://www.npgsql.org/doc/connection-string-parameters.html) |

Common connection string parameters:

| Parameter | Default | Description |
|-----------|---------|-------------|
| `Host` | `localhost` | PostgreSQL server hostname |
| `Port` | `5432` | PostgreSQL server port |
| `Database` | — | Database name |
| `Username` | — | PostgreSQL username |
| `Password` | — | PostgreSQL password |
| `SSL Mode` | `Prefer` | Set to `Require` for cloud-hosted databases |

#### 4. Run the application

Start the application normally. The schema is created automatically on first run:

```bash
cd src/Server/RedGraniteCms.Server.Api
dotnet run
```

### Switching between providers

Switching providers only requires changing `appsettings.json` — no code changes are needed. Note that data is not migrated between providers; each provider has its own storage.

## Running the Project
1. Start the server:
   - Navigate to the server directory:
     ```bash
     cd src/Server/RedGraniteCms.Server.Api
     ```
   - Run the server:
     ```bash
     dotnet run
     ```
2. Start the client:
   - Navigate to the client directory:
     ```bash
     cd src/Client/RedGraniteCms.Client.Web
     ```
   - Run the client (development mode with Vite):
     ```bash
     npm run dev
     ```
3. Access the application:
   - Open your browser and navigate to `http://localhost:3000` (or the specified port).

## Client Scripts

| Command | Description |
|---------|-------------|
| `npm run dev` | Start development server with hot reload |
| `npm run build` | Build for production |
| `npm run preview` | Preview production build locally |
| `npm run test` | Run tests with Vitest |
