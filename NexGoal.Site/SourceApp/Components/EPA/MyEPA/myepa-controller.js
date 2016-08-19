'use strict';
/*global epaModule*/

epaModule.controller('EmployeeEPAController',
["$location", 'SelfEvalutorModel', 'CommonModel', 'enumerationsFactory', 'utilitiesFactory', 'historyFactory', 'baseUrl',
function ($location, SelfEvalutorModel, CommonModel, enumerationsFactory, utilitiesFactory, historyFactory, baseUrl) {
    var vm = this;

    vm.myEPAs = SelfEvalutorModel.myEPAs;
    vm.targetTypeEnum = enumerationsFactory.targetType;
    vm.periods = [];
    vm.filters = {};

    function initFilter() {
        var filter = historyFactory.getState();
        return filter ? filter : {
            periodIds: null
        };
    }

    vm.query = function (filter) {
        filter.periodIds = utilitiesFactory.getSelectedData(vm.periods, 'PeriodId');

        SelfEvalutorModel.getEPAByEmployee(filter.periodIds).then(function () {
            vm.myEPAs = SelfEvalutorModel.myEPAs;
        });

        historyFactory.saveState(vm.filter);
    };

    vm.viewEPADetail = function (epa) {
        $location.path(baseUrl + 'SelfEvaluation/Detail/' + epa.EPAId);
    };

    vm.viewEPAResult = function (epa) {
        if (epa.HasMonthlyEPA && epa.Period.TargetTypeId === vm.targetTypeEnum.SemiAnnual) {
            $location.path(baseUrl + 'EPA/SemiDetail/' + epa.EPAId);
        } else {
            $location.path(baseUrl + 'EPA/Detail/' + epa.EPAId);
        }
    };

    function initialize() {
        vm.filter = initFilter();

        CommonModel.getAllPeriodIncludeImport().then(function () {
            vm.periods = utilitiesFactory.groupingPeriods(CommonModel.periodOptions);
            vm.query(vm.filters);
        });
    }

    initialize();
}]);

epaModule.controller('EmployeeEPAInfoController',
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

    vm.getLastScore = function (kpi) {
        var lastScore = "";

        angular.forEach(vm.epaInfo.LastRating, function (lastKpi) {
            if (lastKpi.KPIId === kpi.KPIId) {
                lastScore = lastKpi.Score;
            }
        });

        return lastScore;
    };

    function getLastStar(kpi) {
        var star = 0

        angular.forEach(vm.epaInfo.LastRating, function (lastKpi) {
            if (lastKpi.KPIId === kpi.KPIId) {
                star = lastKpi.Rate;
            }
        });

        return star;
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
        var score = 0
        var isEvaluateMaxScore = true;
        angular.forEach(kpiGroup.KPIs, function (kpi) {
            score += vm.getLastScore(kpi);
            var star = getLastStar(kpi);
            if (star !== vm.maxStar) {
                isEvaluateMaxScore = false;
            }
        })

        if (isEvaluateMaxScore) {
            return kpiGroup.Weight;
        }

        return score;
    };

    vm.getTotalLastScore = function () {
        var total = 0;
        angular.forEach(vm.epaInfo.KPIGroups, function (kpiGroup) {
            total += vm.getSubLastScore(kpiGroup);
        })

        return total;
    };

    vm.setAutoSave = function () {
        vm.autoSave = !vm.autoSave;

        initAutoSave();
    };

    vm.convertScoreToGrade = function (score) {
        return EmployeeEPAModel.convertScoreToGrade(score, vm.standardGrades);
    };

    vm.calculateSubTotalScore = function (kpiGroup) {
        var score = 0;
        var isEvaluateMaxScore = true;
        angular.forEach(kpiGroup.KPIs, function (kpi) {
            if (kpi.CurrentEvaluation.Score) {
                score += kpi.CurrentEvaluation.Score;
                if (kpi.CurrentEvaluation.Rate !== vm.maxStar) {
                    isEvaluateMaxScore = false;
                }
            }
        });

        if (isEvaluateMaxScore) {
            return kpiGroup.Weight;
        }
        return score;
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

    initialize();
}]);