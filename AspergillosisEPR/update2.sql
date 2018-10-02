USE ASPEPR3;
ALTER TABLE [PatientNACDates] ADD [LastObservationPoint] datetime2 NULL;

GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20181001115601_AddLastObservationPoint', N'2.1.0-rtm-30799');

GO


