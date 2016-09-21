(function (reportModule) {
    "use strict";

    reportModule.directive("gridMasterReport", masterReportDirective);

    masterReportDirective.$inject = ["ReportModel"];

    function masterReportDirective(ReportModel) {
        function renderEvaluatorCommentCell(cell) {
            return '<a class="evaluator-comment" data-employee-id=' + cell.Value + '><i class="fa fa-search" aria-hidden="true"></i></a>';
        }

        function renderNextObjectives(cell) {
            return '<a class="next-objectives" data-employee-id=' + cell.Value + '><i class="fa fa-search" aria-hidden="true"></i></a>';
        }

        function renderNominations(cell) {
            var htmlString = '<table class="table employee-award-table">';

            angular.forEach(cell.Value, function (nomination) {
                htmlString += "<tr><td width=100>" + nomination.Title + "</td><td width=80 class='text-center'>" + nomination.StatusToString + "</tr>";
            });

            htmlString += '</table>';

            return htmlString;
        }

        function groupData(data) {
            var groups = [];

            angular.forEach(data, function (record) {
                var groupIndex = groups.map(function (item) { return item.GroupId }).indexOf(record.PAFocus.Employee.Group.GroupId);
                var group = {};

                if (groupIndex === -1) {
                    group.GroupName = record.PAFocus.Employee.Group.GroupName;
                    group.GroupId = record.PAFocus.Employee.Group.GroupId;
                    group.Members = [];

                    groups.push(group);
                } else {
                    group = groups[groupIndex];
                }

                group.Members.push(record);
            });

            return groups;
        };

        function generateGroupHeaders() {
            return [{
                Columns: [
                    { HeaderText: "Team", Name: "TeamName", MergeRowNumber: 2 },
                    { HeaderText: "#", Name: "SymbolCount", MergeRowNumber: 2 },
                    { HeaderText: "Nick Name", Name: "NickName", MergeRowNumber: 2 },
                    { HeaderText: "Full Name", Name: "FullName", MergeRowNumber: 2 },
                    { HeaderText: "Job Title", Name: "JobTitle", MergeRowNumber: 2 },
                    { HeaderText: "Reviewer's Adjustment", Name: "ReviewerAdjustment", MergeRowNumber: 2 },
                    { HeaderText: "PA Final Score", Name: "PAFinalScore", MergeRowNumber: 2 },
                    { HeaderText: "Evaluator's Comment", Name: "EvaluatorComment", MergeRowNumber: 2 },
                    { HeaderText: "Contribution", Name: "Contribution", MergeRowNumber: 2 },
                    { HeaderText: "Applicable to promote", Name: "ApplicableToPromote", MergeRowNumber: 2 },
                    { HeaderText: "Bonus Given", Name: "BonusGiven", MergeRowNumber: 2 },
                    { HeaderText: "Bonus Ranking", Name: "BonusRanking", MergeRowNumber: 2 },
                    { HeaderText: "Accessing of Objective achievement", Name: "ObjectiveAchievement", MergeRowNumber: 2 },
                    { HeaderText: "Next Objectives", Name: "NextObjectives", MergeRowNumber: 2 },
                    { HeaderText: "Award", Name: "AwardName", MergeColNumber: 2 },
                    { HeaderText: "Evaluation by PM", Name: "ProjectName", MergeColNumber: 3 }
                ]
            }];
        }

        function generateColumnHeaders() {
            return [
               { HeaderText: "Team", Width: 80, Name: "TeamName", CellAlign: "center" },
               { HeaderText: "#", Width: 50, Name: "SymbolCount", CellAlign: "center" },
               { HeaderText: "Nick Name", Width: 80, Name: "NickName", CellAlign: "center" },
               { HeaderText: "Full Name", Width: 150, Name: "FullName" },
               { HeaderText: "Job Title", Width: 150, Name: "JobTitle" },
               { HeaderText: "Reviewer's Adjustment", Width: 80, Name: "ReviewerAdjustment", CellAlign: "center" },
               { HeaderText: "PA Final Score", Width: 60, Name: "PAFinalScore", CellAlign: "center" },
               { HeaderText: "Evaluator's Comment", Width: 80, Name: "EvaluatorComment", CellAlign: "center", CustomCellRender: renderEvaluatorCommentCell },
               { HeaderText: "Contribution", Width: 80, Name: "Contribution", CellAlign: "center" },
               { HeaderText: "Applicable to promote", Width: 50, Name: "ApplicableToPromote", CellAlign: "center" },
               { HeaderText: "Bonus Given", Width: 50, Name: "BonusGiven", CellAlign: "center" },
               { HeaderText: "Bonus Ranking", Width: 50, Name: "BonusRanking", CellAlign: "center" },
               { HeaderText: "Accessing of Objective achievement", Width: 80, Name: "ObjectiveAchievement", CellAlign: "center" },
               { HeaderText: "Next Objectives", Width: 80, Name: "NextObjectives", CellAlign: "center", CustomCellRender: renderNextObjectives },
               { HeaderText: "Title", Width: 100, Name: "AwardName", CustomCellRender: renderNominations },
               { HeaderText: "Status", Width: 80, Name: "AwardStatus" },
               { HeaderText: "Project Name ", Width: 80, Name: "ProjectName" },
               { HeaderText: "Score", Width: 80, Name: "ProjectScore" },
               { HeaderText: "Comment", Width: 80, Name: "ProjectComment", CellAlign: "center" }
            ];
        }

        function generateBodyRows(groupReports) {
            var rows = [];
            angular.forEach(groupReports, function (group) {
                var index = 1;
                var firstRow = true;

                angular.forEach(group.Members, function (member) {
                    var row = {
                        Columns: []
                    };

                    if (firstRow) {
                        row.Columns.push({ Name: "TeamName", Value: group.GroupName, MergeRowNumber: group.Members.length });
                        firstRow = false;
                    }

                    row.Columns.push({ Name: "SymbolCount", Value: index });
                    row.Columns.push({ Name: "NickName", Value: member.PAFocus.Employee.NickName });
                    row.Columns.push({ Name: "FullName", Value: member.PAFocus.Employee.FullName });
                    row.Columns.push({ Name: "JobTitle", Value: member.PAFocus.Employee.JobTitle.JobTitleName });
                    row.Columns.push({ Name: "ReviewerAdjustment", Value: member.ReviewerAdjustment });
                    row.Columns.push({ Name: "PAFinalScore", Value: member.PAFocus.FinalScore });
                    row.Columns.push({ Name: "EvaluatorComment", Value: member.PAFocus.Employee.EmployeeId });
                    row.Columns.push({ Name: "Contribution", Value: member.PAFocus.Contribution });
                    row.Columns.push({ Name: "ApplicableToPromote", Value: member.PAFocus.IsReadyPromotion ? "Yes" : "No" });
                    row.Columns.push({ Name: "BonusGiven", Value: member.PAFocus.BonusGiven });
                    row.Columns.push({ Name: "BonusRanking", Value: member.PAFocus.BonusRanking });
                    row.Columns.push({ Name: "ObjectiveAchievement", Value: member.PAFocus.LastCompletedPercentObjective + "%" });
                    row.Columns.push({ Name: "NextObjectives", Value: member.PAFocus.Employee.EmployeeId });
                    row.Columns.push({ Name: "AwardName", Value: member.Nominations, MergeColNumber: 2 });
                    row.Columns.push({ Name: "ProjectName", Value: 1 });
                    row.Columns.push({ Name: "ProjectScore", Value: 1 });
                    row.Columns.push({ Name: "ProjectComment", Value: 1 });

                    rows.push(row);
                    index++;
                });
            });

            return rows;
        }

        return {
            restrict: "A",
            scope: {
                gridData: "="
            },
            link: function (scope, iElement) {
                scope.modalInstance = null;
                var gridOptions = {
                    customizeHeader: true,
                    gridExpandHeight: 500
                };

                scope.$watch("gridData", function (gridData) {
                    var groupReports = groupData(gridData);
                    var groupHeaders = generateGroupHeaders();
                    var columnHeaders = generateColumnHeaders();
                    var bodyRows = generateBodyRows(groupReports);
                    var element = $(iElement);

                    gridOptions.groupHeaderRows = groupHeaders;
                    gridOptions.columns = columnHeaders;
                    gridOptions.bodyRows = bodyRows;

                    element.fanexGrid(gridOptions);

                    element.find(".evaluator-comment").bind("click", function () {
                        var $self = $(this);
                        var employeeId = $self.data("employee-id");

                        if (employeeId) {
                            ReportModel.openPopupEvaluatorComment(employeeId);
                        }
                    });

                    element.find(".next-objectives").bind("click", function () {
                        var $self = $(this);
                        var employeeId = $self.data("employee-id");

                        if (employeeId) {
                            ReportModel.openPopupNextObjectives(employeeId);
                        }
                    });
                });
            }
        };
    }
})(angular.module("reportModule"));