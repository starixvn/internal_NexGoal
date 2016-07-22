CREATE TABLE [dbo].[KPIGroup] (
    [KPIGroupId]   INT            NOT NULL,
    [Name]         NVARCHAR (500) NOT NULL,
    [IsActive]     BIT            NULL,
    [Description]  NVARCHAR (MAX) NULL,
    [LogNo]        VARCHAR (20)   NULL,
    [CreatedBy]    INT            NULL,
    [CreatedDate]  DATETIME       NULL,
    [ModifiedBy]   INT            NULL,
    [ModifiedDate] DATETIME       NULL,
    [DisplayOrder] INT            NULL
);

