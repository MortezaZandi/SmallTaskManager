# SQL Database Connection and First-Time Setup

## 1. Set the connection string

Connection string is read from **appsettings.json** under `AppSettings.ConnectionString`.

### Option A: Edit appsettings.json

Open `SmallTask/appsettings.json` and set your connection string:

```json
{
  "AppSettings": {
    "ConnectionString": "Server=YOUR_SERVER;Database=SmallTaskDb;Trusted_Connection=True;TrustServerCertificate=True;",
    "AttachmentStoragePath": "Attachments"
  }
}
```

**Examples:**

| Scenario | Connection string |
|--------|--------------------|
| **SQL Server LocalDB** (default) | `Server=(localdb)\\MSSQLLocalDB;Database=SmallTaskDb;Trusted_Connection=True;TrustServerCertificate=True;` |
| **Named SQL Server instance** | `Server=AsusZ390TUF;Database=SmallTaskDb;Trusted_Connection=True;TrustServerCertificate=True;` |
| **SQL Server with login** | `Server=localhost;Database=SmallTaskDb;User Id=sa;Password=YourPassword;TrustServerCertificate=True;` |
| **SQL Express** | `Server=.\\SQLEXPRESS;Database=SmallTaskDb;Trusted_Connection=True;TrustServerCertificate=True;` |

- Replace `YOUR_SERVER` with your machine name, `(localdb)\\MSSQLLocalDB`, or `localhost`.
- Replace `SmallTaskDb` with the database name you want (it will be created if it doesn’t exist).

### Option B: Override in appsettings.Development.json

For local development only, you can override in `appsettings.Development.json`:

```json
{
  "AppSettings": {
    "ConnectionString": "Server=(localdb)\\MSSQLLocalDB;Database=SmallTaskDb_Dev;Trusted_Connection=True;TrustServerCertificate=True;"
  }
}
```

### Option C: User secrets (no connection string in source control)

From the project folder:

```powershell
cd SmallTask
dotnet user-secrets set "AppSettings:ConnectionString" "Server=.;Database=SmallTaskDb;Trusted_Connection=True;TrustServerCertificate=True;"
```

User secrets override values from appsettings when running locally.

---

## 2. Initialize the database (first time)

The app **automatically applies migrations** when it starts. No manual step is required if you just run the app.

1. Set the connection string (step 1).
2. Run the application:
   - From Visual Studio: F5 or **Start**.
   - From command line: `dotnet run` (from the `SmallTask` folder).
3. On first run, `db.Database.Migrate()` in `Program.cs` will:
   - Create the database if it does not exist.
   - Create all tables (Users, Groups, Tasks, Labels, TaskLabels, Comments, Attachments) from the initial migration.

You do **not** need to run `dotnet ef database update` manually unless you prefer to apply migrations before starting the app.

---

## 3. Manual migration (optional)

If you want to create or update the database without starting the web app:

```powershell
cd SmallTask
dotnet ef database update
```

Requires **Microsoft.EntityFrameworkCore.Tools** (already in the project). If you get “dotnet ef not found”:

```powershell
dotnet tool install --global dotnet-ef
```

---

## 4. Troubleshooting

| Problem | What to do |
|--------|------------|
| **Cannot connect to LocalDB** | Install [SQL Server Express LocalDB](https://learn.microsoft.com/en-us/sql/database-engine/configure-windows/sql-server-express-localdb) or use a full SQL Server instance and change the connection string. |
| **Login failed for user** | Use a connection string with `User Id=...;Password=...` and ensure the user has `dbcreator` (to create DB) and rights on the database. |
| **TrustServerCertificate** | For local/dev, add `TrustServerCertificate=True;` to the connection string to avoid certificate validation. |
| **Database already exists** | Migrate() only applies pending migrations; it will not drop the database. To start over, delete the database in SQL Server Management Studio or run `dotnet ef database drop`, then run the app again. |
