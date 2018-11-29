CREATE TABLE [dbo].[MemberRoles] (
    [Id]       INT NOT NULL,
    [MemberId] INT NOT NULL,
    [RoleId]   INT NOT NULL,
    CONSTRAINT [PK_MemberRoles] PRIMARY KEY CLUSTERED ([Id] ASC)
);

