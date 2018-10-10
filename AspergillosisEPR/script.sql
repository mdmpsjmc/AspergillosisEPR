
DROP INDEX [IX_PatientNACDates_PatientId] ON [PatientNACDates];

GO

CREATE TABLE [TestTypes] (
    [ID] int NOT NULL IDENTITY,
    [Name] nvarchar(max) NULL,
    CONSTRAINT [PK_TestTypes] PRIMARY KEY ([ID])
);

GO

CREATE TABLE [PatientTestResult] (
    [ID] int NOT NULL IDENTITY,
    [TestTypeId] int NOT NULL,
    [PatientId] int NOT NULL,
    [UnitOfMeasurementId] int NOT NULL,
    [Value] decimal(18, 2) NOT NULL,
    [Range] nvarchar(max) NULL,
    [SampleId] nvarchar(max) NULL,
    [DateTaken] datetime2 NOT NULL,
    CONSTRAINT [PK_PatientTestResult] PRIMARY KEY ([ID]),
    CONSTRAINT [FK_PatientTestResult_Patients_PatientId] FOREIGN KEY ([PatientId]) REFERENCES [Patients] ([ID]) ON DELETE CASCADE,
    CONSTRAINT [FK_PatientTestResult_TestTypes_TestTypeId] FOREIGN KEY ([TestTypeId]) REFERENCES [TestTypes] ([ID]) ON DELETE CASCADE,
    CONSTRAINT [FK_PatientTestResult_UnitOfMeasurements_UnitOfMeasurementId] FOREIGN KEY ([UnitOfMeasurementId]) REFERENCES [UnitOfMeasurements] ([ID]) ON DELETE CASCADE
);

GO

CREATE INDEX [IX_PatientNACDates_PatientId] ON [PatientNACDates] ([PatientId]);

GO

CREATE INDEX [IX_PatientTestResult_PatientId] ON [PatientTestResult] ([PatientId]);

GO

CREATE INDEX [IX_PatientTestResult_TestTypeId] ON [PatientTestResult] ([TestTypeId]);

GO

CREATE INDEX [IX_PatientTestResult_UnitOfMeasurementId] ON [PatientTestResult] ([UnitOfMeasurementId]);

GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20181010061227_AddTestTypes', N'2.1.0-rtm-30799');

GO


