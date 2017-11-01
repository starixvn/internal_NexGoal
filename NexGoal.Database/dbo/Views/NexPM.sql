
CREATE VIEW [dbo].[NexPM]
AS
SELECT tg.StartDate
	,tg.EndDate
	,STUFF((
			SELECT CAST(', ' AS VARCHAR(MAX)) + rl.NAME
			FROM NexEPA.dbo.ProjectMemberRole AS pmr WITH (NOLOCK)
				INNER JOIN NexEPA.dbo.ProjectRole AS rl WITH (NOLOCK) ON pmr.ProjectRoleId = rl.ProjectRoleId
			WHERE pmr.ProjectId = mb.ProjectId
				AND pmr.EmployeeId = mb.EmployeeId
			FOR XML PATH('')
			), 1, 1, '') AS Roles
	,epa.FinalScore AS Score
	,epa.FinalGrade AS Grade
	,pc.Comment
	,pr.RedmineProjectId AS ProjectId
	,pr.[Name] AS ProjectName
	,ee.EmployeeId
	,tp.ProjectManager
FROM NexEPA.dbo.[Target] AS tg WITH (NOLOCK)
	INNER JOIN NexEPA.dbo.TargetProject AS tp WITH (NOLOCK) ON tg.TargetId = tp.TargetId
	INNER JOIN NexEPA.dbo.EPA AS epa WITH (NOLOCK) ON tg.TargetId = epa.TargetId
	INNER JOIN NexEPA.dbo.EPAProcess AS pc WITH (NOLOCK) ON epa.EPAId = pc.EPAId
	INNER JOIN NexEPA.dbo.Project AS pr WITH (NOLOCK) ON tp.ProjectId = pr.ProjectId
	INNER JOIN NexEPA.dbo.Employee AS ee WITH (NOLOCK) ON epa.EmployeeId = ee.EmployeeId
	LEFT JOIN NexEPA.dbo.ProjectMember AS mb WITH (NOLOCK) ON mb.EmployeeId = ee.EmployeeId
	AND pr.ProjectId = mb.ProjectId
WHERE ee.IsDeleted = 0
	AND ee.[Enabled] = 1
GO
EXECUTE sp_addextendedproperty @name = N'MS_DiagramPaneCount', @value = 1, @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'VIEW', @level1name = N'NexPM';


GO
EXECUTE sp_addextendedproperty @name = N'MS_DiagramPane1', @value = N'[0E232FF0-B466-11cf-A24F-00AA00A3EFFF, 1.00]
Begin DesignProperties = 
   Begin PaneConfigurations = 
      Begin PaneConfiguration = 0
         NumPanes = 4
         Configuration = "(H (1[40] 4[20] 2[20] 3) )"
      End
      Begin PaneConfiguration = 1
         NumPanes = 3
         Configuration = "(H (1 [50] 4 [25] 3))"
      End
      Begin PaneConfiguration = 2
         NumPanes = 3
         Configuration = "(H (1 [50] 2 [25] 3))"
      End
      Begin PaneConfiguration = 3
         NumPanes = 3
         Configuration = "(H (4 [30] 2 [40] 3))"
      End
      Begin PaneConfiguration = 4
         NumPanes = 2
         Configuration = "(H (1 [56] 3))"
      End
      Begin PaneConfiguration = 5
         NumPanes = 2
         Configuration = "(H (2 [66] 3))"
      End
      Begin PaneConfiguration = 6
         NumPanes = 2
         Configuration = "(H (4 [50] 3))"
      End
      Begin PaneConfiguration = 7
         NumPanes = 1
         Configuration = "(V (3))"
      End
      Begin PaneConfiguration = 8
         NumPanes = 3
         Configuration = "(H (1[56] 4[18] 2) )"
      End
      Begin PaneConfiguration = 9
         NumPanes = 2
         Configuration = "(H (1 [75] 4))"
      End
      Begin PaneConfiguration = 10
         NumPanes = 2
         Configuration = "(H (1[66] 2) )"
      End
      Begin PaneConfiguration = 11
         NumPanes = 2
         Configuration = "(H (4 [60] 2))"
      End
      Begin PaneConfiguration = 12
         NumPanes = 1
         Configuration = "(H (1) )"
      End
      Begin PaneConfiguration = 13
         NumPanes = 1
         Configuration = "(V (4))"
      End
      Begin PaneConfiguration = 14
         NumPanes = 1
         Configuration = "(V (2))"
      End
      ActivePaneConfig = 0
   End
   Begin DiagramPane = 
      Begin Origin = 
         Top = 0
         Left = 0
      End
      Begin Tables = 
      End
   End
   Begin SQLPane = 
   End
   Begin DataPane = 
      Begin ParameterDefaults = ""
      End
      Begin ColumnWidths = 9
         Width = 284
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
      End
   End
   Begin CriteriaPane = 
      Begin ColumnWidths = 11
         Column = 1440
         Alias = 900
         Table = 1170
         Output = 720
         Append = 1400
         NewValue = 1170
         SortType = 1350
         SortOrder = 1410
         GroupBy = 1350
         Filter = 1350
         Or = 1350
         Or = 1350
         Or = 1350
      End
   End
End
', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'VIEW', @level1name = N'NexPM';

