CREATE PROCEDURE [dbo].[PMSetting_Get]
	@PMIds VARCHAR(MAX)
	,@EmployeeId INT = NULL
AS
/*
Created: 20190107@Elton.Ton
Task: get PM setting
DB: bodb_NexGoal
*/
BEGIN
	SET NOCOUNT ON;

	SELECT e.EmployeeId
		,e.Code
		,e.FullName
		,e.NickName
		,jt.Name AS JobTitle
		,pms.ProjectManagerId
	FROM dbo.Employee e WITH (NOLOCK)
		INNER JOIN dbo.JobTitle jt WITH (NOLOCK) ON e.JobTitleId = jt.JobTitleId
		INNER JOIN dbo.PMSetting pms WITH (NOLOCK) ON e.EmployeeId = pms.MemberId
	WHERE EXISTS (
			SELECT 1
			FROM dbo.func_SplitString(@PMIds, ',') pm
			WHERE pms.ProjectManagerId = pm.val
		)
		AND (@EmployeeId IS NULL OR pms.MemberId = @EmployeeId)
	ORDER BY e.NickName
END

GO


