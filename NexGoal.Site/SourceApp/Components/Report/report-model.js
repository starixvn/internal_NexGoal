'use strict';
/*global reportModule*/

reportModule.factory('ReportModel',
['$modal', 'reportFactory', 'notificationFactory', 'errorFactory',
function ($modal, reportFactory, notificationFactory, errorFactory) {
    var reportModel = {};

    reportModel.masterReports = [];
    reportModel.paReports = [];
    reportModel.pdfFileName = "";

    function findEmployeeInMasterReport(masterReports, employeeId) {
        var employee = null;
        var employeeIndex = masterReports.map(function (item) { return item.PAFocus.Employee.EmployeeId }).indexOf(employeeId);

        if (employeeIndex !== -1) {
            employee = masterReports[employeeIndex];
        }

        return employee;
    }

    reportModel.openPopupEvaluatorComment = function (employeeId) {
        var employee = findEmployeeInMasterReport(reportModel.masterReports, employeeId);

        if (employee) {
            reportModel.modalInstance = $modal.open({
                templateUrl: "SourceApp/Components/Report/Templates/modalMasterReportEvaluatorComment.html",
                controller: "ModalMasterReportEvaluatorCommentController as modalMasterReportEvaluatorCommentCtrl",
                backdrop: "static",
                windowClass: "modal-confirm-center popup-lg",
                resolve: {
                    evaluatorComment: function () {
                        return angular.copy(employee.EvaluatorComment);
                    }
                }
            });

            return reportModel.modalInstance.result;
        }
    };

    reportModel.getMasterReport = function () {
        var data = [];
        var forrest = {
            ReviewerAdjustment: 3,
            EvaluatorComment: "Evaluator Comment",
            PAFocus: {
                FinalScore: "85.55",
                Contribution: 1,
                BonusGiven: 25,
                BonusRanking: 2,
                IsReadyPromotion: false,
                LastCompletedPercentObjective: 80,
                NextDescriptionObjective: "Next Description Objective",
                Employee: {
                    EmployeeId: 1,
                    NickName: "Forrest",
                    FullName: "Nguyễn Văn Lâm",
                    JobTitle: {
                        JobTitleName: "Software Engineer"
                    },
                    Group: {
                        GroupId: 1,
                        GroupName: "Nexus"
                    }
                }
            },
            Nominations: [
                {
                    Quarter: 1,
                    Year: 2016,
                    Status: 1
                },
                {
                    Quarter: 2,
                    Year: 2016,
                    Status: 2
                },
                {
                    Quarter: 3,
                    Year: 2016,
                    Status: 3
                },
                {
                    Quarter: "",
                    Year: 2016,
                    Status: 3
                }
            ],
            EmployeeWorkings: [
                {
                    ProjectName: "Project A",
                    DecryptedScore: 5,
                    Comment: "Project A's Comment"
                },
                {
                    ProjectName: "Project B",
                    DecryptedScore: 4,
                    Comment: "Project B's Comment"
                },
                {
                    ProjectName: "Project A",
                    DecryptedScore: 5,
                    Comment: "Project B's Comment"
                }
            ]
        };
        var tim = {
            ReviewerAdjustment: 3,
            EvaluatorComment: "Evaluator Comment",
            PAFocus: {
                FinalScore: 85,
                Contribution: 1,
                BonusGiven: 25,
                BonusRanking: 2,
                IsReadyPromotion: false,
                LastCompletedPercentObjective: 80,
                NextDescriptionObjective: "Next Description Objective",
                Employee: {
                    EmployeeId: 2,
                    NickName: "Tim",
                    FullName: "Trịnh Đăng Thắng",
                    JobTitle: {
                        JobTitleName: "Software Engineer"
                    },
                    Group: {
                        GroupId: 1,
                        GroupName: "Nexus"
                    }
                }
            },
            Nominations: [
                {
                    Quarter: 1,
                    Year: 2016,
                    Status: 2
                },
                {
                    Quarter: 2,
                    Year: 2016,
                    Status: 3
                },
                {
                    Quarter: 3,
                    Year: 2016,
                    Status: 1
                },
                {
                    Quarter: "",
                    Year: 2016,
                    Status: 2
                }
            ],
            EmployeeWorkings: [
                {
                    ProjectName: "Project A",
                    DecryptedScore: 5,
                    Comment: "Project A's Comment"
                },
                {
                    ProjectName: "Project B",
                    DecryptedScore: 4,
                    Comment: "Project B's Comment"
                },
                {
                    ProjectName: "Project A",
                    DecryptedScore: 5,
                    Comment: "Project B's Comment"
                }
            ]
        };
        var quinn = {
            ReviewerAdjustment: 3,
            EvaluatorComment: "Evaluator Comment",
            PAFocus: {
                FinalScore: 85,
                Contribution: 1,
                BonusGiven: 25,
                BonusRanking: 2,
                IsReadyPromotion: false,
                LastCompletedPercentObjective: 80,
                NextDescriptionObjective: "Next Description Objective",
                Employee: {
                    EmployeeId: 3,
                    NickName: "Quinn",
                    FullName: "Võ Kim Quang",
                    JobTitle: {
                        JobTitleName: "Software Engineer / Team Leader"
                    },
                    Group: {
                        GroupId: 1,
                        GroupName: "Nexus"
                    }
                }
            },
            Nominations: [],
            EmployeeWorkings: [
                {
                    ProjectName: "Project A",
                    DecryptedScore: 5,
                    Comment: "Project A's Comment"
                },
                {
                    ProjectName: "Project B",
                    DecryptedScore: 4,
                    Comment: "Project B's Comment"
                },
                {
                    ProjectName: "Project A",
                    DecryptedScore: 5,
                    Comment: "Project B's Comment"
                }
            ]
        };

        var jacob = {
            ReviewerAdjustment: -1,
            EvaluatorComment: "Evaluator Comment",
            PAFocus: {
                FinalScore: 85,
                Contribution: 1,
                BonusGiven: 25,
                BonusRanking: 2,
                IsReadyPromotion: false,
                LastCompletedPercentObjective: 80,
                NextDescriptionObjective: "Next Description Objective",
                Employee: {
                    EmployeeId: 4,
                    NickName: "Jacob",
                    FullName: "Phan Đình Hải Sơn",
                    JobTitle: {
                        JobTitleName: "Staff Software Engineer / Team Leader"
                    },
                    Group: {
                        GroupId: 2,
                        GroupName: "Olympus"
                    }
                }
            },
            Nominations: [],
            EmployeeWorkings: [
                {
                    ProjectName: "Project A",
                    DecryptedScore: 5,
                    Comment: "Project A's Comment"
                },
                {
                    ProjectName: "Project B",
                    DecryptedScore: 4,
                    Comment: "Project B's Comment"
                },
                {
                    ProjectName: "Project A",
                    DecryptedScore: 5,
                    Comment: "Project B's Comment"
                }
            ]
        };
        var kaden = {
            ReviewerAdjustment: -1,
            EvaluatorComment: "Evaluator Comment",
            PAFocus: {
                FinalScore: 85,
                Contribution: 1,
                BonusGiven: 25,
                BonusRanking: 2,
                IsReadyPromotion: false,
                LastCompletedPercentObjective: 80,
                NextDescriptionObjective: "Next Description Objective",
                Employee: {
                    EmployeeId: 5,
                    NickName: "Kaden",
                    FullName: "Trịnh Đăng Khoa",
                    JobTitle: {
                        JobTitleName: "Software Engineer"
                    },
                    Group: {
                        GroupId: 2,
                        GroupName: "Olympus"
                    }
                }
            },
            Nominations: [],
            EmployeeWorkings: [
                {
                    ProjectName: "Project A",
                    DecryptedScore: 5,
                    Comment: "Project A's Comment"
                },
                {
                    ProjectName: "Project B",
                    DecryptedScore: 4,
                    Comment: "Project B's Comment"
                },
                {
                    ProjectName: "Project A",
                    DecryptedScore: 5,
                    Comment: "Project B's Comment"
                }
            ]
        };

        data.push(forrest);
        data.push(tim);
        data.push(quinn);
        data.push(jacob);
        data.push(kaden);

        reportModel.masterReports = data;
    };

    reportModel.requestExportEPA = function (epaId) {
        return reportFactory.requestExportEPA(epaId)
            .success(function (fileName) {
                reportModel.pdfFileName = fileName;
            }).error(function (response, status) {
                errorFactory.errorHandle(status);
            });
    };

    reportModel.requestExportSemiEPA = function (epaId) {
        return reportFactory.requestExportSemiEPA(epaId)
            .success(function (fileName) {
                reportModel.pdfFileName = fileName;
            }).error(function (response, status) {
                errorFactory.errorHandle(status);
            });
    };

    reportModel.getPAReport = function (periodIds, groupIds, employeeId) {
        return reportFactory.getPAReport(periodIds, groupIds, employeeId)
        .success(function (paReports) {
            reportModel.paReports = paReports;
        }).error(function (response, status) {
            errorFactory.errorHandle(status);
        });
    };

    return reportModel;
}]);