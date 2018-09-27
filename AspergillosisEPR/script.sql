
CREATE TABLE [PatientNACDates] (
    [ID] int NOT NULL IDENTITY,
    [PatientId] int NOT NULL,
    [ProbableStartOfDisease] datetime2 NULL,
    [DefiniteStartOfDisease] datetime2 NULL,
    [DateOfDiagnosis] datetime2 NULL,
    [FirstSeenAtNAC] datetime2 NOT NULL,
    [CPABand] int NULL,
    CONSTRAINT [PK_PatientNACDates] PRIMARY KEY ([ID]),
    CONSTRAINT [FK_PatientNACDates_Patients_PatientId] FOREIGN KEY ([PatientId]) REFERENCES [Patients] ([ID]) ON DELETE CASCADE
);

GO

CREATE UNIQUE INDEX [IX_PatientNACDates_PatientId] ON [PatientNACDates] ([PatientId]);

GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20180927081418_AddPatientNACDates', N'2.1.0-rtm-30799');

GO


