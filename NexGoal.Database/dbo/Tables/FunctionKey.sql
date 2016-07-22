CREATE TABLE [dbo].[FunctionKey] (
    [FunctionId]       INT            NOT NULL,
    [FunctionKey]      VARCHAR (50)   NOT NULL,
    [Name]             NVARCHAR (200) NOT NULL,
    [ParentFunctionId] INT            NULL,
    [IsActive]         BIT            NULL,
    [IsReadOnly]       BIT            NULL
);

