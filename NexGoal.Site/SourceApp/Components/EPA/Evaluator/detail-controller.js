'use strict';
/*global epaModule*/

epaModule.controller('DetailEvaluatorController',
["$scope", '$interval', '$translate', '$routeParams', 'EmployeeEPAModel', 'EvaluatorManagementModel', 'CommonModel',
    'utilitiesFactory', 'enumerationsFactory', 'baseUrl', 'historyFactory',
function ($scope, $interval, $translate, $routeParams, EmployeeEPAModel, EvaluatorManagementModel, CommonModel,
    utilitiesFactory, enumerationsFactory, baseUrl, historyFactory) {
    vm.autoSave = true;
    vm.autoSaveInterval;
    vm.hrmSummary = {};
    vm.employeeProjects = [];
    vm.permission = 0;
    vm.epaId = parseInt($routeParams.id, 10);
    vm.evaluatorId = parseInt($routeParams.evaluatorId, 10);
    vm.maxStar = utilitiesFactory.maxStar;
    vm.dataSaved = false;
    vm.evaluatedDate = null;
    vm.standardGrades = CommonModel.standardGrades;
    vm.policyEnum = enumerationsFactory.policy;
    vm.statusEndProcessEnum = enumerationsFactory.statusEndProcess;
    vm.ratingTypeEnum = enumerationsFactory.ratingType;
    function initAutoSave() {
        if (vm.autoSave) {
            vm.autoSaveInterval = $interval(function () {
                vm.save(vm.epaInfo);
            }, 300000);
        } else {
            $interval.cancel(vm.autoSaveInterval);
        }
    }
    function checkStatus(epaInfo) {
        if (!epaInfo.IsAllowedToEdit) {
            vm.hoveringLeave = null;
        }
        else {
            vm.hoveringLeave = function (kpiGroup, kpi) {
                if (kpi.CurrentEvaluation.Rate > 0) {
                    if (kpiGroup.NumberKPIOptional === 0) {
                        var score = (kpi.Weight * kpi.CurrentEvaluation.Rate) / vm.maxStar;
                        score = utilitiesFactory.splitNumber(score, 2);
                        kpi.CurrentEvaluation.Score = score;
                    }
                    else {
                        EvaluatorManagementModel.scoreSpreadForKPIGroupHasKPIOptional(kpiGroup, vm.maxStar);
                    }
                    calculateSubTotalScore(kpiGroup);
                    EvaluatorManagementModel.calculateScore(vm);
                }
            };
        }
    }
    function BindingCurrentEvaluatorGroupScore(evaluatorId) {
        angular.forEach(vm.epaInfo.KPIGroups, function (kpiGroup) {
            var score = 0;
            angular.forEach(kpiGroup.EvaluatorsScore, function (evaluator) {
                if (evaluator.EvaluatorId === evaluatorId) {
                    score= evaluator.Score;
                }
            })
            kpiGroup.EvaluatorScore = score;
        })
    }
    function getEPAInfoByEvaluator() {
        EvaluatorManagementModel.getEPAInfoByEvaluator(vm.epaId).then(function () {
            vm.epaInfo = EvaluatorManagementModel.epaInfo;
            vm.epaInfo.LastPerformanceScore = getTotalLastScore();
            checkStatus(vm.epaInfo);
            BindingCurrentEvaluatorGroupScore(vm.epaInfo.Process.CurrentProcess.EvaluatorId);
            vm.epaInfo.Employee.Avatar = baseUrl + vm.epaInfo.Employee.Avatar;
            vm.epaInfo.Process.Self.TotalScore = utilitiesFactory.parseFloat(vm.epaInfo.Process.Self.TotalScore);
            vm.epaInfo.Process.CurrentProcess.TotalScore = utilitiesFactory.parseFloat(vm.epaInfo.Process.CurrentProcess.TotalScore);
            
            vm.finalScoreOfSelf = vm.epaInfo.Process.Self.TotalScore + vm.epaInfo.DisciplineScore;
            vm.finalScoreOfSelf = EmployeeEPAModel.checkScore(vm.finalScoreOfSelf, 0, vm.epaInfo.Process.Self.MaxPerformanceScore);
            vm.finalScore = vm.epaInfo.Process.CurrentProcess.TotalScore + vm.epaInfo.DisciplineScore;
            vm.finalScore = EmployeeEPAModel.checkScore(vm.finalScore, 0, vm.epaInfo.Process.CurrentProcess.MaxPerformanceScore);
            if (vm.epaInfo.LastFinalScore.length > 0) {
                vm.epaInfo.LastFinalScore = utilitiesFactory.parseFloat(vm.epaInfo.LastFinalScore);
            } else {
                vm.epaInfo.LastFinalScore = null;
            }
            vm.gradeOfSelf = EmployeeEPAModel.convertScoreToGrade(vm.finalScoreOfSelf, vm.standardGrades);
            vm.epaInfo.Grade = EmployeeEPAModel.convertScoreToGrade(vm.finalScore, vm.standardGrades);
            vm.evaluatedDate = vm.epaInfo.Process.CurrentProcess.EvaluateDate ? vm.epaInfo.Process.CurrentProcess.EvaluateDate : null;
            EvaluatorManagementModel.getUserPermissionOnEvaluator().then(function () {
                vm.permission = EvaluatorManagementModel.permission;
                if (vm.permission !== vm.policyEnum.Host) {
                    vm.epaInfo.IsAllowedToEdit = false;
                    vm.hoveringLeave = null;
                }
            });
            EvaluatorManagementModel.getHRMSummary(vm.epaInfo.Employee.EmployeeId).then(function () {
                vm.hrmSummary = EvaluatorManagementModel.hrmSummary;
            });
            EvaluatorManagementModel.getEmployeeProjects(vm.epaInfo.Employee.EmployeeId).then(function () {
                vm.employeeProjects = EvaluatorManagementModel.employeeProjects;
            });
            //initAutoSave();
            $scope.$on("$destroy", function () {
                $interval.cancel(vm.autoSaveInterval);
            });
        });
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

        if (isEvaluateMaxScore) {
            kpiGroup.EvaluatorScore = kpiGroup.Weight;
        } else {
            kpiGroup.EvaluatorScore = score;
        }
    };
        $translate('ScoreInputValid').then(function (message) {
            vm.scoreInputValidMessage = message;
        });
        $translate('EvaluateEPAInvalid').then(function (message) {
            vm.evaluateEPAInvalidMessage = message;
        });
        CommonModel.getStandardGrade().then(function () {
            vm.standardGrades = CommonModel.standardGrades;
            getEPAInfoByEvaluator();
    function getLastStar(kpi) {
        var star = 0
        angular.forEach(vm.epaInfo.LastRating, function (lastKpi) {
            if (lastKpi.KPIId === kpi.KPIId) {
                star = lastKpi.Rate;
            }
        });

        return star;
    };

    function getTotalLastScore() {
        var total = 0;
        angular.forEach(vm.epaInfo.KPIGroups, function (kpiGroup) {
            total += vm.getSubLastScore(kpiGroup);
        })

        return total;
    };

    function initAutoSave() {
        if (vm.autoSave) {
            vm.autoSaveInterval = $interval(function () {
                vm.save(vm.epaInfo);
            }, 300000);
        } else {
            $interval.cancel(vm.autoSaveInterval);
        }
    }

    function checkStatus(epaInfo) {
        if (!epaInfo.IsAllowedToEdit) {
            vm.hoveringLeave = null;
        }
        else {
            vm.hoveringLeave = function (kpiGroup, kpi) {
                if (kpi.CurrentEvaluation.Rate > 0) {
                    if (kpiGroup.NumberKPIOptional === 0) {
                        var score = (kpi.Weight * kpi.CurrentEvaluation.Rate) / vm.maxStar;
                        score = utilitiesFactory.splitNumber(score, 2);
                        kpi.CurrentEvaluation.Score = score;
                    }
                    else {
                        EvaluatorManagementModel.scoreSpreadForKPIGroupHasKPIOptional(kpiGroup, vm.maxStar);
                    }

                    calculateSubTotalScore(kpiGroup);

                    EvaluatorManagementModel.calculateScore(vm);
                }
            };
        }
    }

    function BindingCurrentEvaluatorGroupScore(evaluatorId) {
        angular.forEach(vm.epaInfo.KPIGroups, function (kpiGroup) {
            var score = 0;
            angular.forEach(kpiGroup.EvaluatorsScore, function (evaluator) {
                if (evaluator.EvaluatorId === evaluatorId) {
                    score= evaluator.Score;
                }
            })

            kpiGroup.EvaluatorScore = score;
        })
    }

    function getEPAInfoByEvaluator() {
        EvaluatorManagementModel.getEPAInfoByEvaluator(vm.epaId).then(function () {
            vm.epaInfo = EvaluatorManagementModel.epaInfo;
            vm.epaInfo.LastPerformanceScore = getTotalLastScore();
            checkStatus(vm.epaInfo);
            BindingCurrentEvaluatorGroupScore(vm.epaInfo.Process.CurrentProcess.EvaluatorId);
            vm.epaInfo.Employee.Avatar = baseUrl + vm.epaInfo.Employee.Avatar;
            vm.epaInfo.Process.Self.TotalScore = utilitiesFactory.parseFloat(vm.epaInfo.Process.Self.TotalScore);
            vm.epaInfo.Process.CurrentProcess.TotalScore = utilitiesFactory.parseFloat(vm.epaInfo.Process.CurrentProcess.TotalScore);
            
            vm.finalScoreOfSelf = vm.epaInfo.Process.Self.TotalScore + vm.epaInfo.DisciplineScore;
            vm.finalScoreOfSelf = EmployeeEPAModel.checkScore(vm.finalScoreOfSelf, 0, vm.epaInfo.Process.Self.MaxPerformanceScore);
            vm.finalScore = vm.epaInfo.Process.CurrentProcess.TotalScore + vm.epaInfo.DisciplineScore;
            vm.finalScore = EmployeeEPAModel.checkScore(vm.finalScore, 0, vm.epaInfo.Process.CurrentProcess.MaxPerformanceScore);

            if (vm.epaInfo.LastFinalScore.length > 0) {
                vm.epaInfo.LastFinalScore = utilitiesFactory.parseFloat(vm.epaInfo.LastFinalScore);
            } else {
                vm.epaInfo.LastFinalScore = null;
            }

            vm.gradeOfSelf = EmployeeEPAModel.convertScoreToGrade(vm.finalScoreOfSelf, vm.standardGrades);
            vm.epaInfo.Grade = EmployeeEPAModel.convertScoreToGrade(vm.finalScore, vm.standardGrades);
            vm.evaluatedDate = vm.epaInfo.Process.CurrentProcess.EvaluateDate ? vm.epaInfo.Process.CurrentProcess.EvaluateDate : null;

            EvaluatorManagementModel.getUserPermissionOnEvaluator().then(function () {
                vm.permission = EvaluatorManagementModel.permission;

                if (vm.permission !== vm.policyEnum.Host) {
                    vm.epaInfo.IsAllowedToEdit = false;
                    vm.hoveringLeave = null;
                }
            });

            EvaluatorManagementModel.getHRMSummary(vm.epaInfo.Employee.EmployeeId).then(function () {
                vm.hrmSummary = EvaluatorManagementModel.hrmSummary;
            });

            EvaluatorManagementModel.getEmployeeProjects(vm.epaInfo.Employee.EmployeeId).then(function () {
                vm.employeeProjects = EvaluatorManagementModel.employeeProjects;
            });

            //initAutoSave();

            $scope.$on("$destroy", function () {
                $interval.cancel(vm.autoSaveInterval);
            });
        });
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
            kpiGroup.EvaluatorScore = kpiGroup.Weight;
        } else {
            kpiGroup.EvaluatorScore = score;
        }
    };

    function initialize() {
        $translate('ScoreInputValid').then(function (message) {
            vm.scoreInputValidMessage = message;
        });

        $translate('EvaluateEPAInvalid').then(function (message) {
            vm.evaluateEPAInvalidMessage = message;
        });

        CommonModel.getStandardGrade().then(function () {
            vm.standardGrades = CommonModel.standardGrades;

            getEPAInfoByEvaluator();
        });
    }

    function getLastStar(kpi) {
        var star = 0

        angular.forEach(vm.epaInfo.LastRating, function (lastKpi) {
            if (lastKpi.KPIId === kpi.KPIId) {
                star = lastKpi.Rate;
            }
        });

        return star;
    };

    function getTotalLastScore() {
        var total = 0;
        angular.forEach(vm.epaInfo.KPIGroups, function (kpiGroup) {
            total += vm.getSubLastScore(kpiGroup);
        })

        return total;
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
    }
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

    vm.setAutoSave = function () {
        vm.autoSave = !vm.autoSave;
    vm.viewBySelf = function () {
        return !vm.evaluatorId;
    };

    vm.viewBackButton = function () {
        return !$routeParams.newTab;
    };

    vm.initCollapseExpandCommentByEmployee = function (epaInfo, kpi) {
        if (epaInfo.Process.Self.IsEndProcess && (kpi.EvaluationBySelf.Comment && kpi.EvaluationBySelf.Comment.length > 0)) {
            kpi.ExpandedCommentByEmployee = true;
        }
        else {
            kpi.ExpandedCommentByEmployee = false;
        }
    };

    vm.getClassProgress = function (statusEndProcess) {
        return EmployeeEPAModel.getClassProgress(statusEndProcess);
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

        EvaluatorManagementModel.calculateScore(vm);
    };

    vm.cancelRating = function (kpiGroup, kpi) {
        kpi.CurrentEvaluation.Rate = 0;
        kpi.CurrentEvaluation.Score = null;
        $("#star_" + kpi.KPIId).rating('update', kpi.CurrentEvaluation.Rate);

        if (kpiGroup.HasKPIOptional) {
            EvaluatorManagementModel.scoreSpreadForKPIGroupHasKPIOptional(kpiGroup, vm.maxStar);
        }

        calculateSubTotalScore(kpiGroup);

        EvaluatorManagementModel.calculateScore(vm);
    };

    vm.cancel = function () {
        historyFactory.back();
    };

    vm.save = function (epaInfo) {
        if (EvaluatorManagementModel.validateEPA(epaInfo)) {
            EvaluatorManagementModel.updateEPAScoreByEvaluator(vm);
        }
        else {
            EvaluatorManagementModel.openPopupNotificationInvalidComment(vm, vm.evaluateEPAInvalidMessage);
        }
    };

    vm.generateRatingStar = function () {
        $interval(function () {
            $(".rating-loading").rating();
        }, 1);
    };

    initialize();
}]);