CREATE TABLE [dbo].[CustomValues] (
    [Id]             INT          NOT NULL,
    [CustomizedType] VARCHAR (30) NOT NULL,
    [CustomizedId]   INT          NOT NULL,
    [CustomFieldId]  INT          NOT NULL,
    [Value]          TEXT         NULL,
    CONSTRAINT [PK_CustomValues] PRIMARY KEY CLUSTERED ([Id] ASC)
);

