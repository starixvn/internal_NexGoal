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
            windowClass: "modal-confirm-center",
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

            if (differIndex > 0) {
                record.Nominations.AdditionalRow = true;
                record.Nominations.RealRowCount = record.Nominations.length;

                for (var i = 0; i < differIndex; i++) {
                    record.Nominations.push({ AdditionalRow: true });
                }
            } else if (differIndex < 0) {
                record.EmployeeWorkings.AdditionalRow = true;
                record.EmployeeWorkings.RealRowCount = record.EmployeeWorkings.length;

                for (var j = 0; j < -differIndex; j++) {
                    record.EmployeeWorkings.push({ AdditionalRow: true });
                }
            }
        });
    }

    reportModel.openPopupProjectComment = function (project) {
        reportModel.modalInstance = $modal.open({
            templateUrl: "SourceApp/Components/Report/Templates/modalMasterReportProject.html",
            controller: "ModalMasterReportProjectController as modalMasterReportProjectCtrl",
            backdrop: "static",
            windowClass: "modal-confirm-center",
            resolve: {
                project: function () {
                    return angular.copy(project);
                }
            }
        });

        return reportModel.modalInstance.result;
    };

    //reportModel.openPopupEvaluatorComment = function (comment) {
    //    var modalTitle = "Evaluator's Comment";

    //    return openMasterReportCommentModal(modalTitle, comment);
    //};

    reportModel.openPopupEvaluatorComment = function (employeeId) {
        var employee = findEmployeeInMasterReport(reportModel.masterReports, employeeId);
        var modalTitle = "Evaluator's Comment";
        if (employee) {
            return openMasterReportCommentModal(modalTitle, employee.EvaluatorComment);
        }
    };

    //reportModel.openPopupNextObjectives = function (comment) {
    //    var modalTitle = "Next Objectives";

    //    return openMasterReportCommentModal(modalTitle, comment);
    //}

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