
CREATE VIEW [dbo].[Department]
AS
SELECT DepartmentId,Code,Name,ISNULL(ParentId,999) AS ParentId,ManagerId,DepartmentLevel,
~ISNULL(IsDisabled,0) AS IsActive,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate,1 AS lft, 1 AS rgt,GETDATE() AS SyncDate
FROM NexHRM.dbo.[Department]
UNION
SELECT 999 AS DepartmentId,'Nexcel' AS Code, 'Nexcel' AS Name, null AS ParentId, null AS ManagerId,null AS DepartmentId
,1 AS IsActive,null AS CreatedBy, null AS CreatedDate,null AS ModifiedBy, null AS ModifiedDate,1 As lft, 1 AS rgt,GETDATE() AS SyncDate