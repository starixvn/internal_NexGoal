'use strict';
/*global epaModule*/

epaModule.controller('DetailEmployeeController',
["$scope", "$interval", '$translate', '$routeParams', 'EmployeeEPAModel', 'SelfEvalutorModel', 'CommonModel',
    'utilitiesFactory', 'enumerationsFactory', 'historyFactory', 'baseUrl', 'LastRatingModel', 'ViolationModel',
    'NominationModel',
function ($scope, $interval, $translate, $routeParams, EmployeeEPAModel, SelfEvalutorModel, CommonModel,
    utilitiesFactory, enumerationsFactory, historyFactory, baseUrl, LastRatingModel, ViolationModel,
    NominationModel) {
    var vm = this;

    vm.epaId = parseInt($routeParams.id, 10);
    vm.maxStar = utilitiesFactory.maxStar;
    vm.epaInfo = SelfEvalutorModel.epaInfo;
    vm.hrmSummary = {};
    vm.employeeProjects = [];
    vm.violations = [];
    vm.employeeNominations = [];
    vm.standardGrades = CommonModel.standardGrades;
    vm.targetTypeEnum = enumerationsFactory.targetType;
    vm.statusEndProcessEnum = enumerationsFactory.statusEndProcess;
    vm.ratingTypeEnum = enumerationsFactory.ratingType;
    vm.policyEnum = enumerationsFactory.policy;
    vm.lastTotalScore = 0;
    vm.lastRating = [];

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

	function getHRMSummary(){
		if(vm.epaInfo.Period.TargetTypeId == vm.targetTypeEnum.SemiAnnual){
		SelfEvalutorModel.getHRMSummary(vm.epaInfo.Employee.EmployeeId, vm.epaInfo.Period.PeriodId).then(function () {
                vm.hrmSummary = SelfEvalutorModel.hrmSummary;
            });
		}else{
			SelfEvalutorModel.getHRMSummaryOnProbation(vm.epaInfo.Employee.EmployeeId).then(function () {
                vm.hrmSummary = SelfEvalutorModel.hrmSummary;
            });
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
            vm.epaInfo.LastPerformanceScore = 0;

            if (vm.epaInfo.LastFinalScore.length > 0) {
                vm.epaInfo.LastFinalScore = utilitiesFactory.parseFloat(vm.epaInfo.LastFinalScore);
            } else {
                vm.epaInfo.LastFinalScore = null;
            }

            vm.epaInfo.Grade = EmployeeEPAModel.convertScoreToGrade(vm.finalScore, vm.standardGrades);

            SelfEvalutorModel.getUserPermissionOnEmployee().then(function () {
                var permission = SelfEvalutorModel.permission;

                if (permission !== vm.policyEnum.Host) {
                    vm.epaInfo.IsAllowedToEdit = false;
                    vm.hoveringLeave = null;
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
            if (vm.epaInfo.Period.TargetTypeId == vm.targetTypeEnum.SemiAnnual) {
                LastRatingModel.getLastRating(vm.epaInfo.Employee.EmployeeId, vm.targetTypeEnum.SemiAnnual).then(function () {
                    vm.lastRating = LastRatingModel.groups;
                });
            }

            angular.forEach(vm.epaInfo.KPIGroups, function (kpiGroup) {
                vm.epaInfo.LastPerformanceScore += utilitiesFactory.parseFloat(vm.getSubLastScore(kpiGroup));
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

    vm.getLastScore = function (groupId, kpiId) {
        return LastRatingModel.getLastScore(groupId, kpiId);
    };

    vm.getLastWeight = function (groupId, kpiId) {
        return LastRatingModel.getLastWeight(groupId, kpiId);
    };

    vm.getSubLastWeight = function (kpiGroupId) {
        return LastRatingModel.getSubLastWeight(kpiGroupId);
    };

    vm.getSubLastScore = function (kpiGroupId) {
        return LastRatingModel.getSubLastScore(kpiGroupId);
    };

    vm.getLastTotalScore = function () {
        return LastRatingModel.getLastTotalScore();
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