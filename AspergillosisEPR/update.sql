
ALTER TABLE [PatientSTGQuestionnaires] ADD [OriginalImportedId] nvarchar(max) NULL;

GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20180501112107_AddOriginalImportedIdToPatientSTGQuestionnaires', N'2.0.1-rtm-125');

GO

CREATE TABLE [TemporaryNewPatients] (
    [ID] int NOT NULL IDENTITY,
    [ImportedAsRealPatient] bit NOT NULL,
    [RM2Number] int NOT NULL,
    CONSTRAINT [PK_TemporaryNewPatients] PRIMARY KEY ([ID])
);

GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20180502064230_CreateTemporaryPatient', N'2.0.1-rtm-125');

GO

DECLARE @var0 sysname;
SELECT @var0 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'TemporaryNewPatients') AND [c].[name] = N'RM2Number');
IF @var0 IS NOT NULL EXEC(N'ALTER TABLE [TemporaryNewPatients] DROP CONSTRAINT [' + @var0 + '];');
ALTER TABLE [TemporaryNewPatients] ALTER COLUMN [RM2Number] nvarchar(max) NULL;

GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20180502072559_ChangeRMNumberInNewTemporaryPatients', N'2.0.1-rtm-125');

GO


