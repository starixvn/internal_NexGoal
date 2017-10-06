



CREATE VIEW [dbo].[Employee]
AS
SELECT        Id AS EmployeeId, DomainUserName, Code,FullName, NickName,GroupId,Phone,IdCardNumber AS CardNumber,
Enabled,ISNULL(isDeleted,0) As IsDeleted, 0 AS IsSpecialUser,'' AS Password, JoinedDate, ConfirmedDate,JobTitleId As JobTitle,
ModifiedBy, ModifiedDate, CreatedBy,CreatedDate,Email, GetDate() As SyncDate, 1 AS IsLocal, 0 As HasMonthlyEPA
FROM            NexHRM.dbo.Employee AS View_Employee1 WITH (NOLOCK)