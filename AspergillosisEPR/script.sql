

CREATE TABLE [CaseReportFormFieldOptions] (
    [ID] int NOT NULL IDENTITY,
    [CaseReportFormFieldId] int NOT NULL,
    [CaseReportFormOptionChoiceId] int NOT NULL,
    CONSTRAINT [PK_CaseReportFormFieldOptions] PRIMARY KEY ([ID]),
    CONSTRAINT [FK_CaseReportFormFieldOptions_CaseReportFormFields_CaseReportFormFieldId] FOREIGN KEY ([CaseReportFormFieldId]) REFERENCES [CaseReportFormFields] ([ID]) ON DELETE CASCADE,
    CONSTRAINT [FK_CaseReportFormFieldOptions_CaseReportFormOptionChoices_CaseReportFormOptionChoiceId] FOREIGN KEY ([CaseReportFormOptionChoiceId]) REFERENCES [CaseReportFormOptionChoices] ([ID]) ON DELETE CASCADE
);

GO
ALTER TABLE [CaseReportForms] ADD [IsLocked] bit NOT NULL DEFAULT 0;

GO