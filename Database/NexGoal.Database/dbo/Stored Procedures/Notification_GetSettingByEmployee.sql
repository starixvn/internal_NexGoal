
CREATE PROCEDURE [dbo].[Notification_GetSettingByEmployee] @EmployeeId INT
AS
/*
Created: 20181227@Oliver.Dang
Task: Get Notification settings
DB: bodb_NexGoal
*/
BEGIN
	SET NOCOUNT ON;

	DECLARE @toDay DATETIME = GETDATE()
		,@PRCheckIn SMALLINT = 1
		,@PA SMALLINT = 2
		,@Nomination SMALLINT = 3
		,@PAFocus SMALLINT = 4
		,@EvaluatorProcess SMALLINT = 4
		,@IsNomitor BIT

	-- Check if Nominator or Commitee
	SET @IsNomitor = (
			CASE 
				WHEN EXISTS (
						SELECT 1
						FROM dbo.EOQY AS eoqy WITH (NOLOCK)
						INNER JOIN dbo.EOQYNominatorSetting AS eoqySetting WITH (NOLOCK) ON eoqy.EOQYId = eoqySetting.EOQYId
						WHERE @toDay BETWEEN eoqy.StartDateForNominator
								AND eoqy.EndDateForCommittee
							AND eoqySetting.NominatorId = @EmployeeId
						)
					THEN 1
				ELSE 0
				END
			)

	SELECT r.ResourceId AS [Type]
		,r.[Description]
		,ISNULL(n.ReceiveNotification, 1) AS ReceiveNotification
		,ISNULL(n.ReceiveEmail, 1) AS ReceiveEmail
		,ISNULL(n.ReceiveSkypeMessage,1) AS ReceiveSkypeMessage
	FROM dbo.RefResource AS r WITH (NOLOCK)
	LEFT JOIN dbo.NotificationSetting AS n WITH (NOLOCK) ON r.ResourceId = n.TypeId
		AND n.EmployeeId = @EmployeeId
	WHERE r.RefName = 'NotificationType'
		AND (
			r.ResourceId IN (
				@PRCheckIn
				,@PA
				)
			OR (
				@IsNomitor = 1
				AND r.ResourceId = @Nomination
				)
			OR (
				EXISTS (
					SELECT 1
					FROM func_GetMember(@EmployeeId)
					WHERE ProcessId = @EvaluatorProcess
					)
				AND r.ResourceId = @PAFocus
				)
			)
END
