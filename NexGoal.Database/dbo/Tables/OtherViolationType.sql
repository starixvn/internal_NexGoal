CREATE TABLE [dbo].[OtherViolationType] (
    [Id]           INT           NOT NULL,
    [Name]         NVARCHAR (50) NOT NULL,
    [Value]        FLOAT (53)    NULL,
    [IsActive]     BIT           NOT NULL,
    [CreatedBy]    INT           NULL,
    [CreatedDate]  DATETIME      NULL,
    [ModifiedBy]   INT           NULL,
    [ModifiedDate] DATETIME      NULL
);

