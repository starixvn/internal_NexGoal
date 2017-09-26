


CREATE PROCEDURE [dbo].[PAFocus_Update] @PAFocusXML NVARCHAR(max)
	,@ModifiedBy INT
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg VARCHAR(200) = '' OUTPUT
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

	DECLARE @DocHandler INT
		,@LastObjectiveDate DATETIME

	IF OBJECT_ID('tempdb..#Focus') IS NOT NULL
	BEGIN
		DROP TABLE #Focus;
	END;

	CREATE TABLE #Focus (
		PAFocusId INT
		,LastObjective NVARCHAR(max)
		,NextObjective NVARCHAR(max)
		,IsReadyPromotion NVARCHAR(max)
		,IsPoorPerformance NVARCHAR(max)
		,Contribution NVARCHAR(max)
		,BonusGiven NVARCHAR(max)
		);

	BEGIN TRY
		BEGIN TRANSACTION;

		EXEC sp_xml_preparedocument @DocHandler OUTPUT
			,@PAFocusXML

		INSERT INTO #Focus
		SELECT PAFocusId
			,LastObjective
			,NextObjective
			,IsReadyPromotion
			,IsPoorPerformance
			,Contribution
			,BonusGiven
		FROM OPENXML(@DocHandler, '/ROOT/PAFocus', 1) WITH (PAFocusId INT, LastObjective NVARCHAR(max), NextObjective NVARCHAR(max), IsReadyPromotion NVARCHAR(max), IsPoorPerformance NVARCHAR(max), Contribution NVARCHAR(max), BonusGiven NVARCHAR(max)) AS Data;

		UPDATE l
		SET l.[Description] = f.LastObjective
		FROM dbo.ObjectiveEmployee AS l WITH (NOLOCK)
			INNER JOIN dbo.PAFocus AS pa WITH (NOLOCK) ON pa.LastObjectiveId = l.ObjectiveId
			INNER JOIN #Focus AS f WITH (NOLOCK) ON f.PAFocusId = pa.PAFocusId
		WHERE pa.LastObjectiveId IS NOT NULL

		UPDATE n
		SET n.[Description] = f.NextObjective
		FROM dbo.ObjectiveEmployee AS n WITH (NOLOCK)
			INNER JOIN dbo.PAFocus AS pa WITH (NOLOCK) ON pa.NextObjectiveId = n.ObjectiveId
			INNER JOIN #Focus AS f ON f.PAFocusId = pa.PAFocusId

		UPDATE pa
		SET pa.IsReadyPromotion = f.IsReadyPromotion
			,pa.IsPoorPerformance = f.IsPoorPerformance
			,pa.Contribution = f.Contribution
			,pa.BonusGiven = f.BonusGiven
			,ModifiedBy = @ModifiedBy
			,ModifiedDate = GETDATE()
		FROM dbo.PAFocus AS pa WITH (NOLOCK)
			INNER JOIN #Focus AS f ON f.PAFocusId = pa.PAFocusId

		-- INSERT AND UPDATE WHEN NOT EXIST Last Objective of Employee
		SET @LastObjectiveDate = DATEADD(month, - 1, GETDATE())

		INSERT INTO dbo.ObjectiveEmployee
		SELECT p.EmployeeId
			,NULL
			,pa.LastObjective
			,@LastObjectiveDate
		FROM #Focus AS pa
			INNER JOIN dbo.PAFocus AS p WITH (NOLOCK) ON p.PAFocusId = pa.PAFocusId
		WHERE p.LastObjectiveId IS NULL

		UPDATE pa
		SET pa.LastObjectiveId = o.ObjectiveId
		FROM dbo.PAFocus AS pa WITH (NOLOCK)
			INNER JOIN dbo.ObjectiveEmployee AS o WITH (NOLOCK) ON o.EmployeeId = pa.EmployeeId
		WHERE o.CreatedDate = @LastObjectiveDate
			AND pa.LastObjectiveId IS NULL

		COMMIT TRANSACTION;
	END TRY

	BEGIN CATCH
		ROLLBACK TRANSACTION;

		SELECT @ErrCode = 1
			,@ErrMsg = N'Update PAFocus unsuccessfully.';
	END CATCH

	DROP TABLE #Focus
END