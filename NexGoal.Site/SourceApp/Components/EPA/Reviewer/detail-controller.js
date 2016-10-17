'use strict';
/*global epaModule*/

epaModule.controller('DetailReviewerController',
['$timeout', '$translate', '$routeParams', 'EmployeeEPAModel', 'ReviewerManagementModel', 'CommonModel', "SelfEvalutorModel",
    'utilitiesFactory', 'enumerationsFactory', 'baseUrl', 'historyFactory', 'notificationFactory', 'lockScreenService',
    'ViolationModel', 'NominationModel',
function ($timeout, $translate, $routeParams, EmployeeEPAModel, ReviewerManagementModel, CommonModel, SelfEvalutorModel,
    utilitiesFactory, enumerationsFactory, baseUrl, historyFactory, notificationFactory, lockScreenService,
    ViolationModel, NominationModel) {
    var vm = this;

    vm.violations = [];
    vm.hrmSummary = {};
    vm.employeeProjects = [];
    vm.permission = 0;
    vm.epaId = parseInt($routeParams.id, 10);
    vm.standardGrades = CommonModel.standardGrades;
    vm.policyEnum = enumerationsFactory.policy;
    vm.targetTypeEnum = enumerationsFactory.targetType;
    vm.statusEndProcessEnum = enumerationsFactory.statusEndProcess;
    vm.ratingTypeEnum = enumerationsFactory.ratingType;
    vm.evaluatedDate = null;
    vm.maxStar = utilitiesFactory.maxStar;

    vm.checkEndProcess = function (members) {
        var endProcess = true;

        angular.forEach(members, function (member) {
            if (!member.IsEndProcess) {
                endProcess = false;
            }
        });

        return endProcess;
    };

    vm.convertScoreToGrade = function (score) {
        return EmployeeEPAModel.convertScoreToGrade(score, vm.standardGrades);
    };

    vm.openPopupViewComment = function (comment) {
        EmployeeEPAModel.openPopupViewComment(vm, comment);
    };

    vm.range = function (n) {
        return new Array(n);
    };

    vm.getClassProgress = function (statusEndProcess) {
        return EmployeeEPAModel.getClassProgress(statusEndProcess);
    };

    vm.inputScore = function (currentProcess) {
        var maxSubtractScore = currentProcess.MaxSubtractScore,
        maxAdditionScore = currentProcess.MaxAdditionScore;
        var adjustScore = currentProcess.AdjustScore === "" ? 0 : currentProcess.AdjustScore;
        var defaultAdditionScore = 10, defaultSubtractScore = -10;

        if (adjustScore > maxAdditionScore) {
            if (maxAdditionScore < defaultAdditionScore) {
                notificationFactory.error('AdjustScoreInvalid');
            }

            currentProcess.AdjustScore = maxAdditionScore;
        } else if (adjustScore < maxSubtractScore) {
            if (maxSubtractScore > defaultSubtractScore) {
                notificationFactory.error('AdjustScoreInvalid');
            }

            currentProcess.AdjustScore = maxSubtractScore;
        }


        ReviewerManagementModel.calculateScore(vm.epaInfo, true);
        vm.epaInfo.Grade = EmployeeEPAModel.convertScoreToGrade(currentProcess.TotalScore, vm.standardGrades);
    };

    vm.save = function (epaInfo) {
        var isValidComment = ReviewerManagementModel.validateComment(epaInfo);
        if (!isValidComment) {
            notificationFactory.error('CommentRequire');
        }
        else {
            var totalcore = epaInfo.Process.CurrentProcess.TotalScore;

            if (ReviewerManagementModel.validateEvaluation(totalcore)) {
                var model = new Array(epaInfo);

                ReviewerManagementModel.adjustScoreByReviewer(vm, model);
            }
        }
    };

    vm.cancel = function () {
        historyFactory.back();
    };

    vm.generateRatingStar = function () {
        $timeout(function () {
            $(".rating-loading").rating();
        }, 1);
    };

    function calculateAverageEvaluator(Evaluators) {
        var totalScore = 0, count = 0;

        angular.forEach(Evaluators, function (evaluator) {
            if (evaluator.TotalScore) {
                totalScore += utilitiesFactory.parseFloat(evaluator.TotalScore);
                count++;
            }
        });

        return count > 0 ? utilitiesFactory.splitNumber(totalScore / count, 2) : 0;
    }

    function CalculateEvaluatorScoreOnKPI(evaluators) {
        var averageScore = 0;

        if (evaluators.length > 0) {
            var score = 0;
            var length = 0;
            angular.forEach(evaluators, function (evaluator) {
                if (evaluator.ScoreEncode) {
                    score += evaluator.Score;
                    length++;
                }
            });

            averageScore = score / length;
        }

        return utilitiesFactory.splitNumber(averageScore, 2);
    }

    function CalculateEvaluatorScoreOnGroup(kpiGroup) {
        if (kpiGroup.EvaluatorsScore.length === 0) {
            return 0;
        }


        var score = 0;
        var length = 0;
        angular.forEach(kpiGroup.EvaluatorsScore, function (evaluator) {
            if (evaluator.Score !== 0) {
                score += evaluator.Score;
                length++;
            }
        });

        return utilitiesFactory.splitNumber(score / length, 2);
    }

    function getHRMSummary() {
        if (vm.epaInfo.Period.TargetTypeId === vm.targetTypeEnum.SemiAnnual) {
            SelfEvalutorModel.getHRMSummary(vm.epaInfo.Employee.EmployeeId, vm.epaInfo.Period.PeriodId).then(function () {
                vm.hrmSummary = SelfEvalutorModel.hrmSummary;
            });
        } else {
            SelfEvalutorModel.getHRMSummaryOnProbation(vm.epaInfo.Employee.EmployeeId).then(function () {
                vm.hrmSummary = SelfEvalutorModel.hrmSummary;
            });
        }
    }

    function getEPAInfoByReviewer() {
        lockScreenService.start();
        ReviewerManagementModel.getEPAInfoByReviewer(vm.epaId).then(function () {
            vm.epaInfo = ReviewerManagementModel.epaInfo;
            vm.epaInfo.Employee.Avatar = baseUrl + vm.epaInfo.Employee.Avatar;
            var averageScoreEvaluators = calculateAverageEvaluator(vm.epaInfo.Process.Evaluators);

            angular.forEach(vm.epaInfo.KPIGroups, function (kpiGroup) {
                angular.forEach(kpiGroup.KPIs, function (kpi) {
                    kpi.EvaluatorScore = CalculateEvaluatorScoreOnKPI(kpi.EvaluationByEvaluators);
                });
                var scoreOfGroup = CalculateEvaluatorScoreOnGroup(kpiGroup);

                kpiGroup.EvaluatorScore = scoreOfGroup;
            });

            // self
            vm.epaInfo.Process.Self.TotalScore = utilitiesFactory.parseFloat(vm.epaInfo.Process.Self.TotalScore);

            // evaluator
            angular.forEach(vm.epaInfo.Process.Evaluators, function (evaluator) {
                evaluator.TotalScore = utilitiesFactory.parseFloat(evaluator.TotalScore);
            });
            vm.epaInfo.Process.AverageScoreByEvaluators = averageScoreEvaluators;

            // reviewer
            var maxAdditionScore = parseFloat((vm.epaInfo.Process.CurrentProcess.MaxPerformanceScore - averageScoreEvaluators).toFixed(2));

            vm.epaInfo.Process.CurrentProcess.MaxSubtractScore = -10;
            vm.epaInfo.Process.CurrentProcess.MaxAdditionScore = maxAdditionScore > 10 ? 10 : maxAdditionScore;
            vm.epaInfo.Process.CurrentProcess.TotalScore = vm.epaInfo.Process.CurrentProcess.TotalScore !== "" ?
                                                            utilitiesFactory.parseFloat(vm.epaInfo.Process.CurrentProcess.TotalScore) :
                                                            utilitiesFactory.parseFloat(vm.epaInfo.Process.AverageScoreByEvaluators);
            vm.evaluatedDate = vm.epaInfo.Process.CurrentProcess.EvaluateDate ? vm.epaInfo.Process.CurrentProcess.EvaluateDate : null;

            vm.epaInfo.Grade = EmployeeEPAModel.convertScoreToGrade(vm.epaInfo.Process.CurrentProcess.TotalScore, vm.standardGrades);

            if (vm.epaInfo.LastFinalScore.length > 0) {
                vm.epaInfo.LastFinalScore = utilitiesFactory.parseFloat(vm.epaInfo.LastFinalScore);
            } else {
                vm.epaInfo.LastFinalScore = null;
            }

            ReviewerManagementModel.getUserPermissionOnReviewer().then(function () {
                vm.permission = ReviewerManagementModel.permission;

                if (vm.permission !== vm.policyEnum.Host) {
                    vm.epaInfo.IsAllowedToEdit = false;
                }
            });

            getHRMSummary();

            SelfEvalutorModel.getEmployeeProjects(vm.epaInfo.Employee.EmployeeId, vm.epaInfo.Period.PeriodId).then(function () {
                vm.employeeProjects = SelfEvalutorModel.employeeProjects;
            });

            ViolationModel.getViolationsByPeriod(vm.epaInfo.Period.PeriodId, vm.epaInfo.Employee.EmployeeId).then(function () {
                vm.violations = ViolationModel.violationsByPeriod;
            });

            NominationModel.getNominationByPeriodId(vm.epaInfo.Employee.EmployeeId, vm.epaInfo.Period.PeriodId).then(function () {
                vm.employeeNominations = NominationModel.employeeNominations;
            });
        })["finally"](function () {
            lockScreenService.stop();
        });
    }

    function initialize() {
        $translate('ScoreInputValid').then(function (message) {
            vm.scoreInputValidMessage = message;
        });

        CommonModel.getStandardGrade().then(function () {
            vm.standardGrades = CommonModel.standardGrades;

            getEPAInfoByReviewer();
        });
    }

    initialize();
}]);