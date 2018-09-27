
DECLARE @var0 sysname;
SELECT @var0 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'PatientPulmonaryFunctionTests') AND [c].[name] = N'DateTaken');
IF @var0 IS NOT NULL EXEC(N'ALTER TABLE [PatientPulmonaryFunctionTests] DROP CONSTRAINT [' + @var0 + '];');
ALTER TABLE [PatientPulmonaryFunctionTests] ALTER COLUMN [DateTaken] datetime2 NULL;

GO

ALTER TABLE [PatientPulmonaryFunctionTests] ADD [ResultValue] decimal(18, 2) NOT NULL DEFAULT 0.0;

GO

CREATE INDEX [IX_PatientPulmonaryFunctionTests_PatientId] ON [PatientPulmonaryFunctionTests] ([PatientId]);

GO

ALTER TABLE [PatientPulmonaryFunctionTests] ADD CONSTRAINT [FK_PatientPulmonaryFunctionTests_Patients_PatientId] FOREIGN KEY ([PatientId]) REFERENCES [Patients] ([ID]) ON DELETE CASCADE;

GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20180822092318_AddPFTValue', N'2.1.0-rtm-30799');

GO

ALTER TABLE [PatientPulmonaryFunctionTests] ADD [PredictedValue] decimal(18, 2) NOT NULL DEFAULT 0.0;

GO

CREATE INDEX [IX_PatientPulmonaryFunctionTests_PulmonaryFunctionTestId] ON [PatientPulmonaryFunctionTests] ([PulmonaryFunctionTestId]);

GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20180822110946_AddPFTPredictedValue', N'2.1.0-rtm-30799');

GO

