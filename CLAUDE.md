# Project Rules

## Migrations
- **NEVER** modify `AppDbContextModelSnapshot.cs` or any existing migration files in `QLDA.Migrator/Migrations/`. These are immutable snapshots of database state. Changes require a new migration via `ef.bat add`.
