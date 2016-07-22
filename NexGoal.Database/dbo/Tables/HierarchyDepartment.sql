CREATE TABLE [dbo].[HierarchyDepartment] (
    [ParentId]     INT           NULL,
    [DepartmentId] INT           NULL,
    [Level]        INT           NULL,
    [ParentIds]    NVARCHAR (30) NULL
);

