CREATE TABLE [dbo].[EmployeeRole] (
    [EmployeeId] INT      NOT NULL,
    [RoleId]     SMALLINT NOT NULL,
    CONSTRAINT [PK_EmployeeRole] PRIMARY KEY CLUSTERED ([EmployeeId] ASC, [RoleId] ASC)
);

