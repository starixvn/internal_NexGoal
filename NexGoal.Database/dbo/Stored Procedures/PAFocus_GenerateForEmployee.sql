



CREATE PROCEDURE [dbo].[PAFocus_GenerateForEmployee] @PeriodId INT
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg NVARCHAR(200) = '' OUTPUT
AS
/*
Created: 20160830@Razor.Nguyen
Task: PA Focus
DB: NexGoal
Original: 

Revisions:  
Param's Explanation:

*/
BEGIN
	SET NOCOUNT ON;

	DECLARE @lastObjectiveId INT
		,@nextObjectiveId INT
		,@EmployeeId INT
		,@GroupId int

	CREATE TABLE #EmployeeIds (EmployeeId INT,GroupId int)

	INSERT INTO #EmployeeIds
	SELECT DISTINCT e.EmployeeId,e.GroupId
	FROM dbo.Employee AS e WITH (NOLOCK)
	INNER JOIN dbo.SettingDepartmentFlow as a WITH (NOLOCK) on a.DepartmentId = e.GroupId
	WHERE e.IsDeleted = 0
		AND e.[Enabled] = 1
		AND e.EmployeeId NOT IN (
			SELECT EmployeeId
			FROM dbo.PAFocus WITH (NOLOCK)
			WHERE PeriodId = @PeriodId
			)
		AND e.EmployeeId NOT IN (
			SELECT DISTINCT EvaluatorId
			FROM dbo.SettingDepartmentFlow
			WHERE IsGroup=1
			)

	BEGIN TRY
		BEGIN TRANSACTION

		WHILE (
				EXISTS (
					SELECT 1
					FROM #EmployeeIds
					)
				)
		BEGIN
			SELECT @EmployeeId = EmployeeId,@GroupId = GroupId
			FROM #EmployeeIds

			SELECT TOP 1 @lastObjectiveId = ObjectiveId
			FROM dbo.ObjectiveEmployee WITH (NOLOCK)
			WHERE EmployeeId = @EmployeeId
			ORDER BY CreatedDate DESC

			INSERT INTO [dbo].[ObjectiveEmployee]
			VALUES (
				@EmployeeId
				,NULL
				,NULL
				,GETDATE()
				)

			SET @nextObjectiveId = SCOPE_IDENTITY()

			INSERT INTO dbo.PAFocus
			SELECT @PeriodId
				,@EmployeeId
				,@GroupId
				,@lastObjectiveId
				,@nextObjectiveId
				,NULL
				,NULL
				,NULL
				,NULL
				,GetDate()

			EXEC dbo.PAFocusProcess_Generate @PeriodId
				,@EmployeeId
				,@GroupId
				,@ErrCode OUTPUT
				,@ErrMsg OUTPUT

			DELETE
			FROM #EmployeeIds
			WHERE EmployeeId = @EmployeeId
		END

		COMMIT TRANSACTION;
	END TRY

	BEGIN CATCH
		ROLLBACK TRANSACTION;

		SELECT @ErrCode = 1
			,@ErrMsg = N'Generate PAFocus unsuccessfully.EmployeeId = ' + Convert(VARCHAR(10), @EmployeeId);
	END CATCH;

	DROP TABLE #EmployeeIds;
END