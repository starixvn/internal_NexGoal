'use strict';
/*global epaModule*/

epaModule.factory('SelfEvalutorModel',
['$modal', 'EmployeeEPAModel', 'employeeEPAFactory', 'utilitiesFactory', 'notificationFactory', 'errorFactory', "integrateFactory",
function ($modal, EmployeeEPAModel, employeeEPAFactory, utilitiesFactory, notificationFactory, errorFactory, integrateFactory) {
    var selfEvalutorModel = this;

    selfEvalutorModel.myEPAs = [];
    selfEvalutorModel.groups = [];
    selfEvalutorModel.members = [];
    selfEvalutorModel.memberEPAs = [];
    selfEvalutorModel.epaInfo = {};
    selfEvalutorModel.permission = 0;
    selfEvalutorModel.hrmSummary = {};
    selfEvalutorModel.employeeProjects = [];

    // common
    selfEvalutorModel.calculateScore = function (vm) {
        var totalScore = 0;

        angular.forEach(vm.epaInfo.KPIGroups, function (kpiGroup) {
            totalScore += kpiGroup.SelfDecodeScore;
        });

        vm.epaInfo.Process.CurrentProcess.TotalScore = utilitiesFactory.parseFloat(totalScore);
        var maxTotalScore = vm.epaInfo.Process.CurrentProcess.MaxTotalScore;
        vm.epaInfo.Process.CurrentProcess.TotalScore = EmployeeEPAModel.checkScore(vm.epaInfo.Process.CurrentProcess.TotalScore, 0, maxTotalScore);

        vm.finalScore = totalScore;
        vm.finalScore = EmployeeEPAModel.checkScore(vm.finalScore, 0, vm.epaInfo.Process.CurrentProcess.MaxPerformanceScore);

        vm.epaInfo.Grade = EmployeeEPAModel.convertScoreToGrade(vm.finalScore, vm.standardGrades);
    };

    selfEvalutorModel.scoreSpreadForKPIGroupHasKPIOptional = function (kpiGroup, maxStar) {
        var score, averageWeight, totalStarEvaluated = 0,
            numberKPIOptional = kpiGroup.NumberKPIOptional;
        var kpis = [];

        angular.forEach(kpiGroup.KPIs, function (kpi) {
            if (kpi.CurrentEvaluation.Rate > 0) {
                kpis.push(kpi);
                totalStarEvaluated += kpi.CurrentEvaluation.Rate;
            }

            if (!kpi.IsRequiredEvaluate && kpi.CurrentEvaluation.Rate > 0) {
                numberKPIOptional -= 1;
            }
        });

        var totalKPIMustEvaluate = kpiGroup.KPIs.length - numberKPIOptional;
        averageWeight = kpiGroup.Weight / totalKPIMustEvaluate;
        averageWeight = utilitiesFactory.splitNumber(averageWeight, 2);

        angular.forEach(kpis, function (kpi) {
            kpi.Weight = averageWeight;
            score = (averageWeight * kpi.CurrentEvaluation.Rate) / maxStar;
            kpi.CurrentEvaluation.Score = utilitiesFactory.splitNumber(score, 2);
        });
    };

    selfEvalutorModel.openPopupNotificationInvalidComment = function (vm, message) {
        vm.modalInstance = $modal.open({
            templateUrl: 'SourceApp/Components/EPA/Templates/modalNotification.html',
            controller: "ModalNotificationController as modalNotificationCtrl",
            backdrop: 'static',
            resolve: {
                message: function () {
                    return message;
                }
            },
            windowClass: "modal-confirm-center"
        });

        return vm.modalInstance.result;
    };

    selfEvalutorModel.validateEPA = function (epaInfo) {
        var isValid = true;
        var employeeId = epaInfo.Employee.EmployeeId;

        angular.forEach(epaInfo.KPIGroups, function (kpiGroup) {
            angular.forEach(kpiGroup.KPIs, function (kpi) {
                var score = utilitiesFactory.parseFloat(kpi.CurrentEvaluation.Score);

                if (kpi.IsRequiredEvaluate && (score <= 0)) {
                    isValid = false;
                    kpi.CurrentEvaluation.EvaluateInvalid = true;
                } else {
                    kpi.CurrentEvaluation.EvaluateInvalid = false;
                }

                if (kpi.HasComment && (!kpi.CurrentEvaluation.Comment || kpi.CurrentEvaluation.Comment.length === 0)
                    && (kpi.IsRequiredComment || (epaInfo.RequiredCommentOnMaxScore && !kpi.IsRequiredComment && kpi.CurrentEvaluation.IsMaxScore))
                    && (employeeId === kpi.CurrentEvaluation.EvaluatorId)) {
                    isValid = false;
                    kpi.CurrentEvaluation.CommentInvalid = true;
                    kpi.ExpandedComment = true;
                    kpi.cssClass = "note";
                } else {
                    kpi.CurrentEvaluation.CommentInvalid = false;
                    kpi.cssClass = "";
                }
            });
        });

        return isValid;
    };

    // self
    selfEvalutorModel.getUserPermissionOnEmployee = function () {
        return employeeEPAFactory.getUserPermissionOnEmployee()
        .success(function (permission) {
            selfEvalutorModel.permission = parseInt(permission, 10);
        })
        .error(function (response, status) {
            errorFactory.errorHandle(status);
        });
    };

    selfEvalutorModel.getEPAByEmployee = function (periodIds) {
        return employeeEPAFactory.getEPAByEmployee(periodIds)
        .success(function (myEPAs) {
            selfEvalutorModel.myEPAs = myEPAs;
        }).error(function (response, status) {
            errorFactory.errorHandle(status);
        });
    };

    selfEvalutorModel.getEPAInfoByEmployee = function (epaId) {
        return employeeEPAFactory.getEPAInfoByEmployee(epaId)
        .success(function (epaInfo) {
            selfEvalutorModel.epaInfo = epaInfo;
        }).error(function (response, status) {
            errorFactory.errorHandle(status);
        });
    };

    selfEvalutorModel.updateEPAScoreByEmployee = function (vm) {
        var criteria = {
            EPAId: vm.epaInfo.EPAId,
            EmployeeId: vm.epaInfo.Employee.EmployeeId,
            EPAProcess: vm.epaInfo.Process.CurrentProcess,
            KPIGroups: vm.epaInfo.KPIGroups
        };

        return employeeEPAFactory.updateEPAScoreByEmployee(criteria)
        .success(function (response) {
            if (response.IsSuccessful) {
                notificationFactory.success("UpdatedSuccessfully");
                vm.dataSaved = true;
            }
            else {
                notificationFactory.error("", response.Message);
            }
        }).error(function (response, status) {
            errorFactory.errorHandle(status);
        });
    };

    // evaluator
    selfEvalutorModel.getUserPermissionOnEvaluator = function () {
        return employeeEPAFactory.getUserPermissionOnEvaluator()
        .success(function (permission) {
            selfEvalutorModel.permission = parseInt(permission, 10);
        })
        .error(function (response, status) {
            errorFactory.errorHandle(status);
        });
    };

    selfEvalutorModel.getGroupByEvaluator = function (periodIds) {
        return employeeEPAFactory.getGroupByEvaluator(periodIds)
        .success(function (groups) {
            selfEvalutorModel.groups = groups;
        }).error(function (response, status) {
            errorFactory.errorHandle(status);
        });
    };

    selfEvalutorModel.getMemberByEvaluator = function (groupIds, periodIds) {
        return employeeEPAFactory.getMemberByEvaluator(groupIds, periodIds)
        .success(function (members) {
            selfEvalutorModel.members = members;
        }).error(function (response, status) {
            errorFactory.errorHandle(status);
        });
    };

    selfEvalutorModel.getEPAByEvaluator = function (periodIds, groupIds, employeeIds) {
        return employeeEPAFactory.getEPAByEvaluator(periodIds, groupIds, employeeIds)
        .success(function (memberEPAs) {
            selfEvalutorModel.memberEPAs = memberEPAs;
        }).error(function (response, status) {
            errorFactory.errorHandle(status);
        });
    };

    selfEvalutorModel.getEPAInfoByEvaluator = function (epaId) {
        return employeeEPAFactory.getEPAInfoByEvaluator(epaId)
        .success(function (epaInfo) {
            selfEvalutorModel.epaInfo = epaInfo;
        }).error(function (response, status) {
            errorFactory.errorHandle(status);
        });
    };

    selfEvalutorModel.updateEPAScoreByEvaluator = function (vm) {
        var criteria = {
            EPAId: vm.epaInfo.EPAId,
            EmployeeId: vm.epaInfo.Employee.EmployeeId,
            EPAProcess: vm.epaInfo.Process.CurrentProcess,
            KPIGroups: vm.epaInfo.KPIGroups
        };

        return employeeEPAFactory.updateEPAScoreByEvaluator(criteria)
        .success(function (response) {
            if (response.IsSuccessful) {
                notificationFactory.success("UpdatedSuccessfully");
                vm.dataSaved = true;
            }
            else {
                notificationFactory.error("", response.Message);
            }
        }).error(function (response, status) {
            errorFactory.errorHandle(status);
        });
    };

    selfEvalutorModel.getEmployeeProjects = function (employeeId, periodId) {
        return integrateFactory.getEmployeeProject(employeeId, periodId).success(function (projects) {
            angular.forEach(projects, function (item) {
                item.CommentCharLimit = 50;
            });

            selfEvalutorModel.employeeProjects = projects;
        }).error(function (response, status) {
            errorFactory.errorHandle(status);
        });
    };

    selfEvalutorModel.getHRMSummary = function (employeeId, periodId) {
        return integrateFactory.getHRMSummary(employeeId, periodId).success(function (hrmSummary) {
            var total = {};

            total.AverageWorkingHours = 0;
            total.NumberLateIn = 0;
            total.NumberEarlyOut = 0;
            total.NumberMissingCheckInOut = 0;
            total.NumberOvertime = 0;
            total.NumberUnpaidLeave = 0;
            total.NumberUnplannedAnnualLeave = 0;

            if (hrmSummary.length > 0) {
                angular.forEach(hrmSummary, function (item) {
                    total.AverageWorkingHours += item.AverageWorkingHours;
                    total.NumberLateIn += item.NumberLateIn;
                    total.NumberEarlyOut += item.NumberEarlyOut;
                    total.NumberMissingCheckInOut += item.NumberMissingCheckInOut;
                    total.NumberOvertime += item.NumberOvertime;
                    total.NumberUnpaidLeave += item.NumberUnpaidLeave;
                    total.NumberUnplannedAnnualLeave += item.NumberUnplannedAnnualLeave;

                    item.ConvertedAverageWorkingHours = convertDecimalHoursToString(item.AverageWorkingHours);
                });

                total.ConvertedAverageWorkingHours = convertDecimalHoursToString(total.AverageWorkingHours / hrmSummary.length);
                total.TOEICScore = hrmSummary[0].TOEICScore;

                selfEvalutorModel.hrmSummary.Total = total;
            }

            selfEvalutorModel.hrmSummary.Data = hrmSummary;
        }).error(function (response, status) {
            errorFactory.errorHandle(status);
        });
    };

    function convertDecimalHoursToString(value) {
        var convertedValue = "0";

        if (value && value > 0) {
            var hour = parseInt(value, 10);
            if (hour < 10) {
                hour = '0' + hour;
            }
            hour += 'h';

            var minute = Math.round(((value - parseInt(value, 10)) * 60));
            if (minute < 10) {
                minute = '0' + minute;
            }
            minute += "'";
            convertedValue = hour + minute;
        }

        return convertedValue;
    }

    return selfEvalutorModel;
}]);