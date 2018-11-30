CREATE TABLE [dbo].[Function] (
    [FunctionId]   SMALLINT       IDENTITY (1, 1) NOT NULL,
    [FunctionKey]  VARCHAR (50)   NOT NULL,
    [Name]         NVARCHAR (200) NOT NULL,
    [ParentId]     SMALLINT       NULL,
    [OnlyPolicy]   TINYINT        NULL,
    [DisplayOrder] SMALLINT       NOT NULL,
    [IsActive]     BIT            NOT NULL,
    CONSTRAINT [PK_Function] PRIMARY KEY CLUSTERED ([FunctionId] ASC)
);



