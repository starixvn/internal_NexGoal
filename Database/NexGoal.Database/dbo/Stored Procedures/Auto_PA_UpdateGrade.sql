
CREATE PROCEDURE [dbo].[Auto_PA_UpdateGrade] @Value VARCHAR(MAX)
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg NVARCHAR(200) = '' OUTPUT
AS
/*
Created: 20181116@Tim.Trinh
Task: update grade.
DB: bodb_NexGoal
Original:

Revisions:
Param's Explanation:
    Value : '<ROOT><Row PARecordId="4" GradeId="cipher text" ToeicPolicy="1"></Row></ROOT>'
*/
BEGIN
	SET NOCOUNT ON;
	SET XACT_ABORT ON;

	DECLARE @idoc INT

	CREATE TABLE #tblPARecord (
		PARecordId INT
		,GradeId VARCHAR(MAX)
		,ToeicPolicy SMALLINT
		)

	EXEC sp_xml_preparedocument @idoc OUTPUT
		,@Value

	INSERT INTO #tblPARecord (
		PARecordId
		,GradeId
		,ToeicPolicy
		)
	SELECT PARecordId
		,GradeId
		,ToeicPolicy
	FROM OPENXML(@idoc, '/Root/Row', 1) WITH (PARecordId INT, GradeId VARCHAR(MAX), ToeicPolicy SMALLINT) AS data

	UPDATE pa
	SET pa.OriginalGradeId = tbl.GradeId
		,pa.GradeId = tbl.GradeId
		,pa.Toeic = toeic.Listening + toeic.Reading
		,pa.ToeicPolicy = tbl.ToeicPolicy
	FROM dbo.PARecord AS pa WITH (ROWLOCK, UPDLOCK)
		INNER JOIN #tblPARecord AS tbl WITH (NOLOCK) ON pa.PARecordId = tbl.PARecordId
		LEFT JOIN dbo.TOEIC AS toeic WITH (NOLOCK) ON pa.EmployeeId = toeic.EmpId

	DROP TABLE #tblPARecord
END