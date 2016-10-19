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
        var employeeIndex = masterReports.map(function (item) { return item.PAFocus.Employee.EmployeeId; }).indexOf(employeeId);

        if (employeeIndex !== -1) {
            employee = masterReports[employeeIndex];
        }

        return employee;
    }

    function findProjectInMasterReport(masterReports, employeeId, projectId) {
        var project = null;
        var employee = null;

        employee = findEmployeeInMasterReport(masterReports, employeeId);

        if (employee) {
            var projectIndex = employee.EmployeeWorkings.map(function (item) { return item.ProjectId; }).indexOf(projectId);

            if (projectIndex !== -1) {
                project = employee.EmployeeWorkings[projectIndex];
                project.Employee = {
                    FullName: employee.PAFocus.Employee.FullName,
                    NickName: employee.PAFocus.Employee.NickName
                };
            }
        }

        return project;
    }

    function openMasterReportCommentModal(modalTitle, comment) {
        reportModel.modalInstance = $modal.open({
            templateUrl: "SourceApp/Components/Report/Templates/modalMasterReportComment.html",
            controller: "ModalMasterReportCommentController as modalMasterReportCommentCtrl",
            backdrop: "static",
            windowClass: "modal-confirm-center popup-md",
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

    function correctMasterReportData(masterReports) {
        angular.forEach(masterReports, function (record) {
            var projectIndex = 1;

            record.Nominations = record.Nominations || [];
            record.EmployeeWorkings = record.EmployeeWorkings || [];

            var differIndex = record.EmployeeWorkings.length - record.Nominations.length;

            angular.forEach(record.EmployeeWorkings, function (project) {
                project.ProjectId = projectIndex;
                projectIndex++;
            });

            record.RealNominationCount = record.Nominations ? record.Nominations.length : 0;
            record.RealProjectCount = record.EmployeeWorkings ? record.EmployeeWorkings.length : 0;

            if (differIndex > 0) {
                record.HasAdditionalNomination = true;

                for (var i = 0; i < differIndex; i++) {
                    record.Nominations.push({ IsAdditionalRow: true });
                }
            } else if (differIndex < 0) {
                record.HasAdditionalProject = true;

                for (var j = 0; j < -differIndex; j++) {
                    record.EmployeeWorkings.push({ IsAdditionalRow: true });
                }
            }
        });
    }

    reportModel.openPopupProjectComment = function (employeeId, projectId) {
        var project = findProjectInMasterReport(reportModel.masterReports, employeeId, projectId);

        reportModel.modalInstance = $modal.open({
            templateUrl: "SourceApp/Components/Report/Templates/modalMasterReportProject.html",
            controller: "ModalMasterReportProjectController as modalMasterReportProjectCtrl",
            backdrop: "static",
            windowClass: "modal-confirm-center popup-md",
            resolve: {
                project: function () {
                    return angular.copy(project);
                }
            }
        });

        return reportModel.modalInstance.result;
    };

    reportModel.openPopupCurrentObjective = function (employeeId) {
        var employee = findEmployeeInMasterReport(reportModel.masterReports, employeeId);
        var modalTitle = "Current Objectives";

        if (employee) {
            return openMasterReportCommentModal(modalTitle, employee.PAFocus.LastCompletedPercentObjective);
        }
    };

    reportModel.openPopupEvaluatorComment = function (employeeId) {
        var employee = findEmployeeInMasterReport(reportModel.masterReports, employeeId);
        var modalTitle = "Evaluator's Comment";

        reportModel.modalInstance = $modal.open({
            templateUrl: "SourceApp/Components/Report/Templates/modalMasterReportEvaluatorComment.html",
            controller: "ModalMasterReportEvaluatorCommentController as modalMasterReportEvaluatorCommentCtrl",
            backdrop: "static",
            windowClass: "modal-confirm-center popup-md",
            resolve: {
                evaluatorComments: function () {
                    return angular.copy(employee.EvaluatorComments);
                },
                modalTitle: function () {
                    return angular.copy(modalTitle);
                }
            }
        });

        return reportModel.modalInstance.result;
    };

    reportModel.openPopupNextObjectives = function (employeeId) {
        var employee = findEmployeeInMasterReport(reportModel.masterReports, employeeId);
        var modalTitle = "Next Objectives";
        if (employee) {
            return openMasterReportCommentModal(modalTitle, employee.PAFocus.NextDescriptionObjective);
        }
    };

    reportModel.getMasterReport = function (periodId, groupIds, employeeId) {
        return reportFactory.getMasterReport(periodId, groupIds, employeeId)
            .success(function (masterReports) {
                correctMasterReportData(masterReports);

                reportModel.masterReports = masterReports;
            }).error(function (response, status) {
                errorFactory.errorHandle(status);
            });
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