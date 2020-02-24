CREATE PROCEDURE [dbo].[PRFeedback_GetByPRId] @PerformanceRecordId INT
	,@ActorId INT
	,@ActorCode VARCHAR(20)
AS

/*
Created: 20200220@Drake.Doi
Task: Get feedbacks by PR's ID and EmployeeCode
DB: bodb_NexGoal
Original: 
*/

BEGIN
	SET NOCOUNT ON;

	SELECT feedback.PRFeedbackId,
		feedback.PerformanceRecordId,
		feedback.RoleId,
		feedback.CreatedDate,
		feedback.Content,
		creator.EmployeeId,
		creator.Code,
		creator.FullName,
		creator.NickName
	FROM PRFeedback AS feedback WITH (NOLOCK)
		INNER JOIN dbo.Employee AS creator WITH (NOLOCK) ON creator.EmployeeId = feedback.CreatorId
	WHERE feedback.PerformanceRecordId = @PerformanceRecordId 
		AND creator.EmployeeId = @ActorId
		AND creator.Code = @ActorCode

	SELECT reaction.PRFeedbackId,
		reaction.PRFeedbackReactionId,
		reaction.CreatorId,
		reaction.RoleId,
		reaction.CreatedDate,
		creator.EmployeeId,
		creator.Code,
		creator.FullName,
		creator.NickName
	FROM PRFeedbackReaction AS reaction WITH (NOLOCK)
		INNER JOIN PRFeedback AS feedback WITH (NOLOCK) ON reaction.PRFeedbackId = feedback.PRFeedbackId
		INNER JOIN dbo.Employee AS creator WITH (NOLOCK) ON creator.EmployeeId = feedback.CreatorId
	WHERE feedback.PerformanceRecordId = @PerformanceRecordId 
		AND creator.EmployeeId = @ActorId
		AND creator.Code = @ActorCode
END