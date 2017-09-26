
CREATE PROCEDURE [dbo].[EOQYNomination_GetByNominatorId] @EOQYId INT
	,@NominatorId INT
	,@Query NVARCHAR(200) = NULL
	,@GroupIds NVARCHAR(MAX) = NULL
	,@StatusId TINYINT = NULL
AS
/*
Created: 20160907@Tim.Trinh
Task: Get nomination by NominatorId.
DB: NexGoal
Original: 

Revisions:  
Param's Explanation:

*/
BEGIN
	SET NOCOUNT ON;

	DECLARE @Groups IntId;

	IF @GroupIds IS NULL
	BEGIN
		INSERT INTO @Groups (Id)
		SELECT DISTINCT val
		FROM dbo.fnc_strSplit(@GroupIds, ',')
	END

	SELECT DISTINCT nt.NominationId
		,nt.StatusId
		,setting.RoleId
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
		,nominator.FullName
		,nominator.NickName
	FROM dbo.EOQYNomination AS nt WITH (NOLOCK)
		INNER JOIN dbo.EOQYNominatorSetting AS setting WITH (NOLOCK) ON setting.EOQYId = nt.EOQYId
		INNER JOIN dbo.EOQY AS eoq WITH (NOLOCK) ON nt.EOQYId = eoq.EOQYId
		INNER JOIN dbo.Employee AS nominee WITH (NOLOCK) ON nt.NomineeId = nominee.EmployeeId
		LEFT JOIN dbo.[Group] AS gr WITH (NOLOCK) ON nominee.GroupId = gr.GroupId
		LEFT JOIN dbo.Department AS dept WITH (NOLOCK) ON gr.DepartmentId = dept.DepartmentId
		LEFT JOIN dbo.HRMJobTitle AS job WITH (NOLOCK) ON nominee.JobTitle = job.Id
		INNER JOIN dbo.Employee AS nominator WITH (NOLOCK) ON nt.NominatorId = nominator.EmployeeId
	WHERE eoq.EOQYId = @EOQYId
		AND (
			(
				nt.NominatorId = @NominatorId
				AND setting.RoleId = 1
				AND eoq.StartDate <= CAST(GETDATE() AS DATE)
				)
			OR (
				setting.NominatorId = @NominatorId
				AND setting.RoleId = 2
				--AND eoq.EndDate < CAST(GETDATE() AS DATE)
				)
			)
		AND (
			nominee.FullName LIKE '%' + ISNULL(@Query, nominee.FullName) + '%'
			OR nominee.NickName LIKE '%' + ISNULL(@Query, nominee.NickName) + '%'
			)
		AND (
			ISNULL(@GroupIds, '') = ''
			OR EXISTS (
				SELECT 1
				FROM @Groups
				WHERE Id < 0
					AND gr.GroupId = ABS(Id)
				)
			OR EXISTS (
				SELECT 1
				FROM @Groups
				WHERE Id > 0
					AND dept.DepartmentId = Id
				)
			)
		AND ISNULL(@StatusId, nt.StatusId) = nt.StatusId
END