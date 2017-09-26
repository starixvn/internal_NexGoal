





CREATE PROCEDURE [dbo].[EOQNomination_GetById] @NominationId INT
AS
/*
Created: 20160908@Tim.Trinh
Task: get nomination by id.
DB: NexGoal
Original: 

Revisions:  
Param's Explanation:

*/
BEGIN
	SET NOCOUNT ON

	SELECT nt.NominationId
		,nt.StatusId
		,nt.EOQYId
		,eoq.[Quarter]
		,eoq.[Year]
		,eoq.StartDate
		,eoq.EndDate
		,eoq.RemindDate
		,nominee.EmployeeId
		,nominee.Code
		,nominee.NickName
		,nominee.FullName
		,gr.DepartmentId
		,dept.Code
		,dept.[Name]
		,gr.GroupId
		,gr.Code AS GroupCode
		,gr.[Name] AS GroupName
		,job.Id AS JobTitleId
		,job.[Name] AS JobTitleName
		,nominator.EmployeeId AS NominatorId
		,nominator.NickName
		,nominator.FullName
	FROM dbo.EOQYNomination AS nt WITH (NOLOCK)
		INNER JOIN dbo.EOQY AS eoq WITH (NOLOCK) ON nt.EOQYId = eoq.EOQYId
		INNER JOIN dbo.Employee AS nominee WITH (NOLOCK) ON nt.NomineeId = nominee.EmployeeId
		LEFT JOIN dbo.[Group] AS gr WITH (NOLOCK) ON nominee.GroupId = gr.GroupId
		LEFT JOIN dbo.Department AS dept WITH (NOLOCK) ON gr.DepartmentId = dept.DepartmentId
		LEFT JOIN dbo.HRMJobTitle AS job WITH (NOLOCK) ON nominee.JobTitle = job.Id
		INNER JOIN dbo.Employee AS nominator WITH (NOLOCK) ON nt.NominatorId = nominator.EmployeeId
	WHERE nt.NominationId = @NominationId

	SELECT dt.QuestionId
		,question.[Name]
		,dt.Comment
	FROM dbo.EOQYNomination AS nt WITH (NOLOCK)
		INNER JOIN dbo.EOQYNominationDetail AS dt WITH (NOLOCK) ON nt.NominationId = dt.NominationId
		INNER JOIN dbo.EOQQuestion AS question WITH (NOLOCK) ON dt.QuestionId = question.QuestionId
	WHERE nt.NominationId = @NominationId
END