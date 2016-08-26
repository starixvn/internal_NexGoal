/****** Object:  User Defined Function dbo.fncGetDataForWriteLog_Discipline_IgnoredViolations    Script Date: 7/21/2016 2:45:40 PM ******/

CREATE FUNCTION [dbo].[fncGetDataForWriteLog_Discipline_IgnoredViolations] (@IgnoredViolations NVARCHAR(MAX))
RETURNS NVARCHAR(MAX)
AS
BEGIN
	DECLARE @Data NVARCHAR(MAX)		

	SELECT @Data = CONCAT('<r><vis>', @IgnoredViolations, '</vis></r>')
	RETURN @Data
END
