/****** Object:  Stored Procedure dbo.EpaScoreRatingType_Get    Script Date: 7/21/2016 2:45:54 PM ******/

CREATE PROCEDURE [dbo].[EpaScoreRatingType_Get] @ScoreRatingTypeId INT = NULL
	,@IsActive BIT = 1
AS
/*
Created: 2015-06-12@Josh.Phan
Task: Get all score rating type.
DB: Redmine
Original: 

Revisions:  
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	SELECT [ScoreRatingTypeId]
		,[Name]
		,[IsActive]
		,[Description]
		,[SelfEvaluated]
		,[Evaluator]
		,[Reviewer]
		,[Management]
		,[Attendance]
	FROM [dbo].[ScoreRatingType] WITH (NOLOCK)
	WHERE [IsActive] = @IsActive
		AND (
			ISNULL(@ScoreRatingTypeId, '') = ''
			OR [ScoreRatingTypeId] = @ScoreRatingTypeId
			)
END
