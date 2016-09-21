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

    function openMasterReportCommentModal(modalTitle, comment) {
        reportModel.modalInstance = $modal.open({
            templateUrl: "SourceApp/Components/Report/Templates/modalMasterReportComment.html",
            controller: "ModalMasterReportCommentController as modalMasterReportCommentCtrl",
            backdrop: "static",
            windowClass: "modal-confirm-center popup-lg",
            resolve: {
                comment: function () {
                    return angular.copy(comment);
                },
                modalTitle: function () {
                    return angular.copy(modalTitle);
                }
            }
        });

        return reportModel.modalInstance.result;
    }

    reportModel.openPopupEvaluatorComment = function (employeeId) {
        var employee = findEmployeeInMasterReport(reportModel.masterReports, employeeId);
        var modalTitle = "Evaluator's Comment";
        if (employee) {
            return openMasterReportCommentModal(modalTitle, employee.EvaluatorComment);
        }
    };

    reportModel.openPopupNextObjectives = function (employeeId) {
        var employee = findEmployeeInMasterReport(reportModel.masterReports, employeeId);
        var modalTitle = "Next Objectives";
        if (employee) {
            return openMasterReportCommentModal(modalTitle, employee.PAFocus.NextDescriptionObjective);
        }
    }

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
                    StatusToString: "Nominated",
                    Title: "EOQ: 1st 2016"
                },
                {
                    StatusToString: "Shortlisted",
                    Title: "EOQ: 2nd 2016"
                },
                {
                    StatusToString: "Winner",
                    Title: "EOQ: 3rd 2016"
                },
                {
                    StatusToString: "Winner",
                    Title: "EOY: 2016"
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
                    StatusToString: "Shortlisted",
                    Title: "EOQ: 1st 2016"
                },
                {
                    StatusToString: "Winner",
                    Title: "EOQ: 2nd 2016"
                },
                {
                    StatusToString: "Nominated",
                    Title: "EOQ: 3rd 2016"
                },
                {
                    StatusToString: "Shortlisted",
                    Title: "EOY: 2016"
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