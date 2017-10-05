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

	BEGIN TRY
		BEGIN TRANSACTION

		UPDATE dbo.EPA
		SET FinalScore = epaS.FinalScore
			,DisciplineScore = epaS.DisciplineScore
			,Grade = epaS.Grade
			,ToeicPolicyScore = epaS.ToeicPolicyScore
			,[Status] = 1 -- closed
			,ModifiedBy = @ModifiedBy
			,ModifiedDate = GETDATE()
		FROM dbo.EPA AS epa WITH (NOLOCK)
			INNER JOIN #tblEPAScore AS epaS WITH (NOLOCK) ON epa.EPAId = epaS.EPAId

		COMMIT TRANSACTION

		SELECT @ErrCode = 0
			,@ErrMsg = ''
	END TRY

	BEGIN CATCH
		ROLLBACK TRANSACTION

		SELECT @ErrCode = 1
			,@ErrMsg = N'Update final score unsuccessfully.'
	END CATCH

	DROP TABLE #tblEPAScore
END
