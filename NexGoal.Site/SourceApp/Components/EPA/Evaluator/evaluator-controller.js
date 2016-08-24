'use strict';
/*global epaModule*/

epaModule.controller('EvaluatorController',
["$location", 'EmployeeEPAModel', 'SelfEvalutorModel', 'CommonModel', 'utilitiesFactory', "lockScreenService",
    'enumerationsFactory', 'historyFactory', 'baseUrl',
function ($location, EmployeeEPAModel, SelfEvalutorModel, CommonModel, utilitiesFactory, lockScreenService,
    enumerationsFactory, historyFactory, baseUrl) {
    var vm = this;

    vm.memberEPAs = [];
    vm.members = [];
    vm.targetTypeEnum = enumerationsFactory.targetType;
    vm.filter = {};

    vm.handleOnPeriodClose = function () {
        var periodIds = utilitiesFactory.getSelectedData(vm.periods, "PeriodId");

        if (periodIds !== vm.filter.periodIds) {
            vm.filter.periodIds = periodIds;

            getGroupsByPeriods(periodIds);
        }
    };

    vm.handleOnGroupClose = function () {
        var groupIds = utilitiesFactory.getSelectedData(vm.groupOptions, "GroupId");

        if (groupIds !== vm.filter.groupIds) {
            vm.filter.groupIds = groupIds;

            getMemberByEvaluator(vm.filter.periodIds, vm.filter.groupIds);
        }
    };

    vm.query = function () {
        vm.filter.employeeIds = utilitiesFactory.getSelectedData(vm.members, 'EmployeeId');

        getEPAByEvaluator(vm.filter.periodIds, vm.filter.groupIds, vm.filter.employeeIds);
    };

    vm.viewEPADetail = function (memberEPA) {
        $location.path(baseUrl + 'Evaluator/Detail/' + memberEPA.EPAId);
    };

    vm.viewEPAResult = function (memberEPA) {
        $location.path(baseUrl + 'EPA/Detail/' + memberEPA.EPAId);
    };

    function initFilter() {
        var filter = historyFactory.getState();
        return filter ? filter : {
            groupIds: '',
            employeeIds: '',
            periodIds: ''
        };
    }

    function getEPAByEvaluator(periodIds, groupIds, employeeIds) {
        lockScreenService.start();

        SelfEvalutorModel.getEPAByEvaluator(periodIds, groupIds, employeeIds).then(function () {
            vm.memberEPAs = SelfEvalutorModel.memberEPAs;
        })["finally"](function () {
            lockScreenService.stop();
        });

        historyFactory.saveState(vm.filter);
    }

    function getMemberByEvaluator(periodIds, groupIds) {
        SelfEvalutorModel.getMemberByEvaluator(periodIds, groupIds).then(function () {
            vm.members = SelfEvalutorModel.members;

            utilitiesFactory.setSelectAll(vm.members, "EmployeeId");

            vm.filter.employeeIds = utilitiesFactory.getSelectedData(vm.members, "EmployeeId");
        });
    }

    function getGroupsByPeriods(periodIds) {
        SelfEvalutorModel.getGroupByEvaluator(periodIds).then(function () {
            vm.groupOptions = SelfEvalutorModel.groups;

            utilitiesFactory.setSelectAll(vm.groupOptions, "GroupId");

            vm.filter.groupIds = utilitiesFactory.getSelectedData(vm.groupOptions, "GroupId");

            getMemberByEvaluator(vm.filter.periodIds, vm.filter.groupIds);
        });
    }

    function initialize() {
        vm.filter = initFilter();

        CommonModel.getAllPeriodByEvaluator().then(function () {
            vm.periods = utilitiesFactory.groupingPeriods(CommonModel.periodOptions);

            vm.filter.periodIds = utilitiesFactory.getSelectedData(vm.periods, "PeriodId");

            getGroupsByPeriods(vm.filter.periodIds);

            vm.query();
        });
    }

    initialize();
}]);

epaModule.controller('MemberEPAInfoOfEvaluatorController',
["$scope", '$interval', '$translate', '$routeParams', 'EmployeeEPAModel', 'SelfEvalutorModel', 'CommonModel',
    'utilitiesFactory', 'enumerationsFactory', 'baseUrl', 'historyFactory',
function ($scope, $interval, $translate, $routeParams, EmployeeEPAModel, SelfEvalutorModel, CommonModel,
    utilitiesFactory, enumerationsFactory, baseUrl, historyFactory) {
    var vm = this;

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
    vm.targetTypeEnum = enumerationsFactory.targetType;
    vm.statusEndProcessEnum = enumerationsFactory.statusEndProcess;
    vm.ratingTypeEnum = enumerationsFactory.ratingType;
    vm.hasRatingByStar = false;
    vm.showRatingScoreByStar = true;

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

    function getLastStar(kpi) {
        var star = 0;

        angular.forEach(vm.epaInfo.LastRating, function (lastKpi) {
            if (lastKpi.KPIId === kpi.KPIId) {
                star = lastKpi.Rate;
            }
        });

        return star;
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

    function getTotalLastScore() {
        var total = 0;
        angular.forEach(vm.epaInfo.KPIGroups, function (kpiGroup) {
            total += vm.getSubLastScore(kpiGroup);
        });

        return total;
    }

    vm.setAutoSave = function () {
        vm.autoSave = !vm.autoSave;

        initAutoSave();
    };

    vm.convertScoreToGrade = function (score) {
        return EmployeeEPAModel.convertScoreToGrade(score, vm.standardGrades);
    };

    vm.calculateEmployeeSubTotalScore = function (kpiGroup) {
        var score = 0;
        var isEvaluateMaxScore = true;

        angular.forEach(kpiGroup.KPIs, function (kpi) {
            if (kpi.EvaluationBySelf.Score) {
                score += kpi.EvaluationBySelf.Score;
                if (kpi.EvaluationBySelf.Rate !== vm.maxStar) {
                    isEvaluateMaxScore = false;
                }
            }
        });

        if (isEvaluateMaxScore) {
            return kpiGroup.Weight;
        }
        return score;
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

        SelfEvalutorModel.calculateScore(vm, false);
    };

    vm.cancelRating = function (kpiGroup, kpi) {
        kpi.CurrentEvaluation.Rate = 0;
        kpi.CurrentEvaluation.Score = null;
        $("#star_" + kpi.KPIId).rating('update', kpi.CurrentEvaluation.Rate);

        if (kpiGroup.HasKPIOptional) {
            SelfEvalutorModel.scoreSpreadForKPIGroupHasKPIOptional(kpiGroup, vm.maxStar);
        }

        SelfEvalutorModel.calculateScore(vm, false);
    };

    vm.cancel = function () {
        historyFactory.back();
    };

    vm.save = function (epaInfo) {
        if (SelfEvalutorModel.validateEPA(epaInfo)) {
            SelfEvalutorModel.updateEPAScoreByEvaluator(vm);
        }
        else {
            SelfEvalutorModel.openPopupNotificationInvalidComment(vm, vm.evaluateEPAInvalidMessage);
        }
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
                        SelfEvalutorModel.scoreSpreadForKPIGroupHasKPIOptional(kpiGroup, vm.maxStar);
                    }

                    SelfEvalutorModel.calculateScore(vm, false);
                }
            };
        }
    }

    function getEPAInfoByEvaluator() {
        SelfEvalutorModel.getEPAInfoByEvaluator(vm.epaId).then(function () {
            vm.epaInfo = SelfEvalutorModel.epaInfo;
            vm.epaInfo.LastPerformanceScore = getTotalLastScore();
            checkStatus(vm.epaInfo);

            if (vm.epaInfo.RatingType.RatingTypeId === vm.ratingTypeEnum.Star
                    || vm.epaInfo.RatingType.RatingTypeId === vm.ratingTypeEnum.HalfStar
                    || (vm.epaInfo.RatingType.RatingTypeId === vm.ratingTypeEnum.Score
                        && $.grep(vm.epaInfo.KPIGroups, function (e) { return e.NumberKPIOptional > 0; }).length > 0)) {
                vm.hasRatingByStar = true;
            }

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

            SelfEvalutorModel.getUserPermissionOnEvaluator().then(function () {
                vm.permission = SelfEvalutorModel.permission;

                if (vm.permission !== vm.policyEnum.Host) {
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

            getEPAInfoByEvaluator();
        });
    }

    initialize();
}]);