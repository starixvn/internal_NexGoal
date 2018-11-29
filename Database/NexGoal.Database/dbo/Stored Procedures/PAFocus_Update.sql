
CREATE PROCEDURE [dbo].[PAFocus_Update] @ActorId INT
	,@Value NVARCHAR(MAX)
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg NVARCHAR(200) = '' OUTPUT
AS
/*
Created: 20181010@Tim.Trinh
Task: PA Focus
DB: bodb_NexGoal
Original:

Revisions:
Param's Explanation:
    Value : '<Root><Row PAFocusId="4" CurrentObjective="encrypted" NextObjective="encrypted"
        IsReadyPromotion="encrypted" Contribution="encrypted" BonusGiven="encrypted"></Row></ROOT>'
*/
BEGIN
	SET NOCOUNT ON;

	DECLARE @idoc INT
		,@LastObjectiveDate DATE = DATEADD(MONTH, - 1, GETDATE())

	CREATE TABLE #tblPAFocus (
		PAFocusId INT
		,CurrentObjective NVARCHAR(MAX)
		,NextObjective NVARCHAR(MAX)
		,IsReadyPromotion NVARCHAR(MAX)
		,Contribution NVARCHAR(MAX)
		,BonusGiven NVARCHAR(MAX)
		)

	EXEC sp_xml_preparedocument @idoc OUTPUT
		,@Value

	INSERT INTO #tblPAFocus (
		PAFocusId
		,CurrentObjective
		,NextObjective
		,IsReadyPromotion
		,Contribution
		,BonusGiven
		)
	SELECT PAFocusId
		,CurrentObjective
		,NextObjective
		,IsReadyPromotion
		,Contribution
		,BonusGiven
	FROM OPENXML(@idoc, '/Root/Row', 1) WITH (PAFocusId INT, CurrentObjective NVARCHAR(MAX), NextObjective NVARCHAR(MAX), IsReadyPromotion NVARCHAR(MAX), Contribution NVARCHAR(MAX), BonusGiven NVARCHAR(MAX)) AS Data

	UPDATE l
	SET l.[Description] = f.CurrentObjective
	FROM dbo.EmployeeObjective AS l WITH (ROWLOCK, UPDLOCK)
		INNER JOIN dbo.PAFocus AS pa WITH (NOLOCK) ON pa.LastObjectiveId = l.ObjectiveId
		INNER JOIN #tblPAFocus AS f WITH (NOLOCK) ON f.PAFocusId = pa.PAFocusId
	WHERE pa.LastObjectiveId IS NOT NULL

	UPDATE n
	SET n.[Description] = f.NextObjective
	FROM dbo.EmployeeObjective AS n WITH (ROWLOCK, UPDLOCK)
		INNER JOIN dbo.PAFocus AS pa WITH (NOLOCK) ON pa.NextObjectiveId = n.ObjectiveId
		INNER JOIN #tblPAFocus AS f ON f.PAFocusId = pa.PAFocusId

	UPDATE pa
	SET pa.IsReadyPromotion = f.IsReadyPromotion
		,pa.Contribution = f.Contribution
		,pa.BonusGiven = f.BonusGiven
		,ModifiedBy = @ActorId
		,ModifiedDate = GETDATE()
	FROM dbo.PAFocus AS pa WITH (NOLOCK)
		INNER JOIN #tblPAFocus AS f ON f.PAFocusId = pa.PAFocusId

	-- INSERT AND UPDATE WHEN NOT EXIST Last Objective of Employee
	INSERT INTO dbo.EmployeeObjective (
		EmployeeId
		,PercentCompleted
		,[Description]
		,CreatedDate
		)
	SELECT p.EmployeeId
		,NULL
		,pa.CurrentObjective
		,@LastObjectiveDate
	FROM #tblPAFocus AS pa
		INNER JOIN dbo.PAFocus AS p WITH (NOLOCK) ON p.PAFocusId = pa.PAFocusId
	WHERE p.LastObjectiveId IS NULL
		AND ISNULL(pa.CurrentObjective, '') <> ''

	UPDATE pa
	SET pa.LastObjectiveId = o.ObjectiveId
	FROM dbo.PAFocus AS pa WITH (ROWLOCK, UPDLOCK)
		INNER JOIN dbo.EmployeeObjective AS o WITH (NOLOCK) ON o.EmployeeId = pa.EmployeeId
	WHERE o.CreatedDate = @LastObjectiveDate
		AND pa.LastObjectiveId IS NULL

	IF @@ERROR > 0
	BEGIN
		SELECT @ErrCode = 1
			,@ErrMsg = 'Failed to update PA Focus.'

		RETURN
	END

	DROP TABLE #tblPAFocus
END
