CREATE TABLE [dbo].[RoleFunction] (
    [RoleId]     SMALLINT NOT NULL,
    [FunctionId] SMALLINT NOT NULL,
    [Policy]     TINYINT  NOT NULL,
    CONSTRAINT [PK_RoleFunction] PRIMARY KEY CLUSTERED ([RoleId] ASC, [FunctionId] ASC)
);

