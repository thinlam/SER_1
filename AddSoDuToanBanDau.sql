-- Add SoDuToanBanDau column to DuAn table
IF NOT EXISTS(SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME='DuAn' AND COLUMN_NAME='SoDuToanBanDau')
BEGIN
    ALTER TABLE [DuAn] ADD [SoDuToanBanDau] bigint NULL;
    PRINT 'Column SoDuToanBanDau added successfully.';
END
ELSE
BEGIN
    PRINT 'Column SoDuToanBanDau already exists.';
END

-- Add to migration history if table exists and migration not yet applied
IF EXISTS(SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME='__EFMigrationsHistory')
BEGIN
    IF NOT EXISTS(SELECT 1 FROM [__EFMigrationsHistory] WHERE MigrationId='20260417102135_AddSoDuToanBanDauColumnToDuAn')
    BEGIN
        INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
        VALUES ('20260417102135_AddSoDuToanBanDauColumnToDuAn', '8.0.15');
        PRINT 'Migration history updated.';
    END
END
ELSE
BEGIN
    PRINT '__EFMigrationsHistory table does not exist - migration history not recorded.';
END
