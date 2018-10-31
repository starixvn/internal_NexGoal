CREATE VIEW [dbo].[Employee]
AS
SELECT Id AS EmployeeId
	,DomainUsername
	,Code
	,FullName
	,NickName
	,GroupId
	,[Enabled]
	,ISNULL(isDeleted, 0) AS IsDeleted
	,CAST(JoinedDate AS DATE) AS JoinedDate
	,CAST(ConfirmedDate AS DATE) AS ConfirmedDate
	,JobTitleId
	,Email
	,1 AS IsLocal
FROM [$(NexHRM)].dbo.Employee WITH (NOLOCK)

