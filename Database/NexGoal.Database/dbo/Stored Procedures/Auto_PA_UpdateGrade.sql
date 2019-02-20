
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
    Value : '<ROOT><Row PARecordId="4" OriginalGradeId="cipher text" GradeId="cipher text" ToeicPolicy="1" StatusId="1"></Row></ROOT>'
*/
BEGIN
	SET NOCOUNT ON;
	SET XACT_ABORT ON;

	DECLARE @PAInProgress TINYINT = 3
		,@PAClosed TINYINT = 4
		,@PARecordId INT
		,@PerformanceRecordId INT
		,@idoc INT

	CREATE TABLE #tblPARecord (
		PARecordId INT
		,OriginalGradeId VARCHAR(MAX)
		,GradeId VARCHAR(MAX)
		,ToeicPolicy SMALLINT
		,StatusId TINYINT
		)

	EXEC sp_xml_preparedocument @idoc OUTPUT
		,@Value

	INSERT INTO #tblPARecord (
		PARecordId
		,OriginalGradeId
		,GradeId
		,ToeicPolicy
		,StatusId
		)
	SELECT PARecordId
		,OriginalGradeId
		,GradeId
		,ToeicPolicy
		,StatusId
	FROM OPENXML(@idoc, '/Root/Row', 1) WITH (PARecordId INT, OriginalGradeId VARCHAR(MAX), GradeId VARCHAR(MAX), ToeicPolicy SMALLINT, StatusId TINYINT) AS data

	UPDATE pa
	SET pa.OriginalGradeId = tbl.OriginalGradeId
		,pa.GradeId = tbl.GradeId
		,pa.Toeic = toeic.Listening + toeic.Reading
		,pa.ToeicPolicy = tbl.ToeicPolicy
	FROM dbo.PARecord AS pa WITH (ROWLOCK, UPDLOCK)
		INNER JOIN #tblPARecord AS tbl WITH (NOLOCK) ON pa.PARecordId = tbl.PARecordId
		LEFT JOIN dbo.TOEIC AS toeic WITH (NOLOCK) ON pa.EmployeeId = toeic.EmpId

	UPDATE record
	SET record.GradeId = pa.GradeId
	FROM dbo.PerformanceRecord AS record WITH (ROWLOCK, UPDLOCK)
		INNER JOIN dbo.PARecord AS pa WITH (NOLOCK) ON record.PerformanceRecordId = pa.PerformanceRecordId
		AND pa.StatusId = @PAClosed
		INNER JOIN #tblPARecord AS tbl WITH (NOLOCK) ON pa.PARecordId = tbl.PARecordId

	DELETE tbl
	FROM #tblPARecord AS tbl
	WHERE tbl.StatusId = @PAInProgress
		OR (
			tbl.StatusId = @PAClosed
			AND EXISTS (
				SELECT 1
				FROM dbo.PARecord AS pa WITH (NOLOCK)
					INNER JOIN dbo.PerformanceRecord AS record WITH (NOLOCK) ON pa.PerformanceRecordId = record.PerformanceRecordId
				WHERE pa.PARecordId = tbl.PARecordId
				)
			)

	WHILE EXISTS (
			SELECT 1
			FROM #tblPARecord WITH (NOLOCK)
			)
	BEGIN
		SELECT TOP 1 @PARecordId = PARecordId
		FROM #tblPARecord WITH (NOLOCK)

		INSERT INTO dbo.PerformanceRecord (
			EmployeeId
			,CreatorId
			,GradeId
			,CreatedDate
			,TypeId
			,RoleId
			)
		SELECT EmployeeId
			,- 1
			,GradeId
			,GETDATE()
			,4 -- system PA
			,255
		FROM dbo.PARecord WITH (NOLOCK)
		WHERE PARecordId = @PARecordId

		SET @PerformanceRecordId = SCOPE_IDENTITY()

		UPDATE pa
		SET pa.PerformanceRecordId = @PerformanceRecordId
		FROM dbo.PARecord AS pa WITH (ROWLOCK, UPDLOCK)
		WHERE pa.PARecordId = @PARecordId

		DELETE
		FROM #tblPARecord
		WHERE PARecordId = @PARecordId
	END

	DROP TABLE #tblPARecord
END