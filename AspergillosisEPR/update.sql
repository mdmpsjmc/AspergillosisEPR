ALTER TABLE [CaseReportFormFields] DROP CONSTRAINT [FK_CaseReportFormFields_CaseReportFormSections_CaseReportFormSectionID];

GO

ALTER TABLE [CaseReportFormSections] DROP CONSTRAINT [FK_CaseReportFormSections_CaseReportFormResults_CaseReportFormResultID];

GO

DROP TABLE [CaseReportFormResults];

GO

EXEC sp_rename N'CaseReportFormSections.CaseReportFormResultID', N'CaseReportFormID', N'COLUMN';

GO

EXEC sp_rename N'CaseReportFormSections.IX_CaseReportFormSections_CaseReportFormResultID', N'IX_CaseReportFormSections_CaseReportFormID', N'INDEX';

GO

EXEC sp_rename N'CaseReportFormFields.CaseReportFormSectionID', N'CaseReportFormSectionId', N'COLUMN';

GO

EXEC sp_rename N'CaseReportFormFields.IX_CaseReportFormFields_CaseReportFormSectionID', N'IX_CaseReportFormFields_CaseReportFormSectionId', N'INDEX';

GO

DECLARE @var0 sysname;
SELECT @var0 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'CaseReportFormSections') AND [c].[name] = N'Name');
IF @var0 IS NOT NULL EXEC(N'ALTER TABLE [CaseReportFormSections] DROP CONSTRAINT [' + @var0 + '];');
ALTER TABLE [CaseReportFormSections] ALTER COLUMN [Name] nvarchar(max) NOT NULL;

GO

DECLARE @var1 sysname;
SELECT @var1 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'CaseReportFormFields') AND [c].[name] = N'Label');
IF @var1 IS NOT NULL EXEC(N'ALTER TABLE [CaseReportFormFields] DROP CONSTRAINT [' + @var1 + '];');
ALTER TABLE [CaseReportFormFields] ALTER COLUMN [Label] nvarchar(max) NOT NULL;

GO

DROP INDEX [IX_CaseReportFormFields_CaseReportFormSectionId] ON [CaseReportFormFields];
DECLARE @var2 sysname;
SELECT @var2 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'CaseReportFormFields') AND [c].[name] = N'CaseReportFormSectionId');
IF @var2 IS NOT NULL EXEC(N'ALTER TABLE [CaseReportFormFields] DROP CONSTRAINT [' + @var2 + '];');
ALTER TABLE [CaseReportFormFields] ALTER COLUMN [CaseReportFormSectionId] int NOT NULL;
CREATE INDEX [IX_CaseReportFormFields_CaseReportFormSectionId] ON [CaseReportFormFields] ([CaseReportFormSectionId]);

GO

ALTER TABLE [CaseReportFormFields] ADD [CaseReportFormID] int NULL;

GO

CREATE TABLE [CaseReportForms] (
    [ID] int NOT NULL IDENTITY,
    [CaseReportFormCategoryId] int NOT NULL,
    [Name] nvarchar(max) NULL,
    CONSTRAINT [PK_CaseReportForms] PRIMARY KEY ([ID]),
    CONSTRAINT [FK_CaseReportForms_CaseReportFormCategories_CaseReportFormCategoryId] FOREIGN KEY ([CaseReportFormCategoryId]) REFERENCES [CaseReportFormCategories] ([ID]) ON DELETE CASCADE
);

GO

CREATE INDEX [IX_CaseReportFormFields_CaseReportFormID] ON [CaseReportFormFields] ([CaseReportFormID]);

GO

CREATE INDEX [IX_CaseReportForms_CaseReportFormCategoryId] ON [CaseReportForms] ([CaseReportFormCategoryId]);

GO

ALTER TABLE [CaseReportFormFields] ADD CONSTRAINT [FK_CaseReportFormFields_CaseReportForms_CaseReportFormID] FOREIGN KEY ([CaseReportFormID]) REFERENCES [CaseReportForms] ([ID]) ON DELETE NO ACTION;

GO

ALTER TABLE [CaseReportFormFields] ADD CONSTRAINT [FK_CaseReportFormFields_CaseReportFormSections_CaseReportFormSectionId] FOREIGN KEY ([CaseReportFormSectionId]) REFERENCES [CaseReportFormSections] ([ID]) ON DELETE CASCADE;

GO

ALTER TABLE [CaseReportFormSections] ADD CONSTRAINT [FK_CaseReportFormSections_CaseReportForms_CaseReportFormID] FOREIGN KEY ([CaseReportFormID]) REFERENCES [CaseReportForms] ([ID]) ON DELETE NO ACTION;

GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20180313124527_UpdateCaseReportForms', N'2.0.1-rtm-125');

GO

ALTER TABLE [CaseReportFormFields] DROP CONSTRAINT [FK_CaseReportFormFields_CaseReportForms_CaseReportFormID];

GO

ALTER TABLE [CaseReportFormFields] DROP CONSTRAINT [FK_CaseReportFormFields_CaseReportFormSections_CaseReportFormSectionId];

GO

ALTER TABLE [CaseReportFormSections] DROP CONSTRAINT [FK_CaseReportFormSections_CaseReportForms_CaseReportFormID];

GO

DROP INDEX [IX_CaseReportFormSections_CaseReportFormID] ON [CaseReportFormSections];

GO

DECLARE @var3 sysname;
SELECT @var3 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'CaseReportFormSections') AND [c].[name] = N'CaseReportFormID');
IF @var3 IS NOT NULL EXEC(N'ALTER TABLE [CaseReportFormSections] DROP CONSTRAINT [' + @var3 + '];');
ALTER TABLE [CaseReportFormSections] DROP COLUMN [CaseReportFormID];

GO

EXEC sp_rename N'CaseReportFormFields.CaseReportFormID', N'CaseReportFormId', N'COLUMN';

GO

EXEC sp_rename N'CaseReportFormFields.IX_CaseReportFormFields_CaseReportFormID', N'IX_CaseReportFormFields_CaseReportFormId', N'INDEX';

GO

DECLARE @var4 sysname;
SELECT @var4 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'CaseReportFormFields') AND [c].[name] = N'CaseReportFormSectionId');
IF @var4 IS NOT NULL EXEC(N'ALTER TABLE [CaseReportFormFields] DROP CONSTRAINT [' + @var4 + '];');
ALTER TABLE [CaseReportFormFields] ALTER COLUMN [CaseReportFormSectionId] int NULL;

GO

CREATE TABLE [CaseReportFormFormSections] (
    [ID] int NOT NULL IDENTITY,
    [CaseReportFormId] int NOT NULL,
    [CaseReportFormSectionId] int NOT NULL,
    CONSTRAINT [PK_CaseReportFormFormSections] PRIMARY KEY ([ID]),
    CONSTRAINT [FK_CaseReportFormFormSections_CaseReportForms_CaseReportFormId] FOREIGN KEY ([CaseReportFormId]) REFERENCES [CaseReportForms] ([ID]) ON DELETE CASCADE
);

GO

CREATE INDEX [IX_CaseReportFormFormSections_CaseReportFormId] ON [CaseReportFormFormSections] ([CaseReportFormId]);

GO

ALTER TABLE [CaseReportFormFields] ADD CONSTRAINT [FK_CaseReportFormFields_CaseReportForms_CaseReportFormId] FOREIGN KEY ([CaseReportFormId]) REFERENCES [CaseReportForms] ([ID]) ON DELETE NO ACTION;

GO

ALTER TABLE [CaseReportFormFields] ADD CONSTRAINT [FK_CaseReportFormFields_CaseReportFormSections_CaseReportFormSectionId] FOREIGN KEY ([CaseReportFormSectionId]) REFERENCES [CaseReportFormSections] ([ID]) ON DELETE NO ACTION;

GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20180313134855_AddCaseReportFormFormSectionModel', N'2.0.1-rtm-125');

GO

CREATE TABLE [CaseReportFormPatientResults] (
    [ID] int NOT NULL IDENTITY,
    [CaseReportFormFieldId] int NOT NULL,
    [CaseReportFormId] int NOT NULL,
    [DateAnswer] datetime2 NULL,
    [NumericAnswer] decimal(18, 2) NULL,
    [PatientId] int NOT NULL,
    [TextAnswer] nvarchar(max) NULL,
    CONSTRAINT [PK_CaseReportFormPatientResults] PRIMARY KEY ([ID]),
    CONSTRAINT [FK_CaseReportFormPatientResults_CaseReportFormFields_CaseReportFormFieldId] FOREIGN KEY ([CaseReportFormFieldId]) REFERENCES [CaseReportFormFields] ([ID]) ON DELETE CASCADE,
    CONSTRAINT [FK_CaseReportFormPatientResults_CaseReportForms_CaseReportFormId] FOREIGN KEY ([CaseReportFormId]) REFERENCES [CaseReportForms] ([ID]) ON DELETE CASCADE,
    CONSTRAINT [FK_CaseReportFormPatientResults_Patients_PatientId] FOREIGN KEY ([PatientId]) REFERENCES [Patients] ([ID]) ON DELETE CASCADE
);

GO

CREATE TABLE [CaseReportFormPatientResultOptionChoices] (
    [ID] int NOT NULL IDENTITY,
    [CaseReportFormOptionChoiceId] int NOT NULL,
    [CaseReportFormPatientResultId] int NOT NULL
);

GO

CREATE INDEX [IX_CaseReportFormFormSections_CaseReportFormSectionId] ON [CaseReportFormFormSections] ([CaseReportFormSectionId]);

GO

CREATE INDEX [IX_CaseReportFormPatientResultOptionChoices_CaseReportFormPatientResultId] ON [CaseReportFormPatientResultOptionChoices] ([CaseReportFormPatientResultId]);

GO

CREATE INDEX [IX_CaseReportFormPatientResults_CaseReportFormFieldId] ON [CaseReportFormPatientResults] ([CaseReportFormFieldId]);

GO

CREATE INDEX [IX_CaseReportFormPatientResults_CaseReportFormId] ON [CaseReportFormPatientResults] ([CaseReportFormId]);

GO

CREATE INDEX [IX_CaseReportFormPatientResults_PatientId] ON [CaseReportFormPatientResults] ([PatientId]);

GO

ALTER TABLE [CaseReportFormFormSections] ADD CONSTRAINT [FK_CaseReportFormFormSections_CaseReportFormSections_CaseReportFormSectionId] FOREIGN KEY ([CaseReportFormSectionId]) REFERENCES [CaseReportFormSections] ([ID]) ON DELETE CASCADE;

GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20180314142701_AddCaseReportFormResultsModels', N'2.0.1-rtm-125');

GO

ALTER TABLE [CaseReportFormPatientResults] ADD [CaseReportFormResultID] int NULL;

GO

CREATE TABLE [CaseReportFormResults] (
    [ID] int NOT NULL IDENTITY,
    [CaseReportFormId] int NOT NULL,
    [DateTaken] datetime2 NOT NULL,
    [PatientId] int NOT NULL,
    CONSTRAINT [PK_CaseReportFormResults] PRIMARY KEY ([ID]),
    CONSTRAINT [FK_CaseReportFormResults_CaseReportForms_CaseReportFormId] FOREIGN KEY ([CaseReportFormId]) REFERENCES [CaseReportForms] ([ID]) ON DELETE CASCADE,
    CONSTRAINT [FK_CaseReportFormResults_Patients_PatientId] FOREIGN KEY ([PatientId]) REFERENCES [Patients] ([ID]) ON DELETE CASCADE
);

GO

CREATE INDEX [IX_CaseReportFormPatientResults_CaseReportFormResultID] ON [CaseReportFormPatientResults] ([CaseReportFormResultID]);

GO

CREATE INDEX [IX_CaseReportFormResults_CaseReportFormId] ON [CaseReportFormResults] ([CaseReportFormId]);

GO

CREATE INDEX [IX_CaseReportFormResults_PatientId] ON [CaseReportFormResults] ([PatientId]);

GO

ALTER TABLE [CaseReportFormPatientResults] ADD CONSTRAINT [FK_CaseReportFormPatientResults_CaseReportFormResults_CaseReportFormResultID] FOREIGN KEY ([CaseReportFormResultID]) REFERENCES [CaseReportFormResults] ([ID]) ON DELETE NO ACTION;

GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20180315110215_AddCaseReportFormResult', N'2.0.1-rtm-125');

GO

ALTER TABLE [CaseReportFormPatientResults] DROP CONSTRAINT [FK_CaseReportFormPatientResults_CaseReportFormResults_CaseReportFormResultID];

GO

EXEC sp_rename N'CaseReportFormPatientResults.CaseReportFormResultID', N'CaseReportFormResultId', N'COLUMN';

GO

EXEC sp_rename N'CaseReportFormPatientResults.IX_CaseReportFormPatientResults_CaseReportFormResultID', N'IX_CaseReportFormPatientResults_CaseReportFormResultId', N'INDEX';

GO

ALTER TABLE [CaseReportFormResults] ADD [CaseReportFormCategoryId] int NOT NULL DEFAULT 0;

GO

DROP INDEX [IX_CaseReportFormPatientResults_CaseReportFormResultId] ON [CaseReportFormPatientResults];
DECLARE @var5 sysname;
SELECT @var5 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'CaseReportFormPatientResults') AND [c].[name] = N'CaseReportFormResultId');
IF @var5 IS NOT NULL EXEC(N'ALTER TABLE [CaseReportFormPatientResults] DROP CONSTRAINT [' + @var5 + '];');
ALTER TABLE [CaseReportFormPatientResults] ALTER COLUMN [CaseReportFormResultId] int NOT NULL;
CREATE INDEX [IX_CaseReportFormPatientResults_CaseReportFormResultId] ON [CaseReportFormPatientResults] ([CaseReportFormResultId]);

GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20180315123436_AddCategoryIdToCaseReportFormResult', N'2.0.1-rtm-125');

GO


