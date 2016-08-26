/****** Object:  Stored Procedure dbo.EPA_GetStandardGrade    Script Date: 7/21/2016 2:45:45 PM ******/

CREATE PROCEDURE [dbo].[EPA_GetStandardGrade]
AS
/*
Created: 20151002@Tim.Trinh
Task: Get list standard grade.
DB: SPM
Original: 
 
Param's Explanation:

*/
BEGIN
	SET NOCOUNT ON;

	SELECT GradeId
		,[Name]
		,RTRIM(Code) AS Code
		,LeftScore
		,RightScore
	FROM dbo.StandardGrade
END
