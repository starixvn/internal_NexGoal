CREATE PROCEDURE [dbo].[EPA_UpdateFinalScoreEndEvaluation] @ModifiedBy INT
	,@EPAScores VARCHAR(MAX)
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg VARCHAR(200) = '' OUTPUT
AS
/*
Created: 20160523@Tim.Trinh
Task: Update final score.
DB: SMSEPA
Original: 

Revisions:  
Param's Explanation:
	@EPAScores = '<ROOT>
					<EPA EPAId='' FinalScore='' DisciplineScore='' Grade='' ToeicPolicyScore=''></EPA>
				</ROOT>'
*/
BEGIN
	SET NOCOUNT ON;

	CREATE TABLE #tblEPAScore (
		EPAId INT
		,FinalScore NVARCHAR(MAX)
		,DisciplineScore FLOAT
		,Grade NVARCHAR(MAX)
		,ToeicPolicyScore SMALLINT
		)

	BEGIN TRY
		DECLARE @DocHandler INT

		EXEC sp_xml_preparedocument @DocHandler OUTPUT
			,@EPAScores

		INSERT INTO #tblEPAScore (
			EPAId
			,FinalScore
			,DisciplineScore
			,Grade
			,ToeicPolicyScore
			)
		SELECT EPAId
			,FinalScore
			,DisciplineScore
			,Grade
			,ToeicPolicyScore
		FROM OPENXML(@DocHandler, '/ROOT/EPA', 1) WITH (EPAId INT, FinalScore NVARCHAR(MAX), DisciplineScore FLOAT, Grade NVARCHAR(MAX), ToeicPolicyScore SMALLINT) AS Data

		EXEC sp_xml_removedocument @DocHandler
	END TRY

	BEGIN CATCH
		SELECT @ErrCode = 1
			,@ErrMsg = N'Parameter invalid.'

		RETURN
	END CATCH

	UPDATE epa
	SET epa.FinalScore = epaS.FinalScore
		,epa.DisciplineScore = epaS.DisciplineScore
		,epa.Grade = epaS.Grade
		,epa.ToeicPolicyScore = epaS.ToeicPolicyScore
		,epa.[Status] = 1 -- closed
		,epa.ModifiedBy = @ModifiedBy
		,epa.ModifiedDate = GETDATE()
	FROM dbo.EPA AS epa WITH (ROWLOCK, UPDLOCK)
		INNER JOIN #tblEPAScore AS epaS WITH (NOLOCK) ON epa.EPAId = epaS.EPAId

	DROP TABLE #tblEPAScore
END
