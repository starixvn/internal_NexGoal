

CREATE PROCEDURE [dbo].[NexGoalTools_UpdateEPAScore] @epaInfoXml NVARCHAR(MAX)
	,@processDetailXml NVARCHAR(MAX)
	,@epaDetailXml NVARCHAR(MAX)
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg VARCHAR(200) = '' OUTPUT
AS
/*
Created: 20160830@Tim.Trinh
Task: update score.
DB: NexGoal
Original: 

Revisions:  
Param's Explanation:
• @epaInfoXml=N'
     <ROOT>
		 <EPA EPAId='' FinalScore='' Grade=''></EPA>
	</ROOT>'
• @processDetailXml=N'
     <ROOT>
		 <EPAProcess EPAId='' ProcessId='' EvaluatorId='' TotalScore='' AdjustScore=''></EPAProcess>
	</ROOT>'
• @epaDetailXml=N'
     <ROOT>
		 <EPADetail EPAId='' ProcessId='' EvaluatorId='' KPIGroupId='' KPIId='' Score='' Rate=''></EPADetail>
	</ROOT>'
• @ErrCode:
	= 0: Transaction recorded successfully.
	> 0: Specific Error.
• @ErrMsg: Error description.

*/
BEGIN
	SET NOCOUNT ON;

	CREATE TABLE #EPA (
		EPAId INT
		,FinalScore NVARCHAR(MAX)
		,Grade NVARCHAR(MAX)
		)

	CREATE TABLE #EPAProcess (
		EPAId INT
		,ProcessId INT
		,EvaluatorId INT
		,TotalScore NVARCHAR(MAX)
		,AdjustScore NVARCHAR(MAX)
		)

	CREATE TABLE #EPADetail (
		EPAId INT
		,ProcessId INT
		,EvaluatorId INT
		,KPIGroupId INT
		,KPIId INT
		,Score NVARCHAR(MAX)
		,Rate NVARCHAR(MAX)
		)

	BEGIN TRY
		DECLARE @DocHandler INT

		EXEC sp_xml_preparedocument @DocHandler OUTPUT
			,@epaInfoXml

		INSERT INTO #EPA
		SELECT EPAId
			,FinalScore
			,Grade
		FROM OPENXML(@DocHandler, '/ROOT/EPA', 1) WITH (EPAId INT, FinalScore NVARCHAR(MAX), Grade NVARCHAR(MAX)) AS Data

		EXEC sp_xml_preparedocument @DocHandler OUTPUT
			,@processDetailXml

		INSERT INTO #EPAProcess
		SELECT EPAId
			,ProcessId
			,EvaluatorId
			,TotalScore
			,AdjustScore
		FROM OPENXML(@DocHandler, '/ROOT/EPAProcess', 1) WITH (EPAId INT, ProcessId INT, EvaluatorId INT, TotalScore NVARCHAR(MAX), AdjustScore NVARCHAR(MAX)) AS Data

		EXEC sp_xml_preparedocument @DocHandler OUTPUT
			,@epaDetailXml

		INSERT INTO #EPADetail
		SELECT EPAId
			,ProcessId
			,EvaluatorId
			,KPIGroupId
			,KPIId
			,Score
			,Rate
		FROM OPENXML(@DocHandler, '/ROOT/EPADetail', 1) WITH (EPAId INT, ProcessId INT, EvaluatorId INT, KPIGroupId INT, KPIId INT, Score NVARCHAR(MAX), Rate NVARCHAR(MAX)) AS Data

		EXEC sp_xml_removedocument @DocHandler
	END TRY

	BEGIN CATCH
		SELECT @ErrCode = 1
			,@ErrMsg = N'Update unsuccessfully. Parameter invalid.'

		RETURN
	END CATCH

	BEGIN TRY
		BEGIN TRANSACTION

		UPDATE epa
		SET epa.FinalScore = tmp.FinalScore
			,epa.Grade = tmp.Grade
		FROM dbo.EPA AS epa WITH (ROWLOCK, UPDLOCK)
			INNER JOIN #EPA AS tmp WITH (NOLOCK) ON epa.EPAId = tmp.EPAId

		UPDATE pc
		SET pc.TotalScore = tmp.TotalScore
			,pc.AdjustScore = tmp.AdjustScore
		FROM dbo.EPAProcess AS pc WITH (ROWLOCK, UPDLOCK)
			INNER JOIN #EPAProcess AS tmp WITH (NOLOCK) ON pc.EPAId = tmp.EPAId
			AND pc.ProcessId = tmp.ProcessId
			AND pc.EvaluatorId = tmp.EvaluatorId

		UPDATE dt
		SET dt.Score = tmp.Score
			,dt.Rate = tmp.Rate
		FROM dbo.EPADetail AS dt WITH (ROWLOCK, UPDLOCK)
			INNER JOIN #EPADetail AS tmp WITH (NOLOCK) ON dt.EPAId = tmp.EPAId
			AND dt.ProcessId = tmp.ProcessId
			AND dt.EvaluatorId = tmp.EvaluatorId
			AND dt.KPIGroupId = tmp.KPIGroupId
			AND dt.KPIId = tmp.KPIId

		COMMIT TRANSACTION
	END TRY

	BEGIN CATCH
		ROLLBACK TRANSACTION

		SELECT @ErrCode = 1
			,@ErrMsg = N'Update unsuccessfully.'
	END CATCH

	DROP TABLE #EPA

	DROP TABLE #EPAProcess

	DROP TABLE #EPADetail
END