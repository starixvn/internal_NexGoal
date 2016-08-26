/****** Object:  User Defined Function dbo.fncGetDataForWriteLog_EPA_AdjustScore    Script Date: 7/21/2016 2:45:41 PM ******/

CREATE FUNCTION [dbo].[fncGetDataForWriteLog_EPA_AdjustScore] (@EPADetails NVARCHAR(MAX))
RETURNS NVARCHAR(MAX)
AS
BEGIN
	DECLARE @Data NVARCHAR(MAX)	

	SELECT @Data = CONCAT('<r><eds>', @EPADetails,'</eds></r>')

	RETURN @Data
END
