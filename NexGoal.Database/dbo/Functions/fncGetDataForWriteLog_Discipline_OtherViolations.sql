/****** Object:  User Defined Function dbo.fncGetDataForWriteLog_Discipline_OtherViolations    Script Date: 7/21/2016 2:45:41 PM ******/

CREATE FUNCTION [dbo].[fncGetDataForWriteLog_Discipline_OtherViolations] (@EmployeeId INT)
RETURNS NVARCHAR(MAX)
AS
BEGIN
	DECLARE @Data NVARCHAR(MAX)		
	DECLARE @Violations NVARCHAR(MAX)

	SET @Violations = ''

	SELECT @Violations = @Violations
			+ '<vi><wd>' + CAST([WorkDate] AS NVARCHAR(20))
			+ '</wd><n>' + Name
			+ '</n><sc>' + CAST([Score] AS NVARCHAR(10))
			+ '</sc><c>' + Comment
			+ '</c></vi>'
	FROM [dbo].[EmployeeOtherViolation] WITH (NOLOCK)
	WHERE [EmpId] = @EmployeeId

	SELECT @Data = CONCAT('<r><eid>', @EmployeeId, '</eid><vis>', @Violations, '</vis></r>')

	RETURN @Data
END
