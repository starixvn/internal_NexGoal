


CREATE VIEW [dbo].[EmployeeSkypeConversation]
AS
SELECT emp.Id AS EmployeeId
	  ,c.ConversationId
FROM NexHRM.dbo.Employee AS emp WITH (NOLOCK)
LEFT JOIN NexBot.dbo.ConversationUser AS c WITH (NOLOCK) ON emp.NickName = c.NickName
WHERE emp.isDeleted = 0 AND emp.[Enabled] = 1