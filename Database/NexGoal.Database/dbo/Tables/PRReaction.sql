CREATE TABLE [dbo].[PRReaction](
	[PRReactionId] [int] IDENTITY(1,1) NOT NULL,
	[PerformanceRecordId] [int] NOT NULL,
	[CreatorId] [int] NOT NULL,
	[RoleId] [tinyint] NOT NULL,
	[CreatedDate] [datetime] NOT NULL,
 CONSTRAINT [PK_PRAgree] PRIMARY KEY CLUSTERED 
(
	[PRReactionId] ASC
)
)