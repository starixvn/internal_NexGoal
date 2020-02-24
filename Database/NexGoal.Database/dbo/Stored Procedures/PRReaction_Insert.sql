CREATE PROCEDURE [dbo].[PRReaction_GetByPRId] @PerformanceRecordId INT
	,@ActorId INT
	,@ActorCode VARCHAR(20)
AS

/*
Created: 20200213@Drake.Doi
Task: Get reactions by PR's ID
DB: bodb_NexGoal
Original: 
*/

BEGIN
	SET NOCOUNT ON;

	SELECT reaction.PRReactionId,
		reaction.PerformanceRecordId,
		reaction.RoleId,
		reaction.CreatedDate,
		creator.EmployeeId,
		creator.Code,
		creator.FullName,
		creator.NickName
	FROM PRReaction AS reaction WITH (NOLOCK)
		INNER JOIN dbo.Employee AS creator WITH (NOLOCK) ON creator.EmployeeId = reaction.CreatorId
	WHERE reaction.PerformanceRecordId = @PerformanceRecordId
		AND reaction.CreatorId = @ActorId
		AND creator.Code = @ActorCode
END