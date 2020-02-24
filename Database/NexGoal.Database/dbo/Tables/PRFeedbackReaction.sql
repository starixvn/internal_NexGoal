CREATE TABLE [dbo].[PRFeedbackReaction](
	[PRFeedbackReactionId] [int] IDENTITY(1,1) NOT NULL,
	[PRFeedbackId] [int] NOT NULL,
	[CreatorId] [int] NOT NULL,
	[RoleId] [tinyint] NOT NULL,
	[CreatedDate] [datetime] NOT NULL,
 CONSTRAINT [PK_PRFeedbackReaction] PRIMARY KEY CLUSTERED 
(
	[PRFeedbackReactionId] ASC
)
)