
CREATE PROCEDURE [dbo].[Auto_PA_UpdateStatus] @ErrCode INT = 0 OUTPUT
	,@ErrMsg NVARCHAR(200) = '' OUTPUT
AS
/*
Created: 20180919@Tim.Trinh
Task: Update PA status.
DB: bodb_NexGoal
Original:

Revisions:
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;
	SET XACT_ABORT ON;

	DECLARE @Today DATE = GETDATE()
		,@PAOpened TINYINT = 2
		,@PAInProgress TINYINT = 3
		,@PAClosed TINYINT = 4
		,@HRProcessId TINYINT = 1
		,@ReviewerProcessId TINYINT = 5

	UPDATE pa
	SET pa.StatusId = (CASE 
			WHEN @Today >= hr.StartDate
				AND (
					@Today <= reviewer.EndDate
					OR @Today <= period.EndDate
					)
				AND pa.StatusId <> @PAInProgress THEN @PAInProgress
			WHEN period.EndDate < @Today
				AND reviewer.EndDate < @Today
				AND pa.StatusId <> @PAClosed THEN @PAClosed
			ELSE pa.StatusId END
			)
	FROM dbo.PARecord AS pa WITH (ROWLOCK, UPDLOCK)
		INNER JOIN dbo.Period AS period WITH (NOLOCK) ON pa.PeriodId = period.PeriodId
		AND period.IsActive = 1
		INNER JOIN (
		SELECT DISTINCT pa1.PARecordId
			,StartDate
			,EndDate
		FROM dbo.PARecord AS pa1 WITH (NOLOCK)
			INNER JOIN dbo.PAProcess AS process WITH (NOLOCK) ON pa1.PARecordId = process.PARecordId
		WHERE pa1.StatusId IN (@PAOpened, @PAInProgress)
			AND ProcessId = @HRProcessId
		) AS hr ON pa.PARecordId = hr.PARecordId
		INNER JOIN dbo.PAProcess AS reviewer WITH (NOLOCK) ON pa.PARecordId = reviewer.PARecordId
		AND reviewer.ProcessId = @ReviewerProcessId
	WHERE pa.StatusId IN (@PAOpened, @PAInProgress)
END
