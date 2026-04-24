# Merge Plan: ChuyenDoiSo QLDA → Quan_Ly_Du_An

## Context
**Source**: `C:\Users\tranp\source\repos\ChuyenDoiSo\DesktopModules\MVC\QuanLyDuAn\SER` (prioritized)
**Target**: `C:\Users\tranp\source\repos\Quan_Ly_Du_An`

Merge QLDA module from ChuyenDoiSo monorepo into standalone Quan_Ly_Du_An repo, prioritizing ChuyenDoiSo source.

## User Decisions
- **Migrations**: Squash - delete all, create single InitialCreate from ChuyenDoiSo
- **BuildingBlocks**: Keep Quan_Ly_Du_An's existing BuildingBlocks
- **Docs/Plans**: Keep Quan_Ly_Du_An's docs and plans folders
- **Existing Code**: DO NOT delete - overwrite/merge with ChuyenDoiSo (preserve Quan_Ly_Du_An logic)

## Phase 1: Backup & Preparation

### Step 1.1: Backup Current State
```bash
# Backup target repo before merge
cd C:\Users\tranp\source\repos\Quan_Ly_Du_An
git stash push -m "backup-before-merge"
git status
```

**Note**: DO NOT delete existing QLDA folders - will overwrite with ChuyenDoiSo files to preserve existing logic.

## Phase 2: Copy & Merge Source Files

### Step 2.1: Copy QLDA Projects (Overwrite)
```bash
# Copy from ChuyenDoiSo to Quan_Ly_Du_An
xcopy "C:\Users\tranp\source\repos\ChuyenDoiSo\DesktopModules\MVC\QuanLyDuAn\SER\QLDA.Application" "C:\Users\tranp\source\repos\Quan_Ly_Du_An\QLDA.Application" /E /I /Y
xcopy "C:\Users\tranp\source\repos\ChuyenDoiSo\DesktopModules\MVC\QuanLyDuAn\SER\QLDA.Domain" "C:\Users\tranp\source\repos\Quan_Ly_Du_An\QLDA.Domain" /E /I /Y
xcopy "C:\Users\tranp\source\repos\ChuyenDoiSo\DesktopModules\MVC\QuanLyDuAn\SER\QLDA.Infrastructure" "C:\Users\tranp\source\repos\Quan_Ly_Du_An\QLDA.Infrastructure" /E /I /Y
xcopy "C:\Users\tranp\source\repos\ChuyenDoiSo\DesktopModules\MVC\QuanLyDuAn\SER\QLDA.Persistence" "C:\Users\tranp\source\repos\Quan_Ly_Du_An\QLDA.Persistence" /E /I /Y
xcopy "C:\Users\tranp\source\repos\ChuyenDoiSo\DesktopModules\MVC\QuanLyDuAn\SER\QLDA.WebApi" "C:\Users\tranp\source\repos\Quan_Ly_Du_An\QLDA.WebApi" /E /I /Y
xcopy "C:\Users\tranp\source\repos\ChuyenDoiSo\DesktopModules\MVC\QuanLyDuAn\SER\QLDA.Migrator" "C:\Users\tranp\source\repos\Quan_Ly_Du_An\QLDA.Migrator" /E /I /Y
xcopy "C:\Users\tranp\source\repos\ChuyenDoiSo\DesktopModules\MVC\QuanLyDuAn\SER\QLDA.CrossCuttingConcerns" "C:\Users\tranp\source\repos\Quan_Ly_Du_An\QLDA.CrossCuttingConcerns" /E /I /Y
copy "C:\Users\tranp\source\repos\ChuyenDoiSo\DesktopModules\MVC\QuanLyDuAn\SER\SER.sln" "C:\Users\tranp\source\repos\Quan_Ly_Du_An\SER.sln" /Y
```

### Step 2.2: Update Project References
Update `.csproj` files to reference local BuildingBlocks instead of ChuyenDoiSo paths:
- Change `..\..\..\BuildingBlocks\` to `..\BuildingBlocks\src\`
- Update Solution file to include correct paths

## Phase 3: Migration Squashing

### Step 3.1: Delete Old Migrations
```bash
cd C:\Users\tranp\source\repos\Quan_Ly_Du_An
# Delete all migration files
Remove-Item -Recurse -Force QLDA.Persistence\Migrations
# Delete MigratorDbContextFactory snapshot
Remove-Item -Force QLDA.Persistence\*\*DbContextFactoryModelSnapshot.cs
```

### Step 3.2: Create New Single Migration
```bash
# Create fresh InitialCreate migration
dotnet ef migrations add InitialCreate --project QLDA.Persistence --startup-project QLDA.WebApi
```

### Step 3.3: Update Migrator
Ensure Migrator project references correct DbContext and can run migrations.

## Phase 4: Verification

### Step 4.1: Build Verification
```bash
cd C:\Users\tranp\source\repos\Quan_Ly_Du_An
dotnet build SER.sln
```

### Step 4.2: Database Migration Test
```bash
# Apply migrations to test database
dotnet ef database update --project QLDA.Persistence --startup-project QLDA.WebApi
```

### Step 4.3: Review Changes
```bash
# Review all changes before committing (user will commit manually)
git status
git diff
```

## Critical Files to Modify

| File | Change |
|------|--------|
| `QLDA.*.csproj` | Update BuildingBlocks reference paths |
| `SER.sln` | Update project paths |
| `QLDA.Persistence/Migrations/` | Delete migrations folder, recreate InitialCreate |
| All QLDA project files | Overwrite with ChuyenDoiSo versions (preserve existing logic) |

## Risk Assessment

| Risk | Severity | Mitigation |
|------|----------|------------|
| Data loss if DB exists | HIGH | Only squash if DB is empty/dev |
| Broken project refs | MEDIUM | Verify all paths after copy |
| Build failures | MEDIUM | Run build verification immediately |

## Success Criteria
1. All QLDA projects copied successfully
2. BuildingBlocks references updated correctly
3. Single InitialCreate migration created
4. Solution builds without errors
5. Git commit pushed to remote