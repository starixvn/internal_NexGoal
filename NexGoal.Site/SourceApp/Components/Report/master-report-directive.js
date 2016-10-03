(function (reportModule) {
    "use strict";

    reportModule.directive("gridMasterReport", masterReportDirective);

    masterReportDirective.$inject = ["ReportModel"];

    function masterReportDirective(ReportModel) {
        var focusEvaluationLevel = [
            {
                Value: 1,
                Name: "Strongly disagrees"
            },
            {
                Value: 2,
                Name: "Disagree"
            },
            {
                Value: 3,
                Name: "Neither agree nor disagree"
            },
            {
                Value: 4,
                Name: "Agree"
            },
            {
                Value: 5,
                Name: "Strongly agree"
            }
        ];

        function getFocusEvaluationLevel(value) {
            var level = {
                Value: 0,
                Name: ""
            }
            var itemIndex = focusEvaluationLevel.map(function (item) { return item.Value }).indexOf(value);

            if (itemIndex != -1) {
                level = angular.copy(focusEvaluationLevel[itemIndex]);
            }

            return level.Name;
        }

        function renderEvaluatorCommentCell(cell) {
            return '<a class="evaluator-comment" data-employee-id=' + cell.Value + '><i class="fa fa-search" aria-hidden="true"></i></a>';
        }

        function renderNextObjectives(cell) {
            return '<a class="next-objectives" data-employee-id=' + cell.Value + '><i class="fa fa-search" aria-hidden="true"></i></a>';
        }

        function renderExpandEmployeeAward(nominationGroup, projectGroup, employeeId) {
            var htmlString = "";

            if (projectGroup.RealRowCount && projectGroup.RealRowCount > 1) {
                htmlString = "<table class='employee-award-table' data-employee-id='" + employeeId + "' data-collapse='false' data-visible='false'>";
                htmlString += "<tbody>";
                htmlString += "<tr class='row-header'>" +
                    "<td class='col-title'></td>" +
                    "<td class='col-common'></td>" +
                    "<td class='col-project-name'></td>" +
                    "<td class='col-score'></td>" +
                    "<td class='col-common'></td>" +
                    "</tr>";

                var firstProject = true;
                for (var i = 0; i < projectGroup.Items.length; i++) {
                    var nomination = nominationGroup.Items[i];
                    var project = projectGroup.Items[i];
                    var row = "<tr>";

                    if (nomination.IsAdditionalRow) {
                        row += "<td colspan='2'></td>";
                    } else {
                        row += "<td>" + nomination.Title + "</td<td class='text-center'>" + nomination.StatusToString + "</td>";
                    }

                    if (project.IsAdditionalRow) {
                        row += "<td colspan='3'></td>";
                    } else {

                        if (firstProject) {
                            row += "<td><a class='col-projects-expand' data-employee-id='" + employeeId +
                                "'><i class='fa fa-caret-down'></i></a>&nbsp;" + project.ProjectName + "</td>";
                            firstProject = false;
                        } else {
                            row += "<td>&nbsp;&nbsp;&nbsp;&nbsp;" + project.ProjectName + "</td>";
                        }

                        row += "<td class='text-center'>" + project.DecryptedScore +
                            "&nbsp;<i class='fa fa-star'></i></td><td class='text-center'>" +
                            "<a class='project-comment' data-employee-id=" + project.EmployeeId + " data-project-id=" + project.ProjectId +
                            "><i class='fa fa-search'></i></a></td>";
                    }

                    row += "</tr>";

                    htmlString += row;
                }

                htmlString += "</tbody>";
                htmlString += "</table>"
            }

            return htmlString;
        }

        function renderCollapseEmployeeAward(nominationGroup, projectGroup, employeeId) {
            var htmlString = "<table class='employee-award-table' data-employee-id='" + employeeId + "' data-collapse='true' data-visible='true'>";

            htmlString += "<tbody>";
            htmlString += "<tr class='row-header'>" +
                "<td class='col-title'></td>" +
                "<td class='col-common'></td>" +
                "<td class='col-project-name'></td>" +
                "<td class='col-score'></td>" +
                "<td class='col-common'></td>" +
                "</tr>";

            if (projectGroup.RealRowCount && projectGroup.RealRowCount > 1) {
                var firstProject = true;
                for (var i = 0; i < projectGroup.Items.length; i++) {
                    var nomination = nominationGroup.Items[i];
                    var project = projectGroup.Items[i];
                    var row = "<tr>";

                    if (nomination.IsAdditionalRow) {
                        row += "<td colspan='2'></td>";
                    } else {
                        row += "<td>" + nomination.Title + "</td<td class='text-center'>" + nomination.StatusToString + "</td>";
                    }

                    if (project.IsAdditionalRow || !firstProject) {
                        row += "<td colspan='3'></td>";
                    } else {
                        row += "<td><a class='col-projects-expand' data-employee-id='" + employeeId +
                            "'><i class='fa fa-caret-down'></i></a>&nbsp;" + project.ProjectName + "</td>" +
                            "<td class='text-center'>" + project.DecryptedScore +
                            "&nbsp;<i class='fa fa-star'></i></td><td class='text-center'>" +
                            "<a class='project-comment' data-employee-id=" + project.EmployeeId + " data-project-id=" + project.ProjectId +
                            "><i class='fa fa-search'></i></a></td>";
                        firstProject = false;
                    }

                    row += "</tr>";

                    htmlString += row;

                    if (projectGroup.RealRowCount > nominationGroup.RealRowCount &&
                        nominationGroup.RealRowCount === i) {
                        break;
                    }
                }
            } else {
                for (var i = 0; i < projectGroup.Items.length; i++) {
                    var nomination = nominationGroup.Items[i];
                    var project = projectGroup.Items[i];
                    var row = "<tr>";

                    if (nomination.IsAdditionalRow) {
                        row += "<td colspan='2'></td>";
                    } else {
                        row += "<td>" + nomination.Title + "</td<td class='text-center'>" + nomination.StatusToString + "</td>";
                    }

                    if (project.IsAdditionalRow) {
                        row += "<td colspan='3'</td>";
                    } else {
                        row += "<td>" + project.ProjectName + "</td><td class='text-center'>" + project.DecryptedScore +
                            "&nbsp;<i class='fa fa-star'></i></td><td class='text-center'>" +
                            "<a class='project-comment' data-employee-id=" + project.EmployeeId + " data-project-id=" + project.ProjectId +
                            "><i class='fa fa-search'></i></a></td>";
                    }

                    row += "</tr>";

                    htmlString += row;
                }
            }

            htmlString += "</tbody>";
            htmlString += "</table>";

            return htmlString;
        }

        function renderEmployeeAward(cell) {
            var employee = cell.Value;
            var renderCollapseHtml = "";
            var renderExpandHtml = "";
            var renderHtml = "";

            renderHtml += renderCollapseEmployeeAward(employee.Nomination, employee.Project, employee.EmployeeId);
            renderHtml += renderExpandEmployeeAward(employee.Nomination, employee.Project, employee.EmployeeId);

            return renderHtml;
        }

        function groupData(data) {
            var groups = [];

            angular.forEach(data, function (record) {
                var groupIndex = groups.map(function (item) { return item.GroupId; }).indexOf(record.PAFocus.Employee.Group.GroupId);
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
        }

        function generateGroupHeaders() {
            return [{
                Columns: [
                    { HeaderText: "#", Name: "SymbolCount", MergeRowNumber: 2 },
                    { HeaderText: "Nick Name", Name: "NickName", MergeRowNumber: 2 },
                    { HeaderText: "Full Name", Name: "FullName", MergeRowNumber: 2 },
                    { HeaderText: "Job Title", Name: "JobTitle", MergeRowNumber: 2 },
                    { HeaderText: "Reviewer's Adjustment", Name: "ReviewerAdjustment", MergeRowNumber: 2 },
                    { HeaderText: "PA Final Score", Name: "PAFinalScore", MergeRowNumber: 2 },
                    { HeaderText: "Evaluator's Comment", Name: "EvaluatorComment", MergeRowNumber: 2 },
                    { HeaderText: "Suitable for team goal", Name: "Contribution", MergeRowNumber: 2 },
                    { HeaderText: "Deserve rewards (salary increase and bonus)", Name: "BonusGiven", MergeRowNumber: 2 },
                    { HeaderText: "Applicable to promote", Name: "ApplicableToPromote", MergeRowNumber: 2 },
                    { HeaderText: "Poor Performance", Name: "IsPoorPerformance", MergeRowNumber: 2 },
                    { HeaderText: "Assessing of Current Objective Achievement", Name: "ObjectiveAchievement", MergeRowNumber: 2 },
                    { HeaderText: "Next Objectives", Name: "NextObjectives", MergeRowNumber: 2 },
                    { HeaderText: "Award", Name: "AwardName", MergeColNumber: 2 },
                    { HeaderText: "Evaluation by PM", Name: "ProjectName", MergeColNumber: 3 }
                ]
            }];
        }

        function generateColumnHeaders() {
            return [
               { HeaderText: "#", Width: 50, Name: "SymbolCount", ShowCollapseExpand: true },
               { HeaderText: "Nick Name", Width: 80, Name: "NickName", CellAlign: "center" },
               { HeaderText: "Full Name", Width: 200, Name: "FullName" },
               { HeaderText: "Job Title", Width: 150, Name: "JobTitle" },
               { HeaderText: "Reviewer's Adjustment", Width: 80, Name: "ReviewerAdjustment", CellAlign: "center" },
               { HeaderText: "PA Final Score", Width: 60, Name: "PAFinalScore", CellAlign: "center" },
               { HeaderText: "Evaluator's Comment", Width: 80, Name: "EvaluatorComment", CellAlign: "center", CustomCellRender: renderEvaluatorCommentCell },
               { HeaderText: "Suitable for team goal", Width: 150, Name: "Contribution", CellAlign: "center" },
               { HeaderText: "Deserve rewards (salary increase and bonus)", Width: 150, Name: "BonusGiven", CellAlign: "center" },
               { HeaderText: "Applicable to promote", Width: 50, Name: "ApplicableToPromote", CellAlign: "center" },
               { HeaderText: "Accessing of Objective achievement", Width: 150, Name: "ObjectiveAchievement", CellAlign: "center" },
               { HeaderText: "Poor Performance", Name: "IsPoorPerformance", Width: 80, CellAlign: "center" },
               { HeaderText: "Next Objectives", Width: 80, Name: "NextObjectives", CellAlign: "center", CustomCellRender: renderNextObjectives },
               { HeaderText: "Title", Width: 100, Name: "AwardName", CustomCellRender: renderEmployeeAward, TdClass: 'col-employee-award' },
               { HeaderText: "Status", Width: 80, Name: "AwardStatus" },
               { HeaderText: "Project Name ", Width: 150, Name: "ProjectName" },
               { HeaderText: "Score", Width: 80, Name: "ProjectScore" },
               { HeaderText: "Comment", Width: 80, Name: "ProjectComment", CellAlign: "center" }
            ];
        }

        function generateBodyRows(groupReports) {
            var rows = [];
            angular.forEach(groupReports, function (group) {
                var itemCount = 1;
                var row = {
                    Columns: [
                        { Name: "SymbolCount", Value: group.GroupName, MergeColNumber: 18, ShowCollapseExpand: true }
                    ],
                    SubRows: []
                };

                angular.forEach(group.Members, function (member) {
                    var subRow = {
                        Columns: []
                    };
                    var employee = {
                        EmployeeId: member.PAFocus.Employee.EmployeeId,
                        Nomination: {
                            Items: member.Nominations,
                            HasAdditionRow: member.HasAdditionalNomination,
                            RealRowCount: member.RealNominationCount
                        },
                        Project: {
                            Items: member.EmployeeWorkings,
                            HasAdditionRow: member.HasAdditionalProject,
                            RealRowCount: member.RealProjectCount
                        }
                    };

                    subRow.Columns.push({ Name: "SymbolCount", Value: itemCount });
                    subRow.Columns.push({ Name: "NickName", Value: member.PAFocus.Employee.NickName });
                    subRow.Columns.push({ Name: "FullName", Value: member.PAFocus.Employee.FullName });
                    subRow.Columns.push({ Name: "JobTitle", Value: member.PAFocus.Employee.JobTitle.JobTitleName });
                    subRow.Columns.push({ Name: "ReviewerAdjustment", Value: member.ReviewerAdjustment });
                    subRow.Columns.push({ Name: "PAFinalScore", Value: member.PAFocus.FinalScore });
                    subRow.Columns.push({ Name: "EvaluatorComment", Value: member.PAFocus.Employee.EmployeeId });
                    subRow.Columns.push({ Name: "Contribution", Value: getFocusEvaluationLevel(member.PAFocus.Contribution) });
                    subRow.Columns.push({ Name: "BonusGiven", Value: getFocusEvaluationLevel(member.PAFocus.BonusGiven) });
                    subRow.Columns.push({ Name: "ApplicableToPromote", Value: member.PAFocus.IsReadyPromotion ? "Yes" : "No" });
                    subRow.Columns.push({ Name: "IsPoorPerformance", Value: member.PAFocus.IsPoorPerformance ? "Yes" : "No" });
                    subRow.Columns.push({ Name: "ObjectiveAchievement", Value: getFocusEvaluationLevel(member.PAFocus.LastCompletedPercentObjective) });
                    subRow.Columns.push({ Name: "NextObjectives", Value: member.PAFocus.Employee.EmployeeId });
                    subRow.Columns.push({ Name: "AwardName", Value: employee, MergeColNumber: 5 });

                    row.SubRows.push(subRow);

                    itemCount++;
                });

                rows.push(angular.copy(row));
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
                    //fullHeight: false,
                    //gridHeight: 600,
                    showTree: true
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

                    element.find(".project-comment").bind("click", function () {
                        var $self = $(this);
                        var employeeId = $self.data("employee-id");
                        var projectId = $self.data("project-id");

                        if (employeeId) {
                            ReportModel.openPopupProjectComment(employeeId, projectId);
                        }
                    });

                    element.find(".col-projects-expand").bind("click", function () {
                        var $self = $(this);
                        var employeeId = $self.data("employee-id");
                        var tables = $(".employee-award-table[data-employee-id=" + employeeId + "]");

                        console.log($(tables[0]).data("visible"));
                        console.log($(tables[1]).data("visible"));
                        console.log("----------------------------");
                        console.log($(tables[0]).attr("data-visible"));
                        console.log($(tables[1]).attr("data-visible"));
                        console.log("----------------------------");

                        for (var i = 0; i < tables.length; i++) {
                            var $table = $(tables[i]);
                            var visible = $table.attr("data-visible") === "true";

                            $table.attr("data-visible", !visible);
                        }
                    });
                });
            }
        };
    }
})(angular.module("reportModule"));