CREATE PROCEDURE [dbo].[EpaAuto_UpdateFinalScoreEndEvaluation] @EPAScores VARCHAR(MAX)
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg VARCHAR(200) = '' OUTPUT
AS
/*
Created: 2015-08-26@Josh.Phan
Task: Check end process.
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

	UPDATE epa
	SET epa.FinalScore = epaS.FinalScore
		,epa.DisciplineScore = epaS.DisciplineScore
		,epa.Grade = epaS.Grade
		,epa.ToeicPolicyScore = epaS.ToeicPolicyScore
		,epa.[Status] = 1 -- closed
	FROM dbo.EPA AS epa WITH (ROWLOCK, UPDLOCK)
		INNER JOIN #tblEPAScore AS epaS WITH (NOLOCK) ON epa.EPAId = epaS.EPAId

	DROP TABLE #tblEPAScore
END