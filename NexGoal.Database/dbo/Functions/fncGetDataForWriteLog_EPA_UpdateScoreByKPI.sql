/****** Object:  User Defined Function dbo.fncGetDataForWriteLog_EPA_UpdateScoreByKPI    Script Date: 7/21/2016 2:45:41 PM ******/

CREATE FUNCTION [dbo].[fncGetDataForWriteLog_EPA_UpdateScoreByKPI] (@EvaluatorId INT
				,@KPIDetails NVARCHAR(MAX))
RETURNS NVARCHAR(MAX)
AS
BEGIN
	DECLARE @Data NVARCHAR(MAX)	

	SELECT @Data = CONCAT('<r><eva>', @EvaluatorId,'</eva><kds>', @KPIDetails,'</kds></r>')

	RETURN @Data
END
