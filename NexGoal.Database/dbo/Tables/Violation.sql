CREATE TABLE [dbo].[Violation] (
    [ViolationId]  INT            IDENTITY (1, 1) NOT NULL,
    [EmployeeId]   INT            NULL,
    [Description]  NVARCHAR (MAX) NULL,
    [IsActive]     BIT            NULL,
    [EffectedDate] DATETIME       NULL,
    [CreatedBy]    INT            NULL,
    [CreateDate]   DATETIME       NULL,
    [ModifiedBy]   INT            NULL,
    [ModifiedDate] DATETIME       NULL,
    CONSTRAINT [PK_ViolationId] PRIMARY KEY CLUSTERED ([ViolationId] ASC)
);

