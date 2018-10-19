
CREATE TABLE [PatientMRCScores] (
    [ID] int NOT NULL IDENTITY,
    [PatientId] int NOT NULL,
    [DateTaken] datetime2 NOT NULL,
    [Score] nvarchar(max) NULL,
    CONSTRAINT [PK_PatientMRCScores] PRIMARY KEY ([ID]),
    CONSTRAINT [FK_PatientMRCScores_Patients_PatientId] FOREIGN KEY ([PatientId]) REFERENCES [Patients] ([ID]) ON DELETE CASCADE
);

GO

CREATE INDEX [IX_PatientMRCScores_PatientId] ON [PatientMRCScores] ([PatientId]);

GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20181018112339_CreateMRCScoreTable', N'2.1.0-rtm-30799');

GO


