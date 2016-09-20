(function (reportModule) {
    "use strict";

    reportModule.directive("gridMasterReport", masterReportDirective);

    masterReportDirective.$inject = [];

    function masterReportDirective() {
        function groupData(data) {
            var groups = [];

            return group;
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
               { HeaderText: "#", Width: 80, Name: "SymbolCount", CellAlign: "center" },
               { HeaderText: "Nick Name", Width: 80, Name: "NickName", CellAlign: "center" },
               { HeaderText: "Full Name", Width: 80, Name: "FullName" },
               { HeaderText: "Job Title", Width: 80, Name: "JobTitle" },
               { HeaderText: "Reviewer's Adjustment", Width: 80, Name: "ReviewerAdjustment", CellAlign: "center" },
               { HeaderText: "PA Final Score", Width: 80, Name: "PAFinalScore", CellAlign: "center" },
               { HeaderText: "Evaluator's Comment", Width: 80, Name: "EvaluatorComment", CellAlign: "center" },
               { HeaderText: "Contribution", Width: 80, Name: "Contribution", CellAlign: "center" },
               { HeaderText: "Applicable to promote", Width: 80, Name: "ApplicableToPromote", CellAlign: "center" },
               { HeaderText: "Bonus Given", Width: 80, Name: "BonusGiven", CellAlign: "center" },
               { HeaderText: "Bonus Ranking", Width: 80, Name: "BonusRanking", CellAlign: "center" },
               { HeaderText: "Accessing of Objective achievement", Width: 80, Name: "ObjectiveAchievement", CellAlign: "center" },
               { HeaderText: "Next Objectives", Width: 80, Name: "NextObjectives", CellAlign: "center" },
               { HeaderText: "Award Name ", Width: 80, Name: "AwardName" },
               { HeaderText: "Status", Width: 80, Name: "AwardStatus" },
               { HeaderText: "Project Name ", Width: 80, Name: "ProjectName" },
               { HeaderText: "Score", Width: 80, Name: "ProjectScore" },
               { HeaderText: "Comment", Width: 80, Name: "ProjectComment", CellAlign: "center" }
            ];
        }

        function generateBodyRows() {
            var rows = [];

            for (var i = 1; i < 100; i++) {
                var currentRow = {
                    Columns: [
                        { Name: "TeamName", Value: i },
                        { Name: "SymbolCount", Value: i },
                        { Name: "NickName", Value: i },
                        { Name: "FullName", Value: i },
                        { Name: "JobTitle", Value: i },
                        { Name: "ReviewerAdjustment", Value: i },
                        { Name: "PAFinalScore", Value: i },
                        { Name: "EvaluatorComment", Value: i },
                        { Name: "Contribution", Value: i },
                        { Name: "ApplicableToPromote", Value: i },
                        { Name: "BonusGiven", Value: i },
                        { Name: "BonusRanking", Value: i },
                        { Name: "ObjectiveAchievement", Value: i },
                        { Name: "NextObjectives", Value: i },
                        { Name: "AwardName", Value: i },
                        { Name: "AwardStatus", Value: i },
                        { Name: "ProjectName", Value: i },
                        { Name: "ProjectScore", Value: i },
                        { Name: "ProjectComment", Value: i }
                    ]
                };

                rows.push(currentRow);
            }

            return rows;
        }

        return {
            restrict: "A",
            scope: {
                gridData: "="
            },
            link: function ($scope, iElement) {
                var gridOptions = {
                    customizeHeader: true,
                    gridExpandHeight: 500,
                    numberOfFrozenColumn: 7
                };

                $scope.$watch("gridData", function (gridData) {
                    var groupReports = groupData(gridData);
                    var groupHeaders = generateGroupHeaders();
                    var columnHeaders = generateColumnHeaders();
                    var bodyRows = generateBodyRows();

                    gridOptions.groupHeaderRows = groupHeaders;
                    gridOptions.columns = columnHeaders;
                    gridOptions.bodyRows = bodyRows;

                    $(iElement).fanexGrid(gridOptions);
                });
            }
        };
    }
})(angular.module("reportModule"));