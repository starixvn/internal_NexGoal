'use strict';
/*global epaModule*/

epaModule.controller('DetailEmployeeController',
["$scope", "$interval", '$translate', '$routeParams', 'EmployeeEPAModel', 'SelfEvalutorModel', 'CommonModel',
    'utilitiesFactory', 'enumerationsFactory', 'historyFactory', 'baseUrl',
function ($scope, $interval, $translate, $routeParams, EmployeeEPAModel, SelfEvalutorModel, CommonModel,
    utilitiesFactory, enumerationsFactory, historyFactory, baseUrl) {
    var vm = this;

    vm.epaId = parseInt($routeParams.id, 10);
    vm.maxStar = utilitiesFactory.maxStar;
    vm.autoSave = true;
    vm.autoSaveInterval;
    vm.dataSaved = false;
    vm.evaluatedDate = null;

    vm.epaInfo = SelfEvalutorModel.epaInfo;
    vm.hrmSummary = {};
    vm.employeeProjects = [];
    vm.standardGrades = CommonModel.standardGrades;
    vm.targetTypeEnum = enumerationsFactory.targetType;
    vm.statusEndProcessEnum = enumerationsFactory.statusEndProcess;
    vm.ratingTypeEnum = enumerationsFactory.ratingType;
    vm.policyEnum = enumerationsFactory.policy;
    vm.lastTotalScore = 0;


    function initAutoSave() {
        if (vm.autoSave) {
            vm.autoSaveInterval = $interval(function () {
                vm.save(vm.epaInfo);
            }, 60000);
        } else {
            $interval.cancel(vm.autoSaveInterval);
        }
    }

    function checkStatus(epaInfo) {
        if (!epaInfo.IsAllowedToEdit) {
            vm.hoveringLeave = null;
        } else {
            vm.hoveringLeave = function (kpiGroup, kpi) {
                if (kpi.CurrentEvaluation.Rate > 0) {
                    if (!kpiGroup.HasKPIOptional) {
                        var score = (kpi.Weight * kpi.CurrentEvaluation.Rate) / vm.maxStar;
                        score = utilitiesFactory.splitNumber(score, 2);
                        kpi.CurrentEvaluation.Score = score;
                    }
                    else {
                        SelfEvalutorModel.scoreSpreadForKPIGroupHasKPIOptional(kpiGroup, vm.maxStar);
                    }

                    calculateSubTotalScore(kpiGroup);

                    SelfEvalutorModel.calculateScore(vm, true);
                }
            };
        }
    }

    function getEPAInfoByEmployee() {
        SelfEvalutorModel.getEPAInfoByEmployee(vm.epaId).then(function () {
            vm.epaInfo = SelfEvalutorModel.epaInfo;
            checkStatus(vm.epaInfo);

            vm.epaInfo.Employee.Avatar = baseUrl + vm.epaInfo.Employee.Avatar;
            vm.epaInfo.Process.CurrentProcess.TotalScore = utilitiesFactory.parseFloat(vm.epaInfo.Process.CurrentProcess.TotalScore);
            vm.finalScore = vm.epaInfo.Process.CurrentProcess.TotalScore + vm.epaInfo.DisciplineScore;
            vm.finalScore = EmployeeEPAModel.checkScore(vm.finalScore, 0, vm.epaInfo.Process.CurrentProcess.MaxPerformanceScore);

            if (vm.epaInfo.LastFinalScore.length > 0) {
                vm.epaInfo.LastFinalScore = utilitiesFactory.parseFloat(vm.epaInfo.LastFinalScore);
            } else {
                vm.epaInfo.LastFinalScore = null;
            }

            vm.epaInfo.Grade = EmployeeEPAModel.convertScoreToGrade(vm.finalScore, vm.standardGrades);
            vm.evaluatedDate = vm.epaInfo.Process.CurrentProcess.EvaluateDate ? vm.epaInfo.Process.CurrentProcess.EvaluateDate : null;

            SelfEvalutorModel.getUserPermissionOnEmployee().then(function () {
                var permission = SelfEvalutorModel.permission;

                if (permission !== vm.policyEnum.Host) {
                    vm.epaInfo.IsAllowedToEdit = false;
                    vm.hoveringLeave = null;
                }
            });

            SelfEvalutorModel.getHRMSummary(vm.epaInfo.Employee.EmployeeId).then(function () {
                vm.hrmSummary = SelfEvalutorModel.hrmSummary;
            });

            SelfEvalutorModel.getEmployeeProjects(vm.epaInfo.Employee.EmployeeId).then(function () {
                vm.employeeProjects = SelfEvalutorModel.employeeProjects;
            });

            //initAutoSave();

            $scope.$on("$destroy", function () {
                $interval.cancel(vm.autoSaveInterval);
            });
        });
    }

    function initialize() {
        $translate('ScoreInputValid').then(function (message) {
            vm.scoreInputValidMessage = message;
        });

        $translate('EvaluateEPAInvalid').then(function (message) {
            vm.evaluateEPAInvalidMessage = message;
        });

        CommonModel.getStandardGrade().then(function () {
            vm.standardGrades = CommonModel.standardGrades;

            getEPAInfoByEmployee();
        });
    }

    function getLastStar(kpi) {
        var star = 0;

        angular.forEach(vm.epaInfo.LastRating, function (lastKpi) {
            if (lastKpi.KPIId === kpi.KPIId) {
                star = lastKpi.Rate;
            }
        });

        return star;
    }

    function calculateSubTotalScore(kpiGroup) {
        var score = 0;
        var isEvaluateMaxScore = true;
        angular.forEach(kpiGroup.KPIs, function (kpi) {
            if (kpi.CurrentEvaluation.Score) {
                score += kpi.CurrentEvaluation.Score;
            }
            if (kpi.CurrentEvaluation.Rate !== vm.maxStar) {
                isEvaluateMaxScore = false;
            }
        });

        if (isEvaluateMaxScore) {
            kpiGroup.SelfDecodeScore = kpiGroup.Weight;
        } else {
            kpiGroup.SelfDecodeScore = score;
        }
    };

    vm.getLastScore = function (kpi) {
        var lastScore = "";

        angular.forEach(vm.epaInfo.LastRating, function (lastKpi) {
            if (lastKpi.KPIId === kpi.KPIId) {
                lastScore = lastKpi.Score;
            }
        });

        return lastScore;
    };

    vm.getLastWeight = function (kpi) {
        var lastWeight = "";

        angular.forEach(vm.epaInfo.LastRating, function (lastKpi) {
            if (lastKpi.KPIId === kpi.KPIId) {
                lastWeight = lastKpi.Weight;
            }
        });

        return lastWeight;
    };

    vm.getSubLastScore = function (kpiGroup) {
        var score = 0,
            isEvaluateMaxScore = true;
        angular.forEach(kpiGroup.KPIs, function (kpi) {
            score += vm.getLastScore(kpi);
            var star = getLastStar(kpi);
            if (star !== vm.maxStar) {
                isEvaluateMaxScore = false;
            }
        });

        if (isEvaluateMaxScore) {
            return kpiGroup.Weight;
        }

        return score;
    };

    vm.getTotalLastScore = function () {
        var total = 0;
        angular.forEach(vm.epaInfo.KPIGroups, function (kpiGroup) {
            total += vm.getSubLastScore(kpiGroup);
        });

        return total;
    };

    vm.setAutoSave = function () {
        vm.autoSave = !vm.autoSave;

        initAutoSave();
    };

    vm.convertScoreToGrade = function (score) {
        return EmployeeEPAModel.convertScoreToGrade(score, vm.standardGrades);
    };

    vm.getClassProgress = function (statusEndProcess) {
        return EmployeeEPAModel.getClassProgress(statusEndProcess);
    };

    vm.cancelRating = function (kpiGroup, kpi) {
        kpi.CurrentEvaluation.Rate = 0;
        $("#star_" + kpi.KPIId).rating('update', kpi.CurrentEvaluation.Rate);
        kpi.CurrentEvaluation.Score = null;

        if (kpiGroup.HasKPIOptional) {
            SelfEvalutorModel.scoreSpreadForKPIGroupHasKPIOptional(kpiGroup, vm.maxStar);
        }

        calculateSubTotalScore(kpiGroup);
        SelfEvalutorModel.calculateScore(vm, true);
    };

    vm.getNotificationInputScore = function (kpi) {
        var weight = utilitiesFactory.splitNumber(kpi.Weight, 2);

        return EmployeeEPAModel.getNotificationInputScore(0, weight, vm.scoreInputValidMessage);
    };

    vm.inputScore = function (kpi) {
        var weight = utilitiesFactory.splitNumber(kpi.Weight, 2),
            score = kpi.CurrentEvaluation.Score ? parseFloat(kpi.CurrentEvaluation.Score) : 0;

        score = utilitiesFactory.splitNumber(score, 2);
        kpi.CurrentEvaluation.Rate = 0;

        if (!EmployeeEPAModel.validateScoreInput(score, 0, weight, vm.scoreInputValidMessage)) {
            kpi.CurrentEvaluation.Score = null;
        }

        SelfEvalutorModel.calculateScore(vm, true);
    };

    vm.save = function (epaInfo) {
        if (SelfEvalutorModel.validateEPA(epaInfo)) {
            SelfEvalutorModel.updateEPAScoreByEmployee(vm);
        } else {
            SelfEvalutorModel.openPopupNotificationInvalidComment(vm, vm.evaluateEPAInvalidMessage);
        }
    };

    vm.cancel = function () {
        historyFactory.back();
    };

    vm.generateRatingStar = function () {
        $interval(function () {
            $(".rating-loading").rating();
        }, 1);
    };

    initialize();
}]);