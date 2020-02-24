CREATE PROCEDURE [dbo].[PRFeedbackReaction_GetByFeedbackId] @PRFeedbackId INT
	,@ActorId INT
	,@ActorCode VARCHAR(20)
AS

/*
Created: 20200213@Drake.Doi
Task: Get feedback reactions by feedback's ID
DB: bodb_NexGoal
Original: 
*/

BEGIN
	SET NOCOUNT ON;

	SELECT reaction.PRFeedbackReactionId,
		reaction.PRFeedbackId,
		reaction.RoleId,
		reaction.CreatedDate,
		creator.EmployeeId,
		creator.Code,
		creator.FullName,
		creator.NickName
	FROM PRFeedbackReaction AS reaction WITH (NOLOCK)
		INNER JOIN PRFeedback AS fb WITH (NOLOCK) ON reaction.PRFeedbackId = fb.PRFeedbackId
		INNER JOIN dbo.Employee AS creator WITH (NOLOCK) ON creator.EmployeeId = reaction.CreatorId
	WHERE reaction.PRFeedbackId = @PRFeedbackId
		AND creator.Code = @ActorCode
		AND creator.EmployeeId = @ActorId
END