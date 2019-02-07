USE AspEPR3;

ALTER TABLE [PatientNACDates] ADD [FollowUp3MonthsDrug] nvarchar(max) NULL;

GO

ALTER TABLE [PatientNACDates] ADD [InitialDrug] nvarchar(max) NULL;

GO

ALTER TABLE [PatientNACDates] ADD [ReferralDate] datetime2 NULL;

GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20190206083003_AddInitialDrugsToNacDates', N'2.1.0-rtm-30799');

GO


