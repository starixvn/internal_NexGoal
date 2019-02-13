TRUNCATE TABLE dbo.[Function]
GO
SET IDENTITY_INSERT dbo.[Function] ON
INSERT dbo.[Function] (FunctionId, FunctionKey, [Name], ParentId, OnlyPolicy, DisplayOrder, IsActive)
VALUES (1, N'MemberPage', N'Member Page', NULL, 2, 10, 1)
	,(2, N'PerformanceAppraisal', N'Performance Appraisal', NULL, NULL, 20, 1)
	,(3, N'EvaluationPeriod', N'PA Period', 2, NULL, 21, 1)
	,(4, N'EvaluationFlow', N'PA Flow', 2, NULL, 22, 1)
	,(5, N'PAFocus', N'PA Focus', 2, NULL, 23, 1)
	,(6, N'Nomination', N'Nomination', NULL, NULL, 30, 1)
	,(7, N'NominationPeriod', N'Nomination Period', 6, NULL, 31, 1)
	,(8, N'NominationSelection', N'Nomination Selection', 6, 2, 32, 1)
	,(9, N'Report', N'Report', NULL, 1, 40, 1)
	,(10, N'PAReport', N'PA Report', 9, 1, 41, 1)
	,(11, N'NominationReport', N'Nomination Report', 9, 1, 42, 1)
	,(12, N'MasterReport', N'Master Report', 9, 1, 43, 1)
	,(13, N'AdminCenter', N'Admin Center', NULL, NULL, 50, 1)
	,(14, N'RoleAssignment', N'Role Assignment', 13, NULL, 51, 1)
	,(15, N'RolePermission', N'Role Permission', 13, NULL, 52, 1)
	,(16, N'PMAssignment', N'PM Assignment', 13, 2, 53, 1)
	,(17, N'Discipline', N'Discipline', 13, NULL, 54, 1)
SET IDENTITY_INSERT [dbo].[Function] OFF
GO

TRUNCATE TABLE dbo.Menu
GO
SET IDENTITY_INSERT dbo.Menu ON
INSERT dbo.Menu (MenuId, [Name], [Description], Link, ParentId, FunctionId, DisplayOrder, IsActive)
VALUES (1, N'My Page', NULL, N'/home/index', NULL, NULL, 10, 1)
	,(2, N'Member Page', NULL, N'/member/index', NULL, 1, 20, 1)
	,(3, N'Performance Appraisal', NULL, NULL, NULL, 2, 30, 1)
	,(4, N'PA Period', NULL, N'/period/index', 3, 3, 31, 1)
	,(5, N'PA Flow', NULL, N'/flow/index', 3, 4, 32, 1)
	,(6, N'PA Focus', NULL, N'/pafocus/index', 3, 5, 33, 1)
	,(7, N'Nomination', NULL, NULL, NULL, 6, 40, 1)
	,(8, N'Nomination Period', NULL, N'/NominationPeriod/index', 7, 7, 41, 1)
	,(9, N'Nomination Selection', NULL, N'/NominationAssessment/index', 7, 8, 42, 1)
	,(10, N'Report', NULL, NULL, NULL, 9, 50, 1)
	,(11, N'PA Report', NULL, N'/report/PAReport', 10, 10, 51, 1)
	,(12, N'Nomination Report', NULL, N'/NominationReport/index', 10, 11, 52, 1)
	,(13, N'Master Report', NULL, N'/MasterReport/Index', 10, 12, 53, 1)
	,(14, N'Admin Center', NULL, NULL, NULL, 13, 60, 1)
	,(15, N'Role Assignment', NULL, N'/employee/index', 14, 14, 61, 1)
	,(16, N'Role Permission', NULL, N'/role/index', 14, 15, 62, 1)
	,(17, N'PM Assignment', NULL, N'/PMSetting/index', 14, 16, 63, 1)
	,(18, N'Discipline', NULL, N'/discipline/index', 14, 17, 64, 1)
SET IDENTITY_INSERT dbo.Menu OFF
GO
