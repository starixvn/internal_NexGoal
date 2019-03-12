



CREATE VIEW [dbo].[Employee]
AS
SELECT emp.Id AS EmployeeId
	,emp.DomainUsername
	,emp.Code
	,emp.FullName
	,emp.NickName
	,emp.GroupId
	,emp.[Enabled]
	,emp.isDeleted AS IsDeleted
	,CAST(emp.JoinedDate AS DATE) AS JoinedDate
	,CAST(emp.ConfirmedDate AS DATE) AS ConfirmedDate
	,emp.JobTitleId
	,emp.Email
	,empc.ConversationId
FROM [$(NexHRM)].dbo.Employee AS emp WITH (NOLOCK)
LEFT JOIN [$(NexHRM)].dbo.EmployeeConversation AS empc WITH (NOLOCK) ON emp.Id = empc.EmpId

