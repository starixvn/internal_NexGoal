CREATE TABLE [dbo].[MailConfig] (
    [EmployeeId] INT     NOT NULL,
    [Type]       TINYINT NOT NULL,
    [Notify]     BIT     NOT NULL,
    CONSTRAINT [PK_MailConfig] PRIMARY KEY CLUSTERED ([EmployeeId] ASC, [Type] ASC)
);

